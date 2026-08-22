import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { CompanyAPI } from '../../api/endpoints'
import { extractError } from '../../api/client'
import { useAuth } from '../../context/AuthContext'
import { Loader, EmptyState, Notice, PageHeader } from '../../components/Atoms'

export default function CompaniesList() {
  const { user } = useAuth()
  const [companies, setCompanies] = useState(null)
  const [error, setError] = useState('')
  const [showCreate, setShowCreate] = useState(false)

  function load() {
    setError('')
    CompanyAPI.list()
      .then((res) => setCompanies(res.data))
      .catch((err) => setError(extractError(err, 'Could not load companies.')))
  }

  useEffect(load, [])

  return (
    <div className="max-w-5xl mx-auto px-5 py-12">
      <PageHeader
        eyebrow="Registry"
        title="Companies"
        subtitle="Every employer with an open case file in HireAI."
        right={
          user && (
            <button className="btn-secondary" onClick={() => setShowCreate((v) => !v)}>
              {showCreate ? 'Cancel' : '+ New company'}
            </button>
          )
        }
      />

      {showCreate && (
        <CreateCompanyForm
          onCreated={() => {
            setShowCreate(false)
            load()
          }}
        />
      )}

      {error && <Notice tone="error">{error}</Notice>}
      {!companies && !error && <Loader label="Loading registry" />}
      {companies && companies.length === 0 && (
        <EmptyState title="No companies yet" hint="Be the first to register one." />
      )}

      <div className="grid sm:grid-cols-2 gap-4">
        {companies?.map((c) => (
          <Link key={c.id} to={`/companies/${c.id}`} className="file-card p-5 pt-6 hover:border-brass/60 transition-colors group">
            <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted mb-2">
              ID #{c.id}
            </p>
            <h3 className="font-display text-lg font-semibold group-hover:text-brass transition-colors">{c.name}</h3>
            {c.location && <p className="text-sm text-mutedCool mt-1">{c.location}</p>}
            {c.description && <p className="text-sm text-muted mt-3 line-clamp-2">{c.description}</p>}
          </Link>
        ))}
      </div>
    </div>
  )
}

function CreateCompanyForm({ onCreated }) {
  const [form, setForm] = useState({ name: '', description: '', website: '', location: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [created, setCreated] = useState(null)

  async function submit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const res = await CompanyAPI.create(form)
      setCreated(res.data)
      onCreated()
    } catch (err) {
      setError(extractError(err, 'Could not create company.'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="file-card p-6 mb-8">
      <h2 className="font-display text-lg font-semibold mb-1">New company</h2>
      <p className="text-sm text-muted mb-4">
        Note the ID shown after creation — HR accounts need it to register under this company.
      </p>
      <form onSubmit={submit} className="grid sm:grid-cols-2 gap-4">
        <div className="sm:col-span-2">
          <label className="field-label">Name</label>
          <input required className="field-input" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
        </div>
        <div>
          <label className="field-label">Website</label>
          <input className="field-input" value={form.website} onChange={(e) => setForm({ ...form, website: e.target.value })} />
        </div>
        <div>
          <label className="field-label">Location</label>
          <input className="field-input" value={form.location} onChange={(e) => setForm({ ...form, location: e.target.value })} />
        </div>
        <div className="sm:col-span-2">
          <label className="field-label">Description</label>
          <textarea className="field-textarea" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        </div>
        <div className="sm:col-span-2">
          <Notice tone="error">{error}</Notice>
          {created && <Notice tone="success">Created — Company ID is {created.id}.</Notice>}
        </div>
        <div className="sm:col-span-2">
          <button disabled={loading} className="btn-primary">
            {loading ? 'Filing…' : 'Create company'}
          </button>
        </div>
      </form>
    </div>
  )
}
