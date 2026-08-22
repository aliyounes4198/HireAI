import { api } from './client'

export const AuthAPI = {
  register: (dto) => api.post('/Auth/register', dto),
  login: (dto) => api.post('/Auth/login', dto),
}

export const JobAPI = {
  list: () => api.get('/Job'),
  listOpen: () => api.get('/Job/open'),
  get: (id) => api.get(`/Job/${id}`),
  create: (job) => api.post('/Job', job),
}

export const CompanyAPI = {
  list: () => api.get('/Company'),
  get: (id) => api.get(`/Company/${id}`),
  create: (company) => api.post('/Company', company),
}

export const CandidateProfileAPI = {
  get: () => api.get('/CandidateProfile'),
  create: (dto) => api.post('/CandidateProfile', dto),
  update: (dto) => api.put('/CandidateProfile', dto),
}

export const ResumeAPI = {
  list: () => api.get('/Resume'),
  upload: (file) => {
    const form = new FormData()
    form.append('file', file)
    return api.post('/Resume/upload', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },
  remove: (id) => api.delete(`/Resume/${id}`),
}

export const ApplicationAPI = {
  create: (application) => api.post('/Application', application),
  myApplications: () => api.get('/Application/my-applications'),
  companyApplications: () => api.get('/Application/company'),
  get: (id) => api.get(`/Application/${id}`),
  updateStatus: (id, status) => api.put(`/Application/${id}/status`, status, {
    headers: { 'Content-Type': 'application/json' },
  }),
}

export const JobMatchAPI = {
  match: (resumeId, jobId) => api.post(`/JobMatch/${resumeId}/${jobId}`),
  companyMatches: () => api.get('/JobMatch/company'),
}

export const AIAnalysisAPI = {
  analyze: (resumeId) => api.post(`/AIAnalysis/${resumeId}`),
}
