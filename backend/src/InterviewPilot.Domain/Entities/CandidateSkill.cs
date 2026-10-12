using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Domain.Entities;

public class CandidateSkill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CandidateProfileId { get; set; }
    public CandidateProfile? CandidateProfile { get; set; }

    public Guid SkillId { get; set; }
    public Skill? Skill { get; set; }

    public int YearsExperience { get; set; }
    public SkillProficiency Proficiency { get; set; } = SkillProficiency.Intermediate;
}
