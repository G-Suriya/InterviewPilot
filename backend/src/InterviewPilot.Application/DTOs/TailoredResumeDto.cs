namespace InterviewPilot.Application.DTOs;

public class TailoredResumeDto
{
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string TailoredProfessionalSummary { get; set; } = string.Empty;
    public List<string> HighlightedKeySkills { get; set; } = [];
    public List<TailoredExperienceBulletDto> ExperienceBullets { get; set; } = [];
    public double PredictedAtsScore { get; set; }
}

public class TailoredExperienceBulletDto
{
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<string> OptimizedBullets { get; set; } = [];
}
