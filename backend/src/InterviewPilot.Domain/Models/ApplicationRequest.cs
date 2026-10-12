namespace InterviewPilot.Domain.Models;

public class ApplicationRequest
{
    public Guid JobId { get; set; }
    public Guid CandidateProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string ResumeUrl { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public Dictionary<string, string> Answers { get; set; } = [];
}
