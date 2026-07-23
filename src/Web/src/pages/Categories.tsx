import { useState, useEffect } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Utensils, Car, Gamepad2, HeartPulse, GraduationCap, TrendingUp, HelpCircle, Plus, Briefcase, ShoppingBag } from 'lucide-react';
import { api, getUserId } from '../lib/api';
import { CategoryModal } from '../components/CategoryModal';
import { Skeleton } from '../components/ui/Skeleton';


const getCategoryDetails = (cat: any) => {
  let IconComponent = HelpCircle;
  if (cat.icon) {
    switch (cat.icon.toLowerCase()) {
      case 'utensils':
        IconComponent = Utensils;
        break;
      case 'car':
        IconComponent = Car;
        break;
      case 'gamepad':
        IconComponent = Gamepad2;
        break;
      case 'briefcase':
      case 'trendingup':
        IconComponent = TrendingUp;
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
    switch (cat.name) {
      case 'Alimentação':
        IconComponent = Utensils;
        break;
      case 'Salário':
        IconComponent = TrendingUp;
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

  let color = cat.color || '#6b7280';
  if (color === 'orange') color = '#f97316';
  else if (color === 'green') color = '#16a34a';
  else if (color === 'blue') color = '#2563eb';
  else if (color === 'purple') color = '#9333ea';
  else if (color === 'red') color = '#e11d48';
  else if (color === 'yellow') color = '#eab308';

  return { icon: IconComponent, color };
};

export function Categories({ startDate, endDate, isModalOpen, setIsModalOpen }: { startDate?: string; endDate?: string; isModalOpen: boolean; setIsModalOpen: (open: boolean) => void; key?: string }) {
  const [loading, setLoading] = useState(true);
  const [categories, setCategories] = useState<any[]>([]);
  const [transactions, setTransactions] = useState<any[]>([]);
  const userId = getUserId();

  const loadData = async () => {
    if (!userId) return;
    try {
      setLoading(true);
      const cats = await api.getCategories();
      setCategories(cats || []);

      const txs = await api.getTransactions(userId);
      setTransactions(txs || []);
    } catch (err) {
      console.error('Error loading categories page:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [userId]);



  let totalExpense = 0;
  const categorySums: { [key: string]: number } = {};

  transactions.forEach((tx: any) => {
    if (tx.type === 1) {
      const txDate = tx.transactionDate ? tx.transactionDate.substring(0, 10) : '';
      if (startDate && txDate < startDate) return;
      if (endDate && txDate > endDate) return;

      categorySums[tx.categoryId] = (categorySums[tx.categoryId] || 0) + tx.amount;
      totalExpense += tx.amount;
    }
  });

  const categoryList = categories.map((cat: any) => {
    const amount = categorySums[cat.id] || 0;
    const percentage = totalExpense > 0 ? Math.round((amount / totalExpense) * 100) : 0;
    const details = getCategoryDetails(cat);
    
    return {
      id: cat.id,
      name: cat.name,
      amount,
      percentage,
      color: details.color,
      icon: details.icon
    };
  });

  return (
    <div className="space-y-8 animate-in fade-in duration-700">
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Categorias</h2>
          <p className="text-on-surface-variant mt-1">Gerencie a distribuição do seu capital por categoria.</p>
        </div>
        <Button variant="outline" size="sm" onClick={() => setIsModalOpen(true)}>
          <Plus size={16} />
          Nova Categoria
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {loading ? (
          Array.from({ length: 6 }).map((_, i) => (
            <Card key={i} className="flex flex-col justify-between h-48">
              <div>
                <div className="flex justify-between items-start mb-4">
                  <Skeleton className="w-12 h-12 rounded-xl shrink-0" />
                  <Skeleton className="w-10 h-5 rounded-full" />
                </div>
                <Skeleton className="h-6 w-2/3 mb-2" />
                <Skeleton className="h-4 w-1/3" />
              </div>
              <Skeleton className="h-1.5 w-full rounded-full mt-4" />
            </Card>
          ))
        ) : (
          categoryList.map((cat) => {
          const Icon = cat.icon;
          return (
            <Card key={cat.id} className="hover:bg-surface-low transition-all group flex flex-col justify-between h-48">
              <div>
                <div className="flex justify-between items-start mb-4">
                  <div className="p-3 rounded-xl bg-surface-low" style={{ color: cat.color }}>
                    <Icon size={24} />
                  </div>
                  <span className="text-[10px] font-bold px-2 py-0.5 rounded-full bg-surface-low text-on-surface-variant">
                    {cat.percentage}%
                  </span>
                </div>
                <h4 className="text-xl font-bold text-on-surface">{cat.name}</h4>
                <p className="text-sm text-on-surface-variant mt-1">
                  R$ {cat.amount.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                </p>
              </div>
              <div className="mt-4 h-1.5 w-full bg-surface-low rounded-full overflow-hidden">
                <div 
                  className="h-full rounded-full transition-all duration-1000" 
                  style={{ width: `${cat.percentage}%`, backgroundColor: cat.color }} 
                />
              </div>
            </Card>
          );
        }))}
      </div>
    </div>
  );
}
