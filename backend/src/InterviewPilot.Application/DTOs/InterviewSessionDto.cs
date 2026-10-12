namespace InterviewPilot.Application.DTOs;

public class InterviewSessionDto
{
    public Guid SessionId { get; set; }
    public Guid JobId { get; set; }
    public string RoleTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public double ReadinessScore { get; set; }
    public string FeedbackSummary { get; set; } = string.Empty;
    public List<InterviewQuestionDto> Questions { get; set; } = [];
}

public class InterviewQuestionDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Category { get; set; } = "Technical";
    public string? CandidateAnswer { get; set; }
    public double? Score { get; set; }
    public string? Feedback { get; set; }
    public string? ModelStarAnswer { get; set; }
}

public class AnswerEvaluationDto
{
    public Guid QuestionId { get; set; }
    public double Score { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public string Strengths { get; set; } = string.Empty;
    public string AreasOfImprovement { get; set; } = string.Empty;
    public string ModelAnswerRecommendation { get; set; } = string.Empty;
}
