import { useEffect, useState } from 'react'
import { CandidateProfileAPI } from '../../api/endpoints'
import { extractError } from '../../api/client'
import { Loader, Notice, PageHeader } from '../../components/Atoms'

const EMPTY = { phone: '', skills: '', education: '', experience: '', linkedIn: '', github: '' }

export default function CandidateProfile() {
  const [form, setForm] = useState(EMPTY)
  const [exists, setExists] = useState(false)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)

  useEffect(() => {
    CandidateProfileAPI.get()
      .then((res) => {
        setForm({
          phone: res.data.phone || '',
          skills: res.data.skills || '',
          education: res.data.education || '',
          experience: res.data.experience || '',
          linkedIn: res.data.linkedIn || '',
          github: res.data.github || '',
        })
        setExists(true)
      })
      .catch(() => setExists(false))
      .finally(() => setLoading(false))
  }, [])

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setSuccess(false)
    setSaving(true)
    try {
      if (exists) {
        await CandidateProfileAPI.update(form)
      } else {
        await CandidateProfileAPI.create(form)
        setExists(true)
      }
      setSuccess(true)
    } catch (err) {
      setError(extractError(err, 'Could not save profile.'))
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <Loader label="Opening dossier" />

  return (
    <div className="max-w-2xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="Candidate Dossier"
        title="My Profile"
        subtitle={
          exists
            ? 'Keep this current — it feeds every AI match run against your resumes.'
            : 'Create your profile before uploading a resume or applying to jobs.'
        }
      />

      <form onSubmit={handleSubmit} className="file-card p-6 space-y-4">
        <div className="grid sm:grid-cols-2 gap-4">
          <div>
            <label className="field-label">Phone</label>
            <input className="field-input" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
          </div>
          <div>
            <label className="field-label">LinkedIn</label>
            <input className="field-input" value={form.linkedIn} onChange={(e) => setForm({ ...form, linkedIn: e.target.value })} placeholder="linkedin.com/in/…" />
          </div>
        </div>
        <div>
          <label className="field-label">GitHub</label>
          <input className="field-input" value={form.github} onChange={(e) => setForm({ ...form, github: e.target.value })} placeholder="github.com/…" />
        </div>
        <div>
          <label className="field-label">Skills</label>
          <textarea className="field-textarea" value={form.skills} onChange={(e) => setForm({ ...form, skills: e.target.value })} placeholder="C#, ASP.NET Core, SQL, Git…" />
        </div>
        <div>
          <label className="field-label">Education</label>
          <textarea className="field-textarea" value={form.education} onChange={(e) => setForm({ ...form, education: e.target.value })} />
        </div>
        <div>
          <label className="field-label">Experience</label>
          <textarea className="field-textarea" value={form.experience} onChange={(e) => setForm({ ...form, experience: e.target.value })} />
        </div>

        <Notice tone="error">{error}</Notice>
        {success && <Notice tone="success">Profile saved.</Notice>}

        <button disabled={saving} className="btn-primary">
          {saving ? 'Saving…' : exists ? 'Save changes' : 'Create profile'}
        </button>
      </form>
    </div>
  )
}
