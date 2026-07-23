import { 
  LayoutDashboard, 
  Receipt, 
  Wallet, 
  Layers, 
  BarChart3, 
  User, 
  Settings 
} from 'lucide-react';
import { NavItem } from './types';

export const NAVIGATION_ITEMS: NavItem[] = [
  { id: 'dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { id: 'transactions', label: 'Transações', icon: Receipt },
  { id: 'wallet', label: 'Carteira', icon: Wallet },
  { id: 'categories', label: 'Categorias', icon: Layers },
  { id: 'reports', label: 'Relatórios', icon: BarChart3 },
  { id: 'profile', label: 'Perfil', icon: User },
];

export const MOCK_TRANSACTIONS = [
  {
    id: '1',
    title: 'Supermercado Premium',
    category: 'Alimentação',
    amount: 450.20,
    date: 'Hoje, 14:20',
    type: 'expense' as const,
    status: 'completed' as const,
  },
  {
    id: '2',
    title: 'Salário Corporativo',
    category: 'Receita',
    amount: 12000.00,
    date: 'Ontem',
    type: 'income' as const,
    status: 'completed' as const,
  },
  {
    id: '3',
    title: 'Posto Shell',
    category: 'Transporte',
    amount: 320.00,
    date: 'Ontem',
    type: 'expense' as const,
    status: 'completed' as const,
  },
  {
    id: '4',
    title: 'Aluguel Apartamento',
    category: 'Moradia',
    amount: 3800.00,
    date: '05 Set 2023',
    type: 'expense' as const,
    status: 'completed' as const,
  },
];
