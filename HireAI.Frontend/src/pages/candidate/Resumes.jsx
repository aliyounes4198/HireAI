import { useEffect, useRef, useState } from 'react'
import { ResumeAPI, AIAnalysisAPI } from '../../api/endpoints'
import { extractError } from '../../api/client'
import { Loader, Notice, EmptyState, PageHeader } from '../../components/Atoms'
import MatchSeal from '../../components/MatchSeal'

export default function Resumes() {
  const [resumes, setResumes] = useState(null)
  const [error, setError] = useState('')
  const [uploading, setUploading] = useState(false)
  const [uploadError, setUploadError] = useState('')
  const [analyses, setAnalyses] = useState({}) // resumeId -> analysis result
  const [analyzing, setAnalyzing] = useState(null)
  const fileRef = useRef(null)

  function load() {
    setError('')
    ResumeAPI.list()
      .then((res) => setResumes(res.data))
      .catch((err) => setError(extractError(err, 'Could not load resumes.')))
  }

  useEffect(load, [])

  async function handleUpload(e) {
    e.preventDefault()
    const file = fileRef.current?.files?.[0]
    if (!file) return
    setUploadError('')
    setUploading(true)
    try {
      await ResumeAPI.upload(file)
      fileRef.current.value = ''
      load()
    } catch (err) {
      setUploadError(extractError(err, 'Upload failed.'))
    } finally {
      setUploading(false)
    }
  }

  async function handleDelete(id) {
    if (!confirm('Delete this resume? This cannot be undone.')) return
    await ResumeAPI.remove(id)
    load()
  }

  async function handleAnalyze(id) {
    setAnalyzing(id)
    try {
      const res = await AIAnalysisAPI.analyze(id)
      setAnalyses((prev) => ({ ...prev, [id]: res.data }))
    } catch (err) {
      setAnalyses((prev) => ({ ...prev, [id]: { error: extractError(err, 'Analysis failed.') } }))
    } finally {
      setAnalyzing(null)
    }
  }

  return (
    <div className="max-w-3xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="Evidence Locker"
        title="My Resumes"
        subtitle="Upload a PDF to extract its text for AI matching and analysis. One file, one skill scan."
      />

      <form onSubmit={handleUpload} className="file-card p-6 mb-8 flex flex-wrap items-center gap-4">
        <input
          ref={fileRef}
          type="file"
          accept="application/pdf"
          required
          className="field-input flex-1 min-w-[220px] file:mr-3 file:py-1.5 file:px-3 file:rounded-[2px] file:border-0 file:bg-brass file:text-ink2 file:font-semibold file:text-xs cursor-pointer"
        />
        <button disabled={uploading} className="btn-primary shrink-0">
          {uploading ? 'Extracting…' : 'Upload PDF'}
        </button>
        {uploadError && <div className="w-full"><Notice tone="error">{uploadError}</Notice></div>}
      </form>

      {error && <Notice tone="error">{error}</Notice>}
      {!resumes && !error && <Loader label="Loading resumes" />}
      {resumes && resumes.length === 0 && (
        <EmptyState title="No resumes on file" hint="Upload a PDF above to get started." />
      )}

      <div className="space-y-4">
        {resumes?.map((r) => {
          const a = analyses[r.id]
          return (
            <div key={r.id} className="file-card p-5 pt-6">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h3 className="font-display text-base font-semibold">{r.fileName}</h3>
                  <p className="font-mono text-[10px] text-muted mt-1">
                    Uploaded {new Date(r.uploadDate).toLocaleDateString()}
                  </p>
                </div>
                <div className="flex gap-2 shrink-0">
                  <button className="btn-secondary !px-3 !py-1.5 text-xs" onClick={() => handleAnalyze(r.id)} disabled={analyzing === r.id}>
                    {analyzing === r.id ? 'Scanning…' : 'Run AI analysis'}
                  </button>
                  <button className="btn-danger !px-3 !py-1.5 text-xs" onClick={() => handleDelete(r.id)}>
                    Delete
                  </button>
                </div>
              </div>

              {a?.error && (
                <div className="mt-4">
                  <Notice tone="error">{a.error}</Notice>
                </div>
              )}

              {a && !a.error && (
                <div className="mt-5 pt-5 border-t border-line flex flex-col sm:flex-row gap-5 items-start">
                  <MatchSeal score={a.score} label="AI SCORE" />
                  <div className="flex-1 space-y-3">
                    <p className="text-sm text-ivory/90">{a.feedback}</p>
                    <SkillList label="Matched skills" value={a.matchedSkills} tone="approved" />
                    <SkillList label="Missing skills" value={a.missingSkills} tone="rejected" />
                  </div>
                </div>
              )}
            </div>
          )
        })}
      </div>
    </div>
  )
}

function SkillList({ label, value, tone }) {
  const items = (value || '').split(',').map((s) => s.trim()).filter(Boolean)
  if (items.length === 0) return null
  const cls =
    tone === 'approved'
      ? 'text-stamp-approved border-stamp-approved/40 bg-stamp-approved/10'
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
