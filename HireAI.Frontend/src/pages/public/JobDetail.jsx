import { useEffect, useState } from 'react'

import { useParams, Link } from 'react-router-dom'

import { JobAPI, ResumeAPI, ApplicationAPI } from '../../api/endpoints'

import { extractError } from '../../api/client'

import { useAuth } from '../../context/AuthContext'

import { Loader, Notice } from '../../components/Atoms'

import StatusChip from '../../components/StatusChip'

import MatchSeal from '../../components/MatchSeal'
export default function JobDetail() {
  const { id } = useParams()
  const { user } = useAuth()
  const [job, setJob] = useState(null)
  const [error, setError] = useState('')

  const [resumes, setResumes] = useState(null)
  const [selectedResume, setSelectedResume] = useState('')
  const [applying, setApplying] = useState(false)
  const [applyError, setApplyError] = useState('')
  const [result, setResult] = useState(null)

  useEffect(() => {
    JobAPI.get(id)
      .then((res) => setJob(res.data))
      .catch((err) => setError(extractError(err, 'Job not found.')))
  }, [id])

  useEffect(() => {
    if (user?.role === 'Candidate') {
      ResumeAPI.list()
        .then((res) => {
          setResumes(res.data)
          if (res.data[0]) setSelectedResume(String(res.data[0].id))
        })
        .catch(() => setResumes([]))
    }
  }, [user])

  async function handleApply(e) {
    e.preventDefault()
    setApplyError('')
    setApplying(true)
    try {
      const res = await ApplicationAPI.create({
        jobId: Number(id),
        resumeId: Number(selectedResume),
      })
      setResult(res.data)
    } catch (err) {
      setApplyError(extractError(err, 'Could not submit application.'))
    } finally {
      setApplying(false)
    }
  }

  if (error) {
    return (
      <div className="max-w-3xl mx-auto px-5 py-16">
        <Notice tone="error">{error}</Notice>
      </div>
    )
  }
  if (!job) return <Loader label="Opening file" />

  return (
    <div className="max-w-3xl mx-auto px-5 py-12">
      <Link to="/" className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted hover:text-brass">
        ← Back to job board
      </Link>

      <div className="file-card p-7 pt-8 mt-4">
        <div className="flex items-start justify-between gap-3 mb-4">
          <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted">
            File #{String(job.id).padStart(4, '0')} · {job.companyName}
          </p>
          <StatusChip status={job.status} />
        </div>

        <h1 className="text-2xl md:text-3xl font-display font-semibold mb-2">{job.title}</h1>
        {job.location && <p className="text-sm text-mutedCool mb-6">{job.location}</p>}

        <Section title="Description" body={job.description} />
        <Section title="Requirements" body={job.requirements} />

        <p className="font-mono text-[10px] text-muted mt-8">
          Logged {new Date(job.createdAt).toLocaleDateString()}
        </p>
      </div>

      {/* Apply flow */}
      {user?.role === 'Candidate' && (
        <div className="file-card p-7 pt-8 mt-6">
          <h2 className="font-display text-xl font-semibold mb-1">Apply to this posting</h2>
          <p className="text-sm text-muted mb-5">
            Submitting runs your resume through the AI matcher instantly.
          </p>

          {result ? (
            <div className="flex flex-col sm:flex-row gap-6 items-start">
              <MatchSeal score={result.aiMatch?.matchScore ?? 0} />
              <div className="flex-1 space-y-3">
                <Notice tone="success">Application submitted — status: {result.status}</Notice>
                {result.aiMatch && (
                  <>
                    <SkillRow label="Matched" items={result.aiMatch.matchedSkills} tone="approved" />
                    <SkillRow label="Missing" items={result.aiMatch.missingSkills} tone="rejected" />
                    {result.aiMatch.recommendation && (
                      <p className="text-sm text-ivory bg-ink2 border border-line rounded-[2px] p-3">
                        {result.aiMatch.recommendation}
                      </p>
                    )}
                  </>
                )}
                <Link to="/applications" className="btn-secondary inline-flex mt-1">
                  View my applications
                </Link>
              </div>
            </div>
          ) : resumes === null ? (
            <Loader label="Loading resumes" />
          ) : resumes.length === 0 ? (
            <Notice tone="info">
              You need a resume on file before applying.{' '}
              <Link to="/resumes" className="text-brass hover:underline">
                Upload one now →
              </Link>
            </Notice>
          ) : (
            <form onSubmit={handleApply} className="space-y-4">
              <div>
                <label className="field-label">Resume to submit</label>
                <select
                  className="field-input"
                  value={selectedResume}
                  onChange={(e) => setSelectedResume(e.target.value)}
                >
                  {resumes.map((r) => (
                    <option key={r.id} value={r.id}>
                      {r.fileName}
                    </option>
                  ))}
                </select>
              </div>
              <Notice tone="error">{applyError}</Notice>
              <button disabled={applying} className="btn-primary">
                {applying ? 'Scoring your fit…' : 'Submit application'}
              </button>
            </form>
          )}
        </div>
      )}

      {!user && (
        <Notice tone="info">
          <span className="mr-1">
            <Link to="/login" className="text-brass hover:underline">
              Sign in
            </Link>{' '}
            as a candidate to apply to this job.
          </span>
        </Notice>
      )}
    </div>
  )
}

function Section({ title, body }) {
  if (!body) return null
  return (
    <div className="mb-5">
      <p className="field-label">{title}</p>
      <p className="text-sm text-ivory/90 whitespace-pre-line leading-relaxed">{body}</p>
    </div>
  )
}

function SkillRow({ label, items, tone }) {
  const list = Array.isArray(items) ? items : (items || '').split(',').map((s) => s.trim()).filter(Boolean)
  if (list.length === 0) return null
  const cls =
    tone === 'approved'
      ? 'text-stamp-approved border-stamp-approved/40 bg-stamp-approved/10'
      : 'text-stamp-rejected border-stamp-rejected/40 bg-stamp-rejected/10'
  return (
    <div>
      <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted mb-1.5">{label} skills</p>
      <div className="flex flex-wrap gap-1.5">
        {list.map((s) => (
          <span key={s} className={`stamp-chip normal-case ${cls}`}>
            {s}
          </span>
        ))}
      </div>
    </div>
  )
}
