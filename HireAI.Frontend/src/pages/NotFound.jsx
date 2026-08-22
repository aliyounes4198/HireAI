import { Link } from 'react-router-dom'

export default function NotFound() {
  return (
    <div className="max-w-md mx-auto px-5 py-24 text-center">
      <p className="font-mono text-[10px] tracking-[0.2em] uppercase text-brass mb-3">Error 404</p>
      <h1 className="text-3xl font-display font-semibold mb-2">File not found</h1>
      <p className="text-muted text-sm mb-6">This case file doesn't exist or was never opened.</p>
      <Link to="/" className="btn-primary">Back to the job board</Link>
    </div>
  )
}
