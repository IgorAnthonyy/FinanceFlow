import { LayoutDashboard, Receipt, Wallet, User, Plus } from 'lucide-react';
import { cn } from '../../lib/utils';

interface MobileNavProps {
  activeTab: string;
  onTabChange: (id: string) => void;
  onOpenModal: () => void;
}

export function MobileNav({ activeTab, onTabChange, onOpenModal }: MobileNavProps) {
  const items = [
    { id: 'dashboard', icon: LayoutDashboard, label: 'Início' },
    { id: 'transactions', icon: Receipt, label: 'Transações' },
    { id: 'add', icon: Plus, label: '', primary: true },
    { id: 'wallet', icon: Wallet, label: 'Carteira' },
    { id: 'profile', icon: User, label: 'Perfil' },
  ];

  return (
    <nav className="lg:hidden fixed bottom-0 left-0 right-0 h-20 bg-surface-lowest/80 backdrop-blur-2xl border-t border-outline-variant flex items-center justify-around px-4 pb-4 z-50">
      {items.map((item) => {
        if (item.primary) {
          return (
            <button 
              key={item.id}
              onClick={onOpenModal}
              className="w-14 h-14 bg-brand-primary text-brand-background rounded-full flex items-center justify-center relative -top-6 z-10 shadow-xl shadow-brand-primary/30 active:scale-90 transition-transform"
            >
              <Plus size={28} />
            </button>
          );
        }

        const isActive = activeTab === item.id;
        return (
          <button
            key={item.id}
            onClick={() => onTabChange(item.id)}
            className={cn(
              "flex flex-col items-center gap-1 transition-all",
              isActive ? "text-brand-primary" : "text-on-surface-variant hover:text-on-surface"
            )}
          >
            <item.icon size={22} className={cn(isActive && "fill-brand-primary/20")} />
            <span className="text-[10px] font-medium">{item.label}</span>
          </button>
        );
      })}
    </nav>
  );
}
