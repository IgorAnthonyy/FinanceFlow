import { ButtonHTMLAttributes, ReactNode, Key } from 'react';
import { cn } from '../../lib/utils';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children?: ReactNode;
  variant?: 'primary' | 'secondary' | 'ghost' | 'outline';
  size?: 'sm' | 'md' | 'lg' | 'icon';
  className?: string;
  onClick?: () => void;
  key?: Key;
  disabled?: boolean;
  type?: 'button' | 'submit' | 'reset';
}

export function Button({ 
  children, 
  variant = 'primary', 
  size = 'md', 
  className, 
  ...props 
}: ButtonProps) {
  const variants = {
    primary: "bg-brand-primary text-brand-background hover:brightness-110 active:scale-95 shadow-lg shadow-brand-primary/20",
    secondary: "bg-surface-highest text-on-surface hover:bg-surface-bright active:scale-95",
    ghost: "bg-transparent text-on-surface-variant hover:bg-surface-highest hover:text-on-surface active:scale-95",
    outline: "bg-transparent border border-outline-variant text-on-surface hover:bg-surface-highest active:scale-95",
  };

  const sizes = {
    sm: "px-3 py-1.5 text-xs",
    md: "px-6 py-2.5 text-sm",
    lg: "px-8 py-3.5 text-base",
    icon: "p-2",
  };

  return (
    <button
      className={cn(
        "rounded-full font-semibold transition-all duration-200 flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed",
        variants[variant],
        sizes[size],
        className
      )}
      {...props}
    >
      {children}
    </button>
  );
}
