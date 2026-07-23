import React, { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Mail, Lock, User, Wallet, AlertCircle } from 'lucide-react';
import { api, setAuthToken, setUserId, setUserName, setUserEmail } from '../lib/api';

interface LoginProps {
  onLoginSuccess: () => void;
}

function parseJwt(token: string) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window.atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
}

export function Login({ onLoginSuccess }: LoginProps) {
  const [isLogin, setIsLogin] = useState(true);
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    try {
      if (isLogin) {
        const data = await api.login(email, password);
        const jwtString = data.token.token;
        
        if (!jwtString) {
          throw new Error('Formato de resposta de token inválido.');
        }

        setAuthToken(jwtString);
        
        const claims = parseJwt(jwtString);
        if (claims) {
          setUserId(claims.sub || '');
          setUserEmail(claims.email || email);
          setUserName(claims.name || claims.unique_name || 'Usuário');
        } else {
          setUserId('');
          setUserName('Usuário');
          setUserEmail(email);
        }

        onLoginSuccess();
      } else {
        await api.register(name, email, password);
        setSuccess('Cadastro realizado com sucesso! Faça login para continuar.');
        setIsLogin(true);
        setPassword('');
      }
    } catch (err: any) {
      console.error(err);
      setError(err.message || 'Ocorreu um erro inesperado. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-brand-background px-4 relative overflow-hidden">
      {}
      <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-brand-primary/10 rounded-full blur-[120px] pointer-events-none" />
      <div className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-brand-secondary/10 rounded-full blur-[120px] pointer-events-none" />

      <Card glow className="w-full max-w-md p-8 glass-modal relative z-10 space-y-6">
        <div className="flex flex-col items-center text-center space-y-2">
          <div className="w-16 h-16 rounded-2xl bg-brand-primary/10 border border-brand-primary/20 flex items-center justify-center text-brand-primary mb-2">
            <Wallet size={36} />
          </div>
          <h1 className="text-3xl font-extrabold tracking-tight text-on-surface">FinanceFlow</h1>
          <p className="text-sm text-on-surface-variant">
            {isLogin ? 'Faça login para gerenciar suas finanças' : 'Crie sua conta e comece a poupar'}
          </p>
        </div>

        {error && (
          <div className="p-4 rounded-xl bg-brand-error/10 border border-brand-error/20 flex items-start gap-3 text-brand-error text-sm animate-in fade-in duration-200">
            <AlertCircle className="shrink-0 mt-0.5" size={16} />
            <span>{error}</span>
          </div>
        )}

        {success && (
          <div className="p-4 rounded-xl bg-brand-primary/10 border border-brand-primary/20 flex items-start gap-3 text-brand-primary text-sm animate-in fade-in duration-200">
            <AlertCircle className="shrink-0 mt-0.5" size={16} />
            <span>{success}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          {!isLogin && (
            <div className="relative">
              <span className="absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant/60">
                <User size={18} />
              </span>
              <input
                type="text"
                required
                placeholder="Nome Completo"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 pl-12 pr-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim transition-all text-on-surface placeholder:text-on-surface-variant/40"
              />
            </div>
          )}

          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant/60">
              <Mail size={18} />
            </span>
            <input
              type="email"
              required
              placeholder="E-mail"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 pl-12 pr-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim transition-all text-on-surface placeholder:text-on-surface-variant/40"
            />
          </div>

          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant/60">
              <Lock size={18} />
            </span>
            <input
              type="password"
              required
              placeholder="Senha"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full bg-surface-low border border-outline-variant rounded-xl py-3 pl-12 pr-4 text-sm outline-none focus:border-brand-primary/50 focus:bg-surface-dim transition-all text-on-surface placeholder:text-on-surface-variant/40"
            />
          </div>

          <Button type="submit" className="w-full py-4 mt-2 justify-center" disabled={loading}>
            {loading ? 'Carregando...' : isLogin ? 'Entrar' : 'Cadastrar'}
          </Button>
        </form>

        <div className="text-center pt-2">
          <button
            type="button"
            onClick={() => {
              setIsLogin(!isLogin);
              setError('');
              setSuccess('');
            }}
            className="text-xs text-brand-primary hover:underline font-semibold"
          >
            {isLogin ? 'Não tem uma conta? Cadastre-se' : 'Já possui uma conta? Faça Login'}
          </button>
        </div>
      </Card>
    </div>
  );
}