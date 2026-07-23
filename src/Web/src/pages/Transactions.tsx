import { useState, useEffect } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { 
  Search, 
  Filter, 
  ShoppingBag, 
  Briefcase, 
  Car, 
  Gamepad2,
  HelpCircle,
  MoreVertical,
  HeartPulse,
  GraduationCap,
  Utensils,
  TrendingUp
} from 'lucide-react';
import { cn } from '../lib/utils';
import { api, getUserId } from '../lib/api';
import { FilterModal } from '../components/FilterModal';
import { Skeleton } from '../components/ui/Skeleton';


const getCategoryDetails = (cat: any) => {
  let IconComponent = HelpCircle;
  if (cat?.icon) {
    switch (cat.icon.toLowerCase()) {
      case 'utensils':
        IconComponent = ShoppingBag;
        break;
      case 'car':
        IconComponent = Car;
        break;
      case 'gamepad':
        IconComponent = Gamepad2;
        break;
      case 'briefcase':
      case 'trendingup':
        IconComponent = Briefcase;
        break;
      case 'heart':
        IconComponent = HeartPulse;
        break;
      case 'graduation-cap':
        IconComponent = GraduationCap;
        break;
      case 'shopping-bag':
        IconComponent = ShoppingBag;
        break;
      default:
        IconComponent = HelpCircle;
        break;
    }
  } else {
    switch (cat?.name) {
      case 'Alimentação':
        IconComponent = ShoppingBag;
        break;
      case 'Salário':
        IconComponent = Briefcase;
        break;
      case 'Transporte':
        IconComponent = Car;
        break;
      case 'Lazer':
        IconComponent = Gamepad2;
        break;
      default:
        IconComponent = HelpCircle;
        break;
    }
  }

  let color = cat?.color || '#6b7280';
  if (color === 'orange') color = '#f97316';
  else if (color === 'green') color = '#16a34a';
  else if (color === 'blue') color = '#2563eb';
  else if (color === 'purple') color = '#9333ea';
  else if (color === 'red') color = '#e11d48';
  else if (color === 'yellow') color = '#eab308';

  return { icon: IconComponent, color };
};

interface TransactionsProps {
  startDate: string;
  endDate: string;
  isFilterModalOpen: boolean;
  setIsFilterModalOpen: (open: boolean) => void;
  filters: any;
  setFilters: (filters: any) => void;
  key?: string;
}

