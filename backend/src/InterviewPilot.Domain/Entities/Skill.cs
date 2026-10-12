namespace InterviewPilot.Domain.Entities;

public class Skill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid? ParentSkillId { get; set; }
    public Skill? ParentSkill { get; set; }

    public ICollection<Skill> SubSkills { get; set; } = new List<Skill>();
    public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
}
