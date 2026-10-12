// InterviewPilot .NET 10 Web API Client

export const API_BASE_URL =
  (typeof process !== "undefined" && process.env?.VITE_API_URL) ||
  "http://localhost:5055/api";

export type ApiJob = {
  id: string;
  source: string;
  title: string;
  companyName: string;
  description: string;
  location: string;
  isRemote: boolean;
  employmentType: number;
  salaryMin?: number | null;
  salaryMax?: number | null;
  postedDate: string;
  applicationUrl: string;
  applicationMethod: number;
  skills: string[];
};

export type CandidateProfile = {
  id: string;
  userId: string;
  summary: string;
  yearsExperience: number;
  noticePeriodDays: number;
  salaryExpectationMin?: number;
  salaryExpectationMax?: number;
  currency: string;
  remotePreference: number;
  preferredLocations: string[];
  targetRoles: string[];
  excludedCompanies: string[];
  skills: {
    name: string;
    category: string;
    yearsExperience: number;
    proficiency: number;
  }[];
};

export type ApplicationItem = {
  id: string;
  userId: string;
  jobId: string;
  jobTitle: string;
  companyName: string;
  status: number;
  matchScore?: number | null;
  applicationMethod: number;
  externalApplicationId?: string | null;
  notes?: string | null;
  appliedAt?: string | null;
  createdAt: string;
};

export type MatchResult = {
  jobId: string;
  jobTitle: string;
  companyName: string;
  breakdown: {
    overallScore: number;
    technicalSkillMatch: number;
    semanticSimilarity: number;
    experienceMatch: number;
    roleMatch: number;
    locationMatch: number;
    salaryMatch: number;
    preferenceMatch: number;
    matchedSkills: string[];
    missingSkills: string[];
    summary: string;
  };
};

export type TailoredResume = {
  jobId: string;
  jobTitle: string;
  companyName: string;
  tailoredProfessionalSummary: string;
  highlightedKeySkills: string[];
  experienceBullets: { company: string; role: string; optimizedBullets: string[] }[];
  predictedAtsScore: number;
};

export type CoverLetter = {
  jobId: string;
  jobTitle: string;
  companyName: string;
  tone: string;
  fullCoverLetter: string;
};

export type InterviewSession = {
  sessionId: string;
  jobId: string;
  roleTitle: string;
  companyName: string;
  readinessScore: number;
  feedbackSummary: string;
  questions: {
    questionId: string;
    questionText: string;
    category: string;
    modelStarAnswer?: string;
  }[];
};

export type AnswerEvaluation = {
  questionId: string;
  score: number;
  feedback: string;
  strengths: string;
  areasOfImprovement: string;
  modelAnswerRecommendation: string;
};

export type CandidateAnalytics = {
  totalApplications: number;
  shortlistedCount: number;
  applicationsReadyCount: number;
  appliedCount: number;
  interviewsCount: number;
  offersCount: number;
  responseRatePercentage: number;
  interviewRatePercentage: number;
  averageMatchScore: number;
  mostDemandedSkills: { skillName: string; jobCount: number }[];
};

