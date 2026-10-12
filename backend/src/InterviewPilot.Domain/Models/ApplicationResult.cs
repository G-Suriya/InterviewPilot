namespace InterviewPilot.Domain.Models;

public class ApplicationResult
{
    public bool Success { get; set; }
    public string? ExternalApplicationId { get; set; }
    public string? Message { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
