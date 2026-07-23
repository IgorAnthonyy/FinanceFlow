import { LucideIcon } from 'lucide-react';

export type TransactionType = 'income' | 'expense';

export interface Transaction {
  id: string;
  title: string;
  category: string;
  amount: number;
  date: string;
  type: TransactionType;
  status: 'completed' | 'processing' | 'scheduled';
}

export interface Account {
  id: string;
  name: string;
  balance: number;
  type: string;
  color: string;
  icon: string;
}

export interface NavItem {
  label: string;
  icon: LucideIcon;
  id: string;
}
