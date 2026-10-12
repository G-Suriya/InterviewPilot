using InterviewPilot.Application.Services;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Enums;
using Xunit;

namespace InterviewPilot.UnitTests;

public class MatchingEngineTests
{
    [Fact]
    public void CalculateMatch_WithMatchingSkillsAndLocation_ReturnsHighOverallScore()
    {
        // Arrange
        var engine = new MatchingEngine();

        var dotNetSkill = new Skill { Name = ".NET", Category = "Backend" };
        var csharpSkill = new Skill { Name = "C#", Category = "Language" };

        var candidate = new CandidateProfile
        {
            YearsExperience = 5,
            RemotePreference = RemotePreference.Any,
            PreferredLocations = "Remote, San Francisco",
            TargetRoles = "Senior .NET Engineer, Software Engineer",
            SalaryExpectationMin = 150000,
            Skills = [
                new CandidateSkill { Skill = dotNetSkill, YearsExperience = 5 },
                new CandidateSkill { Skill = csharpSkill, YearsExperience = 5 }
            ]
        };

        var job = new Job
        {
            Title = "Senior .NET Engineer",
            CompanyName = "Acme Corp",
            Location = "Remote",
            IsRemote = true,
            SalaryMin = 160000,
            SalaryMax = 200000,
            Skills = [
                new JobSkill { Skill = dotNetSkill, IsRequired = true },
                new JobSkill { Skill = csharpSkill, IsRequired = true }
            ]
        };

        // Act
        var result = engine.CalculateMatch(candidate, job);

        // Assert
        Assert.True(result.Breakdown.OverallScore >= 85.0);
        Assert.Equal(100.0, result.Breakdown.TechnicalSkillMatch);
        Assert.Equal(100.0, result.Breakdown.LocationMatch);
        Assert.Contains(".NET", result.Breakdown.MatchedSkills);
        Assert.Empty(result.Breakdown.MissingSkills);
    }

    [Fact]
    public void CalculateMatch_WithExcludedCompany_SetsPreferenceMatchToZero()
    {
        // Arrange
        var engine = new MatchingEngine();

        var candidate = new CandidateProfile
        {
            YearsExperience = 3,
            ExcludedCompanies = "BadCompany, ExcludedCorp"
        };

        var job = new Job
        {
            Title = "Software Developer",
            CompanyName = "BadCompany Inc",
            Location = "Remote"
        };

        // Act
        var result = engine.CalculateMatch(candidate, job);

        // Assert
        Assert.Equal(0.0, result.Breakdown.PreferenceMatch);
    }
}
