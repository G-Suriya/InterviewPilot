using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Domain.Entities;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Source { get; set; } = string.Empty;
    public string SourceJobId { get; set; } = string.Empty;

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    public string ApplicationUrl { get; set; } = string.Empty;
    public ApplicationMethod ApplicationMethod { get; set; } = ApplicationMethod.Manual;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobSkill> Skills { get; set; } = new List<JobSkill>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
