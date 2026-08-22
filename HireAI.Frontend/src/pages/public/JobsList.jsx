import { useEffect, useMemo, useState } from 'react'

import { Link } from 'react-router-dom'

import { JobAPI } from '../../api/endpoints'

import { extractError } from '../../api/client'

import { Loader, EmptyState, Notice, PageHeader } from '../../components/Atoms'

import StatusChip from '../../components/StatusChip'

export default function JobsList() {
  const [jobs, setJobs] = useState(null)
  const [error, setError] = useState('')
  const [query, setQuery] = useState('')
  const [openOnly, setOpenOnly] = useState(true)

  useEffect(() => {
    let cancelled = false
    setJobs(null)
    setError('')
    const call = openOnly ? JobAPI.listOpen() : JobAPI.list()
    call
      .then((res) => !cancelled && setJobs(res.data))
      .catch((err) => !cancelled && setError(extractError(err, 'Could not load jobs.')))
    return () => {
      cancelled = true
    }
  }, [openOnly])

  const filtered = useMemo(() => {
    if (!jobs) return []
    const q = query.trim().toLowerCase()
    if (!q) return jobs
    return jobs.filter(
      (j) =>
        j.title?.toLowerCase().includes(q) ||
        j.companyName?.toLowerCase().includes(q) ||
        j.location?.toLowerCase().includes(q)
    )
  }, [jobs, query])

  return (
    <div className="max-w-6xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="Open Requisitions"
        title="Job Board"
        subtitle="Every posting here has been logged to the case file registry. Search, open a file, and apply — the AI will score your fit the moment you do."
      />

      <div className="flex flex-wrap items-center gap-3 mb-8">
        <input
          className="field-input max-w-sm"
          placeholder="Search title, company, location…"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />
        <button
          onClick={() => setOpenOnly((v) => !v)}
          className={`stamp-chip cursor-pointer ${
            openOnly
              ? 'text-stamp-approved border-stamp-approved/50 bg-stamp-approved/10'
              : 'text-muted border-line'
          }`}
        >
          {openOnly ? 'Showing: Open only' : 'Showing: All statuses'}
        </button>
      </div>

      {error && <Notice tone="error">{error}</Notice>}
      {!jobs && !error && <Loader label="Pulling case files" />}

      {jobs && filtered.length === 0 && (
        <EmptyState title="No matching postings" hint="Try a different search term, or check back soon." />
      )}

      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {filtered.map((job) => (
          <Link
            to={`/jobs/${job.id}`}
            key={job.id}
            className="file-card p-5 pt-6 hover:border-brass/60 transition-colors group"
          >
            <div className="flex items-start justify-between gap-2 mb-3">
              <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted">
                File #{String(job.id).padStart(4, '0')}
              </p>
              <StatusChip status={job.status} />
            </div>
            <h3 className="font-display text-lg font-semibold text-ivory group-hover:text-brass transition-colors leading-snug">
              {job.title}
            </h3>
            <p className="text-sm text-mutedCool mt-1">{job.companyName}</p>
            {job.location && <p className="text-xs text-muted mt-2">{job.location}</p>}
            <p className="text-sm text-muted mt-3 line-clamp-2">{job.description}</p>
          </Link>
        ))}
      </div>
    </div>
  )
}
