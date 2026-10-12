import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useMemo, useState } from "react";
import { fetchJobs, type Job } from "@/lib/jobs.functions";
import {
  ArrowRight,
  Bot,
  CheckCircle2,
  FileText,
  Filter,
  MapPin,
  Mic,
  Radar,
  Search,
  Send,
  Sparkles,
  Target,
  Wifi,
  Zap,
} from "lucide-react";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "ApplyPilot — Auto-Apply to Jobs & Ace Every Interview" },
      {
        name: "description",
        content:
          "ApplyPilot auto-applies to hundreds of matched jobs with tailored resumes and trains you with AI mock interviews. Land more interviews with zero busywork.",
      },
      { property: "og:title", content: "ApplyPilot — Auto-Apply to Jobs & Ace Every Interview" },
      {
        property: "og:description",
        content:
          "Auto-apply to matched jobs with tailored resumes and practice with AI mock interviews.",
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
  "Google",
  "Stripe",
  "Airbnb",
  "Figma",
  "Notion",
  "Shopify",
  "Datadog",
  "Vercel",
  "Linear",
  "Ramp",
];

const steps = [
  {
    icon: Radar,
    title: "We scan 50+ job boards",
    body: "Tell us your role, salary floor, and dealbreakers. Our matcher scans every new posting within minutes of it going live.",
  },
  {
    icon: FileText,
    title: "Resume rewritten per job",
    body: "Every application gets a tailored resume and cover letter tuned to the posting's keywords — ATS-safe, human-readable.",
  },
  {
    icon: Send,
    title: "Auto-applied for you",
    body: "We fill the forms, answer screening questions, and submit. You approve in one tap or let it run fully hands-free.",
  },
  {
    icon: Mic,
    title: "AI mock interviews",
    body: "When a recruiter bites, train with voice-based mock interviews built from that exact job description.",
  },
];

const stats = [
  { value: "12,400+", label: "interviews landed" },
  { value: "38x", label: "more applications sent" },
  { value: "9 days", label: "median time to first interview" },
  { value: "94%", label: "resume ATS pass rate" },
];

const jobs = [
  { role: "Senior Frontend Engineer", co: "Vercel", location: "Remote", salary: 185, remote: true },
  { role: "Product Designer", co: "Linear", location: "Remote", salary: 160, remote: true },
  { role: "Full-stack Developer", co: "Ramp", location: "New York", salary: 175, remote: false },
  { role: "React Engineer", co: "Notion", location: "San Francisco", salary: 170, remote: false },
  { role: "Backend Engineer", co: "Stripe", location: "Remote", salary: 190, remote: true },
  { role: "DevOps Engineer", co: "Datadog", location: "New York", salary: 165, remote: false },
  { role: "Mobile Engineer", co: "Airbnb", location: "San Francisco", salary: 180, remote: false },
  { role: "ML Engineer", co: "Google", location: "Remote", salary: 210, remote: true },
  { role: "Frontend Engineer", co: "Shopify", location: "Remote", salary: 150, remote: true },
  { role: "Design Engineer", co: "Figma", location: "New York", salary: 172, remote: false },
];

const remoteOptions = [
  { value: "any", label: "Any" },
  { value: "remote", label: "Remote only" },
  { value: "onsite", label: "On-site" },
] as const;

const plans = [
  {
    name: "Scout",
    price: "$0",
    tag: "Try it free",
    features: ["10 auto-applies / month", "1 resume profile", "Basic job matching", "Email support"],
    featured: false,
  },
  {
    name: "Pilot",
    price: "$29",
    tag: "Most popular",
    features: [
      "Unlimited auto-applies",
      "Tailored resume per job",
      "AI mock interviews",
      "Recruiter follow-up emails",
      "Priority support",
    ],
    featured: true,
  },
  {
    name: "Copilot+",
    price: "$79",
    tag: "White glove",
    features: [
      "Everything in Pilot",
      "Human career coach review",
      "Salary negotiation scripts",
      "LinkedIn profile overhaul",
    ],
    featured: false,
  },
];

