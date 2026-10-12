using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Application.DTOs;

public class CandidateProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public int NoticePeriodDays { get; set; }
    public decimal? SalaryExpectationMin { get; set; }
    public decimal? SalaryExpectationMax { get; set; }
    public string Currency { get; set; } = "INR";
    public RemotePreference RemotePreference { get; set; }
    public List<string> PreferredLocations { get; set; } = [];
    public List<string> TargetRoles { get; set; } = [];
    public List<string> ExcludedCompanies { get; set; } = [];
    public List<CandidateSkillDto> Skills { get; set; } = [];
}

public class CandidateSkillDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public SkillProficiency Proficiency { get; set; }
}
