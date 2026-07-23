const GATEWAY_API = import.meta.env.VITE_API_URL || "http://localhost:5000/api";
const AUTH_API = `${GATEWAY_API}/auth`;
const WALLET_API = `${GATEWAY_API}/wallet`;
const TRANSACTIONS_API = `${GATEWAY_API}/transactions`;

export function getAuthToken(): string | null {
  return localStorage.getItem('ff_token');
}

export function setAuthToken(token: string) {
  localStorage.setItem('ff_token', token);
}

export function getUserId(): string | null {
  return localStorage.getItem('ff_user_id');
}

export function setUserId(userId: string) {
  localStorage.setItem('ff_user_id', userId);
}

export function getUserName(): string | null {
  return localStorage.getItem('ff_user_name');
}

export function setUserName(userName: string) {
  localStorage.setItem('ff_user_name', userName);
}

export function getUserEmail(): string | null {
  return localStorage.getItem('ff_user_email');
}

export function setUserEmail(userEmail: string) {
  localStorage.setItem('ff_user_email', userEmail);
}

export function logout() {
  localStorage.removeItem('ff_token');
  localStorage.removeItem('ff_user_id');
  localStorage.removeItem('ff_user_name');
  localStorage.removeItem('ff_user_email');
}

async function parseError(response: Response, defaultMessage: string): Promise<string> {
  const cleanParamError = (msg: string) => {
    return msg.replace(/\s*\(Parameter\s+['"].*?['"]\)/gi, '');
  };

  try {
    const text = await response.text();
    try {
      const json = JSON.parse(text);
      if (json.errors) {
        const errorMessages = Object.entries(json.errors)
          .map(([field, msgs]) => {
            const messages = Array.isArray(msgs) ? msgs.join(', ') : String(msgs);
            const cleanField = field.startsWith('$.') ? field.substring(2) : field;
            // Tradução de campos comuns para português
            let translatedField = cleanField;
            if (cleanField.toLowerCase() === 'amount') translatedField = 'Valor';
            else if (cleanField.toLowerCase() === 'description') translatedField = 'Descrição';
            else if (cleanField.toLowerCase() === 'title') translatedField = 'Título';
            else if (cleanField.toLowerCase() === 'categoryid') translatedField = 'Categoria';
            else if (cleanField.toLowerCase() === 'bankaccountid') translatedField = 'Conta Bancária';
            else if (cleanField.toLowerCase() === 'bankname') translatedField = 'Instituição Financeira';
            else if (cleanField.toLowerCase() === 'accountname') translatedField = 'Nome da Conta';
            else if (cleanField.toLowerCase() === 'balance') translatedField = 'Saldo';
            
            return `${translatedField}: ${cleanParamError(messages)}`;
          })
          .join('\n');
        if (errorMessages) {
          return errorMessages;
        }
      }
      const rawMsg = json.message || json.title || defaultMessage;
      return cleanParamError(rawMsg);
    } catch {
      return cleanParamError(text || defaultMessage);
    }
  } catch {
    return defaultMessage;
  }
}

function isTokenCloseToExpiry(token: string, marginSeconds = 300): boolean {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return false;
    const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));
    if (!payload.exp) return false;
    const now = Math.floor(Date.now() / 1000);
    return (payload.exp - now) < marginSeconds;
  } catch {
    return false;
  }
}

let isRefreshing = false;
let refreshPromise: Promise<void> | null = null;

