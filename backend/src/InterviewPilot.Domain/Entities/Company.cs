using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Domain.Entities;

public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? CareerUrl { get; set; }
    public ATSProvider ATSProvider { get; set; } = ATSProvider.Unknown;
    public string? Industry { get; set; }
    public string? Locations { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastChecked { get; set; }

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
