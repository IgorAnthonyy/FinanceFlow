import { useState, useEffect } from 'react';
import { Layout } from './components/layout/Layout';
import { Dashboard } from './pages/Dashboard';
import { Transactions } from './pages/Transactions';
import { Wallet } from './pages/Wallet';
import { Categories } from './pages/Categories';
import { Reports } from './pages/Reports';
import { Profile } from './pages/Profile';
import { TransactionModal } from './components/TransactionModal';
import { CategoryModal } from './components/CategoryModal';
import { FilterModal } from './components/FilterModal';
import { Login } from './pages/Login';
import { getAuthToken, getUserName } from './lib/api';

const pathToTab = (path: string): string => {
  switch (path) {
    case '/':
    case '/dashboard':
      return 'dashboard';
    case '/transactions':
      return 'transactions';
    case '/credit':
      return 'wallet';
    case '/categories':
      return 'categories';
    case '/reports':
      return 'reports';
    case '/profile':
      return 'profile';
    default:
      return 'dashboard';
  }
};

const tabToPath = (tab: string): string => {
  switch (tab) {
    case 'dashboard':
      return '/dashboard';
    case 'transactions':
      return '/transactions';
    case 'wallet':
      return '/credit';
    case 'categories':
      return '/categories';
    case 'reports':
      return '/reports';
    case 'profile':
      return '/profile';
    default:
      return '/dashboard';
  }
};

export default function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userName, setUserName] = useState('');
  const [activeTab, setActiveTab] = useState(() => pathToTab(window.location.pathname));
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isCategoryModalOpen, setIsCategoryModalOpen] = useState(false);
  const [isFilterModalOpen, setIsFilterModalOpen] = useState(false);
  const [filters, setFilters] = useState<any>({});
  const [refreshKey, setRefreshKey] = useState(0);

  const [startDate, setStartDate] = useState(() => {
    const d = new Date();
    d.setMonth(d.getMonth() - 2);
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${d.getFullYear()}-${month}-${day}`;
  });
  const [endDate, setEndDate] = useState(() => {
    const d = new Date();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${d.getFullYear()}-${month}-${day}`;
  });

  useEffect(() => {
    const tokenExists = getAuthToken() !== null;
    setIsAuthenticated(tokenExists);
    if (tokenExists) {
      setUserName(getUserName() || 'Usuário FinanceFlow');
    }
  }, []);

  useEffect(() => {
    const handlePopState = () => {
      setActiveTab(pathToTab(window.location.pathname));
    };
    window.addEventListener('popstate', handlePopState);
    return () => window.removeEventListener('popstate', handlePopState);
  }, []);

  const handleTabChange = (tabId: string) => {
    setActiveTab(tabId);
    const path = tabToPath(tabId);
    window.history.pushState(null, '', path);
  };

  const triggerRefresh = () => {
    setRefreshKey(prev => prev + 1);
  };

  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
    setUserName(getUserName() || 'Usuário FinanceFlow');
  };

  const handleLogout = () => {
    setIsAuthenticated(false);
    handleTabChange('dashboard');
    setUserName('');
  };

  if (!isAuthenticated) {
    return <Login onLoginSuccess={handleLoginSuccess} />;
  }

  const renderContent = () => {
    switch (activeTab) {
      case 'dashboard':
        return <Dashboard key={refreshKey + startDate + endDate} startDate={startDate} endDate={endDate} onOpenModal={() => setIsModalOpen(true)} />;
      case 'transactions':
        return (
          <Transactions 
            key={refreshKey + startDate + endDate} 
            startDate={startDate} 
            endDate={endDate} 
            isFilterModalOpen={isFilterModalOpen}
            setIsFilterModalOpen={setIsFilterModalOpen}
            filters={filters}
            setFilters={setFilters}
          />
        );
      case 'wallet':
        return <Wallet key={refreshKey + startDate + endDate} startDate={startDate} endDate={endDate} />;
      case 'categories':
        return (
          <Categories 
            key={refreshKey + startDate + endDate} 
            startDate={startDate} 
            endDate={endDate} 
            isModalOpen={isCategoryModalOpen}
            setIsModalOpen={setIsCategoryModalOpen}
          />
        );
      case 'reports':
        return <Reports key={refreshKey + startDate + endDate} startDate={startDate} endDate={endDate} />;
      case 'profile':
        return <Profile onLogout={handleLogout} onProfileUpdate={(newName) => setUserName(newName)} />;
      default:
        return <Dashboard key={refreshKey + startDate + endDate} startDate={startDate} endDate={endDate} onOpenModal={() => setIsModalOpen(true)} />;
    }
  };

  return (
    <>
      <Layout 
        activeTab={activeTab} 
        onTabChange={handleTabChange} 
        onOpenModal={() => setIsModalOpen(true)}
        userName={userName}
        startDate={startDate}
        endDate={endDate}
        onStartDateChange={setStartDate}
        onEndDateChange={setEndDate}
      >
        {renderContent()}
      </Layout>

      <TransactionModal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        onSuccess={triggerRefresh}
      />

      <CategoryModal 
        isOpen={isCategoryModalOpen} 
        onClose={() => setIsCategoryModalOpen(false)} 
        onSuccess={triggerRefresh}
      />

      <FilterModal 
        isOpen={isFilterModalOpen} 
        onClose={() => setIsFilterModalOpen(false)} 
        onApplyFilters={setFilters} 
        currentFilters={filters} 
      />
    </>
  );
}
