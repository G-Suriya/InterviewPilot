namespace InterviewPilot.Domain.Models;

public class RawJob
{
    public string Source { get; set; } = string.Empty;
    public string SourceJobId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public string? EmploymentType { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public DateTime PostedDate { get; set; }
    public string ApplicationUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}
