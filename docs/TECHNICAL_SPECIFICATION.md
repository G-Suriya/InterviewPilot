# AI Job Intelligence & Application Portal
## Implementation & Technical Design Document

- **Version**: 1.0
- **Status**: Implementation Blueprint
- **Target Stack**: .NET 10 + Angular + Azure + Azure OpenAI
- **Primary Goal**: Discover, analyze, rank, prepare, and—where officially supported—submit job applications across multiple job sources.

---

## 1. Executive Summary

The proposed platform is an AI-powered job discovery and application management system that aggregates opportunities from multiple authorized job sources and company career portals.

The system allows a candidate to define:
- Job titles
- Skills
- Experience
- Preferred locations
- Remote/hybrid preference
- Salary expectations
- Employment type
- Target companies
- Excluded companies
- Notice period
- Other career preferences

The platform will then:
1. Discover jobs.
2. Normalize job information.
3. Remove duplicates.
4. Extract skills and requirements using AI.
5. Compare jobs against the candidate's profile.
6. Generate a match score.
7. Rank opportunities.
8. Tailor the candidate's resume.
9. Generate cover letters.
10. Prepare application answers.
11. Track applications.
12. Submit through authorized APIs where supported.
13. Otherwise provide an assisted application workflow.
14. Learn from application/interview outcomes.

> [!IMPORTANT]
> The platform is **not** designed around unrestricted scraping or automated browser actions. Each source uses an official API, permitted feed, public ATS endpoint, or an appropriate user-assisted workflow.

---

## 2. Product Vision: The AI Career Operating System

Instead of the candidate manually performing repetitive manual steps:
```
Search jobs → Read JD → Compare resume → Decide whether to apply → Modify resume → Write cover letter → Answer questions → Apply → Track application → Prepare for interview
```

The platform automates this into an intelligent pipeline:
```
Candidate Profile
        ↓
Job Discovery
        ↓
AI Understanding
        ↓
AI Matching
        ↓
Opportunity Ranking
        ↓
Resume / Cover Letter
        ↓
Application Preparation
        ↓
Approval
        ↓
Application
        ↓
Tracking
        ↓
Interview Preparation
        ↓
Outcome Learning
```

---

## 3. Objectives

### 3.1 Primary Objectives
- Aggregate jobs from multiple authorized sources.
- Support location, skill-based, and natural-language job search.
- Create an AI-powered candidate profile and resume knowledge base (Candidate RAG).
- Match jobs against the candidate profile with deterministic and semantic scoring.
- Deduplicate job postings across sources.
- Prioritize high-quality opportunities.
- Generate job-specific application materials (tailored resume, cover letters, Q&A).
- Track applications via a Kanban board.
- Support authorized automated application submissions with human approval gates.
- Provide interview preparation and company intelligence.
- Analyze application performance and provide a feedback loop.

---

## 4. Initial Target Sources

### Job Portals
- Naukri
- LinkedIn
- Monster
- Indeed
- Dice
- Wellfound

### Freelance Platforms
- Upwork
- Freelancer
- Other permitted freelance platforms

### Hackathons & Challenges
- Devpost
- HackerEarth
- HackerRank events
- Company-sponsored hackathons

### Company Career Portals (ATS)
- Greenhouse
- Lever
- Workday
- Ashby
- SmartRecruiters
- Oracle Recruiting
- SAP SuccessFactors
- Company-specific career portals

### Core Architecture Rule: Adapter Pattern
Do not hard-code application logic around a particular website.
```
IJobSourceAdapter
        │
        ├── GreenhouseAdapter
        ├── LeverAdapter
        ├── UpworkAdapter
        ├── CompanyPortalAdapter
        └── FutureAdapter
```
This enables adding new sources without altering core platform logic.

---

## 5. Compliance-Aware Source Strategy

Three distinct integration modes ensure strict adherence to platform terms and policies:

```
Mode A — Official API:
Portal ──► Official API ──► Job Integration Service (Used whenever available)

Mode B — Public ATS / Feed:
Company ──► Public Job Board / ATS ──► API / Feed ──► Job Ingestion

Mode C — User-Assisted:
User provides job URL ──► AI analyzes job ──► AI prepares application ──► User reviews & submits
```

---

## 6. High-Level Architecture

