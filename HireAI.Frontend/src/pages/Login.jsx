import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { Notice } from '../components/Atoms'

export default function Login() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [form, setForm] = useState({ email: '', password: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const res = await login(form.email, form.password)
    setLoading(false)
    if (res.ok) {
      navigate(location.state?.from || '/', { replace: true })
    } else {
      setError(res.error)
    }
  }

  return (
    <div className="max-w-md mx-auto px-5 py-16">
      <p className="font-mono text-[10px] tracking-[0.2em] uppercase text-brass mb-2">Access — Case File</p>
      <h1 className="text-3xl font-display font-semibold mb-1">Sign in</h1>
      <p className="text-muted text-sm mb-8">Enter your credentials to open your dossier.</p>

      <form onSubmit={handleSubmit} className="file-card p-6 space-y-4">
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
            className="field-input"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            placeholder="••••••••"
          />
        </div>

        <Notice tone="error">{error}</Notice>

        <button type="submit" disabled={loading} className="btn-primary w-full">
          {loading ? 'Verifying…' : 'Sign in'}
        </button>
      </form>

      <p className="text-sm text-muted mt-5 text-center">
        No dossier yet?{' '}
        <Link to="/register" className="text-brass hover:underline">
          Register
        </Link>
      </p>
    </div>
  )
}
