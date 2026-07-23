import React, { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { User, Mail, LogOut, Camera, Save, X, Edit2 } from 'lucide-react';
import { getUserName, getUserEmail, logout, api, setUserName, setUserEmail } from '../lib/api';

interface ProfileProps {
  onLogout: () => void;
  onProfileUpdate?: (name: string) => void;
}

export function Profile({ onLogout, onProfileUpdate }: ProfileProps) {
  const currentName = getUserName() || 'Usuário FinanceFlow';
  const currentEmail = getUserEmail() || 'usuario@financeflow.com';

  const [isEditing, setIsEditing] = useState(false);
  const [name, setName] = useState(currentName);
  const [email, setEmail] = useState(currentEmail);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogoutClick = () => {
    logout();
    onLogout();
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    try {
      await api.updateProfile(name, email);
      setUserName(name);
      setUserEmail(email);
      onProfileUpdate?.(name);
      setSuccess('Perfil atualizado com sucesso!');
      setIsEditing(false);
    } catch (err: any) {
      setError(err.message || 'Erro ao atualizar perfil.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div className="flex flex-col items-center text-center">
        <div className="relative group">
          <div className="w-32 h-32 rounded-full border-4 border-brand-primary/20 p-1 overflow-hidden transition-transform group-hover:scale-105">
            <img 
              src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?q=80&w=2070&auto=format&fit=crop" 
              alt="Profile" 
              className="w-full h-full object-cover rounded-full"
            />
          </div>
          <button className="absolute bottom-1 right-1 p-2 bg-brand-primary text-brand-background rounded-full shadow-lg hover:scale-110 active:scale-95 transition-all">
            <Camera size={16} />
          </button>
        </div>
        <h2 className="mt-4 text-2xl font-bold text-on-surface">{currentName}</h2>
        <p className="text-on-surface-variant text-sm">{currentEmail}</p>
      </div>

      <div className="space-y-4">
        {success && (
          <div className="p-4 bg-brand-primary/10 border border-brand-primary/20 text-brand-primary rounded-xl text-sm font-semibold">
            {success}
          </div>
        )}
        
        {error && (
          <div className="p-4 bg-brand-error/10 border border-brand-error/20 text-brand-error rounded-xl text-sm font-semibold">
            {error}
          </div>
        )}

        <div className="flex justify-between items-center px-4">
          <h3 className="text-lg font-bold opacity-50">Informações Pessoais</h3>
          {!isEditing && (
            <Button variant="ghost" size="sm" className="flex items-center gap-1" onClick={() => setIsEditing(true)}>
              <Edit2 size={16} />
              Editar Perfil
            </Button>
          )}
        </div>

        {isEditing ? (
          <Card className="p-6">
            <form onSubmit={handleSave} className="space-y-6">
              <div className="space-y-2">
                <label className="text-sm font-medium text-on-surface-variant/80">Nome Completo</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant/50" size={18} />
                  <input
                    type="text"
                    required
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    className="w-full pl-10 pr-4 py-2 bg-surface-low border border-outline rounded-lg text-on-surface focus:outline-none focus:border-brand-primary transition-colors"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-on-surface-variant/80">E-mail</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant/50" size={18} />
                  <input
                    type="email"
                    required
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className="w-full pl-10 pr-4 py-2 bg-surface-low border border-outline rounded-lg text-on-surface focus:outline-none focus:border-brand-primary transition-colors"
                  />
                </div>
              </div>

              <div className="flex gap-4 pt-2">
                <Button type="submit" disabled={loading} className="flex-1 flex items-center justify-center gap-2">
                  <Save size={18} />
                  {loading ? 'Salvando...' : 'Salvar Alterações'}
                </Button>
                <Button type="button" variant="outline" onClick={() => { setIsEditing(false); setName(currentName); setEmail(currentEmail); }} className="flex items-center justify-center gap-2">
                  <X size={18} />
                  Cancelar
                </Button>
              </div>
            </form>
          </Card>
        ) : (
          <Card className="p-2 divide-y divide-outline-variant">
            <div className="flex items-center justify-between p-4">
              <div className="flex items-center gap-4">
                <User className="text-brand-secondary" size={20} />
                <div>
                  <p className="text-xs text-on-surface-variant uppercase tracking-widest font-bold">Nome</p>
                  <p className="text-sm font-bold text-on-surface mt-0.5">{currentName}</p>
                </div>
              </div>
            </div>
            
            <div className="flex items-center justify-between p-4">
              <div className="flex items-center gap-4">
                <Mail className="text-brand-secondary" size={20} />
                <div>
                  <p className="text-xs text-on-surface-variant uppercase tracking-widest font-bold">E-mail</p>
                  <p className="text-sm font-bold text-on-surface mt-0.5">{currentEmail}</p>
                </div>
              </div>
            </div>
          </Card>
        )}

        <Button variant="ghost" className="w-full mt-8 text-brand-error hover:bg-brand-error/10" onClick={handleLogoutClick}>
          <LogOut size={20} />
          Encerrar Sessão
        </Button>
      </div>
    </div>
  );
}
