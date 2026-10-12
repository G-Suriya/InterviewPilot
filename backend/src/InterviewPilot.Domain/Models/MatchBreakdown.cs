namespace InterviewPilot.Domain.Models;

public class MatchBreakdown
{
    public double OverallScore { get; set; }
    public double TechnicalSkillMatch { get; set; }
    public double SemanticSimilarity { get; set; }
    public double ExperienceMatch { get; set; }
    public double RoleMatch { get; set; }
    public double LocationMatch { get; set; }
    public double SalaryMatch { get; set; }
    public double PreferenceMatch { get; set; }
    public List<string> MatchedSkills { get; set; } = [];
    public List<string> MissingSkills { get; set; } = [];
    public string Summary { get; set; } = string.Empty;
}
