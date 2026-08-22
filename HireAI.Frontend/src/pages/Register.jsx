import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { Notice } from '../components/Atoms'

export default function Register() {
  const { register, rememberName } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({
    fullName: '',
    email: '',
    password: '',
    role: 'Candidate',
    companyId: '',
  })
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const dto = {
      fullName: form.fullName,
      email: form.email,
      password: form.password,
      role: form.role,
      companyId: form.companyId ? Number(form.companyId) : null,
    }
    const res = await register(dto)
    setLoading(false)
    if (res.ok) {
      rememberName(form.fullName)
      setSuccess(true)
      setTimeout(() => navigate('/login'), 1200)
    } else {
      setError(res.error)
    }
  }

  return (
    <div className="max-w-md mx-auto px-5 py-16">
      <p className="font-mono text-[10px] tracking-[0.2em] uppercase text-brass mb-2">Open a new case file</p>
      <h1 className="text-3xl font-display font-semibold mb-1">Register</h1>
      <p className="text-muted text-sm mb-8">Create your account as a candidate or as HR staff.</p>

      <form onSubmit={handleSubmit} className="file-card p-6 space-y-4">
        <div className="grid grid-cols-2 gap-2">
          {['Candidate', 'HR'].map((r) => (
            <button
              type="button"
              key={r}
              onClick={() => setForm({ ...form, role: r })}
              className={`py-2.5 rounded-[2px] border text-sm font-body transition-colors ${
                form.role === r
                  ? 'border-brass text-brass bg-brass/10'
                  : 'border-line text-mutedCool hover:text-ivory'
              }`}
            >
              {r === 'Candidate' ? 'I\u2019m a Candidate' : 'I\u2019m HR staff'}
            </button>
          ))}
        </div>

        <div>
          <label className="field-label">Full name</label>
          <input
            required
            className="field-input"
            value={form.fullName}
            onChange={(e) => setForm({ ...form, fullName: e.target.value })}
            placeholder="Ali Younes"
          />
        </div>
        <div>
          <label className="field-label">Email</label>
          <input
            type="email"
            required
            className="field-input"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            placeholder="you@company.com"
          />
        </div>
        <div>
          <label className="field-label">Password</label>
          <input
            type="password"
            required
            minLength={6}
            className="field-input"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            placeholder="••••••••"
          />
        </div>

        {form.role === 'HR' && (
          <div>
            <label className="field-label">Company ID (optional)</label>
            <input
              type="number"
              className="field-input"
              value={form.companyId}
              onChange={(e) => setForm({ ...form, companyId: e.target.value })}
              placeholder="e.g. 1"
            />
            <p className="text-xs text-muted mt-1.5 leading-relaxed">
              Enter the numeric ID of your company if you know it. If your company
              isn't registered yet, leave this blank — you can create it from the
              Companies page after signing in and share its ID with your admin to
              have your account linked.
            </p>
          </div>
        )}

        <Notice tone="error">{error}</Notice>
        {success && <Notice tone="success">Registered. Redirecting to sign in…</Notice>}

        <button type="submit" disabled={loading} className="btn-primary w-full">
          {loading ? 'Filing…' : 'Create account'}
        </button>
      </form>

      <p className="text-sm text-muted mt-5 text-center">
        Already registered?{' '}
        <Link to="/login" className="text-brass hover:underline">
          Sign in
        </Link>
      </p>
    </div>
  )
}
