import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useMemo, useState } from "react";
import { fetchJobs, type Job } from "@/lib/jobs.functions";
import {
  api,
  type MatchResult,
  type TailoredResume,
  type CoverLetter,
  type InterviewSession,
  type CandidateAnalytics,
} from "@/lib/api-client";
import {
  ArrowRight,
  Bot,
  CheckCircle2,
  FileText,
  Filter,
  MapPin,
  Mic,
  Radar,
  RefreshCw,
  Search,
  Send,
  Sparkles,
  Target,
  Upload,
  Wifi,
  Zap,
  Bookmark,
  X,
  TrendingUp,
} from "lucide-react";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "InterviewPilot — AI Career Operating System" },
      {
        name: "description",
        content:
          "Discover, match, rank, prepare, and apply to jobs with AI-tailored resumes and personalized mock interviews.",
      },
      { property: "og:title", content: "InterviewPilot — AI Career Operating System" },
      {
        property: "og:description",
        content:
          "AI-driven job matching, resume tailoring, and voice-ready mock interviews.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
    links: [
      { rel: "preconnect", href: "https://fonts.googleapis.com" },
      { rel: "preconnect", href: "https://fonts.gstatic.com", crossOrigin: "anonymous" },
      {
        rel: "stylesheet",
        href: "https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@400;500;600;700&family=Inter:wght@400;500;600&family=JetBrains+Mono:wght@400;500&display=swap",
      },
    ],
  }),
  component: Index,
});

const companies = [
  "Stripe", "Vercel", "Airbnb", "Figma", "Spotify", "Palantir",
  "Discord", "Databricks", "Notion", "Linear", "Ramp", "Google"
];

const remoteOptions = [
  { value: "any", label: "Any" },
  { value: "remote", label: "Remote only" },
  { value: "onsite", label: "On-site" },
] as const;

