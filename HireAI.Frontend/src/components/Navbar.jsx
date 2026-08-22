import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function Item({ to, children, end }) {
  return (
    <NavLink
      to={to}
      end={end}
      className={({ isActive }) =>
        `px-3 py-1.5 text-sm font-body transition-colors rounded-[2px] ${
          isActive ? 'text-brass' : 'text-mutedCool hover:text-ivory'
        }`
      }
    >
      {children}
    </NavLink>
  )
}

export default function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  return (
    <header className="sticky top-0 z-40 border-b border-line bg-ink/90 backdrop-blur">
      <div className="max-w-6xl mx-auto px-5 h-16 flex items-center justify-between gap-6">
        <NavLink to="/" className="flex items-center gap-2.5 shrink-0">
          <span className="w-8 h-8 rounded-[3px] bg-brass text-ink2 font-display font-bold text-lg flex items-center justify-center">
            H
          </span>
          <span className="font-display text-lg tracking-tight">
            Hire<span className="text-brass">AI</span>
          </span>
          <span className="hidden sm:inline font-mono text-[9px] uppercase tracking-[0.2em] text-muted border border-line px-1.5 py-0.5 rounded-[2px] ml-1">
            Case&nbsp;Files
          </span>
        </NavLink>

        <nav className="flex items-center gap-1 overflow-x-auto">
          <Item to="/" end>Jobs</Item>
          <Item to="/companies">Companies</Item>

          {user?.role === 'Candidate' && (
            <>
              <Item to="/profile">Profile</Item>
              <Item to="/resumes">Resumes</Item>
              <Item to="/applications">My Applications</Item>
            </>
          )}

          {user?.role === 'HR' && (
            <>
              <Item to="/hr/post-job">Post a Job</Item>
              <Item to="/hr/applications">Applications</Item>
              <Item to="/hr/matches">AI Matches</Item>
            </>
          )}
        </nav>

        <div className="flex items-center gap-3 shrink-0">
          {user ? (
            <>
              <div className="hidden md:flex flex-col items-end leading-tight">
                <span className="text-xs text-ivory">{user.email}</span>
                <span className="font-mono text-[9px] tracking-[0.14em] uppercase text-brass">{user.role}</span>
              </div>
              <button
                onClick={() => {
                  logout()
                  navigate('/')
                }}
                className="btn-secondary !px-3 !py-1.5 text-xs"
              >
                Sign out
              </button>
            </>
          ) : (
            <>
              <NavLink to="/login" className="btn-secondary !px-3 !py-1.5 text-xs">
                Sign in
              </NavLink>
              <NavLink to="/register" className="btn-primary !px-3 !py-1.5 text-xs">
                Register
              </NavLink>
            </>
          )}
        </div>
      </div>
    </header>
  )
}
