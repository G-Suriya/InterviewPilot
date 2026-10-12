using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Domain.Entities;

public class CandidateProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Summary { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public int NoticePeriodDays { get; set; }
    public decimal? SalaryExpectationMin { get; set; }
    public decimal? SalaryExpectationMax { get; set; }
    public string Currency { get; set; } = "INR";
    public RemotePreference RemotePreference { get; set; } = RemotePreference.Any;
    public string PreferredLocations { get; set; } = string.Empty; // Comma-delimited
    public string TargetRoles { get; set; } = string.Empty; // Comma-delimited
    public string ExcludedCompanies { get; set; } = string.Empty;

    public ICollection<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
}
