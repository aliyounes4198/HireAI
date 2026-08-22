import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { JobAPI } from '../../api/endpoints'
import { extractError } from '../../api/client'
import { Notice, PageHeader } from '../../components/Atoms'

const EMPTY = { title: '', description: '', requirements: '', location: '' }

export default function PostJob() {
  const [form, setForm] = useState(EMPTY)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const res = await JobAPI.create(form)
      navigate(`/jobs/${res.data.jobId}`)
    } catch (err) {
      setError(extractError(err, 'Could not post job. Make sure your account is linked to a company.'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-2xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="New Requisition"
        title="Post a Job"
        subtitle="This opens a new case file, visible immediately on the public job board."
      />

      <form onSubmit={handleSubmit} className="file-card p-6 space-y-4">
        <div>
          <label className="field-label">Title</label>
          <input required className="field-input" value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} placeholder="Backend Developer" />
        </div>
        <div>
          <label className="field-label">Location</label>
          <input className="field-input" value={form.location} onChange={(e) => setForm({ ...form, location: e.target.value })} placeholder="Beirut, Lebanon (Remote)" />
        </div>
        <div>
          <label className="field-label">Description</label>
          <textarea required className="field-textarea" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        </div>
        <div>
          <label className="field-label">Requirements</label>
          <textarea
            required
            className="field-textarea"
            value={form.requirements}
            onChange={(e) => setForm({ ...form, requirements: e.target.value })}
            placeholder="List required skills clearly — e.g. C#, ASP.NET Core, SQL, Entity Framework, Git, REST API — the AI matcher scans this text for recognized skills."
          />
        </div>

        <Notice tone="error">{error}</Notice>

        <button disabled={loading} className="btn-primary">
          {loading ? 'Filing…' : 'Post job'}
        </button>
      </form>
    </div>
  )
}
