namespace InterviewPilot.Application.DTOs;

public class ParsedResumeDto
{
    public string RawText { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<string> ExtractedSkills { get; set; } = [];
    public List<ParsedExperienceDto> Experiences { get; set; } = [];
    public List<ParsedEducationDto> Education { get; set; } = [];
}

public class ParsedExperienceDto
{
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ParsedEducationDto
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public int? GraduationYear { get; set; }
}
