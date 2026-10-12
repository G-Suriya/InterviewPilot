// InterviewPilot .NET 10 Web API Client

export const API_BASE_URL =
  (typeof process !== "undefined" && process.env?.VITE_API_URL) ||
  "http://localhost:5000/api";

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
  status: number; // 0=New, 1=Shortlisted, 2=ApplicationReady, 3=Applied, 4=Assessment, 5=Interview, 6=Offer, etc.
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

  async getJobById(id: string): Promise<ApiJob> {
    const res = await fetch(`${API_BASE_URL}/jobs/${id}`);
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },

  async syncJobs(source = "Greenhouse"): Promise<{ source: string; newJobsIngested: number }> {
    const res = await fetch(`${API_BASE_URL}/jobs/sync?source=${source}`, { method: "POST" });
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
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

  async evaluateMatch(jobId: string): Promise<MatchResult> {
    const res = await fetch(`${API_BASE_URL}/matching/evaluate/${jobId}`, { method: "POST" });
    if (!res.ok) throw new Error(`API error: ${res.statusText}`);
    return res.json();
  },
};
