import { createServerFn } from "@tanstack/react-start";

export type Job = {
  id: string;
  role: string;
  co: string;
  location: string;
  salary: number; // in $k, 0 if unknown
  remote: boolean;
  url: string;
  source: string;
};

const GREENHOUSE = ["airbnb", "stripe", "figma", "discord", "databricks"];
const LEVER = ["palantir", "spotify"];

async function getJson(url: string): Promise<any> {
  const r = await fetch(url, { headers: { "User-Agent": "Mozilla/5.0 AutoApply" } });
  if (!r.ok) throw new Error(`${url} ${r.status}`);
  return r.json();
}

const isRemote = (s: string) => /remote|anywhere|worldwide/i.test(s);
function parseSalary(s?: string | number | null): number {
  if (typeof s === "number") return s > 1000 ? Math.round(s / 1000) : s;
  if (!s) return 0;
  const m = String(s).replace(/,/g, "").match(/(\d+(?:\.\d+)?)\s*(k)?/i);
  if (!m) return 0;
  const n = parseFloat(m[1] ?? "0");
  return m[2] ? Math.round(n) : n > 1000 ? Math.round(n / 1000) : 0;
}

async function remotive(): Promise<Job[]> {
  const d = await getJson("https://remotive.com/api/remote-jobs?limit=60");
  return d.jobs.map((j: any) => ({
    id: `remotive-${j.id}`, role: j.title, co: j.company_name,
    location: j.candidate_required_location || "Remote", salary: parseSalary(j.salary),
    remote: true, url: j.url, source: "Remotive",
  }));
}
async function arbeitnow(): Promise<Job[]> {
  const d = await getJson("https://www.arbeitnow.com/api/job-board-api");
  return d.data.slice(0, 60).map((j: any) => ({
    id: `arbeitnow-${j.slug}`, role: j.title, co: j.company_name, location: j.location,
    salary: 0, remote: !!j.remote, url: j.url, source: "Arbeitnow",
  }));
}
async function remoteok(): Promise<Job[]> {
  const d = await getJson("https://remoteok.com/api");
  return d.filter((j: any) => j.id && j.position).slice(0, 60).map((j: any) => ({
    id: `remoteok-${j.id}`, role: j.position, co: j.company, location: j.location || "Remote",
    salary: parseSalary(j.salary_min), remote: true, url: j.url, source: "RemoteOK",
  }));
}
async function greenhouse(board: string): Promise<Job[]> {
  const d = await getJson(`https://boards-api.greenhouse.io/v1/boards/${board}/jobs`);
  return d.jobs.slice(0, 30).map((j: any) => {
    const loc = j.location?.name ?? "";
    return {
      id: `gh-${board}-${j.id}`, role: j.title, co: board.charAt(0).toUpperCase() + board.slice(1),
      location: loc, salary: 0, remote: isRemote(loc), url: j.absolute_url, source: "Career page",
    };
  });
}
async function lever(co: string): Promise<Job[]> {
  const d = await getJson(`https://api.lever.co/v0/postings/${co}?mode=json`);
  return d.slice(0, 30).map((j: any) => {
    const loc = j.categories?.location ?? "";
    return {
      id: `lever-${co}-${j.id}`, role: j.text, co: co.charAt(0).toUpperCase() + co.slice(1),
      location: loc, salary: parseSalary(j.salaryRange?.min), remote: isRemote(loc) || j.workplaceType === "remote",
      url: j.hostedUrl, source: "Career page",
    };
  });
}

export const fetchJobs = createServerFn({ method: "GET" }).handler(async () => {
  const tasks = [remotive(), arbeitnow(), remoteok(), ...GREENHOUSE.map(greenhouse), ...LEVER.map(lever)];
  const results = await Promise.allSettled(tasks);
  const jobs: Job[] = [];
  const failed: string[] = [];
  results.forEach((r) => {
    if (r.status === "fulfilled") jobs.push(...r.value);
    else { console.error("job source failed", r.reason); failed.push(String(r.reason?.message ?? r.reason)); }
  });
  return { jobs, failedCount: failed.length, fetchedAt: new Date().toISOString() };
});
