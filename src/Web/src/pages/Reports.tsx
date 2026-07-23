import React, { useState, useEffect } from 'react';
import { Card } from '../components/ui/Card';
import { Skeleton } from '../components/ui/Skeleton';
import { 
  AreaChart, 
  Area, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  BarChart,
  Bar
} from 'recharts';
import { api } from '../lib/api';

interface ReportsProps {
  startDate: string;
  endDate: string;
  key?: string;
}

export function Reports({ startDate, endDate }: ReportsProps) {
  const [report, setReport] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadReport = async () => {
      try {
        setLoading(true);
        const data = await api.getMonthlyEvolution(startDate, endDate);
        setReport(data);
      } catch (err) {
        console.error('Error loading reports:', err);
      } finally {
        setLoading(false);
      }
    };
    loadReport();
  }, [startDate, endDate]);

  const chartData = report
    ? report.map((m: any) => ({
        month: m.monthName,
        income: m.income,
        expense: m.expense
      }))
    : [];

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Relatórios Analíticos</h2>
          <p className="text-on-surface-variant mt-1">Dados consolidados do seu comportamento financeiro.</p>
        </div>
      </div>

      {loading ? (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card className="h-96 flex flex-col justify-between p-8">
            <div className="space-y-4">
              <Skeleton className="h-6 w-1/3" />
              <Skeleton className="h-4 w-1/4" />
            </div>
            <Skeleton className="h-56 w-full" />
          </Card>
          <Card className="h-96 flex flex-col justify-between p-8">
            <div className="space-y-4">
              <Skeleton className="h-6 w-1/3" />
              <Skeleton className="h-4 w-1/4" />
            </div>
            <Skeleton className="h-56 w-full" />
          </Card>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card className="h-96">
            <h3 className="text-lg font-bold mb-8">Receitas vs Despesas</h3>
            <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgba(0, 0, 0, 0.05)" vertical={false} />
                  <XAxis dataKey="month" axisLine={false} tickLine={false} tick={{fontSize: 10, fill: '#8c9199'}} />
                  <Tooltip 
                    cursor={{fill: 'rgba(0, 0, 0, 0.02)'}}
                    contentStyle={{ backgroundColor: '#ffffff', border: '1px solid #e1e2e5', borderRadius: '8px', color: '#1a1c1e' }}
                  />
                  <Bar dataKey="income" fill="#ff6b00" radius={[4, 4, 0, 0]} />
                  <Bar dataKey="expense" fill="#ef4444" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </Card>

          <Card className="h-96">
            <h3 className="text-lg font-bold mb-8">Fluxo de Caixa</h3>
            <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={chartData}>
                  <defs>
                    <linearGradient id="colorFlow" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#ffa726" stopOpacity={0.3}/>
                      <stop offset="95%" stopColor="#ffa726" stopOpacity={0}/>
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgba(0, 0, 0, 0.05)" vertical={false} />
                  <XAxis dataKey="month" axisLine={false} tickLine={false} tick={{fontSize: 10, fill: '#8c9199'}} />
                  <Tooltip 
                    contentStyle={{ backgroundColor: '#ffffff', border: '1px solid #e1e2e5', borderRadius: '8px', color: '#1a1c1e' }}
                  />
                  <Area type="monotone" dataKey="income" stroke="#ffa726" fillOpacity={1} fill="url(#colorFlow)" strokeWidth={3} />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          </Card>
        </div>
      )}
    </div>
  );
}
