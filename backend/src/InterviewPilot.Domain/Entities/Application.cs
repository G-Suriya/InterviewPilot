using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Domain.Entities;

public class Application
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid? ResumeVersionId { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.New;
    public double? MatchScore { get; set; }
    public ApplicationMethod ApplicationMethod { get; set; } = ApplicationMethod.Manual;
    public string? ExternalApplicationId { get; set; }
    public string? Notes { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
