namespace InterviewPilot.Domain.Models;

public class JobDetails : RawJob
{
    public List<string> RequiredSkills { get; set; } = [];
    public List<string> PreferredSkills { get; set; } = [];
    public int? ExperienceYears { get; set; }
}