export function Transactions({ startDate, endDate, isFilterModalOpen, setIsFilterModalOpen, filters, setFilters }: TransactionsProps) {
  const [isInitialLoading, setIsInitialLoading] = useState(true);
  const [loading, setLoading] = useState(false);
  const [transactions, setTransactions] = useState<any[]>([]);
  const [categories, setCategories] = useState<any[]>([]);
  const [searchVal, setSearchVal] = useState('');
  const [appliedSearch, setAppliedSearch] = useState('');
  const userId = getUserId();

  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  // Reset to first page on filter or search updates
  useEffect(() => {
    setCurrentPage(1);
  }, [filters, appliedSearch, startDate, endDate]);

  useEffect(() => {
    if (!userId) return;

    const loadData = async () => {
      try {
        setLoading(true);
        const payload = {
          userId,
          ...filters,
          startDate: startDate || filters.startDate,
          endDate: endDate || filters.endDate,
          searchTerm: appliedSearch || undefined,
          pageNumber: currentPage,
          pageSize: 10
        };
        const response = await api.getFilteredTransactions(payload);
        setTransactions(response.items || []);
        setTotalPages(response.totalPages || 1);
        setTotalCount(response.totalCount || 0);
        
        const cats = await api.getCategories();
        setCategories(cats || []);
      } catch (err) {
        console.error('Error fetching transactions:', err);
      } finally {
        setLoading(false);
        setIsInitialLoading(false);
      }
    };

    loadData();
  }, [userId, filters, appliedSearch, startDate, endDate, currentPage]);


  const filteredTxs = transactions;


  const categoryBudgets = [
    { name: 'Alimentação', limit: 1500, color: '#4be277' },
    { name: 'Transporte', limit: 500, color: '#0566d9' },
    { name: 'Lazer', limit: 800, color: '#ffb4ab' },
  ];

  const budgetSummary = categoryBudgets.map(b => {
    let current = 0;
    transactions.forEach((tx: any) => {
      if (tx.type === 1) {
        const cat = categories.find((c: any) => c.id === tx.categoryId);
        if (cat && cat.name === b.name) {
          current += tx.amount;
        }
      }
    });
    return { ...b, current };
  });

  return (
    <div className="space-y-6 animate-in fade-in duration-700">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Transações</h2>
          <p className="text-on-surface-variant mt-1">Gerencie seu histórico e atividades financeiras.</p>
        </div>
        <div className="flex gap-2">
          <Button size="sm" onClick={() => setIsFilterModalOpen(true)}>
            Filtros Avançados
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="md:col-span-3">
          <div className="flex items-center gap-4 mb-6">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant" size={16} />
              <input 
                type="text" 
                placeholder="Procurar por nome, categoria ou descrição..."
                value={searchVal}
                onChange={(e) => setSearchVal(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    setAppliedSearch(searchVal);
                  }
                }}
                className="w-full bg-surface-low border border-outline-variant rounded-lg py-2 pl-10 pr-4 text-sm outline-none focus:border-brand-primary/50 transition-all text-on-surface placeholder:text-on-surface-variant/40"
              />
            </div>
            <Button onClick={() => setAppliedSearch(searchVal)} className="rounded-lg font-bold px-6">
              Buscar
            </Button>
          </div>

          <div className="space-y-1">
            <div className="grid grid-cols-12 px-4 py-2 text-[10px] uppercase tracking-widest text-on-surface-variant font-bold">
              <div className="col-span-6 md:col-span-5">Transação</div>
              <div className="hidden md:block col-span-2">Categoria</div>
              <div className="hidden md:block col-span-2">Data</div>
              <div className="col-span-6 md:col-span-3 text-right">Valor</div>
            </div>
            
            <div className={cn("divide-y divide-outline-variant transition-opacity", loading && "opacity-50 pointer-events-none")}>
              {isInitialLoading ? (
                Array.from({ length: 5 }).map((_, i) => (
                  <div key={i} className="grid grid-cols-12 items-center px-4 py-4 rounded-xl gap-4">
                    <div className="col-span-6 md:col-span-5 flex items-center gap-3">
                      <Skeleton className="w-10 h-10 rounded-full shrink-0" />
                      <div className="space-y-2 flex-1">
                        <Skeleton className="h-4 w-3/4" />
                        <Skeleton className="h-3 w-1/2 md:hidden" />
                      </div>
                    </div>
                    <div className="hidden md:block col-span-2">
                      <Skeleton className="h-5 w-16 rounded-full" />
                    </div>
                    <div className="hidden md:block col-span-2">
                      <Skeleton className="h-4 w-20" />
                    </div>
                    <div className="col-span-6 md:col-span-3 flex items-center justify-end text-right">
                      <Skeleton className="h-4 w-24" />
                    </div>
                  </div>
                ))
              ) : filteredTxs.length === 0 ? (
                <div className="text-center py-12 text-on-surface-variant text-sm">
                  Nenhuma transação encontrada.
                </div>
              ) : (
                filteredTxs.map((tx) => {
                  const cat = categories.find((c: any) => c.id === tx.categoryId);

                  const catName = cat ? cat.name : 'Outros';
                  const details = getCategoryDetails(cat || { name: catName });
                  const Icon = details.icon;
                  const dateFormatted = new Date(tx.transactionDate).toLocaleDateString('pt-BR');
                  
                  return (
                    <div key={tx.id} className="grid grid-cols-12 items-center px-4 py-4 hover:bg-surface-low transition-colors rounded-xl cursor-pointer group">
                      <div className="col-span-6 md:col-span-5 flex items-center gap-3">
                        <div className="w-10 h-10 rounded-full bg-surface-low flex items-center justify-center group-hover:scale-110 transition-transform">
                          <Icon size={18} style={{ color: details.color }} />
                        </div>
                        <div>
                          <p className="text-sm font-bold">{tx.title}</p>
                          <p className="text-[10px] text-on-surface-variant lg:hidden">{catName} • {dateFormatted}</p>
                        </div>
                      </div>
                      <div className="hidden md:block col-span-2 text-xs text-on-surface-variant">
                        <span className="px-2 py-0.5 rounded-full bg-surface-low">{catName}</span>
                      </div>
                      <div className="hidden md:block col-span-2 text-xs text-on-surface-variant">
                        {dateFormatted}
                      </div>
                      <div className="col-span-6 md:col-span-3 flex items-center justify-end gap-3 text-right">
                        <p className={cn(
                          "text-sm font-bold",
                          tx.type === 0 ? "text-brand-primary" : "text-on-surface"
                        )}>
                          {tx.type === 0 ? '+' : '-'} R$ {tx.amount.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                        </p>
                        <button className="p-1 hover:bg-surface-dim rounded-md transition-colors opacity-0 group-hover:opacity-100">
                          <MoreVertical size={16} className="text-on-surface-variant" />
                        </button>
                      </div>
                    </div>
                  );
                })
              )}
            </div>
            
            {/* Pagination Controls */}
            {!isInitialLoading && totalPages > 1 && (
              <div className="flex flex-col sm:flex-row items-center justify-between border-t border-outline-variant pt-6 mt-6 px-2 gap-4">
                <span className="text-xs text-on-surface-variant">
                  Exibindo página <span className="font-bold text-on-surface">{currentPage}</span> de <span className="font-bold text-on-surface">{totalPages}</span> (Total: {totalCount} transações)
                </span>
                <div className="flex items-center gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                    disabled={currentPage === 1 || loading}
                    className="h-8 px-3 rounded-lg text-xs"
                  >
                    Anterior
                  </Button>
                  <div className="flex items-center gap-1">
                    {Array.from({ length: totalPages }).map((_, idx) => {
                      const pageNum = idx + 1;
                      if (totalPages > 5 && pageNum !== 1 && pageNum !== totalPages && Math.abs(pageNum - currentPage) > 1) {
                        if (pageNum === 2 || pageNum === totalPages - 1) {
                          return <span key={pageNum} className="text-xs text-on-surface-variant px-1 select-none">...</span>;
                        }
                        return null;
                      }
                      return (
                        <button
                          key={pageNum}
                          onClick={() => setCurrentPage(pageNum)}
                          disabled={loading}
                          className={cn(
                            "w-8 h-8 rounded-lg text-xs font-bold transition-all",
                            currentPage === pageNum
                              ? "bg-brand-primary text-brand-background shadow-md"
                              : "text-on-surface-variant hover:text-on-surface hover:bg-surface-low"
                          )}
                        >
                          {pageNum}
                        </button>
                      );
                    })}
                  </div>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                    disabled={currentPage === totalPages || loading}
                    className="h-8 px-3 rounded-lg text-xs"
                  >
                    Próximo
                  </Button>
                </div>
              </div>
            )}
          </div>
        </Card>

        <Card className="h-fit space-y-6">
          <h3 className="font-bold text-lg">Resumo Mensal</h3>
          
          <div className="space-y-4">
            <div className="space-y-4 pt-2">
              <h4 className="text-xs font-bold uppercase tracking-wider text-on-surface-variant">Metas de Categoria</h4>
              {isInitialLoading ? (
                Array.from({ length: 3 }).map((_, i) => (
                  <div key={i} className="space-y-2">
                    <div className="flex justify-between">
                      <Skeleton className="h-3 w-16" />
                      <Skeleton className="h-3 w-20" />
                    </div>
                    <Skeleton className="h-1.5 w-full rounded-full" />
                  </div>
                ))
              ) : (
                budgetSummary.map((goal, i) => (
                  <div key={i} className="space-y-1.5">
                    <div className="flex justify-between text-[11px]">
                      <span className="text-on-surface-variant font-medium">{goal.name}</span>
                      <span className="font-bold">R$ {goal.current.toLocaleString('pt-BR', { maximumFractionDigits: 0 })} / {goal.limit}</span>
                    </div>
                    <div className="h-1 w-full bg-surface-low rounded-full overflow-hidden">
                      <div 
                        className="h-full rounded-full transition-all duration-1000" 
                        style={{ 
                          width: `${Math.min((goal.current / goal.limit) * 100, 100)}%`,
                          backgroundColor: goal.color
                        }} 
                      />
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
