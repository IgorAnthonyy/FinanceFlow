import { motion, AnimatePresence } from 'motion/react';
import { X, Utensils, Car, Gamepad2, TrendingUp, HeartPulse, GraduationCap, Briefcase, ShoppingBag } from 'lucide-react';
import { Button } from './ui/Button';
import { useState, useEffect } from 'react';
import { api } from '../lib/api';

interface CategoryModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const PRESET_ICONS = [
  { name: 'utensils', icon: Utensils, label: 'Alimentação' },
  { name: 'car', icon: Car, label: 'Transporte' },
  { name: 'gamepad', icon: Gamepad2, label: 'Lazer' },
  { name: 'briefcase', icon: Briefcase, label: 'Trabalho/Salário' },
  { name: 'heart', icon: HeartPulse, label: 'Saúde' },
  { name: 'graduation-cap', icon: GraduationCap, label: 'Educação' },
  { name: 'shopping-bag', icon: ShoppingBag, label: 'Compras' },
];

const PRESET_COLORS = [
  { name: 'orange', value: '#f97316' },
  { name: 'green', value: '#16a34a' },
  { name: 'blue', value: '#2563eb' },
  { name: 'purple', value: '#9333ea' },
  { name: 'red', value: '#e11d48' },
  { name: 'yellow', value: '#eab308' },
];

export function CategoryModal({ isOpen, onClose, onSuccess }: CategoryModalProps) {
  const [name, setName] = useState('');
  const [selectedIcon, setSelectedIcon] = useState('utensils');
  const [selectedColor, setSelectedColor] = useState('orange');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
      const mainEl = document.querySelector('main');
      if (mainEl) {
        mainEl.style.overflow = 'hidden';
      }
    } else {
      document.body.style.overflow = '';
      const mainEl = document.querySelector('main');
      if (mainEl) {
        mainEl.style.overflow = '';
      }
    }
    return () => {
      document.body.style.overflow = '';
      const mainEl = document.querySelector('main');
      if (mainEl) {
        mainEl.style.overflow = '';
      }
    };
  }, [isOpen]);

  const handleSubmit = async () => {
    if (!name.trim()) {
      setError('Por favor, digite o nome da categoria.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      await api.createCategory(name.trim(), selectedIcon, selectedColor);
      onSuccess();
      onClose();
      setName('');
      setSelectedIcon('utensils');
      setSelectedColor('orange');
    } catch (err: any) {
      console.error(err);
      setError(err.message || 'Falha ao criar categoria.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/30 backdrop-blur-sm z-[60]"
          />
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: 20 }}
            className="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-lg glass-modal p-8 rounded-2xl z-[70] shadow-2xl space-y-6"
          >
            <div className="flex justify-between items-center">
              <h2 className="text-2xl font-bold tracking-tight">Nova Categoria</h2>
              <button 
                onClick={onClose}
                className="p-2 hover:bg-surface-low rounded-full transition-colors text-on-surface-variant hover:text-on-surface"
              >
                <X size={24} />
              </button>
            </div>

            {error && (
              <div className="p-3 rounded-lg bg-brand-error/10 border border-brand-error/20 text-brand-error text-xs">
                {error}
              </div>
            )}

            <div className="space-y-6">
              {}
              <div className="relative group">
                <input 
                  type="text" 
                  placeholder="Nome da categoria (ex: Supermercado)"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 px-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim transition-all text-on-surface placeholder:text-on-surface-variant/40"
                  autoFocus
                />
              </div>

              {}
              <div className="space-y-2">
                <label className="text-xs uppercase tracking-widest text-on-surface-variant font-bold">Selecione o Ícone</label>
                <div className="flex flex-wrap gap-3">
                  {PRESET_ICONS.map((item) => {
                    const IconComponent = item.icon;
                    const isSelected = selectedIcon === item.name;
                    return (
                      <button
                        key={item.name}
                        type="button"
                        onClick={() => setSelectedIcon(item.name)}
                        title={item.label}
                        className={`p-3 rounded-xl border transition-all ${
                          isSelected 
                            ? 'bg-brand-primary/10 border-brand-primary text-brand-primary shadow-md' 
                            : 'bg-surface-low border-outline-variant text-on-surface-variant hover:text-on-surface hover:border-outline'
                        }`}
                      >
                        <IconComponent size={20} />
                      </button>
                    );
                  })}
                </div>
              </div>

              {}
              <div className="space-y-2">
                <label className="text-xs uppercase tracking-widest text-on-surface-variant font-bold">Selecione a Cor</label>
                <div className="flex flex-wrap gap-4 items-center">
                  {PRESET_COLORS.map((item) => {
                    const isSelected = selectedColor === item.name;
                    return (
                      <button
                        key={item.name}
                        type="button"
                        onClick={() => setSelectedColor(item.name)}
                        className={`w-8 h-8 rounded-full border-2 transition-all ${
                          isSelected ? 'border-on-surface scale-110 shadow-lg' : 'border-transparent hover:scale-105'
                        }`}
                        style={{ backgroundColor: item.value }}
                        title={item.name}
                      />
                    );
                  })}
                </div>
              </div>

              {}
              <div className="pt-4 flex gap-4">
                <Button variant="secondary" className="flex-1" onClick={onClose}>
                  Cancelar
                </Button>
                <Button className="flex-1 py-4 justify-center" onClick={handleSubmit} disabled={loading}>
                  {loading ? 'Criando...' : 'Criar Categoria'}
                </Button>
              </div>
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
