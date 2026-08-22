const STYLES = {
  Pending: 'text-stamp-pending border-stamp-pending/50 bg-stamp-pending/10',
  Reviewed: 'text-stamp-reviewed border-stamp-reviewed/50 bg-stamp-reviewed/10',
  Accepted: 'text-stamp-approved border-stamp-approved/50 bg-stamp-approved/10',
  Rejected: 'text-stamp-rejected border-stamp-rejected/50 bg-stamp-rejected/10',
  Open: 'text-stamp-approved border-stamp-approved/50 bg-stamp-approved/10',
  Closed: 'text-muted border-line bg-panel-raised',
}

export default function StatusChip({ status }) {
  const cls = STYLES[status] || 'text-muted border-line bg-panel-raised'
  return <span className={`stamp-chip ${cls}`}>{status}</span>
}