export const api = {
  async getJobs(params?: {
    role?: string;
    location?: string;
    minSalary?: number;
    page?: number;
  }): Promise<ApiJob[]> {
    const query = new URLSearchParams();
    if (params?.role) query.set("role", params.role);
    if (params?.location) query.set("location", params.location);
    if (params?.minSalary) query.set("minSalary", params.minSalary.toString());
    if (params?.page) query.set("page", params.page.toString());

    const res = await fetch(`${API_BASE_URL}/jobs?${query.toString()}`);
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async syncJobs(): Promise<{ newJobsIngested: number; duplicatesSkipped: number }> {
    const res = await fetch(`${API_BASE_URL}/jobs/sync`, { method: "POST" });
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async saveJob(jobId: string): Promise<void> {
    await fetch(`${API_BASE_URL}/jobs/${jobId}/save`, { method: "POST" });
  },

  async getSavedJobs(): Promise<ApiJob[]> {
    const res = await fetch(`${API_BASE_URL}/jobs/saved`);
    if (!res.ok) return [];
    return res.json();
  },

  async getProfile(): Promise<CandidateProfile> {
    const res = await fetch(`${API_BASE_URL}/profile`);
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async updateProfile(profile: Partial<CandidateProfile>): Promise<CandidateProfile> {
    const res = await fetch(`${API_BASE_URL}/profile`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(profile),
    });
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async uploadResume(file: File): Promise<any> {
    const form = new FormData();
    form.append("file", file);
    const res = await fetch(`${API_BASE_URL}/profile/resume`, {
      method: "POST",
      body: form,
    });
    if (!res.ok) throw new Error(`Upload error: ${res.statusText}`);
    return res.json();
  },

  async evaluateMatch(jobId: string): Promise<MatchResult> {
    const res = await fetch(`${API_BASE_URL}/matching/evaluate/${jobId}`, { method: "POST" });
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async tailorResume(jobId: string): Promise<TailoredResume> {
    const res = await fetch(`${API_BASE_URL}/ai/resume-tailor?jobId=${jobId}`, { method: "POST" });
    if (!res.ok) throw new Error(`Tailor error: ${res.statusText}`);
    return res.json();
  },

  async generateCoverLetter(jobId: string, tone = "Professional"): Promise<CoverLetter> {
    const res = await fetch(`${API_BASE_URL}/ai/cover-letter?jobId=${jobId}&tone=${tone}`, { method: "POST" });
    if (!res.ok) throw new Error(`Cover letter error: ${res.statusText}`);
    return res.json();
  },

  async answerQuestion(jobId: string, question: string): Promise<string> {
    const res = await fetch(`${API_BASE_URL}/ai/application-answer?jobId=${jobId}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ question }),
    });
    if (!res.ok) throw new Error(`Answer error: ${res.statusText}`);
    const data = await res.json();
    return data.suggestedAnswer;
  },

  async getApplications(status?: number): Promise<ApplicationItem[]> {
    const url = status !== undefined ? `${API_BASE_URL}/applications?status=${status}` : `${API_BASE_URL}/applications`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async createApplication(jobId: string, method = 0): Promise<ApplicationItem> {
    const res = await fetch(`${API_BASE_URL}/applications?jobId=${jobId}&method=${method}`, { method: "POST" });
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async updateApplicationStatus(id: string, status: number, notes?: string): Promise<ApplicationItem> {
    const res = await fetch(`${API_BASE_URL}/applications/${id}/status`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ status, notes }),
    });
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async detectAts(careerUrl: string): Promise<string> {
    const res = await fetch(`${API_BASE_URL}/companies/detect-ats?careerUrl=${encodeURIComponent(careerUrl)}`, { method: "POST" });
    if (!res.ok) return "Unknown";
    const data = await res.json();
    return data.atsProvider;
  },

  async generateMockInterview(jobId: string): Promise<InterviewSession> {
    const res = await fetch(`${API_BASE_URL}/interview/generate/${jobId}`, { method: "POST" });
    if (!res.ok) throw new Error(`Interview error: ${res.statusText}`);
    return res.json();
  },

  async evaluateAnswer(questionId: string, answer: string): Promise<AnswerEvaluation> {
    const res = await fetch(`${API_BASE_URL}/interview/evaluate/${questionId}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ candidateAnswer: answer }),
    });
    if (!res.ok) throw new Error(`Evaluation error: ${res.statusText}`);
    return res.json();
  },

  async getAnalytics(): Promise<CandidateAnalytics> {
    const res = await fetch(`${API_BASE_URL}/analytics`);
    if (!res.ok) throw new Error(`Analytics error: ${res.statusText}`);
    return res.json();
  },
};
