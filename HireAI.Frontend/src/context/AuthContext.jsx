import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { AuthAPI } from '../api/endpoints'
import { extractError } from '../api/client'

const AuthContext = createContext(null)

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
const NAMEID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'

// The backend issues standard ClaimTypes claims, which .NET serializes into
// their long XML-namespace form inside the JWT payload.
function decodeToken(token) {
  try {
    const payload = token.split('.')[1]
    const json = JSON.parse(
      decodeURIComponent(
        atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
          .split('')
          .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
          .join('')
      )
    )
    return {
      id: Number(json[NAMEID_CLAIM] ?? json.nameid ?? json.sub),
      role: json[ROLE_CLAIM] ?? json.role,
      email: json[EMAIL_CLAIM] ?? json.email,
      exp: json.exp,
    }
  } catch {
    return null
  }
}

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem('hireai_token'))
  const [fullName, setFullName] = useState(() => localStorage.getItem('hireai_name') || '')
  const [ready, setReady] = useState(true)

  const user = useMemo(() => {
    if (!token) return null
    const claims = decodeToken(token)
    if (!claims) return null
    if (claims.exp && claims.exp * 1000 < Date.now()) return null
    return { ...claims, fullName }
  }, [token, fullName])

  useEffect(() => {
    if (token) localStorage.setItem('hireai_token', token)
    else localStorage.removeItem('hireai_token')
  }, [token])

  async function login(email, password) {
    try {
      const res = await AuthAPI.login({ email, password })
      setToken(res.data.token)
      return { ok: true }
    } catch (err) {
      return { ok: false, error: extractError(err, 'Invalid email or password.') }
    }
  }

  async function register(dto) {
    try {
      await AuthAPI.register(dto)
      return { ok: true }
    } catch (err) {
      return { ok: false, error: extractError(err, 'Could not register.') }
    }
  }

  function logout() {
    setToken(null)
    setFullName('')
    localStorage.removeItem('hireai_name')
  }

  function rememberName(name) {
    setFullName(name)
    localStorage.setItem('hireai_name', name)
  }

  return (
    <AuthContext.Provider value={{ user, token, ready, login, register, logout, rememberName }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
