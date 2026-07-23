import { ReactNode, useState } from 'react';
import { Sidebar } from './Sidebar';
import { TopBar } from './TopBar';
import { MobileNav } from './MobileNav';

interface LayoutProps {
  children: ReactNode;
  activeTab: string;
  onTabChange: (id: string) => void;
  onOpenModal: () => void;
  userName: string;
  startDate: string;
  endDate: string;
  onStartDateChange: (val: string) => void;
  onEndDateChange: (val: string) => void;
}

export function Layout({ children, activeTab, onTabChange, onOpenModal, userName, startDate, endDate, onStartDateChange, onEndDateChange }: LayoutProps) {
  return (
    <div className="min-h-screen flex bg-brand-background text-on-surface relative z-0">
      {/* Sidebar */}
      <Sidebar activeTab={activeTab} onTabChange={onTabChange} userName={userName} />

      <div className="flex-1 flex flex-col min-w-0">
        <TopBar 
          onOpenModal={onOpenModal} 
          startDate={startDate}
          endDate={endDate}
          onStartDateChange={onStartDateChange}
          onEndDateChange={onEndDateChange}
        />
        
        <main className="flex-1 overflow-y-auto pb-24 lg:pb-8">
          <div className="max-w-7xl mx-auto p-6 lg:p-8">
            {children}
          </div>
        </main>
      </div>

      {}
      <MobileNav activeTab={activeTab} onTabChange={onTabChange} onOpenModal={onOpenModal} />
      
      {}
      <div className="fixed top-0 right-0 w-[500px] h-[500px] bg-brand-primary/5 blur-[120px] rounded-full -z-10 pointer-events-none" />
      <div className="fixed bottom-0 left-0 w-[400px] h-[400px] bg-brand-secondary/5 blur-[100px] rounded-full -z-10 pointer-events-none" />
    </div>
  );
}
