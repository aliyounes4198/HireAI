import { useEffect, useState } from 'react'
import { ApplicationAPI } from '../../api/endpoints'
import { extractError } from '../../api/client'
import { Loader, Notice, EmptyState, PageHeader } from '../../components/Atoms'
import StatusChip from '../../components/StatusChip'

const STATUSES = ['Pending', 'Reviewed', 'Accepted', 'Rejected']

export default function HRApplications() {
  const [apps, setApps] = useState(null)
  const [error, setError] = useState('')
  const [updating, setUpdating] = useState(null)
  const [filter, setFilter] = useState('All')

  function load() {
    setError('')
    ApplicationAPI.companyApplications()
      .then((res) => setApps(res.data))
      .catch((err) => setError(extractError(err, 'Could not load applications.')))
  }

  useEffect(load, [])

  async function updateStatus(id, status) {
    setUpdating(id)
    try {
      await ApplicationAPI.updateStatus(id, status)
      setApps((prev) => prev.map((a) => (a.id === id ? { ...a, status } : a)))
    } catch (err) {
      setError(extractError(err, 'Could not update status.'))
    } finally {
      setUpdating(null)
    }
  }

  const filtered = apps?.filter((a) => filter === 'All' || a.status === filter)

  return (
    <div className="max-w-4xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="Review Queue"
        title="Applications"
        subtitle="Every candidate who has applied to your company's postings."
      />

      <div className="flex flex-wrap gap-2 mb-6">
        {['All', ...STATUSES].map((s) => (
          <button
            key={s}
            onClick={() => setFilter(s)}
            className={`stamp-chip cursor-pointer normal-case ${
              filter === s ? 'text-brass border-brass/50 bg-brass/10' : 'text-muted border-line'
            }`}
          >
            {s}
          </button>
        ))}
      </div>

      {error && <Notice tone="error">{error}</Notice>}
      {!apps && !error && <Loader label="Loading queue" />}
      {filtered && filtered.length === 0 && <EmptyState title="Nothing here" hint="No applications match this filter." />}

      <div className="space-y-3">
        {filtered?.map((a) => (
          <div key={a.id} className="file-card p-5 pt-6 flex flex-wrap items-center gap-4 justify-between">
            <div>
              <p className="font-display text-base font-semibold">{a.candidateName}</p>
              <p className="text-sm text-mutedCool">{a.jobTitle}</p>
              <p className="font-mono text-[10px] text-muted mt-1">
                {a.resumeFileName} · Applied {new Date(a.appliedDate).toLocaleDateString()}
              </p>
            </div>
            <div className="flex items-center gap-3">
              <StatusChip status={a.status} />
              <select
                className="field-input !py-1.5 !w-auto text-xs"
                value={a.status}
                disabled={updating === a.id}
                onChange={(e) => updateStatus(a.id, e.target.value)}
              >
                {STATUSES.map((s) => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </select>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