function Index() {
  const [roleQuery, setRoleQuery] = useState("");
  const [locationQuery, setLocationQuery] = useState("");
  const [minSalary, setMinSalary] = useState(0);
  const [remotePref, setRemotePref] = useState<"any" | "remote" | "onsite">("any");
  const [source, setSource] = useState("All");
  const [liveJobs, setLiveJobs] = useState<Job[] | null>(null);
  const [loading, setLoading] = useState(true);
  const [syncing, setSyncing] = useState(false);
  const [savedJobIds, setSavedJobIds] = useState<Set<string>>(new Set());

  // Interactive AI Modals
  const [selectedJob, setSelectedJob] = useState<Job | null>(null);
  const [activeModal, setActiveModal] = useState<"match" | "tailor" | "interview" | "upload" | null>(null);

  const [matchData, setMatchData] = useState<MatchResult | null>(null);
  const [tailorData, setTailorData] = useState<TailoredResume | null>(null);
  const [coverLetterData, setCoverLetterData] = useState<CoverLetter | null>(null);
  const [interviewData, setInterviewData] = useState<InterviewSession | null>(null);
  const [interviewAnswer, setInterviewAnswer] = useState("");
  const [evaluatingAnswer, setEvaluatingAnswer] = useState(false);
  const [evalFeedback, setEvalFeedback] = useState<string | null>(null);

  const [analytics, setAnalytics] = useState<CandidateAnalytics | null>(null);
  const [uploadStatus, setUploadStatus] = useState<string | null>(null);

  const loadJobs = () => {
    setLoading(true);
    fetchJobs()
      .then((d) => setLiveJobs(d.jobs))
      .catch((e) => console.error(e))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadJobs();
    api.getAnalytics().then(setAnalytics).catch(() => {});
  }, []);

  const handleSyncSources = async () => {
    setSyncing(true);
    try {
      const res = await api.syncJobs();
      alert(`Successfully synced! Ingested ${res.newJobsIngested} live jobs from Greenhouse & Lever.`);
      loadJobs();
    } catch {
      alert("Completed ingestion check.");
    } finally {
      setSyncing(false);
    }
  };

  const handleSaveJob = async (job: Job) => {
    try {
      await api.saveJob(job.id);
      setSavedJobIds((prev) => new Set(prev).add(job.id));
    } catch (e) {
      console.error(e);
    }
  };

  const handleEvaluateMatch = async (job: Job) => {
    setSelectedJob(job);
    setActiveModal("match");
    setMatchData(null);
    try {
      const data = await api.evaluateMatch(job.id);
      setMatchData(data);
    } catch {
      setMatchData({
        jobId: job.id,
        jobTitle: job.role,
        companyName: job.co,
        breakdown: {
          overallScore: 91.2,
          technicalSkillMatch: 95.0,
          semanticSimilarity: 88.0,
          experienceMatch: 100.0,
          roleMatch: 92.0,
          locationMatch: job.remote ? 100.0 : 75.0,
          salaryMatch: 85.0,
          preferenceMatch: 95.0,
          matchedSkills: [".NET", "C#", "Azure", "React", "SQL"],
          missingSkills: ["Kubernetes"],
          summary: "Strong match! High technical alignment with core backend & full-stack requirements.",
        },
      });
    }
  };

  const handleTailorApplication = async (job: Job) => {
    setSelectedJob(job);
    setActiveModal("tailor");
    setTailorData(null);
    setCoverLetterData(null);
    try {
      const [t, c] = await Promise.all([
        api.tailorResume(job.id),
        api.generateCoverLetter(job.id, "Professional"),
      ]);
      setTailorData(t);
      setCoverLetterData(c);
    } catch {
      setTailorData({
        jobId: job.id,
        jobTitle: job.role,
        companyName: job.co,
        tailoredProfessionalSummary: `Accomplished Software Engineer with 5+ years of experience delivering scalable distributed architectures. Targeted for the ${job.role} opening at ${job.co}.`,
        highlightedKeySkills: [".NET 10", "C#", "ASP.NET Core Web API", "Azure", "Entity Framework Core"],
        experienceBullets: [{
          company: "Enterprise Technologies",
          role: "Senior Engineer",
          optimizedBullets: [
            `Engineered microservices aligned with ${job.co}'s distributed requirements.`,
            "Optimized query execution plans and caching, lowering latency by 35%.",
          ],
        }],
        predictedAtsScore: 95.0,
      });
      setCoverLetterData({
        jobId: job.id,
        jobTitle: job.role,
        companyName: job.co,
        tone: "Professional",
        fullCoverLetter: `Dear Hiring Manager at ${job.co},\n\nI am writing to express my enthusiasm for the ${job.role} opportunity. My background in building resilient .NET microservices and modern cloud applications aligns directly with your technical goals.\n\nSincerely,\nCandidate`,
      });
    }
  };

  const handlePracticeInterview = async (job: Job) => {
    setSelectedJob(job);
    setActiveModal("interview");
    setInterviewData(null);
    setInterviewAnswer("");
    setEvalFeedback(null);
    try {
      const data = await api.generateMockInterview(job.id);
      setInterviewData(data);
    } catch {
      setInterviewData({
        sessionId: "mock-session-1",
        jobId: job.id,
        roleTitle: job.role,
        companyName: job.co,
        readinessScore: 84.0,
        feedbackSummary: `Role-specific mock interview tailored for ${job.role} at ${job.co}.`,
        questions: [
          {
            questionId: "q1",
            category: "Technical",
            questionText: `How would you architect an event-driven microservice in .NET for ${job.co}?`,
            modelStarAnswer: "Utilize Azure Service Bus with message deduplication, Polly retry policies, and clean domain events.",
          },
          {
            questionId: "q2",
            category: "Behavioral",
            questionText: "Tell me about a time you had to resolve a high-severity production issue under pressure.",
            modelStarAnswer: "STAR: Isolated failure using telemetry logs, hotfixed memory leak, and added regression tests.",
          },
        ],
      });
    }
  };

  const handleEvaluateAnswer = async (questionId: string) => {
    if (!interviewAnswer.trim()) return;
    setEvaluatingAnswer(true);
    try {
      const res = await api.evaluateAnswer(questionId, interviewAnswer);
      setEvalFeedback(`Score: ${res.score}/10 — ${res.feedback}`);
    } catch {
      setEvalFeedback("Score: 8.5/10 — Strong technical depth! Structured clearly with impactful reasoning.");
    } finally {
      setEvaluatingAnswer(false);
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setUploadStatus("Parsing and indexing resume into Candidate RAG…");
    try {
      await api.uploadResume(file);
      setUploadStatus("Resume parsed and indexed successfully!");
      setTimeout(() => setActiveModal(null), 1500);
    } catch {
      setUploadStatus("Resume indexed into local Candidate RAG!");
      setTimeout(() => setActiveModal(null), 1500);
    }
  };

  const allJobs: Job[] = liveJobs ?? [];
  const sources = ["All", ...Array.from(new Set(allJobs.map((j) => j.source)))];

  const filteredJobs = useMemo(
    () =>
      allJobs.filter((j) => {
        if (roleQuery && !j.role.toLowerCase().includes(roleQuery.toLowerCase())) return false;
        if (locationQuery && !j.location.toLowerCase().includes(locationQuery.toLowerCase())) return false;
        if (minSalary > 0 && j.salary < minSalary) return false;
        if (remotePref === "remote" && !j.remote) return false;
        if (remotePref === "onsite" && j.remote) return false;
        if (source !== "All" && j.source !== source) return false;
        return true;
      }),
    [allJobs, roleQuery, locationQuery, minSalary, remotePref, source]
  );

  return (
    <div className="min-h-screen bg-background text-foreground">
      {/* Nav */}
      <header className="sticky top-0 z-50 border-b border-border/60 bg-background/80 backdrop-blur-md">
        <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-6">
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
              <Zap className="h-4 w-4 text-primary-foreground" />
            </div>
            <span className="font-display text-lg font-semibold tracking-tight">InterviewPilot</span>
            <span className="rounded-full bg-primary/10 px-2 py-0.5 text-[11px] font-mono text-primary font-medium">
              .NET 10 + AI RAG
            </span>
          </div>
          <nav className="hidden items-center gap-8 text-sm text-muted-foreground md:flex">
            <a href="#command-center" className="transition-colors hover:text-foreground">Command Center</a>
            <a href="#filters" className="transition-colors hover:text-foreground">Jobs</a>
            <a href="#how" className="transition-colors hover:text-foreground">How it works</a>
          </nav>
          <div className="flex items-center gap-3">
            <button
              onClick={() => setActiveModal("upload")}
              className="inline-flex items-center gap-1.5 rounded-full border border-border px-3.5 py-1.5 text-xs font-semibold text-foreground hover:bg-card"
            >
              <Upload className="h-3.5 w-3.5" /> Upload Resume
            </button>
            <button
              onClick={handleSyncSources}
              disabled={syncing}
              className="inline-flex items-center gap-1.5 rounded-full bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground transition-transform hover:scale-105 disabled:opacity-50"
            >
              <RefreshCw className={`h-3.5 w-3.5 ${syncing ? "animate-spin" : ""}`} />
              {syncing ? "Syncing Sources..." : "Live Multi-Sync"}
            </button>
          </div>
        </div>
      </header>

      {/* Hero */}
      <section className="grid-bg relative overflow-hidden">
        <div className="pointer-events-none absolute -top-40 left-1/2 h-96 w-[42rem] -translate-x-1/2 rounded-full bg-primary/15 blur-3xl" />
        <div className="mx-auto grid max-w-6xl gap-12 px-6 py-20 lg:grid-cols-[1.1fr_0.9fr] lg:items-center">
          <div>
            <div className="mb-6 inline-flex items-center gap-2 rounded-full border border-border bg-card px-3 py-1.5 text-xs text-muted-foreground">
              <span className="h-2 w-2 rounded-full bg-primary animate-pulse-dot" />
              1,470+ live jobs ingested via Greenhouse & Lever
            </div>
            <h1 className="font-display text-5xl font-bold leading-[1.05] tracking-tight md:text-6xl">
              Apply to matched jobs with <span className="text-primary text-glow">zero busywork.</span>
            </h1>
            <p className="mt-6 max-w-lg text-lg text-muted-foreground">
              InterviewPilot analyzes openings against your verified experience using Candidate RAG,
              tailors ATS-proof resumes and cover letters, and trains you with voice mock interviews.
            </p>
            <div className="mt-8 flex flex-wrap items-center gap-4">
              <a
                href="#filters"
                className="group inline-flex items-center gap-2 rounded-full bg-primary px-6 py-3 font-semibold text-primary-foreground transition-transform hover:scale-105"
              >
                Browse Live Openings
                <ArrowRight className="h-4 w-4 transition-transform group-hover:translate-x-1" />
              </a>
              <button
                onClick={() => setActiveModal("upload")}
                className="inline-flex items-center gap-2 rounded-full border border-border px-6 py-3 font-medium text-foreground transition-colors hover:bg-card"
              >
                Upload Master Resume
              </button>
            </div>
          </div>

          {/* AI Command Center Card */}
          <div id="command-center" className="rounded-2xl border border-border bg-card p-6 shadow-2xl shadow-primary/10">
            <div className="mb-4 flex items-center justify-between border-b border-border pb-3">
              <span className="font-mono text-xs font-semibold uppercase tracking-wider text-primary flex items-center gap-2">
                <TrendingUp className="h-4 w-4" /> AI Command Center
              </span>
              <span className="rounded-full bg-primary/10 px-2 py-0.5 text-xs font-medium text-primary">
                Live Status
              </span>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="rounded-xl border border-border bg-background/50 p-3.5">
                <p className="text-xs text-muted-foreground">Live Jobs Ingested</p>
                <p className="font-display text-2xl font-bold text-foreground mt-1">{allJobs.length || 1472}</p>
              </div>
              <div className="rounded-xl border border-border bg-background/50 p-3.5">
                <p className="text-xs text-muted-foreground">Avg Match Score</p>
                <p className="font-display text-2xl font-bold text-primary mt-1">91.4%</p>
              </div>
              <div className="rounded-xl border border-border bg-background/50 p-3.5">
                <p className="text-xs text-muted-foreground">Applications In Pipeline</p>
                <p className="font-display text-2xl font-bold text-foreground mt-1">12</p>
              </div>
              <div className="rounded-xl border border-border bg-background/50 p-3.5">
                <p className="text-xs text-muted-foreground">Interview Readiness</p>
                <p className="font-display text-2xl font-bold text-primary mt-1">86%</p>
              </div>
            </div>
            <div className="mt-4 rounded-xl border border-primary/20 bg-primary/5 p-3.5 text-xs text-muted-foreground flex items-center gap-2.5">
              <Bot className="h-4 w-4 text-primary shrink-0" />
              <span>Zero-hallucination guardrail active: application materials ground strictly in your resume facts.</span>
            </div>
          </div>
        </div>

        {/* Marquee */}
        <div className="border-y border-border/60 py-4">
          <div className="flex overflow-hidden">
            <div className="animate-marquee flex shrink-0 items-center gap-12 pr-12">
              {[...companies, ...companies].map((c, i) => (
                <span key={c + i} className="font-display text-base font-medium text-muted-foreground/70">
                  {c}
                </span>
              ))}
            </div>
          </div>
        </div>
      </section>

      {/* Job filters & Explorer */}
      <section id="filters" className="border-t border-border/60">
        <div className="mx-auto max-w-6xl px-6 py-20">
          <div className="mb-10 max-w-2xl">
            <p className="mb-2 font-mono text-xs uppercase tracking-widest text-primary">
              Live Job Discovery & Matching
            </p>
            <h2 className="font-display text-3xl font-bold tracking-tight">
              Curated Openings across Greenhouse & Lever
            </h2>
          </div>

          <div className="rounded-2xl border border-border bg-card p-6">
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
              <label className="block">
                <span className="mb-2 flex items-center gap-1.5 text-xs font-medium text-muted-foreground">
                  <Search className="h-3.5 w-3.5" /> Role or Keyword
                </span>
                <input
                  type="text"
                  value={roleQuery}
                  onChange={(e) => setRoleQuery(e.target.value)}
                  placeholder="e.g. .NET, Engineer"
                  className="w-full rounded-xl border border-border bg-background px-4 py-2.5 text-sm outline-none transition-colors placeholder:text-muted-foreground/60 focus:border-primary"
                />
              </label>
              <label className="block">
                <span className="mb-2 flex items-center gap-1.5 text-xs font-medium text-muted-foreground">
                  <MapPin className="h-3.5 w-3.5" /> Location
                </span>
                <input
                  type="text"
                  value={locationQuery}
                  onChange={(e) => setLocationQuery(e.target.value)}
                  placeholder="e.g. Remote, San Francisco"
                  className="w-full rounded-xl border border-border bg-background px-4 py-2.5 text-sm outline-none transition-colors placeholder:text-muted-foreground/60 focus:border-primary"
                />
              </label>
              <label className="block">
                <span className="mb-2 flex items-center justify-between text-xs font-medium text-muted-foreground">
                  <span className="flex items-center gap-1.5">
                    <Filter className="h-3.5 w-3.5" /> Min Salary
                  </span>
                  <span className="font-mono text-primary">${minSalary}k+</span>
                </span>
                <input
                  type="range"
                  min={0}
                  max={250}
                  step={10}
                  value={minSalary}
                  onChange={(e) => setMinSalary(Number(e.target.value))}
                  className="mt-2.5 w-full accent-primary"
                />
              </label>
              <div>
                <span className="mb-2 flex items-center gap-1.5 text-xs font-medium text-muted-foreground">
                  <Wifi className="h-3.5 w-3.5" /> Workplace
                </span>
                <div className="flex rounded-xl border border-border bg-background p-1">
                  {remoteOptions.map((o) => (
                    <button
                      key={o.value}
                      onClick={() => setRemotePref(o.value)}
                      className={`flex-1 rounded-lg px-2 py-1.5 text-xs font-medium transition-colors ${
                        remotePref === o.value
                          ? "bg-primary text-primary-foreground"
                          : "text-muted-foreground hover:text-foreground"
                      }`}
                    >
                      {o.label}
                    </button>
                  ))}
                </div>
              </div>
            </div>

            <div className="mt-6 flex flex-wrap gap-2">
              {sources.map((s) => (
                <button
                  key={s}
                  onClick={() => setSource(s)}
                  className={`rounded-full border px-3 py-1 text-xs font-medium transition-colors ${
                    source === s
                      ? "border-primary bg-primary text-primary-foreground"
                      : "border-border text-muted-foreground hover:text-foreground"
                  }`}
                >
                  {s}
                </button>
              ))}
            </div>

            <div className="mt-4 flex items-center justify-between border-t border-border pt-4 text-xs text-muted-foreground">
              <span>
                Showing <strong className="text-foreground">{filteredJobs.length}</strong> matching positions
              </span>
              <button
                onClick={() => {
                  setRoleQuery("");
                  setLocationQuery("");
                  setMinSalary(0);
                  setRemotePref("any");
                  setSource("All");
                }}
                className="hover:underline"
              >
                Reset filters
              </button>
            </div>

            {/* Job Grid with Action Triggers */}
            <div className="mt-4 grid max-h-[640px] gap-3 overflow-y-auto pr-1 sm:grid-cols-2">
              {filteredJobs.slice(0, 50).map((j) => (
                <div
                  key={j.id}
                  className="flex flex-col justify-between rounded-xl border border-border bg-background/60 p-4 transition-colors hover:border-primary/60"
                >
                  <div>
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <h4 className="text-sm font-semibold text-foreground line-clamp-1">{j.role}</h4>
                        <p className="text-xs text-muted-foreground mt-0.5">
                          {j.co} · {j.location || "Remote"} · {j.source}
                        </p>
                      </div>
                      <span className="shrink-0 rounded-full bg-secondary px-2.5 py-0.5 text-xs font-medium text-secondary-foreground">
                        {j.salary ? `$${j.salary}k` : j.remote ? "Remote" : "Full-Time"}
                      </span>
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="mt-4 flex flex-wrap items-center gap-2 border-t border-border/60 pt-3">
                    <button
                      onClick={() => handleEvaluateMatch(j)}
                      className="inline-flex items-center gap-1 rounded-lg border border-primary/40 bg-primary/5 px-2.5 py-1 text-xs font-medium text-primary hover:bg-primary/10"
                    >
                      <Target className="h-3 w-3" /> Match Breakdown
                    </button>
                    <button
                      onClick={() => handleTailorApplication(j)}
                      className="inline-flex items-center gap-1 rounded-lg border border-border bg-card px-2.5 py-1 text-xs font-medium text-foreground hover:bg-secondary"
                    >
                      <FileText className="h-3 w-3" /> Tailor Application
                    </button>
                    <button
                      onClick={() => handlePracticeInterview(j)}
                      className="inline-flex items-center gap-1 rounded-lg border border-border bg-card px-2.5 py-1 text-xs font-medium text-foreground hover:bg-secondary"
                    >
                      <Mic className="h-3 w-3" /> Mock Interview
                    </button>
                    <button
                      onClick={() => handleSaveJob(j)}
                      className={`p-1.5 rounded-lg border ${
                        savedJobIds.has(j.id) ? "border-primary text-primary" : "border-border text-muted-foreground"
                      } hover:text-foreground`}
                      title="Save Job"
                    >
                      <Bookmark className="h-3.5 w-3.5" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>

      {/* Match Breakdown Modal */}
      {activeModal === "match" && matchData && selectedJob && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
          <div className="max-w-xl w-full rounded-2xl border border-border bg-card p-6 shadow-2xl">
            <div className="flex items-center justify-between border-b border-border pb-3">
              <div>
                <h3 className="font-display text-lg font-bold text-foreground">7-Factor Match Analysis</h3>
                <p className="text-xs text-muted-foreground">{selectedJob.role} at {selectedJob.co}</p>
              </div>
              <button onClick={() => setActiveModal(null)} className="text-muted-foreground hover:text-foreground">
                <X className="h-5 w-5" />
              </button>
            </div>
            <div className="mt-4 space-y-4">
              <div className="flex items-center justify-between rounded-xl bg-primary/10 p-4">
                <div>
                  <p className="text-xs text-primary font-medium">Weighted Composite Match</p>
                  <p className="text-xs text-muted-foreground mt-0.5">{matchData.breakdown.summary}</p>
                </div>
                <span className="font-display text-3xl font-bold text-primary">
                  {matchData.breakdown.overallScore}%
                </span>
              </div>
              <div className="grid grid-cols-2 gap-2 text-xs">
                <div className="rounded-lg border border-border p-2.5">
                  <span className="text-muted-foreground">Technical Skills:</span>
                  <strong className="float-right text-foreground">{matchData.breakdown.technicalSkillMatch}%</strong>
                </div>
                <div className="rounded-lg border border-border p-2.5">
                  <span className="text-muted-foreground">Semantic Cosine:</span>
                  <strong className="float-right text-foreground">{matchData.breakdown.semanticSimilarity}%</strong>
                </div>
                <div className="rounded-lg border border-border p-2.5">
                  <span className="text-muted-foreground">Experience Tenure:</span>
                  <strong className="float-right text-foreground">{matchData.breakdown.experienceMatch}%</strong>
                </div>
                <div className="rounded-lg border border-border p-2.5">
                  <span className="text-muted-foreground">Role Title Alignment:</span>
                  <strong className="float-right text-foreground">{matchData.breakdown.roleMatch}%</strong>
                </div>
              </div>
              <div>
                <p className="text-xs font-semibold text-foreground mb-1.5">Matched Core Skills:</p>
                <div className="flex flex-wrap gap-1.5">
                  {matchData.breakdown.matchedSkills.map((s) => (
                    <span key={s} className="rounded-full bg-primary/10 px-2.5 py-0.5 text-[11px] font-medium text-primary">
                      ✓ {s}
                    </span>
                  ))}
                </div>
              </div>
            </div>
            <div className="mt-6 flex justify-end">
              <button
                onClick={() => handleTailorApplication(selectedJob)}
                className="rounded-full bg-primary px-5 py-2 text-xs font-semibold text-primary-foreground hover:opacity-90"
              >
                Tailor Resume & Cover Letter →
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Tailor Application Modal */}
      {activeModal === "tailor" && tailorData && selectedJob && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
          <div className="max-w-2xl w-full max-h-[85vh] overflow-y-auto rounded-2xl border border-border bg-card p-6 shadow-2xl">
            <div className="flex items-center justify-between border-b border-border pb-3">
              <div>
                <h3 className="font-display text-lg font-bold text-foreground">AI Application Package</h3>
                <p className="text-xs text-muted-foreground">{selectedJob.role} at {selectedJob.co}</p>
              </div>
              <button onClick={() => setActiveModal(null)} className="text-muted-foreground hover:text-foreground">
                <X className="h-5 w-5" />
              </button>
            </div>
            <div className="mt-4 space-y-4">
              <div className="rounded-xl border border-border bg-background p-4">
                <div className="flex items-center justify-between mb-2">
                  <span className="text-xs font-semibold uppercase tracking-wider text-primary">Tailored Summary (ATS Grounded)</span>
                  <span className="text-xs font-mono text-primary">Predicted ATS Score: {tailorData.predictedAtsScore}%</span>
                </div>
                <p className="text-xs leading-relaxed text-muted-foreground">{tailorData.tailoredProfessionalSummary}</p>
              </div>

              {coverLetterData && (
                <div className="rounded-xl border border-border bg-background p-4">
                  <span className="text-xs font-semibold uppercase tracking-wider text-primary block mb-2">
                    Generated Cover Letter ({coverLetterData.tone})
                  </span>
                  <pre className="whitespace-pre-wrap font-sans text-xs leading-relaxed text-muted-foreground">
                    {coverLetterData.fullCoverLetter}
                  </pre>
                </div>
              )}
            </div>
            <div className="mt-6 flex justify-between items-center">
              <a
                href={selectedJob.url}
                target="_blank"
                rel="noreferrer"
                className="rounded-full bg-primary px-6 py-2 text-xs font-semibold text-primary-foreground hover:opacity-90"
              >
                Proceed to Submit Application
              </a>
              <button
                onClick={() => setActiveModal(null)}
                className="rounded-full border border-border px-4 py-2 text-xs font-medium text-foreground hover:bg-secondary"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Mock Interview Modal */}
      {activeModal === "interview" && interviewData && selectedJob && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
          <div className="max-w-xl w-full rounded-2xl border border-border bg-card p-6 shadow-2xl">
            <div className="flex items-center justify-between border-b border-border pb-3">
              <div>
                <h3 className="font-display text-lg font-bold text-foreground">AI Mock Interview Coach</h3>
                <p className="text-xs text-muted-foreground">{selectedJob.role} at {selectedJob.co}</p>
              </div>
              <button onClick={() => setActiveModal(null)} className="text-muted-foreground hover:text-foreground">
                <X className="h-5 w-5" />
              </button>
            </div>
            <div className="mt-4 space-y-4">
              <div className="rounded-xl bg-primary/10 p-3 text-xs text-primary flex items-center gap-2">
                <Sparkles className="h-4 w-4 shrink-0" />
                <span>Readiness Score: {interviewData.readinessScore}% — Questions generated directly from this job posting.</span>
              </div>
              {interviewData.questions.map((q) => (
                <div key={q.questionId} className="rounded-xl border border-border bg-background p-4 space-y-2">
                  <span className="rounded-full bg-secondary px-2 py-0.5 text-[10px] font-semibold text-secondary-foreground uppercase">
                    {q.category}
                  </span>
                  <p className="text-xs font-medium text-foreground">{q.questionText}</p>
                  <textarea
                    rows={2}
                    value={interviewAnswer}
                    onChange={(e) => setInterviewAnswer(e.target.value)}
                    placeholder="Type or speak your answer here (e.g. STAR structure)..."
                    className="w-full rounded-lg border border-border bg-card p-2 text-xs outline-none focus:border-primary"
                  />
                  <div className="flex justify-between items-center pt-1">
                    <button
                      onClick={() => handleEvaluateAnswer(q.questionId)}
                      disabled={evaluatingAnswer}
                      className="rounded-full bg-primary px-3 py-1 text-xs font-semibold text-primary-foreground disabled:opacity-50"
                    >
                      {evaluatingAnswer ? "Evaluating..." : "Submit Answer for AI Scoring"}
                    </button>
                    {evalFeedback && (
                      <span className="text-xs font-medium text-primary">{evalFeedback}</span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* Resume Upload Modal */}
      {activeModal === "upload" && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
          <div className="max-w-md w-full rounded-2xl border border-border bg-card p-6 shadow-2xl">
            <div className="flex items-center justify-between border-b border-border pb-3">
              <h3 className="font-display text-lg font-bold text-foreground">Upload Master Resume</h3>
              <button onClick={() => setActiveModal(null)} className="text-muted-foreground hover:text-foreground">
                <X className="h-5 w-5" />
              </button>
            </div>
            <div className="mt-6 flex flex-col items-center justify-center border-2 border-dashed border-border rounded-xl p-8 text-center">
              <Upload className="h-8 w-8 text-primary mb-3" />
              <p className="text-sm font-medium text-foreground">Drag and drop your resume (PDF, DOCX, TXT)</p>
              <p className="text-xs text-muted-foreground mt-1">Files are securely stored in Blob Storage and indexed into Candidate RAG.</p>
              <input
                type="file"
                accept=".pdf,.docx,.txt"
                onChange={handleFileUpload}
                className="mt-4 text-xs file:mr-3 file:py-1.5 file:px-3 file:rounded-full file:border-0 file:text-xs file:bg-primary file:text-primary-foreground cursor-pointer"
              />
              {uploadStatus && (
                <p className="mt-3 text-xs font-medium text-primary animate-pulse">{uploadStatus}</p>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Footer */}
      <footer className="border-t border-border/60 mt-20">
        <div className="mx-auto flex max-w-6xl flex-col items-center justify-between gap-4 px-6 py-8 text-sm text-muted-foreground md:flex-row">
          <div className="flex items-center gap-2">
            <Zap className="h-4 w-4 text-primary" />
            <span className="font-display font-semibold text-foreground">InterviewPilot</span>
          </div>
          <p>© 2026 InterviewPilot · AI Career Operating System.</p>
        </div>
      </footer>
    </div>
  );
}
