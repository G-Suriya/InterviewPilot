namespace InterviewPilot.Domain.Entities;

public class Resume
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
    public string? ParsedText { get; set; }
    public string? ParsedSummary { get; set; }
    public bool IsMaster { get; set; } = true;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
