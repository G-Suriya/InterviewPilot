namespace InterviewPilot.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public CandidateProfile? Profile { get; set; }
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}
