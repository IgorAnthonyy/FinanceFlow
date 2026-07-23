import { ReactNode, HTMLAttributes, Key, CSSProperties } from 'react';
import { cn } from '../../lib/utils';

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  children: ReactNode;
  glow?: boolean;
  className?: string;
  style?: CSSProperties;
  key?: Key;
}

export function Card({ children, className, glow, ...props }: CardProps) {
  return (
    <div 
      className={cn(
        "glass-card p-6 rounded-xl transition-all duration-300",
        glow && "glow-primary",
        className
      )}
      {...props}
    >
      {children}
    </div>
  );
}
