using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Application.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? CareerUrl { get; set; }
    public ATSProvider ATSProvider { get; set; }
    public string? Industry { get; set; }
    public string? Locations { get; set; }
    public int ActiveJobsCount { get; set; }
}
