namespace InterviewPilot.Application.DTOs;

public class CandidateAnalyticsDto
{
    public int TotalApplications { get; set; }
    public int ShortlistedCount { get; set; }
    public int ApplicationsReadyCount { get; set; }
    public int AppliedCount { get; set; }
    public int InterviewsCount { get; set; }
    public int OffersCount { get; set; }
    public double ResponseRatePercentage { get; set; }
    public double InterviewRatePercentage { get; set; }
    public double AverageMatchScore { get; set; }
    public List<TopSkillDemandDto> MostDemandedSkills { get; set; } = [];
    public List<SourcePerformanceDto> SourceBreakdown { get; set; } = [];
}

public class TopSkillDemandDto
{
    public string SkillName { get; set; } = string.Empty;
    public int JobCount { get; set; }
}

public class SourcePerformanceDto
{
    public string SourceName { get; set; } = string.Empty;
    public int Applied { get; set; }
    public int Interviews { get; set; }
}
