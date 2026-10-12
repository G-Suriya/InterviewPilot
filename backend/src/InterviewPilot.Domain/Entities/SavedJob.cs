namespace InterviewPilot.Domain.Entities;

public class SavedJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
