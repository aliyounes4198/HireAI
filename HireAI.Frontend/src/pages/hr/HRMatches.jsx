import { useEffect, useMemo, useState } from 'react'
import { JobMatchAPI } from '../../api/endpoints'
import { extractError } from '../../api/client'
import { Loader, Notice, EmptyState, PageHeader } from '../../components/Atoms'
import MatchSeal from '../../components/MatchSeal'

export default function HRMatches() {
  const [matches, setMatches] = useState(null)
  const [error, setError] = useState('')
  const [expanded, setExpanded] = useState(null)

  useEffect(() => {
    JobMatchAPI.companyMatches()
      .then((res) => setMatches(res.data))
      .catch((err) => setError(extractError(err, 'Could not load AI matches.')))
  }, [])

  const sorted = useMemo(
    () => (matches ? [...matches].sort((a, b) => b.matchScore - a.matchScore) : null),
    [matches]
  )

  return (
    <div className="max-w-4xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="Signal Analysis"
        title="AI Matches"
        subtitle="Every resume-to-job scan run for your company's postings, ranked by fit."
      />

      {error && <Notice tone="error">{error}</Notice>}
      {!matches && !error && <Loader label="Loading scans" />}
      {sorted && sorted.length === 0 && (
        <EmptyState title="No matches yet" hint="Scores appear once candidates apply to your postings." />
      )}

      <div className="space-y-3">
        {sorted?.map((m) => {
          const isOpen = expanded === m.id
          return (
            <div key={m.id} className="file-card p-5 pt-6">
              <button
                className="w-full flex items-center gap-4 text-left"
                onClick={() => setExpanded(isOpen ? null : m.id)}
              >
                <MatchSeal score={m.matchScore} size={64} />
                <div className="flex-1 min-w-0">
                  <p className="font-display text-base font-semibold truncate">{m.jobTitle}</p>
                  <p className="text-sm text-mutedCool truncate">{m.resumeFileName}</p>
                  <p className="font-mono text-[10px] text-muted mt-1">
                    Scanned {new Date(m.createdAt).toLocaleString()}
                  </p>
                </div>
                <span className="font-mono text-[10px] text-muted uppercase shrink-0">
                  {isOpen ? 'Hide' : 'Details'}
                </span>
              </button>

              {isOpen && (
                <div className="mt-5 pt-5 border-t border-line space-y-3">
                  {m.recommendation && <p className="text-sm text-ivory/90">{m.recommendation}</p>}
                  <div className="flex flex-wrap gap-4 text-xs text-muted font-mono">
                    <span>Experience relevant: {m.experienceRelevance ? 'yes' : 'no'}</span>
                    <span>Education relevant: {m.educationRelevance ? 'yes' : 'no'}</span>
                  </div>
                  <TagRow label="Matched" value={m.matchedSkills} tone="approved" />
                  <TagRow label="Missing" value={m.missingSkills} tone="rejected" />
                  <TagRow label="Strengths" value={m.strengths} tone="brass" />
                  <TagRow label="Weaknesses" value={m.weaknesses} tone="rejected" />
                </div>
              )}
            </div>
          )
        })}
      </div>
    </div>
  )
}

function TagRow({ label, value, tone }) {
  const items = (value || '').split(',').map((s) => s.trim()).filter(Boolean)
  if (items.length === 0) return null
  const cls =
    tone === 'approved'
      ? 'text-stamp-approved border-stamp-approved/40 bg-stamp-approved/10'
      : tone === 'brass'
      ? 'text-brass border-brass/40 bg-brass/10'
      : 'text-stamp-rejected border-stamp-rejected/40 bg-stamp-rejected/10'
  return (
    <div>
      <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted mb-1.5">{label}</p>
      <div className="flex flex-wrap gap-1.5">
        {items.map((s) => (
          <span key={s} className={`stamp-chip normal-case ${cls}`}>{s}</span>
        ))}
      </div>
    </div>
  )
}
