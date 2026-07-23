import React, { useState, useEffect } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Skeleton } from '../components/ui/Skeleton';
import { 
  TrendingUp, 
  TrendingDown, 
  Wallet as WalletIcon, 
  ShoppingBag,
  Briefcase,
  Car,
  Gamepad2,
  HelpCircle,
  Home,
  HeartPulse,
  GraduationCap,
  Utensils,
  ChevronDown
} from 'lucide-react';
import { cn } from '../lib/utils';
import { 
  AreaChart, 
  Area, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell
} from 'recharts';
import { api, getUserId } from '../lib/api';

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

  let color = cat?.color || '#64748b';
  if (color === 'orange') color = '#f97316';
  else if (color === 'green') color = '#16a34a';
  else if (color === 'blue') color = '#2563eb';
  else if (color === 'purple') color = '#9333ea';
  else if (color === 'red') color = '#e11d48';
  else if (color === 'yellow') color = '#eab308';

  return { icon: IconComponent, color };
};

interface DashboardProps {
  onOpenModal: () => void;
  startDate: string;
  endDate: string;
  key?: string;
}

export function Dashboard({ onOpenModal, startDate, endDate }: DashboardProps) {
  const [loading, setLoading] = useState(true);
  const [evolutionLoading, setEvolutionLoading] = useState(false);
  const [wallet, setWallet] = useState<any>(null);
  const [categories, setCategories] = useState<any[]>([]);
  const [report, setReport] = useState<any>(null);
  const [evolutionData, setEvolutionData] = useState<any[]>([]);
  const userId = getUserId();

  useEffect(() => {
    if (!userId) return;

    const loadData = async () => {
      try {
        setLoading(true);
        setEvolutionLoading(true);
        const [walletData, cats, summary, expenses, latest, evolution] = await Promise.all([
          api.getWallet(userId),
          api.getCategories(),
          api.getMonthlySummary(startDate, endDate),
          api.getExpensesByCategory(startDate, endDate),
          api.getLatestTransactions(startDate, endDate),
          api.getDailyEvolution(startDate, endDate),
        ]);

        setWallet(walletData);
        setCategories(cats || []);
        setReport({
          currentMonthIncome: summary.income,
          currentMonthExpense: summary.expense,
          currentMonthSavings: summary.savings,
          expensesByCategory: expenses,
          latestTransactions: latest
        });
        setEvolutionData(evolution || []);
      } catch (error) {
        console.error('Error loading dashboard data:', error);
      } finally {
        setLoading(false);
        setEvolutionLoading(false);
      }
    };

    loadData();
  }, [userId, startDate, endDate]);

  const balance = wallet ? wallet.totalBalance : 0;
  const income = report ? report.currentMonthIncome : 0;
  const expense = report ? report.currentMonthExpense : 0;
  const savings = report ? report.currentMonthSavings : 0;

  const STAT_CARDS = [
    { label: 'Saldo Atual', value: `R$ ${balance.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`, change: '+2.4%', trend: 'up', icon: WalletIcon, color: 'text-brand-primary' },
    { label: 'Receitas', value: `R$ ${income.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`, change: '+12%', trend: 'up', icon: TrendingUp, color: 'text-brand-secondary' },
    { label: 'Despesas', value: `R$ ${expense.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`, change: '-5%', trend: 'down', icon: TrendingDown, color: 'text-brand-error' },
    { label: 'Economia', value: `R$ ${savings.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`, change: '+8.1%', trend: 'up', icon: TrendingUp, color: 'text-brand-tertiary' },
  ];

  const finalCategoryData = report && report.expensesByCategory && report.expensesByCategory.length > 0
    ? report.expensesByCategory.map((c: any) => {
        const cat = categories.find((catItem: any) => catItem.name === c.categoryName);
        const details = getCategoryDetails(cat || { name: c.categoryName });
        return {
          name: c.categoryName,
          value: c.percentage,
          amount: c.amount,
          color: details.color
        };
      })
    : [
        { name: 'Nenhuma despesa', value: 100, amount: 0, color: '#555' }
      ];

  let tempBalance = balance;
  const areaChartData = evolutionData && evolutionData.length > 0
    ? [...evolutionData].reverse().map((m: any) => {
        const item = {
          name: m.monthName,
          saldo: Math.max(0, tempBalance),
          gastos: m.expense
        };
        tempBalance = tempBalance - m.income + m.expense;
        return item;
      }).reverse()
    : [];

  const latestTransactions = report && report.latestTransactions ? report.latestTransactions : [];

  return (
    <div className="space-y-8 animate-in fade-in duration-700">
      {}
      <section className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Dashboard Overview</h2>
          <p className="text-on-surface-variant mt-1">Bem-vindo de volta! Aqui está o seu resumo financeiro.</p>
        </div>
        <Button onClick={onOpenModal} className="shrink-0">
          Nova Transação
        </Button>
      </section>

      {}
      <section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {STAT_CARDS.map((stat, i) => (
          <Card key={i} glow={i === 0}>
            <div className="flex justify-between items-start mb-4">
              <div className={`p-2 rounded-lg bg-surface-low ${stat.color}`}>
                <stat.icon size={20} />
              </div>
              {loading ? (
                <Skeleton className="h-4 w-10" />
              ) : (
                <span className={`text-xs font-bold ${stat.trend === 'up' ? 'text-brand-primary' : 'text-brand-error'}`}>
                  {stat.change}
                </span>
              )}
            </div>
            <div>
              <p className="text-sm font-medium text-on-surface-variant/70">{stat.label}</p>
              {loading ? (
                <Skeleton className="h-8 w-32 mt-1" />
              ) : (
                <h3 className="text-2xl font-bold mt-1 tracking-tight">{stat.value}</h3>
              )}
            </div>
          </Card>
        ))}
      </section>

      {}
      <section className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Evolução do Patrimônio */}
        <Card className="lg:col-span-2 flex flex-col">
          <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4 mb-8">
            <div>
              <h3 className="text-lg font-bold">Evolução do Patrimônio</h3>
              <p className="text-xs text-on-surface-variant">Saldo vs Gastos</p>
            </div>
            <div className="flex flex-wrap items-center gap-4">
              <div className="flex gap-4">
                <div className="flex items-center gap-1.5 text-xs text-on-surface-variant font-medium">
                  <div className="w-2 h-2 rounded-full bg-brand-primary" /> Saldo
                </div>
                <div className="flex items-center gap-1.5 text-xs text-on-surface-variant font-medium">
                  <div className="w-2 h-2 rounded-full bg-brand-error" /> Gastos
                </div>
              </div>
            </div>
          </div>
          {loading || evolutionLoading ? (
            <div className="h-64 w-full flex items-center justify-center bg-surface-low rounded-xl">
              <Skeleton className="h-full w-full" />
            </div>
          ) : (
            <div className="h-64 w-full">
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={areaChartData}>
                  <defs>
                    <linearGradient id="colorSaldo" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#ff6b00" stopOpacity={0.2}/>
                      <stop offset="95%" stopColor="#ff6b00" stopOpacity={0}/>
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgba(0, 0, 0, 0.05)" vertical={false} />
                  <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{fontSize: 10, fill: '#8c9199'}} />
                  <Tooltip 
                    contentStyle={{ backgroundColor: '#ffffff', border: '1px solid #e1e2e5', borderRadius: '8px', color: '#1a1c1e' }}
                    itemStyle={{ fontSize: '12px' }}
                  />
                  <Area type="monotone" dataKey="saldo" stroke="#ff6b00" fillOpacity={1} fill="url(#colorSaldo)" strokeWidth={3} />
                  <Area type="monotone" dataKey="gastos" stroke="#ef4444" fill="transparent" strokeDasharray="5 5" strokeWidth={2} />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          )}
        </Card>

        {}
        <Card className="flex flex-col">
          <h3 className="text-lg font-bold mb-8">Gasto por Categoria</h3>
          {loading ? (
            <div className="relative flex-1 flex flex-col items-center justify-center min-h-[300px] w-full">
              <Skeleton className="w-40 h-40 rounded-full" />
              <div className="w-full space-y-3 mt-6">
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-full" />
              </div>
            </div>
          ) : (
            <div className="relative flex-1 flex flex-col items-center justify-center">
              <div className="h-48 w-full flex items-center justify-center">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={finalCategoryData}
                      cx="50%"
                      cy="50%"
                      innerRadius={60}
                      outerRadius={80}
                      paddingAngle={5}
                      dataKey="value"
                    >
                      {finalCategoryData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
                <div className="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
                  <span className="text-[10px] uppercase tracking-widest text-on-surface-variant">Total Gastos</span>
                  <span className="text-lg font-bold">R$ {expense.toLocaleString('pt-BR', { maximumFractionDigits: 0 })}</span>
                </div>
              </div>
              <div className="w-full space-y-3 mt-6">
                {finalCategoryData.map((item, i) => (
                  <div key={i} className="flex justify-between items-center px-2">
                    <div className="flex items-center gap-2">
                      <div className="w-3 h-3 rounded-sm" style={{backgroundColor: item.color}} />
                      <span className="text-xs text-on-surface-variant">{item.name}</span>
                    </div>
                    <span className="text-xs font-bold">{item.value}%</span>
                  </div>
                ))}
              </div>
            </div>
          )}
        </Card>
      </section>

      {}
      <section className="space-y-4">
        <div className="flex justify-between items-center">
          <h3 className="text-xl font-bold tracking-tight">Últimas Movimentações</h3>
        </div>
        <Card className="p-2">
          {loading ? (
            <div className="divide-y divide-outline-variant">
              {Array.from({ length: 4 }).map((_, i) => (
                <div key={i} className="flex items-center justify-between p-4">
                  <div className="flex items-center gap-4 flex-1">
                    <Skeleton className="w-12 h-12 rounded-full shrink-0" />
                    <div className="space-y-2 flex-1">
                      <Skeleton className="h-4 w-1/3" />
                      <Skeleton className="h-3 w-1/4" />
                    </div>
                  </div>
                  <div className="text-right space-y-2">
                    <Skeleton className="h-4 w-20 ml-auto" />
                    <Skeleton className="h-3 w-12 ml-auto" />
                  </div>
                </div>
              ))}
            </div>
          ) : latestTransactions.length === 0 ? (
            <div className="text-center py-8 text-on-surface-variant text-sm">
              Nenhuma transação registrada.
            </div>
          ) : (
            <div className="divide-y divide-outline-variant">
              {latestTransactions.map((tx: any) => {
                const cat = categories.find((c: any) => c.id === tx.categoryId);
                const catName = cat ? cat.name : 'Outros';
                const details = getCategoryDetails(cat || { name: catName });
                const Icon = details.icon;
                const typeLabel = tx.type === 0 ? 'Crédito' : 'Débito';
                const dateFormatted = new Date(tx.transactionDate).toLocaleDateString('pt-BR');

                return (
                  <div key={tx.id} className="flex items-center justify-between p-4 hover:bg-surface-low transition-colors rounded-lg group cursor-pointer">
                    <div className="flex items-center gap-4">
                      <div className="w-12 h-12 flex items-center justify-center bg-surface-low rounded-full transition-transform group-hover:scale-110">
                        <Icon size={20} style={{ color: details.color }} />
                      </div>
                      <div>
                        <p className="font-semibold text-sm">{tx.title}</p>
                        <p className="text-[10px] text-on-surface-variant/60">{dateFormatted} • {catName}</p>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className={cn("font-bold text-sm", tx.type === 0 ? 'text-brand-primary' : 'text-on-surface')}>
                        {tx.type === 0 ? '+' : '-'} R$ {tx.amount.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                      </p>
                      <span className={cn(
                        "text-[10px] px-2 py-0.5 rounded-full",
                        tx.type === 0 ? "bg-brand-primary/10 text-brand-primary" : "bg-surface-low text-on-surface-variant"
                      )}>
                        {typeLabel}
                      </span>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </Card>
      </section>
    </div>
  );
}
