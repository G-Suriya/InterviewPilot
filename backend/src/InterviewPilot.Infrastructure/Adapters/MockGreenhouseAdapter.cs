using InterviewPilot.Domain.Interfaces;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Infrastructure.Adapters;

public class MockGreenhouseAdapter : IJobSourceAdapter
{
    public string SourceName => "Greenhouse";

    public Task<IReadOnlyList<RawJob>> FetchJobsAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RawJob> sampleJobs = new List<RawJob>
        {
            new RawJob
            {
                Source = SourceName,
                SourceJobId = "gh-101",
                Title = "Senior .NET Core Engineer",
                Company = "Stripe",
                Description = "Seeking an experienced .NET/C# engineer with Azure, microservices, and distributed systems experience.",
                Location = "Remote",
                IsRemote = true,
                EmploymentType = "FullTime",
                SalaryMin = 180000,
                SalaryMax = 220000,
                PostedDate = DateTime.UtcNow.AddHours(-6),
                ApplicationUrl = "https://boards.greenhouse.io/stripe/jobs/101"
            },
            new RawJob
            {
                Source = SourceName,
                SourceJobId = "gh-102",
                Title = "Full-Stack Engineer (React & .NET)",
                Company = "Vercel",
                Description = "Join our product engineering team building next-generation developer workflows with React, TypeScript, and .NET.",
                Location = "San Francisco, CA",
                IsRemote = true,
                EmploymentType = "FullTime",
                SalaryMin = 160000,
                SalaryMax = 195000,
                PostedDate = DateTime.UtcNow.AddHours(-12),
                ApplicationUrl = "https://boards.greenhouse.io/vercel/jobs/102"
            }
        };

        return Task.FromResult(sampleJobs);
    }

    public Task<JobDetails?> GetJobAsync(string sourceJobId, CancellationToken cancellationToken = default)
    {
        var details = new JobDetails
        {
            Source = SourceName,
            SourceJobId = sourceJobId,
            Title = "Senior .NET Core Engineer",
            Company = "Stripe",
            Description = "Full job description for Stripe .NET role...",
            Location = "Remote",
            IsRemote = true,
            RequiredSkills = [".NET", "C#", "Azure", "SQL"],
            PreferredSkills = ["React", "Docker", "Kubernetes"],
            ExperienceYears = 5,
            ApplicationUrl = "https://boards.greenhouse.io/stripe/jobs/101"
        };

        return Task.FromResult<JobDetails?>(details);
    }
}