function Index() {
  const [roleQuery, setRoleQuery] = useState("");
  const [locationQuery, setLocationQuery] = useState("");
  const [minSalary, setMinSalary] = useState(0);
  const [remotePref, setRemotePref] = useState<"any" | "remote" | "onsite">("any");
  const [source, setSource] = useState("All");
  const [liveJobs, setLiveJobs] = useState<Job[] | null>(null);
  const [loading, setLoading] = useState(true);

  const loadJobs = () => {
    setLoading(true);
    fetchJobs()
      .then((d) => setLiveJobs(d.jobs))
      .catch((e) => console.error(e))
      .finally(() => setLoading(false));
  };
  useEffect(loadJobs, []);

  const allJobs: Job[] =
    liveJobs ??
    jobs.map((j) => ({ ...j, id: j.role + j.co, url: "#", source: "Sample" }));
  const sources = ["All", ...Array.from(new Set(allJobs.map((j) => j.source)))];

  const filteredJobs = useMemo(
    () =>
      allJobs.filter((j) => {
        if (roleQuery && !j.role.toLowerCase().includes(roleQuery.toLowerCase())) return false;
        if (
          locationQuery &&
          !j.location.toLowerCase().includes(locationQuery.toLowerCase())
        )
          return false;
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
            <span className="font-display text-lg font-semibold tracking-tight">ApplyPilot</span>
          </div>
          <nav className="hidden items-center gap-8 text-sm text-muted-foreground md:flex">
            <a href="#how" className="transition-colors hover:text-foreground">How it works</a>
            <a href="#results" className="transition-colors hover:text-foreground">Results</a>
            <a href="#pricing" className="transition-colors hover:text-foreground">Pricing</a>
          </nav>
          <a
            href="#pricing"
            className="rounded-full bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground transition-transform hover:scale-105"
          >
            Start applying
          </a>
        </div>
      </header>

      {/* Hero */}
      <section className="grid-bg relative overflow-hidden">
        <div className="pointer-events-none absolute -top-40 left-1/2 h-96 w-[42rem] -translate-x-1/2 rounded-full bg-primary/15 blur-3xl" />
        <div className="mx-auto grid max-w-6xl gap-12 px-6 py-24 lg:grid-cols-[1.1fr_0.9fr] lg:items-center lg:py-32">
          <div>
            <div className="mb-6 inline-flex items-center gap-2 rounded-full border border-border bg-card px-3 py-1.5 text-xs text-muted-foreground">
              <span className="h-2 w-2 rounded-full bg-primary animate-pulse-dot" />
              2,318 jobs auto-applied in the last 24h
            </div>
            <h1 className="font-display text-5xl font-bold leading-[1.05] tracking-tight md:text-6xl">
              Apply to <span className="text-primary text-glow">200 jobs</span> while you sleep.
            </h1>
            <p className="mt-6 max-w-lg text-lg text-muted-foreground">
              ApplyPilot finds matching roles, rewrites your resume for each one, submits the
              application, then drills you with AI mock interviews. You just show up and get hired.
            </p>
            <div className="mt-8 flex flex-wrap items-center gap-4">
              <a
                href="#pricing"
                className="group inline-flex items-center gap-2 rounded-full bg-primary px-6 py-3 font-semibold text-primary-foreground transition-transform hover:scale-105"
              >
                Auto-apply free
                <ArrowRight className="h-4 w-4 transition-transform group-hover:translate-x-1" />
              </a>
              <a
                href="#how"
                className="inline-flex items-center gap-2 rounded-full border border-border px-6 py-3 font-medium text-foreground transition-colors hover:bg-card"
              >
                See how it works
              </a>
            </div>
            <p className="mt-4 text-xs text-muted-foreground">
              No credit card · Cancel anytime · Your data stays yours
            </p>
          </div>

          {/* Live feed card */}
          <div className="animate-float rounded-2xl border border-border bg-card p-5 shadow-2xl shadow-primary/10">
            <div className="mb-4 flex items-center justify-between">
              <span className="font-mono text-xs text-muted-foreground">LIVE APPLICATION FEED</span>
              <span className="flex items-center gap-1.5 rounded-full bg-primary/10 px-2 py-1 text-xs font-medium text-primary">
                <span className="h-1.5 w-1.5 rounded-full bg-primary animate-pulse-dot" />
                Active
              </span>
            </div>
            <div className="space-y-3">
              {[
                { role: "Senior Frontend Engineer", co: "Vercel", status: "Applied", t: "2m ago" },
                { role: "Product Designer", co: "Linear", status: "Resume tailored", t: "6m ago" },
                { role: "Full-stack Developer", co: "Ramp", status: "Interview booked", t: "18m ago" },
                { role: "React Engineer", co: "Notion", status: "Applied", t: "24m ago" },
              ].map((j) => (
                <div
                  key={j.role + j.co}
                  className="flex items-center justify-between rounded-xl border border-border bg-background/60 px-4 py-3"
                >
                  <div>
                    <p className="text-sm font-medium">{j.role}</p>
                    <p className="text-xs text-muted-foreground">{j.co} · {j.t}</p>
                  </div>
                  <span
                    className={`rounded-full px-2.5 py-1 text-xs font-medium ${
                      j.status === "Interview booked"
                        ? "bg-primary text-primary-foreground"
                        : "bg-secondary text-secondary-foreground"
                    }`}
                  >
                    {j.status}
                  </span>
                </div>
              ))}
            </div>
            <div className="mt-4 flex items-center gap-2 rounded-xl bg-primary/10 px-4 py-3 text-sm text-primary">
              <Bot className="h-4 w-4 shrink-0" />
              Mock interview for Ramp starts in 40 min — you're 82% ready.
            </div>
          </div>
        </div>

        {/* Marquee */}
        <div className="border-y border-border/60 py-5">
          <div className="flex overflow-hidden">
            <div className="animate-marquee flex shrink-0 items-center gap-12 pr-12">
              {[...companies, ...companies].map((c, i) => (
                <span key={c + i} className="font-display text-lg font-medium text-muted-foreground/70">
                  {c}
                </span>
              ))}
            </div>
          </div>
        </div>
      </section>

      {/* How it works */}
      <section id="how" className="mx-auto max-w-6xl px-6 py-24">
        <div className="mb-14 max-w-2xl">
          <p className="mb-3 font-mono text-xs uppercase tracking-widest text-primary">How it works</p>
          <h2 className="font-display text-4xl font-bold tracking-tight">
            From profile to offer letter, on autopilot
          </h2>
        </div>
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
          {steps.map((s, i) => (
            <div
              key={s.title}
              className="group rounded-2xl border border-border bg-card p-6 transition-colors hover:border-primary/50"
            >
              <div className="mb-5 flex items-center justify-between">
                <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-primary/10 text-primary transition-colors group-hover:bg-primary group-hover:text-primary-foreground">
                  <s.icon className="h-5 w-5" />
                </div>
                <span className="font-mono text-xs text-muted-foreground">0{i + 1}</span>
              </div>
              <h3 className="font-display text-lg font-semibold">{s.title}</h3>
              <p className="mt-2 text-sm leading-relaxed text-muted-foreground">{s.body}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Job filters */}
      <section id="filters" className="border-t border-border/60">
        <div className="mx-auto max-w-6xl px-6 py-24">
          <div className="mb-10 max-w-2xl">
            <p className="mb-3 font-mono text-xs uppercase tracking-widest text-primary">
              Focus your search
            </p>
            <h2 className="font-display text-4xl font-bold tracking-tight">
              Only apply where you'd actually say yes
            </h2>
            <p className="mt-4 text-muted-foreground">
              Set your filters once — ApplyPilot only auto-applies to jobs that match every one of
              them. Try them live:
            </p>
          </div>

          <div className="rounded-2xl border border-border bg-card p-6">
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
              <label className="block">
                <span className="mb-2 flex items-center gap-1.5 text-xs font-medium text-muted-foreground">
                  <Search className="h-3.5 w-3.5" /> Role
                </span>
                <input
                  type="text"
                  value={roleQuery}
                  onChange={(e) => setRoleQuery(e.target.value)}
                  placeholder="e.g. Frontend"
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
                  placeholder="e.g. New York"
                  className="w-full rounded-xl border border-border bg-background px-4 py-2.5 text-sm outline-none transition-colors placeholder:text-muted-foreground/60 focus:border-primary"
                />
              </label>
              <label className="block">
                <span className="mb-2 flex items-center justify-between text-xs font-medium text-muted-foreground">
                  <span className="flex items-center gap-1.5">
                    <Filter className="h-3.5 w-3.5" /> Min salary
                  </span>
                  <span className="font-mono text-primary">${minSalary}k+</span>
                </span>
                <input
                  type="range"
                  min={0}
                  max={220}
                  step={5}
                  value={minSalary}
                  onChange={(e) => setMinSalary(Number(e.target.value))}
                  className="mt-2.5 w-full accent-primary"
                />
              </label>
              <div>
                <span className="mb-2 flex items-center gap-1.5 text-xs font-medium text-muted-foreground">
                  <Wifi className="h-3.5 w-3.5" /> Remote
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

            <div className="mt-4 flex items-center justify-between border-t border-border pt-4">
              <p className="text-sm text-muted-foreground">
                {loading ? (
                  "Fetching live jobs from portals and career pages…"
                ) : (
                  <>
                    <span className="font-display font-semibold text-primary">
                      {filteredJobs.length}
                    </span>{" "}
                    of {allJobs.length} live jobs match your filters
                  </>
                )}
              </p>
              <div className="flex gap-4">
                <button
                  onClick={loadJobs}
                  className="text-xs font-medium text-primary hover:underline"
                >
                  Refresh
                </button>
                <button
                  onClick={() => {
                    setRoleQuery("");
                    setLocationQuery("");
                    setMinSalary(0);
                    setRemotePref("any");
                    setSource("All");
                  }}
                  className="text-xs font-medium text-muted-foreground transition-colors hover:text-foreground"
                >
                  Reset filters
                </button>
              </div>
            </div>

            <div className="mt-4 grid max-h-[520px] gap-3 overflow-y-auto pr-1 sm:grid-cols-2">
              {filteredJobs.length === 0 ? (
                <p className="col-span-full rounded-xl border border-border bg-background/60 px-4 py-6 text-center text-sm text-muted-foreground">
                  No jobs match — loosen a filter and we'll start applying again.
                </p>
              ) : (
                filteredJobs.slice(0, 100).map((j) => (
                  <a
                    key={j.id}
                    href={j.url}
                    target="_blank"
                    rel="noreferrer"
                    className="flex items-center justify-between gap-3 rounded-xl border border-border bg-background/60 px-4 py-3 transition-colors hover:border-primary"
                  >
                    <div className="min-w-0">
                      <p className="truncate text-sm font-medium">{j.role}</p>
                      <p className="truncate text-xs text-muted-foreground">
                        {j.co} · {j.location || "—"} · {j.source}
                      </p>
                    </div>
                    <span className="shrink-0 rounded-full bg-secondary px-2.5 py-1 text-xs font-medium text-secondary-foreground">
                      {j.salary ? `$${j.salary}k` : j.remote ? "Remote" : "Apply"}
                    </span>
                  </a>
                ))
              )}
            </div>
          </div>
        </div>
      </section>

      {/* Stats */}
      <section id="results" className="border-y border-border/60 bg-card/40">
        <div className="mx-auto grid max-w-6xl gap-8 px-6 py-16 sm:grid-cols-2 lg:grid-cols-4">
          {stats.map((s) => (
            <div key={s.label} className="text-center">
              <p className="font-display text-4xl font-bold text-primary">{s.value}</p>
              <p className="mt-1 text-sm text-muted-foreground">{s.label}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Interview prep */}
      <section className="mx-auto grid max-w-6xl items-center gap-12 px-6 py-24 lg:grid-cols-2">
        <div>
          <p className="mb-3 font-mono text-xs uppercase tracking-widest text-primary">Interview prep</p>
          <h2 className="font-display text-4xl font-bold tracking-tight">
            Practice the interview before you have it
          </h2>
          <p className="mt-4 text-muted-foreground">
            The moment an application gets a reply, ApplyPilot builds a mock interview from that
            exact job description — voice-based, scored, and brutally honest.
          </p>
          <ul className="mt-8 space-y-4">
            {[
              "Role-specific technical and behavioral questions",
              "Real-time feedback on filler words, pace, and clarity",
              "STAR-format answer coaching with model responses",
              "Readiness score that predicts your real interview outcome",
            ].map((f) => (
              <li key={f} className="flex items-start gap-3 text-sm">
                <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0 text-primary" />
                {f}
              </li>
            ))}
          </ul>
        </div>
        <div className="rounded-2xl border border-border bg-card p-6">
          <div className="mb-4 flex items-center gap-2">
            <Sparkles className="h-4 w-4 text-primary" />
            <span className="font-mono text-xs text-muted-foreground">MOCK INTERVIEW · RAMP · FULL-STACK</span>
          </div>
          <div className="space-y-4">
            <div className="rounded-xl bg-secondary p-4 text-sm">
              "Tell me about a time you shipped under a tight deadline. What did you cut?"
            </div>
            <div className="ml-8 rounded-xl bg-primary/10 p-4 text-sm text-muted-foreground">
              "At my last role we had 3 weeks to launch payments. I cut the multi-currency scope and
              shipped Stripe-only…"
            </div>
            <div className="flex items-center justify-between rounded-xl border border-primary/30 bg-primary/5 px-4 py-3">
              <div className="flex items-center gap-2 text-sm">
                <Target className="h-4 w-4 text-primary" />
                Answer score
              </div>
              <span className="font-display text-xl font-bold text-primary">8.4 / 10</span>
            </div>
            <p className="text-xs text-muted-foreground">
              Feedback: strong structure — add a measurable outcome to close the loop.
            </p>
          </div>
        </div>
      </section>

      {/* Pricing */}
      <section id="pricing" className="border-t border-border/60 bg-card/40">
        <div className="mx-auto max-w-6xl px-6 py-24">
          <div className="mb-14 text-center">
            <p className="mb-3 font-mono text-xs uppercase tracking-widest text-primary">Pricing</p>
            <h2 className="font-display text-4xl font-bold tracking-tight">
              Cheaper than one week unemployed
            </h2>
          </div>
          <div className="grid gap-6 md:grid-cols-3">
            {plans.map((p) => (
              <div
                key={p.name}
                className={`relative rounded-2xl border p-7 ${
                  p.featured
                    ? "border-primary bg-card shadow-2xl shadow-primary/15"
                    : "border-border bg-card"
                }`}
              >
                {p.featured && (
                  <span className="absolute -top-3 left-1/2 -translate-x-1/2 rounded-full bg-primary px-3 py-1 text-xs font-semibold text-primary-foreground">
                    {p.tag}
                  </span>
                )}
                <h3 className="font-display text-lg font-semibold">{p.name}</h3>
                <p className="mt-3">
                  <span className="font-display text-4xl font-bold">{p.price}</span>
                  <span className="text-sm text-muted-foreground"> / month</span>
                </p>
                <ul className="mt-6 space-y-3">
                  {p.features.map((f) => (
                    <li key={f} className="flex items-start gap-2.5 text-sm text-muted-foreground">
                      <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0 text-primary" />
                      {f}
                    </li>
                  ))}
                </ul>
                <a
                  href="#"
                  className={`mt-8 block rounded-full py-2.5 text-center text-sm font-semibold transition-transform hover:scale-[1.02] ${
                    p.featured
                      ? "bg-primary text-primary-foreground"
                      : "border border-border text-foreground hover:bg-secondary"
                  }`}
                >
                  {p.price === "$0" ? "Start free" : "Get " + p.name}
                </a>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="grid-bg relative overflow-hidden border-t border-border/60">
        <div className="pointer-events-none absolute inset-x-0 top-0 h-64 bg-primary/10 blur-3xl" />
        <div className="mx-auto max-w-3xl px-6 py-28 text-center">
          <h2 className="font-display text-4xl font-bold tracking-tight md:text-5xl">
            Your next job is already posted. <span className="text-primary">Apply tonight.</span>
          </h2>
          <p className="mx-auto mt-5 max-w-md text-muted-foreground">
            Set up your profile in 4 minutes. Wake up to applications sent and interviews booked.
          </p>
          <a
            href="#pricing"
            className="mt-8 inline-flex items-center gap-2 rounded-full bg-primary px-8 py-3.5 font-semibold text-primary-foreground transition-transform hover:scale-105"
          >
            Start auto-applying free
            <ArrowRight className="h-4 w-4" />
          </a>
        </div>
      </section>

      <footer className="border-t border-border/60">
        <div className="mx-auto flex max-w-6xl flex-col items-center justify-between gap-4 px-6 py-8 text-sm text-muted-foreground md:flex-row">
          <div className="flex items-center gap-2">
            <Zap className="h-4 w-4 text-primary" />
            <span className="font-display font-semibold text-foreground">ApplyPilot</span>
          </div>
          <p>© 2026 ApplyPilot. Apply smart, not hard.</p>
        </div>
      </footer>
    </div>
  );
}
