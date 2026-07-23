import React, { useState, useEffect } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Wallet as WalletIcon, CreditCard, Landmark, DollarSign, Plus, X, MoreVertical } from 'lucide-react';
import { Skeleton } from '../components/ui/Skeleton';

import { cn } from '../lib/utils';
import { api, getUserId } from '../lib/api';

const getBankColor = (bankName: string) => {
  const name = bankName.toLowerCase();
  if (name.includes('inter')) return '#FF7A00';
  if (name.includes('nubank')) return '#8A05BE';
  if (name.includes('itau') || name.includes('itaú')) return '#EC7000';
  if (name.includes('bradesco')) return '#CC092F';
  if (name.includes('xp')) return '#D2AD5D';
  if (name.includes('carteira') || name.includes('dinheiro') || name.includes('espécie')) return '#4be277';
  return '#adc6ff';
};

export function Wallet({ startDate, endDate }: { startDate?: string; endDate?: string; key?: string }) {
  const [loading, setLoading] = useState(true);
  const [wallet, setWallet] = useState<any>(null);
  const [accounts, setAccounts] = useState<any[]>([]);
  const [isAddAccountOpen, setIsAddAccountOpen] = useState(false);
  const [bankName, setBankName] = useState('');
  const [accountName, setAccountName] = useState('');
  const [initialBalance, setInitialBalance] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const userId = getUserId();

  const loadData = async () => {
    if (!userId) return;
    try {
      setLoading(true);
      const walletData = await api.getWallet(userId);
      setWallet(walletData);

      const accountsData = await api.getBankAccounts(userId);
      setAccounts(accountsData || []);
    } catch (err) {
      console.error('Error fetching wallet data:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [userId]);

  useEffect(() => {
    if (isAddAccountOpen) {
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
  }, [isAddAccountOpen]);

  const handleCreateAccount = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!userId || !bankName || !accountName) return;
    
    setError('');
    setSubmitting(true);

    // Limpa o saldo inicial digitado
    let cleanBalance = initialBalance.replace(',', '.');
    const dotCount = (cleanBalance.match(/\./g) || []).length;
    if (dotCount > 1) {
      const parts = cleanBalance.split('.');
      const decimals = parts.pop();
      cleanBalance = parts.join('') + '.' + decimals;
    }
    cleanBalance = cleanBalance.replace(/[^\d.]/g, '');
    const numBalance = Number(cleanBalance);
    const finalBalance = isNaN(numBalance) ? 0 : numBalance;

    try {
      await api.createBankAccount(userId, bankName, accountName, finalBalance);
      setBankName('');
      setAccountName('');
      setInitialBalance('');
      setIsAddAccountOpen(false);
      await loadData();
    } catch (err: any) {
      console.error(err);
      setError(err.message || 'Falha ao criar conta bancária');
    } finally {
      setSubmitting(false);
    }
  };



  const totalBalance = wallet ? wallet.totalBalance : 0;

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Minha Carteira</h2>
          <p className="text-on-surface-variant mt-1">Visão geral do seu patrimônio e contas.</p>
        </div>
        <Button variant="primary" size="sm" onClick={() => setIsAddAccountOpen(true)}>
          <Plus size={16} />
          Nova Conta
        </Button>
      </div>

      {}
      <Card glow className="relative overflow-hidden">
        <div className="absolute top-0 right-0 p-8 opacity-10">
          <Landmark size={120} />
        </div>
        <div className="relative z-10">
          <p className="text-xs font-bold uppercase tracking-widest text-on-surface-variant mb-2">Patrimônio Líquido</p>
          {loading ? (
            <Skeleton className="h-12 w-64 text-brand-primary" />
          ) : (
            <h1 className="text-5xl font-bold text-brand-primary tracking-tight">
              R$ {totalBalance.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </h1>
          )}
          <div className="flex items-center gap-2 mt-4 text-brand-primary font-medium">
            <span className="text-xs bg-brand-primary/10 px-2 py-0.5 rounded-full">Saldo em contas reais</span>
          </div>
        </div>
      </Card>

      {}
      {loading ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {Array.from({ length: 4 }).map((_, i) => (
            <Card key={i} className="border-l-4 border-l-outline-variant">
              <div className="flex justify-between items-start mb-4">
                <Skeleton className="w-10 h-10 rounded-lg shrink-0" />
                <Skeleton className="w-6 h-6 rounded-full" />
              </div>
              <Skeleton className="h-4 w-2/3 mb-2" />
              <Skeleton className="h-8 w-1/2 mb-3" />
              <Skeleton className="h-3 w-1/3" />
            </Card>
          ))}
        </div>
      ) : accounts.length === 0 ? (
        <div className="text-center py-12 text-on-surface-variant text-sm bg-surface-low rounded-xl border border-dashed border-outline-variant">
          Nenhuma conta bancária registrada. Clique em "Nova Conta" para adicionar uma!
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {accounts.map((acc, i) => {
            const color = getBankColor(acc.bankName);
            const isCash = acc.bankName.toLowerCase().includes('dinheiro') || acc.bankName.toLowerCase().includes('espécie');

            return (
              <Card key={acc.id} className="border-l-4" style={{ borderLeftColor: color }}>
                <div className="flex justify-between items-start mb-4">
                  <div className="p-2 rounded-lg bg-surface-low" style={{ color: color }}>
                    {isCash ? <DollarSign size={20} /> : <Landmark size={20} />}
                  </div>
                  <button className="text-on-surface-variant hover:text-on-surface transition-colors">
                    <MoreVertical size={18} />
                  </button>
                </div>
                <p className="text-sm font-medium text-on-surface-variant">{acc.bankName}</p>
                <h3 className="text-2xl font-bold mt-1">R$ {acc.balance.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</h3>
                <p className="text-[10px] uppercase tracking-widest text-on-surface-variant/60 mt-2">{acc.accountName}</p>
              </Card>
            );
          })}
        </div>
      )}


      {}
      {isAddAccountOpen && (
        <div className="fixed inset-0 z-[60] flex items-center justify-center p-4">
          <div className="fixed inset-0 bg-black/30 backdrop-blur-sm" onClick={() => setIsAddAccountOpen(false)} />
          
          <Card glow className="w-full max-w-md glass-modal p-8 rounded-2xl relative z-10 shadow-2xl space-y-6">
            <div className="flex justify-between items-center">
              <h3 className="text-2xl font-bold tracking-tight text-on-surface">Nova Conta Bancária</h3>
              <button onClick={() => setIsAddAccountOpen(false)} className="p-2 hover:bg-surface-low rounded-full text-on-surface-variant hover:text-on-surface transition-colors">
                <X size={20} />
              </button>
            </div>

            {error && (
              <div className="p-3 rounded-lg bg-brand-error/10 border border-brand-error/20 text-brand-error text-xs">
                {error}
              </div>
            )}

            <form onSubmit={handleCreateAccount} className="space-y-4">
              <div className="space-y-1">
                <label className="text-xs font-bold text-on-surface-variant uppercase">Instituição Financeira</label>
                <input
                  type="text"
                  required
                  placeholder="Ex: Banco Inter, Nubank"
                  value={bankName}
                  onChange={(e) => setBankName(e.target.value)}
                  className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim text-on-surface placeholder:text-on-surface-variant/40"
                />
              </div>

              <div className="space-y-1">
                <label className="text-xs font-bold text-on-surface-variant uppercase">Descrição / Nome da Conta</label>
                <input
                  type="text"
                  required
                  placeholder="Ex: Conta Corrente, Poupança, XP Investimentos"
                  value={accountName}
                  onChange={(e) => setAccountName(e.target.value)}
                  className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim text-on-surface placeholder:text-on-surface-variant/40"
                />
              </div>

              <div className="space-y-1">
                <label className="text-xs font-bold text-on-surface-variant uppercase">Saldo Inicial</label>
                <input
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                  value={initialBalance}
                  onChange={(e) => setInitialBalance(e.target.value)}
                  className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim text-on-surface placeholder:text-on-surface-variant/40"
                />
              </div>

              <div className="pt-4 flex gap-4">
                <Button variant="secondary" className="flex-1" type="button" onClick={() => setIsAddAccountOpen(false)}>
                  Cancelar
                </Button>
                <Button variant="primary" className="flex-1" type="submit" disabled={submitting}>
                  {submitting ? 'Salvando...' : 'Confirmar'}
                </Button>
              </div>
            </form>
          </Card>
        </div>
      )}
    </div>
  );
}
