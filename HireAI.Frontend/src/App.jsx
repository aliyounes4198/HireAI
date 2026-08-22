import { Routes, Route } from 'react-router-dom'
import Navbar from './components/Navbar'
import ProtectedRoute from './components/ProtectedRoute'

import Login from './pages/Login'
import Register from './pages/Register'
import NotFound from './pages/NotFound'

import JobsList from './pages/public/JobsList'
import JobDetail from './pages/public/JobDetail'
import CompaniesList from './pages/public/CompaniesList'
import CompanyDetail from './pages/public/CompanyDetail'

import CandidateProfile from './pages/candidate/CandidateProfile'
import Resumes from './pages/candidate/Resumes'
import MyApplications from './pages/candidate/MyApplications'

import PostJob from './pages/hr/PostJob'
import HRApplications from './pages/hr/HRApplications'
import HRMatches from './pages/hr/HRMatches'

export default function App() {
  return (
    <div className="min-h-screen flex flex-col">
      <Navbar />
      <main className="flex-1">
        <Routes>
          <Route path="/" element={<JobsList />} />
          <Route path="/jobs/:id" element={<JobDetail />} />
          <Route
            path="/companies"
            element={
              <ProtectedRoute>
                <CompaniesList />
              </ProtectedRoute>
            }
          />
          <Route
            path="/companies/:id"
            element={
              <ProtectedRoute>
                <CompanyDetail />
              </ProtectedRoute>
            }
          />

          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          <Route
            path="/profile"
            element={
              <ProtectedRoute roles={['Candidate']}>
                <CandidateProfile />
              </ProtectedRoute>
            }
          />
          <Route
            path="/resumes"
            element={
              <ProtectedRoute roles={['Candidate']}>
                <Resumes />
              </ProtectedRoute>
            }
          />
          <Route
            path="/applications"
            element={
              <ProtectedRoute roles={['Candidate']}>
                <MyApplications />
              </ProtectedRoute>
            }
          />

          <Route
            path="/hr/post-job"
            element={
              <ProtectedRoute roles={['HR']}>
                <PostJob />
              </ProtectedRoute>
            }
          />
          <Route
            path="/hr/applications"
            element={
              <ProtectedRoute roles={['HR']}>
                <HRApplications />
              </ProtectedRoute>
            }
          />
          <Route
            path="/hr/matches"
            element={
              <ProtectedRoute roles={['HR']}>
                <HRMatches />
              </ProtectedRoute>
            }
          />

          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
      <footer className="border-t border-line py-6 mt-10">
        <p className="max-w-6xl mx-auto px-5 font-mono text-[10px] tracking-[0.14em] uppercase text-muted">
          HireAI — Case File System
        </p>
      </footer>
    </div>
  )
}