```
                         ┌────────────────────┐
                         │      Angular       │
                         │    Web Portal      │
                         └─────────┬──────────┘
                                   │
                                   ▼
                         ┌────────────────────┐
                         │ Azure API Manager  │
                         └─────────┬──────────┘
                                   │
                         ┌─────────▼─────────┐
                         │   ASP.NET Core    │
                         │       APIs        │
                         └─────────┬─────────┘
                                   │
          ┌────────────────────────┼───────────────────────┐
          │                        │                       │
          ▼                        ▼                       ▼
   Profile Service           Job Service          Application Service
          │                        │                       │
          └────────────────────────┼───────────────────────┘
                                   │
                          Azure Service Bus
                                   │
          ┌────────────────────────┼────────────────────────┐
          ▼                        ▼                        ▼
   Ingestion Workers          AI Workers             Notification
          │                        │
          ▼                        ▼
   Source Adapters          Azure OpenAI
          │                        │
          ▼                        ▼
     Job Database          Azure AI Search
                                   │
                         ┌─────────┴──────────┐
                         ▼                    ▼
                    Candidate RAG          Job RAG
                         │
                         └─────────┬──────────┘
                                   ▼
                              AI Matching
                                   │
             ┌─────────────────────┼────────────────────┐
             ▼                     ▼                    ▼
        Azure SQL             Cosmos DB            Blob Storage
             │
             ▼
           Redis
```

---

## 7. Technology Stack

- **Frontend**: Angular 19+, TypeScript, Angular Material, RxJS
- **Backend**: .NET 10, ASP.NET Core Web API, Entity Framework Core, Dapper
- **Messaging**: Azure Service Bus
- **Databases**: Azure SQL (Relational data), Azure Cosmos DB (Unstructured AI documents), Azure Redis (Caching & Rate Limiting)
- **AI & Search**: Azure OpenAI (GPT-4o, Embeddings), Azure AI Search (Vector & Hybrid RAG)
- **Storage**: Azure Blob Storage (Resumes, artifacts, documents)
- **Security**: Microsoft Entra ID, Managed Identity, Azure Key Vault, OAuth 2.0
- **DevOps**: GitHub / Azure DevOps, Docker, Terraform, Azure Container Apps (initial) / AKS (scale)
- **Observability**: Application Insights, Azure Monitor, OpenTelemetry

---

## 8. Functional Modules

1. **Authentication & Identity**: Microsoft Entra ID, JWT, OAuth 2.0
2. **Candidate Profile**: Work experience, skills, preferences, salary, notice period
3. **Resume Management**: Parsing, chunking, and vectorization
4. **Job Discovery**: Source adapters and scheduled ingestion
5. **Job Search**: Natural-language and facet-based querying
6. **Job Matching**: Weighted deterministic + semantic match scoring
7. **AI Analysis**: Skill extraction, difficulty estimation, requirement breakdown
8. **Saved Jobs & Bookmarks**: User-curated opportunities
9. **Application Assistant**: Tailored resumes, cover letters, Q&A generation
10. **Application Tracking**: Kanban workflow with status transition lifecycle
11. **Interview Preparation**: Role-specific and company-specific mock interviews
12. **Company Intelligence**: Tech stacks, hiring patterns, ATS identification
13. **Notifications**: In-app and email alerts for matches and milestones
14. **Analytics**: Conversion funnels, skill gap analytics, source performance
15. **Administration**: System health, queue monitoring, worker management
16. **Source Management**: Adapter configurations, rate limits, health checks

---

## 9. Candidate Profile Schema

```
Candidate Profile
 ├── Personal information
 ├── Professional summary
 ├── Experience (Companies, Roles, Projects, Achievements)
 ├── Skills (Languages, Frameworks, Tools, Proficiencies)
 ├── Education & Certifications
 ├── Preferred roles & Seniority
 ├── Preferred locations & Remote preferences
 ├── Salary expectation & Currency
 ├── Notice period & Availability
 └── Target and Excluded companies
```

---

## 10. Resume Management Pipeline

```
Resume (PDF/DOCX/TXT)
       ↓
Blob Storage
       ↓
Document Parser
       ↓
Text Extraction
       ↓
Section Detection
       ↓
Skill Extraction
       ↓
Experience Extraction
       ↓
Embedding Generation
       ↓
Azure AI Search Index
```

---

## 11. Candidate RAG (Retrieval-Augmented Generation)

The candidate's background forms a vectorized knowledge base:
- Master Resumes & Versions
- Projects & Repositories
- Work Experience & Impact Metrics
- Certifications & Technical Skills
- Historical Application Responses & Interview Q&A

**Query Resolution Example**:
*"What experience does the candidate have with Azure Service Bus?"*
The system performs hybrid vector search over the candidate index and retrieves grounded facts before synthesizing an answer.

---

## 12. Preventing AI Hallucination

> [!CAUTION]
> The application generator operates under zero-hallucination constraints:
> - **Forbidden to invent**: Experience, skills, certifications, employers, projects, years of tenure, job responsibilities, or degrees.
> - **Allowed actions**: Rewrite, summarize, reorder, elevate tone, emphasize relevant real experience, and generate context-accurate explanations grounded strictly in candidate records.

---

## 13. Job Ingestion Architecture

```
Scheduler ──► Source Manager ──► Source Adapter ──► Raw Job ──► Normalizer ──► Validator ──► Deduplication ──► Job Database ──► AI Analysis
```

