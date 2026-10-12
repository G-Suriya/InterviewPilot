namespace InterviewPilot.Application.DTOs;

public class CoverLetterDto
{
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Tone { get; set; } = "Professional";
    public string Salutation { get; set; } = string.Empty;
    public string OpeningParagraph { get; set; } = string.Empty;
    public string BodyParagraph { get; set; } = string.Empty;
    public string ClosingParagraph { get; set; } = string.Empty;
    public string FullCoverLetter { get; set; } = string.Empty;
}
