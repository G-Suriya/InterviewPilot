namespace InterviewPilot.Domain.Entities;

public class InterviewSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public string RoleTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public double? OverallReadinessScore { get; set; }
    public string FeedbackSummary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InterviewQuestion> Questions { get; set; } = new List<InterviewQuestion>();
}

public class InterviewQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public InterviewSession? Session { get; set; }

    public string QuestionText { get; set; } = string.Empty;
    public string Category { get; set; } = "Technical"; // Technical, Behavioral, SystemDesign
    public string? CandidateAnswer { get; set; }
    public double? AnswerScore { get; set; }
    public string? Feedback { get; set; }
    public string? ModelStarAnswer { get; set; }
}
