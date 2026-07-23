import { motion, AnimatePresence } from 'motion/react';
import { X, Landmark, Tag } from 'lucide-react';
import { Button } from './ui/Button';
import { useState, useEffect } from 'react';
import { api, getUserId } from '../lib/api';

interface FilterModalProps {
  isOpen: boolean;
  onClose: () => void;
  onApplyFilters: (filters: any) => void;
  currentFilters: any;
}

export function FilterModal({ isOpen, onClose, onApplyFilters, currentFilters }: FilterModalProps) {
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [minAmount, setMinAmount] = useState('');
  const [maxAmount, setMaxAmount] = useState('');
  const [type, setType] = useState<number | null>(null);
  const [categoryId, setCategoryId] = useState('');
  const [bankAccountId, setBankAccountId] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  const [categories, setCategories] = useState<any[]>([]);
  const [accounts, setAccounts] = useState<any[]>([]);
  const [isCategoryDropdownOpen, setIsCategoryDropdownOpen] = useState(false);
  const [isAccountDropdownOpen, setIsAccountDropdownOpen] = useState(false);

  const userId = getUserId();

  useEffect(() => {
    if (isOpen) {
      setStartDate(currentFilters.startDate || '');
      setEndDate(currentFilters.endDate || '');
      setMinAmount(currentFilters.minAmount !== undefined ? String(currentFilters.minAmount) : '');
      setMaxAmount(currentFilters.maxAmount !== undefined ? String(currentFilters.maxAmount) : '');
      setType(currentFilters.type !== undefined ? currentFilters.type : null);
      setCategoryId(currentFilters.categoryId || '');
      setBankAccountId(currentFilters.bankAccountId || '');
      setSearchTerm(currentFilters.searchTerm || '');
      setIsCategoryDropdownOpen(false);
      setIsAccountDropdownOpen(false);

      if (userId) {
        const loadOptions = async () => {
          try {
            const cats = await api.getCategories();
            setCategories(cats || []);

            const accs = await api.getBankAccounts(userId);
            setAccounts(accs || []);
          } catch (err) {
            console.error('Error loading filter options:', err);
          }
        };
        loadOptions();
      }
    }
  }, [isOpen, userId, currentFilters]);

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

  const parseFilterAmount = (val: string) => {
    if (!val) return undefined;
    let clean = val.replace(',', '.');
    const dotCount = (clean.match(/\./g) || []).length;
    if (dotCount > 1) {
      const parts = clean.split('.');
      const decimals = parts.pop();
      clean = parts.join('') + '.' + decimals;
    }
    clean = clean.replace(/[^\d.]/g, '');
    const num = Number(clean);
    return isNaN(num) ? undefined : num;
  };

  const handleApply = () => {
    onApplyFilters({
      startDate: startDate || undefined,
      endDate: endDate || undefined,
      minAmount: parseFilterAmount(minAmount),
      maxAmount: parseFilterAmount(maxAmount),
      type: type !== null ? type : undefined,
      categoryId: categoryId || undefined,
      bankAccountId: bankAccountId || undefined,
      searchTerm: searchTerm || undefined
    });
    onClose();
  };

  const handleClear = () => {
    onApplyFilters({});
    onClose();
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
            className="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-lg p-8 z-[70] space-y-6 text-on-surface"
          >
            {/* Glassmorphism Background Sibling to avoid container rendering clipping on dropdown list */}
            <div className="absolute inset-0 bg-surface-lowest/90 backdrop-blur-2xl border border-black/10 rounded-2xl -z-10 shadow-2xl" />
            <div className="flex justify-between items-center pb-4 mb-6 border-b border-outline-variant">
              <h3 className="text-xl font-bold">Filtros Avançados</h3>
              <button onClick={onClose} className="p-2 hover:bg-surface-low rounded-full transition-colors text-on-surface-variant hover:text-on-surface">
                <X size={24} />
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Tipo de Transação</label>
                <div className="flex p-1 bg-surface-lowest rounded-full border border-outline-variant">
                  <button
                    onClick={() => setType(null)}
                    className={`flex-1 py-2 rounded-full text-xs font-bold transition-all ${type === null ? 'bg-brand-primary text-brand-background shadow-md' : 'text-on-surface-variant hover:text-on-surface'
                      }`}
                  >
                    Todos
                  </button>
                  <button
                    onClick={() => setType(0)}
                    className={`flex-1 py-2 rounded-full text-xs font-bold transition-all ${type === 0 ? 'bg-brand-primary text-brand-background shadow-md' : 'text-on-surface-variant hover:text-on-surface'
                      }`}
                  >
                    Receitas
                  </button>
                  <button
                    onClick={() => setType(1)}
                    className={`flex-1 py-2 rounded-full text-xs font-bold transition-all ${type === 1 ? 'bg-brand-primary text-brand-background shadow-md' : 'text-on-surface-variant hover:text-on-surface'
                      }`}
                  >
                    Despesas
                  </button>
                </div>
              </div>

              <div>
                <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Busca Textual</label>
                <input
                  type="text"
                  placeholder="Título ou descrição..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 text-on-surface"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Data Inicial</label>
                  <input
                    type="date"
                    value={startDate}
                    onChange={(e) => setStartDate(e.target.value)}
                    className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 text-on-surface"
                  />
                </div>
                <div>
                  <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Data Final</label>
                  <input
                    type="date"
                    value={endDate}
                    onChange={(e) => setEndDate(e.target.value)}
                    className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 text-on-surface"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Valor Mínimo (R$)</label>
                  <input
                    type="number"
                    placeholder="Mínimo..."
                    value={minAmount}
                    onChange={(e) => setMinAmount(e.target.value)}
                    className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 text-on-surface"
                  />
                </div>
                <div>
                  <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Valor Máximo (R$)</label>
                  <input
                    type="number"
                    placeholder="Máximo..."
                    value={maxAmount}
                    onChange={(e) => setMaxAmount(e.target.value)}
                    className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 text-on-surface"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="relative">
                  <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Categoria</label>
                  <button
                    type="button"
                    onClick={() => {
                      setIsCategoryDropdownOpen(!isCategoryDropdownOpen);
                      setIsAccountDropdownOpen(false);
                    }}
                    className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 pl-4 pr-10 text-sm outline-none text-on-surface text-left flex items-center justify-between transition-all focus:border-brand-primary/50"
                  >
                    <span className={categoryId ? 'text-on-surface' : 'text-on-surface-variant/40'}>
                      {categories.find((cat: any) => cat.id === categoryId)?.name || 'Todas'}
                    </span>
                    <Tag className="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant pointer-events-none" size={16} />
                  </button>

                  {isCategoryDropdownOpen && (
                    <>
                      <div className="fixed inset-0 z-40" onClick={() => setIsCategoryDropdownOpen(false)} />
                      <div className="absolute left-0 right-0 mt-1 bg-surface border border-outline-variant rounded-xl shadow-2xl z-50 py-1 max-h-40 overflow-y-auto">
                        <button
                          type="button"
                          onClick={() => {
                            setCategoryId('');
                            setIsCategoryDropdownOpen(false);
                          }}
                          className={`w-full text-left px-4 py-2.5 text-sm transition-colors hover:bg-surface-high ${!categoryId ? 'text-brand-primary font-medium bg-surface-high' : 'text-on-surface'
                            }`}
                        >
                          Todas
                        </button>
                        {categories.map((cat: any) => (
                          <button
                            key={cat.id}
                            type="button"
                            onClick={() => {
                              setCategoryId(cat.id);
                              setIsCategoryDropdownOpen(false);
                            }}
                            className={`w-full text-left px-4 py-2.5 text-sm transition-colors hover:bg-surface-high ${categoryId === cat.id ? 'text-brand-primary font-medium bg-surface-high' : 'text-on-surface'
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
                  <label className="text-xs uppercase tracking-wider text-on-surface-variant font-bold block mb-2">Conta</label>
                  <button
                    type="button"
                    onClick={() => {
                      setIsAccountDropdownOpen(!isAccountDropdownOpen);
                      setIsCategoryDropdownOpen(false);
                    }}
                    className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 pl-4 pr-10 text-sm outline-none text-on-surface text-left flex items-center justify-between transition-all focus:border-brand-primary/50"
                  >
                    <span className={bankAccountId ? 'text-on-surface' : 'text-on-surface-variant/40'}>
                      {accounts.find((acc: any) => acc.id === bankAccountId)?.bankName || 'Todas'}
                    </span>
                    <Landmark className="absolute right-3 top-[38px] text-on-surface-variant pointer-events-none" size={16} />
                  </button>

                  {isAccountDropdownOpen && (
                    <>
                      <div className="fixed inset-0 z-40" onClick={() => setIsAccountDropdownOpen(false)} />
                      <div className="absolute left-0 right-0 mt-1 bg-surface border border-outline-variant rounded-xl shadow-2xl z-50 py-1 max-h-40 overflow-y-auto">
                        <button
                          type="button"
                          onClick={() => {
                            setBankAccountId('');
                            setIsAccountDropdownOpen(false);
                          }}
                          className={`w-full text-left px-4 py-2.5 text-sm transition-colors hover:bg-surface-high ${!bankAccountId ? 'text-brand-primary font-medium bg-surface-high' : 'text-on-surface'
                            }`}
                        >
                          Todas
                        </button>
                        {accounts.map((acc: any) => (
                          <button
                            key={acc.id}
                            type="button"
                            onClick={() => {
                              setBankAccountId(acc.id);
                              setIsAccountDropdownOpen(false);
                            }}
                            className={`w-full text-left px-4 py-2.5 text-sm transition-colors hover:bg-surface-high ${bankAccountId === acc.id ? 'text-brand-primary font-medium bg-surface-high' : 'text-on-surface'
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

            <div className="pt-6 mt-6 border-t border-outline-variant flex gap-4">
              <Button variant="secondary" className="flex-1" onClick={handleClear}>
                Limpar Filtros
              </Button>
              <Button className="flex-1 justify-center" onClick={handleApply}>
                Aplicar Filtros
              </Button>
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
