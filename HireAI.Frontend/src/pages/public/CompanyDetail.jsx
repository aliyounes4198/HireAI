import { useEffect, useState } from 'react'

import { Link, useParams } from 'react-router-dom'

import { CompanyAPI, JobAPI } from '../../api/endpoints'

import { extractError } from '../../api/client'

import { Loader, Notice, EmptyState } from '../../components/Atoms'

import StatusChip from '../../components/StatusChip'

export default function CompanyDetail() {
  const { id } = useParams()
  const [company, setCompany] = useState(null)
  const [jobs, setJobs] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    CompanyAPI.get(id)
      .then((res) => setCompany(res.data))
      .catch((err) => setError(extractError(err, 'Company not found.')))
    JobAPI.list()
      .then((res) => setJobs(res.data.filter((j) => j.companyId === Number(id))))
      .catch(() => setJobs([]))
  }, [id])

  if (error) {
    return (
      <div className="max-w-3xl mx-auto px-5 py-16">
        <Notice tone="error">{error}</Notice>
      </div>
    )
  }
  if (!company) return <Loader label="Opening file" />

  return (
    <div className="max-w-3xl mx-auto px-5 py-12">
      <Link to="/companies" className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted hover:text-brass">
        ← Back to registry
      </Link>

      <div className="file-card p-7 pt-8 mt-4">
        <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted mb-2">ID #{company.id}</p>
        <h1 className="text-2xl md:text-3xl font-display font-semibold mb-2">{company.name}</h1>
        {company.location && <p className="text-sm text-mutedCool mb-4">{company.location}</p>}
        {company.description && <p className="text-sm text-ivory/90 leading-relaxed mb-4">{company.description}</p>}
        {company.website && (
          <a href={company.website} target="_blank" rel="noreferrer" className="text-sm text-brass hover:underline">
            {company.website}
          </a>
        )}
      </div>

      <h2 className="font-display text-xl font-semibold mt-8 mb-4">Open postings</h2>
      {jobs === null && <Loader label="Loading postings" />}
      {jobs && jobs.length === 0 && <EmptyState title="No postings from this company yet" />}
      <div className="grid sm:grid-cols-2 gap-4">
        {jobs?.map((job) => (
          <Link key={job.id} to={`/jobs/${job.id}`} className="file-card p-5 pt-6 hover:border-brass/60 transition-colors group">
            <div className="flex items-start justify-between gap-2 mb-2">
              <p className="font-mono text-[10px] tracking-[0.14em] uppercase text-muted">
                File #{String(job.id).padStart(4, '0')}
              </p>
              <StatusChip status={job.status} />
            </div>
            <h3 className="font-display text-lg font-semibold group-hover:text-brass transition-colors">{job.title}</h3>
            {job.location && <p className="text-sm text-mutedCool mt-1">{job.location}</p>}
          </Link>
        ))}
      </div>
    </div>
  )
}