### Core Adapter Interfaces

```csharp
public interface IJobSourceAdapter
{
    Task<IReadOnlyList<RawJob>> FetchJobsAsync(
        JobSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<JobDetails?> GetJobAsync(
        string sourceJobId,
        CancellationToken cancellationToken);
}

public interface IApplicationAdapter
{
    Task<ApplicationResult> SubmitAsync(
        ApplicationRequest request,
        CancellationToken cancellationToken);
}
```

---

## 14. Normalized Job Model

```csharp
public class Job
{
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public string SourceJobId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public DateTime PostedDate { get; set; }
    public string ApplicationUrl { get; set; } = string.Empty;
    public string ApplicationMethod { get; set; } = string.Empty;
}
```

---

## 15. Job Deduplication Pipeline

Jobs sourced across multiple boards are linked to a single canonical entity via multi-stage signal resolution:

```
1. Exact Requisition ID Match
          ↓ (if unresolved)
2. Exact Application URL Match
          ↓ (if unresolved)
3. Normalized Company + Normalized Title + Location
          ↓ (if unresolved)
4. Semantic Vector Similarity (Cosine Similarity >= 0.92)
```

---

## 16. Job AI Analysis Schema

```json
{
  "requiredSkills": ["C#", ".NET 10", "ASP.NET Core", "Azure Service Bus"],
  "preferredSkills": ["Angular", "Docker", "Terraform"],
  "experienceYears": 5,
  "education": ["Bachelor's in Computer Science or equivalent"],
  "locations": ["Bangalore", "Remote"],
  "salary": { "min": 2500000, "max": 3500000, "currency": "INR" },
  "employmentType": "FullTime",
  "responsibilities": ["Design distributed microservices", "Manage event-driven architecture"],
  "technologies": [".NET", "Azure", "Cosmos DB", "Redis"],
  "keywords": ["Clean Architecture", "DDD", "Enterprise"]
}
```

---

## 17. AI Matching Engine

The match score balances deterministic hard requirements and semantic understanding:

| Metric | Weight | Description |
| :--- | :--- | :--- |
| **Technical Skill Match** | 30% | Overlap of candidate skills with required and preferred skills |
| **Semantic Similarity** | 25% | Vector cosine similarity of candidate background to job description |
| **Experience Match** | 15% | Candidate tenure vs. minimum and preferred experience requirements |
| **Role & Seniority Match** | 10% | Alignment of current/target titles with role expectations |
| **Location Match** | 10% | Physical proximity or remote policy compliance |
| **Salary Match** | 5% | Candidate expectation vs. posted compensation range |
| **Preference Match** | 5% | Target company, industry, and notice period alignment |

---

## 18. Skill Ontology & Graph Relationships

Avoid naive keyword misses by grouping skills into ontologies and parent-child hierarchies:
- **.NET Ecosystem**: C#, ASP.NET Core, Web API, EF Core, LINQ, Dependency Injection
- **Azure Cloud**: Azure Functions, Azure Service Bus, AKS, Azure SQL, Key Vault, Application Insights
- **Frontend**: TypeScript, Angular, RxJS, NgRx, Tailwind CSS, Angular Material

---

## 19. Natural Language Search Processing

User query:
> *"Find senior .NET jobs in Chennai and Bangalore, remote allowed, minimum five years, Azure and Angular preferred, salary above ₹25 LPA."*

Converted via intent extraction into an exact structured query filter:
```json
{
  "roles": ["Senior .NET Developer", "Lead .NET Developer", "Full Stack Developer"],
  "locations": ["Chennai", "Bangalore"],
  "remoteAllowed": true,
  "experienceMin": 5,
  "skills": [".NET", "Azure", "Angular"],
  "salaryMin": 2500000
}
```
*Filtering is executed deterministically by Azure AI Search and SQL indexes, not by LLM inference.*

---

## 20. Application Assistant & Tailoring

For every target opportunity:
```
Job Opening ──► AI Analysis ──► Candidate Fit ──► Resume Tailoring ──► Cover Letter ──► Screening Q&A ──► Checklist
```

### Modes of Application Submission
- **Manual Mode**: AI prepares materials; candidate opens link and submits.
- **Assisted Mode**: AI prepares forms; candidate reviews, modifies, and confirms submission through integrated flow.
- **Authorized Automatic Mode**: Used **only** where an official API explicitly permits automated submissions and user-defined rules are fully satisfied:
  - Match score >= Threshold (e.g., 85%)
  - Required skills 100% satisfied
  - Location and compensation within accepted bounds
  - Company not in excluded list

---

## 21. Application Tracking Lifecycle (Kanban)

