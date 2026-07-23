import { useState } from 'react';
import { Plus, Calendar, ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '../ui/Button';
import { cn } from '../../lib/utils';

interface TopBarProps {
  onOpenModal: () => void;
  startDate: string;
  endDate: string;
  onStartDateChange: (val: string) => void;
  onEndDateChange: (val: string) => void;
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '';
  const [year, month, day] = dateStr.split('-');
  return `${day}/${month}/${year}`;
};

const MONTH_NAMES = [
  'janeiro', 'fevereiro', 'março', 'abril', 'maio', 'junho',
  'julho', 'agosto', 'setembro', 'outubro', 'novembro', 'dezembro'
];

const getDaysInMonth = (year: number, month: number) => {
  const firstDayIndex = new Date(year, month, 1).getDay(); // Sunday = 0
  const startDay = firstDayIndex === 0 ? 6 : firstDayIndex - 1; // Shift to Mon = 0
  
  const totalDays = new Date(year, month + 1, 0).getDate();
  const prevMonthTotalDays = new Date(year, month, 0).getDate();
  
  const days: { dateStr: string; dayNum: number; isCurrentMonth: boolean }[] = [];
  
  for (let i = startDay - 1; i >= 0; i--) {
    const prevDay = prevMonthTotalDays - i;
    const prevMonth = month === 0 ? 11 : month - 1;
    const prevYear = month === 0 ? year - 1 : year;
    const prevMonthStr = String(prevMonth + 1).padStart(2, '0');
    days.push({
      dateStr: `${prevYear}-${prevMonthStr}-${String(prevDay).padStart(2, '0')}`,
      dayNum: prevDay,
      isCurrentMonth: false
    });
  }
  
  for (let i = 1; i <= totalDays; i++) {
    const monthStr = String(month + 1).padStart(2, '0');
    days.push({
      dateStr: `${year}-${monthStr}-${String(i).padStart(2, '0')}`,
      dayNum: i,
      isCurrentMonth: true
    });
  }
  
  return days;
};

export function TopBar({ onOpenModal, startDate, endDate, onStartDateChange, onEndDateChange }: TopBarProps) {
  const [openPicker, setOpenPicker] = useState<'start' | 'end' | null>(null);
  const [viewingMonth, setViewingMonth] = useState(() => new Date());

  const handleDayClick = (dateStr: string) => {
    if (openPicker === 'start') {
      onStartDateChange(dateStr);
    } else if (openPicker === 'end') {
      onEndDateChange(dateStr);
    }
    setOpenPicker(null);
  };

  const navigateMonth = (direction: 'prev' | 'next') => {
    setViewingMonth(prev => {
      const nextDate = new Date(prev);
      if (direction === 'prev') {
        nextDate.setMonth(nextDate.getMonth() - 1);
      } else {
        nextDate.setMonth(nextDate.getMonth() + 1);
      }
      return nextDate;
    });
  };

  const openCalendar = (type: 'start' | 'end') => {
    const activeDateStr = type === 'start' ? startDate : endDate;
    if (activeDateStr) {
      const [y, m, d] = activeDateStr.split('-').map(Number);
      setViewingMonth(new Date(y, m - 1, d));
    } else {
      setViewingMonth(new Date());
    }
    setOpenPicker(type);
  };

  const daysGrid = getDaysInMonth(viewingMonth.getFullYear(), viewingMonth.getMonth());

  return (
    <header className="sticky top-0 z-50 w-full h-20 bg-brand-background/60 backdrop-blur-md border-b border-outline-variant flex justify-between items-center px-8 shrink-0">
      <div className="flex items-center gap-4">
        {/* Date Initial */}
        <div className="relative">
          <button 
            onClick={() => openPicker === 'start' ? setOpenPicker(null) : openCalendar('start')}
            className={cn(
              "flex items-center gap-2 bg-surface-low border rounded-xl px-3 py-1.5 text-xs text-on-surface transition-colors cursor-pointer group min-w-[130px] justify-start outline-none",
              openPicker === 'start' ? "border-brand-primary/80" : "border-outline-variant hover:border-brand-primary/50"
            )}
          >
            <Calendar size={14} className={cn("transition-colors", openPicker === 'start' ? "text-brand-primary" : "text-on-surface-variant group-hover:text-brand-primary")} />
            <span className="text-[10px] font-bold text-on-surface-variant/60 uppercase">De:</span>
            <span className="font-semibold text-xs text-on-surface select-none">
              {formatDate(startDate)}
            </span>
          </button>

          {openPicker === 'start' && (
            <>
              <div className="fixed inset-0 z-40" onClick={() => setOpenPicker(null)} />
              <div className="absolute top-12 left-0 bg-surface/95 backdrop-blur-2xl border border-outline-variant rounded-2xl shadow-2xl z-50 p-4 w-[280px] animate-in fade-in slide-in-from-top-2 duration-200">
                <div className="flex items-center justify-between mb-4">
                  <button 
                    onClick={() => navigateMonth('prev')}
                    className="p-1 hover:bg-surface-high rounded-full transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
                  >
                    <ChevronLeft size={16} />
                  </button>
                  <span className="text-xs font-bold capitalize text-on-surface select-none">
                    {MONTH_NAMES[viewingMonth.getMonth()]} de {viewingMonth.getFullYear()}
                  </span>
                  <button 
                    onClick={() => navigateMonth('next')}
                    className="p-1 hover:bg-surface-high rounded-full transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
                  >
                    <ChevronRight size={16} />
                  </button>
                </div>

                <div className="grid grid-cols-7 gap-1 text-[10px] uppercase font-bold text-on-surface-variant/50 text-center mb-2 select-none">
                  <span>seg</span>
                  <span>ter</span>
                  <span>qua</span>
                  <span>qui</span>
                  <span>sex</span>
                  <span>sáb</span>
                  <span>dom</span>
                </div>

                <div className="grid grid-cols-7 gap-1">
                  {daysGrid.map((day, idx) => {
                    const isSelected = startDate === day.dateStr;
                    return (
                      <button
                        key={idx}
                        onClick={() => handleDayClick(day.dateStr)}
                        className={cn(
                          "h-8 w-8 text-xs font-semibold rounded-lg flex items-center justify-center transition-all cursor-pointer",
                          !day.isCurrentMonth && "text-on-surface-variant/20 hover:bg-surface-high/20",
                          day.isCurrentMonth && !isSelected && "text-on-surface hover:bg-surface-high",
                          isSelected && "bg-brand-primary text-brand-background font-bold shadow-md"
                        )}
                      >
                        {day.dayNum}
                      </button>
                    );
                  })}
                </div>

                <div className="flex justify-between items-center mt-4 pt-3 border-t border-outline-variant">
                  <button
                    onClick={() => {
                      const todayStr = new Date().toISOString().split('T')[0];
                      handleDayClick(todayStr);
                    }}
                    className="text-[10px] font-bold text-brand-primary hover:underline cursor-pointer"
                  >
                    Hoje
                  </button>
                  <button
                    onClick={() => setOpenPicker(null)}
                    className="text-[10px] font-bold text-on-surface-variant hover:text-on-surface cursor-pointer"
                  >
                    Fechar
                  </button>
                </div>
              </div>
            </>
          )}
        </div>

        {/* Date Final */}
        <div className="relative">
          <button 
            onClick={() => openPicker === 'end' ? setOpenPicker(null) : openCalendar('end')}
            className={cn(
              "flex items-center gap-2 bg-surface-low border rounded-xl px-3 py-1.5 text-xs text-on-surface transition-colors cursor-pointer group min-w-[130px] justify-start outline-none",
              openPicker === 'end' ? "border-brand-primary/80" : "border-outline-variant hover:border-brand-primary/50"
            )}
          >
            <Calendar size={14} className={cn("transition-colors", openPicker === 'end' ? "text-brand-primary" : "text-on-surface-variant group-hover:text-brand-primary")} />
            <span className="text-[10px] font-bold text-on-surface-variant/60 uppercase">Até:</span>
            <span className="font-semibold text-xs text-on-surface select-none">
              {formatDate(endDate)}
            </span>
          </button>

          {openPicker === 'end' && (
            <>
              <div className="fixed inset-0 z-40" onClick={() => setOpenPicker(null)} />
              <div className="absolute top-12 left-0 bg-surface/95 backdrop-blur-2xl border border-outline-variant rounded-2xl shadow-2xl z-50 p-4 w-[280px] animate-in fade-in slide-in-from-top-2 duration-200">
                <div className="flex items-center justify-between mb-4">
                  <button 
                    onClick={() => navigateMonth('prev')}
                    className="p-1 hover:bg-surface-high rounded-full transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
                  >
                    <ChevronLeft size={16} />
                  </button>
                  <span className="text-xs font-bold capitalize text-on-surface select-none">
                    {MONTH_NAMES[viewingMonth.getMonth()]} de {viewingMonth.getFullYear()}
                  </span>
                  <button 
                    onClick={() => navigateMonth('next')}
                    className="p-1 hover:bg-surface-high rounded-full transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
                  >
                    <ChevronRight size={16} />
                  </button>
                </div>

                <div className="grid grid-cols-7 gap-1 text-[10px] uppercase font-bold text-on-surface-variant/50 text-center mb-2 select-none">
                  <span>seg</span>
                  <span>ter</span>
                  <span>qua</span>
                  <span>qui</span>
                  <span>sex</span>
                  <span>sáb</span>
                  <span>dom</span>
                </div>

                <div className="grid grid-cols-7 gap-1">
                  {daysGrid.map((day, idx) => {
                    const isSelected = endDate === day.dateStr;
                    return (
                      <button
                        key={idx}
                        onClick={() => handleDayClick(day.dateStr)}
                        className={cn(
                          "h-8 w-8 text-xs font-semibold rounded-lg flex items-center justify-center transition-all cursor-pointer",
                          !day.isCurrentMonth && "text-on-surface-variant/20 hover:bg-surface-high/20",
                          day.isCurrentMonth && !isSelected && "text-on-surface hover:bg-surface-high",
                          isSelected && "bg-brand-primary text-brand-background font-bold shadow-md"
                        )}
                      >
                        {day.dayNum}
                      </button>
                    );
                  })}
                </div>

                <div className="flex justify-between items-center mt-4 pt-3 border-t border-outline-variant">
                  <button
                    onClick={() => {
                      const todayStr = new Date().toISOString().split('T')[0];
                      handleDayClick(todayStr);
                    }}
                    className="text-[10px] font-bold text-brand-primary hover:underline cursor-pointer"
                  >
                    Hoje
                  </button>
                  <button
                    onClick={() => setOpenPicker(null)}
                    className="text-[10px] font-bold text-on-surface-variant hover:text-on-surface cursor-pointer"
                  >
                    Fechar
                  </button>
                </div>
              </div>
            </>
          )}
        </div>
      </div>

      <div className="flex items-center gap-4">
        <Button className="hidden md:flex" onClick={onOpenModal}>
          <Plus size={18} />
          Nova Transação
        </Button>

        <div className="h-10 w-10 rounded-full border border-outline-variant overflow-hidden lg:hidden">
          <img
            src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?q=80&w=2070&auto=format&fit=crop"
            alt="Profile"
            className="w-full h-full object-cover"
          />
        </div>
      </div>
    </header>
  );
}
