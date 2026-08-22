# HireAI — Frontend

A React (Vite + Tailwind) frontend for the HireAI ASP.NET Core backend: job board,
candidate profile/resume/apply flow with live AI match scoring, and an HR dashboard
for posting jobs and reviewing applications and AI matches.

## 1. Install & run

```bash
npm install
cp .env.example .env   # adjust VITE_API_URL if your API runs elsewhere
npm run dev
```

Opens on `http://localhost:5173`. It expects the API at `http://localhost:5156/api`
by default (the `http` launch profile in your backend's `launchSettings.json`).

## 2. Required backend change: enable CORS

Your `Program.cs` doesn't currently configure CORS, so the browser will block every
request from the frontend's origin. Add this before `var app = builder.Build();`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

And add `app.UseCors("Frontend");` right after `app.UseHttpsRedirection();` and
before `app.UseAuthentication();`. Without this, every fetch from the frontend
will fail with a CORS error even though the API itself is working fine.

## 3. How the app maps to your API

- **Auth** — `POST /Auth/register`, `POST /Auth/login`. The JWT is decoded
  client-side (no `/me` endpoint exists) to read role/id/email for routing and
  role gating.
- **Jobs** — public browsing via `GET /Job` and `GET /Job/open` (both
  `AllowAnonymous`). Posting a job requires an HR account linked to a company.
- **Companies** — `CompanyController` requires auth on every route (including
  `GET`), so the Companies pages sit behind login for any signed-in user.
- **Candidate flow** — profile (`CandidateProfile`), resume upload/analysis
  (`Resume`, `AIAnalysis`), and applying (`Application`, which auto-triggers
  `JobMatch` server-side and returns the score inline).
- **HR flow** — `Job` (create), `Application/company`,
  `JobMatch/company` for the review dashboards.

## 4. One backend gap worth knowing about

There's no endpoint that lets an HR account attach itself to a company after
registering — `Register` accepts an optional `companyId`, but it must already
exist. The Register page explains this: create a company from the Companies
page after signing in, note its numeric ID, and use that ID next time (or have
an admin update the `Users.CompanyId` column directly) since there's currently
no self-service way to link an existing HR account to a company.

## 5. Design

The UI leans into the "case file" idea — dossier-style cards, a mono/serif type
pairing (IBM Plex Mono + Fraunces + Inter), and a rotated circular "match seal"
as the signature element for AI scores, echoing a rubber ink stamp on a file.
