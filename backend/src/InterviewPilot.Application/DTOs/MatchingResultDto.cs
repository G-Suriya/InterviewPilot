using InterviewPilot.Domain.Models;

namespace InterviewPilot.Application.DTOs;

public class MatchingResultDto
{
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public MatchBreakdown Breakdown { get; set; } = new();
}
