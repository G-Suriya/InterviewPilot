namespace InterviewPilot.Domain.Entities;

public class JobSkill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid SkillId { get; set; }
    public Skill? Skill { get; set; }

    public bool IsRequired { get; set; } = true;
    public double Weight { get; set; } = 1.0;
}
