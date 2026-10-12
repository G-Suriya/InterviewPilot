using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Enums;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Infrastructure.Services;

public class JobNormalizer : IJobNormalizer
{
    public Job Normalize(RawJob raw)
    {
        var job = new Job
        {
            Source = raw.Source,
            SourceJobId = raw.SourceJobId,
            CompanyName = CleanCompanyName(raw.Company),
            Title = CleanTitle(raw.Title),
            Description = raw.Description?.Trim() ?? string.Empty,
            Location = string.IsNullOrWhiteSpace(raw.Location) ? "Remote" : raw.Location.Trim(),
            IsRemote = raw.IsRemote || raw.Location.Contains("Remote", StringComparison.OrdinalIgnoreCase),
            SalaryMin = raw.SalaryMin,
            SalaryMax = raw.SalaryMax,
            PostedDate = raw.PostedDate == default ? DateTime.UtcNow : raw.PostedDate,
            ApplicationUrl = raw.ApplicationUrl ?? string.Empty,
            ApplicationMethod = ApplicationMethod.Manual
        };

        if (raw.EmploymentType?.Contains("Contract", StringComparison.OrdinalIgnoreCase) == true)
            job.EmploymentType = EmploymentType.Contract;
        else if (raw.EmploymentType?.Contains("Part", StringComparison.OrdinalIgnoreCase) == true)
            job.EmploymentType = EmploymentType.PartTime;
        else
            job.EmploymentType = EmploymentType.FullTime;

        return job;
    }

    private static string CleanCompanyName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Unknown";
        return name.Replace(", Inc.", "").Replace(" Inc.", "").Replace(" LLC", "").Trim();
    }

    private static string CleanTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return "Untitled Position";
        return title.Replace("(m/f/d)", "").Replace("(Remote)", "").Trim();
    }
}
