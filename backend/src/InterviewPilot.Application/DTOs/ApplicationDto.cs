using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Application.DTOs;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; }
    public double? MatchScore { get; set; }
    public ApplicationMethod ApplicationMethod { get; set; }
    public string? ExternalApplicationId { get; set; }
    public string? Notes { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
    public string? Notes { get; set; }
}
