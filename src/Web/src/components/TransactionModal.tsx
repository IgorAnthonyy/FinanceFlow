import { motion, AnimatePresence } from 'motion/react';
import { X, Plus, CreditCard, Landmark, Tag } from 'lucide-react';
import { Button } from './ui/Button';
import { useState, useEffect } from 'react';
import { api, getUserId } from '../lib/api';

interface TransactionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export function TransactionModal({ isOpen, onClose, onSuccess }: TransactionModalProps) {
  const [type, setType] = useState<'expense' | 'income'>('expense');
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [bankAccountId, setBankAccountId] = useState('');

  const [categories, setCategories] = useState<any[]>([]);
  const [accounts, setAccounts] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [isCategoryDropdownOpen, setIsCategoryDropdownOpen] = useState(false);
  const [isAccountDropdownOpen, setIsAccountDropdownOpen] = useState(false);

  const userId = getUserId();

  useEffect(() => {
    if (isOpen && userId) {
      const loadOptions = async () => {
        try {
          const cats = await api.getCategories();
          setCategories(cats || []);

          const accs = await api.getBankAccounts(userId);
          setAccounts(accs || []);
        } catch (err) {
          console.error('Error loading modal options:', err);
        }
      };

      loadOptions();
      setAmount('');
      setDescription('');
      setCategoryId('');
      setBankAccountId('');
      setError('');
      setSuccess(false);
      setIsCategoryDropdownOpen(false);
      setIsAccountDropdownOpen(false);
    }
  }, [isOpen, userId]);

  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
      const mainEl = document.querySelector('main');
      if (mainEl) {
        mainEl.style.overflow = 'hidden';
      }
    } else {
      document.body.style.overflow = '';
      const mainEl = document.querySelector('main');
      if (mainEl) {
        mainEl.style.overflow = '';
      }
    }
    return () => {
      document.body.style.overflow = '';
      const mainEl = document.querySelector('main');
      if (mainEl) {
        mainEl.style.overflow = '';
      }
    };
  }, [isOpen]);

  const handleSubmit = async () => {
    if (!userId) {
      setError('Usuário não autenticado.');
      return;
    }

    // Limpa o valor digitado e formata como número JavaScript válido
    let cleanAmountStr = amount.replace(',', '.');
    const dotCount = (cleanAmountStr.match(/\./g) || []).length;
    if (dotCount > 1) {
      const parts = cleanAmountStr.split('.');
      const decimals = parts.pop();
      cleanAmountStr = parts.join('') + '.' + decimals;
    }
    cleanAmountStr = cleanAmountStr.replace(/[^\d.]/g, '');
    const numAmount = Number(cleanAmountStr);

    if (isNaN(numAmount) || numAmount <= 0) {
      setError('Por favor, digite um valor válido maior que zero.');
      return;
    }

    if (!description) {
      setError('Por favor, digite uma descrição.');
      return;
    }
    if (!categoryId) {
      setError('Por favor, selecione uma categoria.');
      return;
    }
    if (!bankAccountId) {
      setError('Por favor, selecione uma conta bancária.');
      return;
    }

    const selectedAccount = accounts.find((acc: any) => acc.id === bankAccountId);
    const balance = selectedAccount ? selectedAccount.balance : 0;

    setLoading(true);
    setError('');

    try {
      const txType = type === 'income' ? 0 : 1;

      await api.createTransaction(
        userId,
        { bankAccountId, balance },
        description,
        description,
        numAmount,
        txType,
        categoryId,
        new Date().toISOString()
      );

      setSuccess(true);
      setTimeout(() => {
        onSuccess();
        onClose();
        setSuccess(false);
      }, 1800);
    } catch (err: any) {
      console.error(err);
      setError(err.message || 'Falha ao criar transação.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/30 backdrop-blur-sm z-[60]"
          />
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: 20 }}
            className="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-lg p-8 z-[70] space-y-6"
          >
            {/* Glassmorphism Background to avoid browser clipping issues with absolute dropdowns */}
            <div className="absolute inset-0 bg-surface-lowest/90 backdrop-blur-2xl border border-black/10 rounded-2xl -z-10 shadow-2xl" />
            {success ? (
              <div className="flex flex-col items-center justify-center py-12 space-y-6 text-center">
                <motion.div
                  initial={{ scale: 0 }}
                  animate={{ scale: 1 }}
                  transition={{ type: "spring", stiffness: 200, damping: 15 }}
                  className="w-20 h-20 bg-brand-primary/10 rounded-full flex items-center justify-center border border-brand-primary/20 text-brand-primary"
                >
                  <motion.svg
                    className="w-10 h-10"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    strokeWidth={3}
                  >
                    <motion.path
                      initial={{ pathLength: 0 }}
                      animate={{ pathLength: 1 }}
                      transition={{ delay: 0.2, duration: 0.5 }}
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M5 13l4 4L19 7"
                    />
                  </motion.svg>
                </motion.div>
                <div className="space-y-2">
                  <h3 className="text-xl font-bold text-on-surface">Transação Criada!</h3>
                  <p className="text-sm text-on-surface-variant max-w-xs">
                    Sincronizando com sua conta bancária de forma segura...
                  </p>
                </div>
                <div className="w-full max-w-xs bg-surface-lowest rounded-full h-1.5 overflow-hidden border border-outline-variant">
                  <motion.div
                    initial={{ width: 0 }}
                    animate={{ width: "100%" }}
                    transition={{ duration: 1.6, ease: "easeInOut" }}
                    className="bg-brand-primary h-full rounded-full"
                  />
                </div>
              </div>
            ) : (
              <>
                <div className="flex justify-between items-center">
                  <h2 className="text-2xl font-bold tracking-tight">Nova Transação</h2>
                  <button
                    onClick={onClose}
                    className="p-2 hover:bg-surface-low rounded-full transition-colors text-on-surface-variant hover:text-on-surface"
                  >
                    <X size={24} />
                  </button>
                </div>

                {error && (
                  <div className="p-3 rounded-lg bg-brand-error/10 border border-brand-error/20 text-brand-error text-xs">
                    {error}
                  </div>
                )}

                <div className="space-y-6">
                  { }
                  <div className="flex p-1 bg-surface-lowest rounded-full border border-outline-variant">
                    <button
                      onClick={() => setType('expense')}
                      className={`flex-1 py-3 rounded-full text-sm font-bold transition-all ${type === 'expense' ? 'bg-brand-error text-brand-background shadow-lg' : 'text-on-surface-variant hover:text-on-surface'
                        }`}
                    >
                      Despesa
                    </button>
                    <button
                      onClick={() => setType('income')}
                      className={`flex-1 py-3 rounded-full text-sm font-bold transition-all ${type === 'income' ? 'bg-brand-primary text-brand-background shadow-lg' : 'text-on-surface-variant hover:text-on-surface'
                        }`}
                    >
                      Receita
                    </button>
                  </div>

                  { }
                  <div className="space-y-2 text-center py-6">
                    <p className="text-xs uppercase tracking-widest text-on-surface-variant font-bold">Valor</p>
                    <div className="flex items-center justify-center gap-2">
                      <span className="text-2xl font-bold opacity-50">R$</span>
                      <input
                        type="number"
                        placeholder="0.00"
                        step="0.01"
                        value={amount}
                        onChange={(e) => setAmount(e.target.value)}
                        className="bg-transparent border-none text-5xl font-bold text-center w-full focus:outline-none placeholder:text-surface-highest text-on-surface"
                        autoFocus
                      />
                    </div>
                  </div>

                  { }
                  <div className="space-y-4">
                    <div className="relative group">
                      <input
                        type="text"
                        placeholder="Descrição (ex: Supermercado)"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim transition-all text-on-surface placeholder:text-on-surface-variant/40"
                      />
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                      <div className="relative">
                        <button
                          type="button"
                          onClick={() => {
                            setIsCategoryDropdownOpen(!isCategoryDropdownOpen);
                            setIsAccountDropdownOpen(false);
                          }}
                          className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 pl-4 pr-10 text-sm outline-none text-on-surface text-left flex items-center justify-between transition-all focus:border-brand-primary/50"
                        >
                          <span className={categoryId ? 'text-on-surface' : 'text-on-surface-variant/40'}>
                            {categories.find((cat: any) => cat.id === categoryId)?.name || 'Categoria'}
                          </span>
                          <Tag className="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant pointer-events-none" size={16} />
                        </button>

                        {isCategoryDropdownOpen && (
                          <>
                            <div className="fixed inset-0 z-40" onClick={() => setIsCategoryDropdownOpen(false)} />
                            <div className="absolute left-0 right-0 mt-1 bg-surface border border-outline-variant rounded-xl shadow-2xl z-50 py-1 max-h-48 overflow-y-auto">
                              {categories.map((cat: any) => (
                                <button
                                  key={cat.id}
                                  type="button"
                                  onClick={() => {
                                    setCategoryId(cat.id);
                                    setIsCategoryDropdownOpen(false);
                                  }}
                                  className={`w-full text-left px-4 py-3 text-sm transition-colors hover:bg-surface-high ${categoryId === cat.id ? 'text-brand-primary font-medium bg-surface-high' : 'text-on-surface'
                                    }`}
                                >
                                  {cat.name}
                                </button>
                              ))}
                            </div>
                          </>
                        )}
                      </div>

                      <div className="relative">
                        <button
                          type="button"
                          onClick={() => {
                            setIsAccountDropdownOpen(!isAccountDropdownOpen);
                            setIsCategoryDropdownOpen(false);
                          }}
                          className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 pl-4 pr-10 text-sm outline-none text-on-surface text-left flex items-center justify-between transition-all focus:border-brand-primary/50"
                        >
                          <span className={bankAccountId ? 'text-on-surface' : 'text-on-surface-variant/40'}>
                            {accounts.find((acc: any) => acc.id === bankAccountId)?.bankName || 'Conta'}
                          </span>
                          <Landmark className="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant pointer-events-none" size={16} />
                        </button>

                        {isAccountDropdownOpen && (
                          <>
                            <div className="fixed inset-0 z-40" onClick={() => setIsAccountDropdownOpen(false)} />
                            <div className="absolute left-0 right-0 mt-1 bg-surface border border-outline-variant rounded-xl shadow-2xl z-50 py-1 max-h-48 overflow-y-auto">
                              {accounts.map((acc: any) => (
                                <button
                                  key={acc.id}
                                  type="button"
                                  onClick={() => {
                                    setBankAccountId(acc.id);
                                    setIsAccountDropdownOpen(false);
                                  }}
                                  className={`w-full text-left px-4 py-3 text-sm transition-colors hover:bg-surface-high ${bankAccountId === acc.id ? 'text-brand-primary font-medium bg-surface-high' : 'text-on-surface'
                                    }`}
                                >
                                  {acc.bankName}
                                </button>
                              ))}
                            </div>
                          </>
                        )}
                      </div>
                    </div>
                  </div>

                  <div className="pt-4 flex gap-4">
                    <Button variant="secondary" className="flex-1" onClick={onClose}>
                      Cancelar
                    </Button>
                    <Button className="flex-1 py-4 justify-center" onClick={handleSubmit} disabled={loading}>
                      {loading ? 'Confirmando...' : 'Confirmar'}
                    </Button>
                  </div>
                </div>
              </>
            )}
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
