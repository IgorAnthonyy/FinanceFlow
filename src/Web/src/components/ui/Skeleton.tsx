import { HTMLAttributes } from 'react';
import { cn } from '../../lib/utils';

interface SkeletonProps extends HTMLAttributes<HTMLDivElement> {
  className?: string;
}

export function Skeleton({ className, ...props }: SkeletonProps) {
  return (
    <div
      className={cn("animate-pulse bg-surface-high rounded-lg", className)}
      {...props}
    />
  );
}