async function fetchWithAuth(url: string, options: RequestInit = {}) {
  let token = getAuthToken();
  
  if (token && isTokenCloseToExpiry(token)) {
    if (!isRefreshing) {
      isRefreshing = true;
      refreshPromise = (async () => {
        try {
          const res = await fetch(`${AUTH_API}/User/renew`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${token}` }
          });
          if (res.ok) {
            const data = await res.json();
            if (data.token) {
              setAuthToken(data.token);
            }
          }
        } catch (err) {
          console.error('Failed to renew token in fetch interceptor:', err);
        } finally {
          isRefreshing = false;
          refreshPromise = null;
        }
      })();
    }
    
    if (refreshPromise) {
      await refreshPromise;
    }
  }

  const headers = new Headers(options.headers || {});
  const activeToken = getAuthToken();
  if (activeToken) {
    headers.set('Authorization', `Bearer ${activeToken}`);
  }
  
  if (options.body && typeof options.body === 'string' && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  const response = await fetch(url, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const errMsg = await parseError(response, `Request failed with status ${response.status}`);
    throw new Error(errMsg);
  }

  const text = await response.text();
  return text ? JSON.parse(text) : null;
}

export const api = {
  async login(email: string, password: string) {
    const res = await fetch(`${AUTH_API}/User/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });
    if (!res.ok) {
      const errMsg = await parseError(res, 'Falha ao realizar login');
      throw new Error(errMsg);
    }
    return res.json();
  },

  async register(name: string, email: string, password: string) {
    const res = await fetch(`${AUTH_API}/User`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name, email, password }),
    });
    if (!res.ok) {
      const errMsg = await parseError(res, 'Falha ao realizar cadastro');
      throw new Error(errMsg);
    }
    return res.status === 200 || res.status === 201 || res.status === 204;
  },

  async getWallet(userId: string) {
    return fetchWithAuth(`${WALLET_API}/Wallet/user/${userId}`);
  },

  async getWalletReport() {
    return fetchWithAuth(`${WALLET_API}/reports/wallet`);
  },

  async getMonthlySummary(startDate?: string, endDate?: string) {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    return fetchWithAuth(`${TRANSACTIONS_API}/reports/dashboard/summary${query}`);
  },

  async getExpensesByCategory(startDate?: string, endDate?: string) {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    return fetchWithAuth(`${TRANSACTIONS_API}/reports/dashboard/expenses-by-category${query}`);
  },

  async getMonthlyEvolution(startDate?: string, endDate?: string) {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    return fetchWithAuth(`${TRANSACTIONS_API}/reports/dashboard/monthly-evolution${query}`);
  },

  async getDailyEvolution(startDate?: string, endDate?: string) {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    return fetchWithAuth(`${TRANSACTIONS_API}/reports/dashboard/daily-evolution${query}`);
  },

  async getLatestTransactions(startDate?: string, endDate?: string) {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const query = params.toString() ? `?${params.toString()}` : '';
    return fetchWithAuth(`${TRANSACTIONS_API}/reports/dashboard/latest-transactions${query}`);
  },

  async updateProfile(name: string, email: string) {
    return fetchWithAuth(`${AUTH_API}/User/profile`, {
      method: 'PUT',
      body: JSON.stringify({ name, email }),
    });
  },

  async getBankAccounts(userId: string) {
    return fetchWithAuth(`${WALLET_API}/BankAccount/user/${userId}`);
  },

  async createBankAccount(userId: string, bankName: string, accountName: string, balance: number) {
    return fetchWithAuth(`${WALLET_API}/BankAccount`, {
      method: 'POST',
      body: JSON.stringify({ userId, bankName, accountName, balance }),
    });
  },

  async getTransactions(userId: string) {
    return fetchWithAuth(`${TRANSACTIONS_API}/transactions/user/${userId}`);
  },

  async getFilteredTransactions(filters: any) {
    return fetchWithAuth(`${TRANSACTIONS_API}/transactions/filters`, {
      method: 'POST',
      body: JSON.stringify(filters)
    });
  },

  async createTransaction(userId: string, bankAccount: { bankAccountId: string; balance: number }, title: string, description: string, amount: number, type: number, categoryId: string, transactionDate: string) {
    return fetchWithAuth(`${TRANSACTIONS_API}/transactions`, {
      method: 'POST',
      body: JSON.stringify({
        userId,
        bankAccount,
        title,
        description,
        amount,
        type,
        categoryId,
        transactionDate,
      }),
    });
  },

  async getCategories() {
    return fetchWithAuth(`${TRANSACTIONS_API}/categories`);
  },
  
  async createCategory(name: string, icon: string, color: string) {
    return fetchWithAuth(`${TRANSACTIONS_API}/categories`, {
      method: 'POST',
      body: JSON.stringify({ name, icon, color }),
    });
  }
};