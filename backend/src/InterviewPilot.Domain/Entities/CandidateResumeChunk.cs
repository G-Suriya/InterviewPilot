namespace InterviewPilot.Domain.Entities;

public class CandidateResumeChunk
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty; // Experience, Project, Skill, Certification
    public string Content { get; set; } = string.Empty;
    public string EmbeddingJson { get; set; } = "[]"; // Serialized float vector
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
