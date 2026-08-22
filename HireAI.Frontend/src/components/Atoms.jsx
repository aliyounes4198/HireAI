export function Loader({ label = 'Loading' }) {
  return (
    <div className="flex items-center gap-3 text-muted font-mono text-xs tracking-wider uppercase py-10 justify-center">
      <span className="relative flex h-3 w-3">
        <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-brass/60" />
        <span className="relative inline-flex rounded-full h-3 w-3 bg-brass" />
      </span>
      {label}…
    </div>
  )
}

export function EmptyState({ title, hint, action }) {
  return (
    <div className="file-card px-8 py-14 text-center">
      <p className="font-display text-lg text-ivory mb-1">{title}</p>
      {hint && <p className="text-sm text-muted max-w-sm mx-auto">{hint}</p>}
      {action && <div className="mt-5 flex justify-center">{action}</div>}
    </div>
  )
}

export function Notice({ tone = 'error', children }) {
  if (!children) return null
  const styles = {
    error: 'border-stamp-rejectedDim bg-stamp-rejected/10 text-stamp-rejected',
    success: 'border-stamp-approvedDim bg-stamp-approved/10 text-stamp-approved',
    info: 'border-line bg-panel-raised text-mutedCool',
  }
  return (
    <div className={`border rounded-[2px] px-4 py-3 text-sm font-body ${styles[tone]}`}>
      {children}
    </div>
  )
}

export function PageHeader({ eyebrow, title, subtitle, right }) {
  return (
    <div className="flex flex-wrap items-end justify-between gap-4 mb-8">
      <div>
        {eyebrow && (
          <p className="font-mono text-[10px] tracking-[0.2em] uppercase text-brass mb-2">{eyebrow}</p>
        )}
        <h1 className="text-3xl md:text-4xl font-display font-semibold">{title}</h1>
        {subtitle && <p className="text-muted text-sm mt-2 max-w-xl">{subtitle}</p>}
      </div>
      {right}
    </div>
  )
}