```
[ NEW ]
   ↓
[ SHORTLISTED ]
   ↓
[ APPLICATION READY ]
   ↓
[ APPLIED ]
   ↓
[ ASSESSMENT ]
   ↓
[ INTERVIEW ]
   ↓
[ OFFER ] ──► [ ACCEPTED ]
   │
   └──► [ REJECTED / WITHDRAWN / EXPIRED ]
```

---

## 22. Database Design

### Azure SQL (Relational Core)
- `Users` (UserId, Email, Name, CreatedAt)
- `CandidateProfiles` (ProfileId, UserId, Summary, YearsExperience, NoticePeriod, SalaryExpectation)
- `Skills` & `CandidateSkills` (SkillId, Name, Category, Proficiency, YearsExperience)
- `Companies` (CompanyId, Name, Website, CareerUrl, ATSProvider, Active)
- `Jobs` (JobId, Source, SourceJobId, CompanyId, Title, Description, Location, Remote, SalaryMin, SalaryMax, PostedDate, ApplicationUrl)
- `JobSkills` (JobId, SkillId, Required, Weight)
- `Applications` (ApplicationId, UserId, JobId, Status, MatchScore, ResumeVersionId, AppliedAt)

### Azure Cosmos DB (Semi-Structured Documents)
- `RawJobs`: Ingested payloads prior to normalization
- `AIJobAnalysis`: Extracted skills, responsibilities, difficulty metadata
- `ResumeAnalysis`: Parsed candidate sections and extracted tokens
- `ApplicationForms`: Dynamic questionnaires and proposed answers

### Azure Blob Storage
- `/resumes`: Master uploaded documents (PDF/DOCX)
- `/resume-versions`: Tailored output resumes
- `/cover-letters`: Generated cover letters
- `/certificates`: Candidate credentials

### Azure Redis
- Semantic query caching
- Rate limiting per source adapter
- Session state and distributed locking

### Azure Service Bus Queues
- `job-ingestion`
- `job-normalization`
- `job-analysis`
- `job-matching`
- `embedding-generation`
- `application-preparation`
- `application-submission`
- `notifications`

---

## 23. API Surface

```http
# Candidate Profile
GET    /api/profile
PUT    /api/profile
POST   /api/profile/skills
GET    /api/profile/resumes
POST   /api/profile/resumes

# Jobs
GET    /api/jobs
GET    /api/jobs/{id}
POST   /api/jobs/search
POST   /api/jobs/{id}/analyze
POST   /api/jobs/{id}/save
DELETE /api/jobs/{id}/save

# Matching
POST   /api/jobs/{id}/match
GET    /api/recommendations

# Applications
POST   /api/applications
GET    /api/applications
GET    /api/applications/{id}
PUT    /api/applications/{id}/status
POST   /api/applications/{id}/prepare
POST   /api/applications/{id}/submit

# AI Engine
POST   /api/ai/resume-tailor
POST   /api/ai/cover-letter
POST   /api/ai/application-answer
POST   /api/ai/interview-preparation
```

---

## 24. Company & Interview Intelligence

- **ATS Detection**: Automates identification of Greenhouse, Lever, Workday, Ashby, and SmartRecruiters from career URLs.
- **Interview Intelligence**:
  - Role-specific system design and coding questions
  - Behavioral questions evaluated via the STAR format
  - Pitch coaching, filler word feedback, and readiness scoring grounded in the exact target job description.

---

## 25. Cost Control & LLM Throttling

```
10,000 Ingested Jobs
        ↓ Deterministic SQL/Search Filters (location, salary, title)
2,000 Potential Candidates
        ↓ Vector Embedding Cosine Similarity
  300 Relevant Candidates
        ↓ LLM Deep Analysis & Match Scoring
   50 Top Ranked Opportunities
```

*This tiered funnel eliminates wasteful token consumption and reduces operational AI cost by over 95%.*

---

## 26. Development Roadmap & Sprints

- **Sprint 1**: Foundation & Scaffolding (.NET 10, Angular, Azure resources, Entra ID, DB setup)
- **Sprint 2**: Candidate Profile & Resume Management (Parser, Blob Storage, Skill extractor)
- **Sprint 3**: Job Discovery & Search Foundation (Jobs DB, search, company registry)
- **Sprint 4**: Ingestion Adapters (Greenhouse, Lever, public feeds, normalization, deduplication)
- **Sprint 5**: AI Analysis & Matching Engine (Embeddings, Azure OpenAI, weighted match scoring)
- **Sprint 6**: Candidate RAG & Search (Azure AI Search, semantic querying)
- **Sprint 7**: Application Assistant (Resume tailoring, cover letter generator, Q&A assistant)
- **Sprint 8**: Application Lifecycle (Kanban tracker, authorized API submissions, notifications)
- **Sprint 9**: Company Intelligence & ATS Auto-Detection
- **Sprint 10**: Interview Coach, Feedback Loop & Production Hardening

