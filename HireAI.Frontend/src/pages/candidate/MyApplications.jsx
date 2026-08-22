import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ApplicationAPI, JobMatchAPI } from '../../api/endpoints'
import { extractError } from '../../api/client'
import { Loader, Notice, EmptyState, PageHeader } from '../../components/Atoms'
import StatusChip from '../../components/StatusChip'
import MatchSeal from '../../components/MatchSeal'

export default function MyApplications() {
  const [apps, setApps] = useState(null)
  const [error, setError] = useState('')
  const [matches, setMatches] = useState({})
  const [loadingMatch, setLoadingMatch] = useState(null)

  useEffect(() => {
    ApplicationAPI.myApplications()
      .then((res) => setApps(res.data))
      .catch((err) => setError(extractError(err, 'Could not load your applications.')))
  }, [])

  async function viewMatch(app) {
    setLoadingMatch(app.id)
    try {
      const res = await JobMatchAPI.match(app.resumeId, app.jobId)
      setMatches((prev) => ({ ...prev, [app.id]: res.data }))
    } catch (err) {
      setMatches((prev) => ({ ...prev, [app.id]: { error: extractError(err, 'Could not compute match.') } }))
    } finally {
      setLoadingMatch(null)
    }
  }

  return (
    <div className="max-w-3xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="Tracking"
        title="My Applications"
        subtitle="Every job you've applied to, with live status and AI match detail on demand."
      />

      {error && <Notice tone="error">{error}</Notice>}
      {!apps && !error && <Loader label="Loading applications" />}
      {apps && apps.length === 0 && (
        <EmptyState
          title="No applications yet"
          hint="Browse the job board and apply — your AI match score shows up right here."
          action={<Link to="/" className="btn-primary">Browse jobs</Link>}
        />
      )}

      <div className="space-y-4">
        {apps?.map((app) => {
          const m = matches[app.id]
          return (
            <div key={app.id} className="file-card p-5 pt-6">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted mb-1">
                    {app.companyName}
                  </p>
                  <Link to={`/jobs/${app.jobId}`} className="font-display text-lg font-semibold hover:text-brass transition-colors">
                    {app.jobTitle}
                  </Link>
                  <p className="text-xs text-muted mt-1">
                    Applied {new Date(app.appliedDate).toLocaleDateString()} · {app.resumeFileName}
                  </p>
                </div>
                <StatusChip status={app.status} />
              </div>

              <div className="mt-4 pt-4 border-t border-line">
                {m?.error && <Notice tone="error">{m.error}</Notice>}
                {m && !m.error ? (
                  <div className="flex flex-col sm:flex-row gap-5 items-start">
                    <MatchSeal score={m.matchScore} />
                    <div className="flex-1 space-y-2">
                      {m.recommendation && <p className="text-sm text-ivory/90">{m.recommendation}</p>}
                      <div className="flex flex-wrap gap-4 text-xs text-muted font-mono">
                        <span>Experience relevant: {m.experienceRelevance ? 'yes' : 'no'}</span>
                        <span>Education relevant: {m.educationRelevance ? 'yes' : 'no'}</span>
                      </div>
                    </div>
                  </div>
                ) : (
                  <button
                    className="btn-secondary !px-3 !py-1.5 text-xs"
                    onClick={() => viewMatch(app)}
                    disabled={loadingMatch === app.id}
                  >
                    {loadingMatch === app.id ? 'Scoring…' : 'View AI match detail'}
                  </button>
                )}
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}
