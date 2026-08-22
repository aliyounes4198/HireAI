import axios from 'axios'

const baseURL = import.meta.env.VITE_API_URL || 'http://localhost:5156/api'

export const api = axios.create({ baseURL })

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('hireai_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      // Token invalid/expired — clear it so the app returns to login state.
      localStorage.removeItem('hireai_token')
      localStorage.removeItem('hireai_user')
    }
    return Promise.reject(err)
  }
)

// Pulls a readable message out of ASP.NET's various error shapes
// (plain string, { message }, or ModelState validation errors object).
export function extractError(err, fallback = 'Something went wrong.') {
  const data = err?.response?.data
  if (!data) return err?.message || fallback
  if (typeof data === 'string') return data
  if (data.message) return data.message
  if (data.title) return data.title
  if (data.errors && typeof data.errors === 'object') {
    const first = Object.values(data.errors)[0]
    if (Array.isArray(first)) return first[0]
  }
  return fallback
}
