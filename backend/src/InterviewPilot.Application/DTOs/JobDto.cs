using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Application.DTOs;

public class JobDto
{
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public DateTime PostedDate { get; set; }
    public string ApplicationUrl { get; set; } = string.Empty;
    public ApplicationMethod ApplicationMethod { get; set; }
    public List<string> Skills { get; set; } = [];
}
