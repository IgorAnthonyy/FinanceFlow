import { NAVIGATION_ITEMS } from '../../constants';
import { cn } from '../../lib/utils';
import { motion } from 'motion/react';

interface SidebarProps {
  activeTab: string;
  onTabChange: (id: string) => void;
  userName: string;
}

export function Sidebar({ activeTab, onTabChange, userName }: SidebarProps) {
  return (
    <aside className="hidden lg:flex flex-col w-72 h-screen sticky top-0 bg-surface-lowest/40 backdrop-blur-3xl border-r border-outline-variant py-8 overflow-y-auto shrink-0 z-40">
      <div className="px-8 mb-12 flex items-center gap-3">
        <div className="w-10 h-10 bg-brand-primary rounded-lg flex items-center justify-center">
          <div className="w-6 h-6 border-4 border-brand-background rounded-sm rotate-45" />
        </div>
        <div>
          <h1 className="text-xl font-bold text-brand-primary leading-none">FinanceFlow</h1>
          <p className="text-[10px] uppercase tracking-[0.2em] text-on-surface-variant font-medium mt-1">Premium Wealth</p>
        </div>
      </div>

      <nav className="flex-1 space-y-1">
        {NAVIGATION_ITEMS.map((item) => {
          const isActive = activeTab === item.id;
          return (
            <button
              key={item.id}
              onClick={() => onTabChange(item.id)}
              className={cn(
                "w-full flex items-center gap-3 px-8 py-3 transition-all relative group",
                isActive ? "text-brand-primary font-bold" : "text-on-surface-variant hover:text-on-surface"
              )}
            >
              {isActive && (
                <motion.div
                  layoutId="activeNav"
                  className="absolute left-0 top-0 bottom-0 w-1 bg-brand-primary"
                  transition={{ type: "spring", stiffness: 300, damping: 30 }}
                />
              )}
              {isActive && (
                <div className="absolute inset-0 bg-brand-primary/5" />
              )}
              <item.icon size={20} className={cn(isActive && "text-brand-primary")} />
              <span className="text-sm">{item.label}</span>
            </button>
          );
        })}
      </nav>

      <div className="px-6 mt-auto">
        <button
          onClick={() => onTabChange('profile')}
          className="w-full text-left glass-card p-4 rounded-xl flex items-center gap-3 hover:bg-surface-high/50 transition-colors cursor-pointer"
        >
          <div className="w-10 h-10 rounded-full bg-surface-highest border border-outline-variant overflow-hidden shrink-0">
            <img 
              src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?q=80&w=2070&auto=format&fit=crop" 
              alt="Profile" 
              className="w-full h-full object-cover"
            />
          </div>
          <div className="overflow-hidden flex-1 min-w-0">
            <p className="text-sm font-bold text-on-surface truncate">{userName}</p>
          </div>
        </button>
      </div>
    </aside>
  );
}
