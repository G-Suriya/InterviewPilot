using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Domain.Models;

public class JobSearchCriteria
{
    public string? RoleQuery { get; set; }
    public string? Location { get; set; }
    public RemotePreference Remote { get; set; } = RemotePreference.Any;
    public decimal? MinSalary { get; set; }
    public int? MinExperienceYears { get; set; }
    public List<string> Skills { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
