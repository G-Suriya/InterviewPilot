using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ICandidateProfileService _profileService;
    // Default demo user ID for Sprint 1 local development
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public ProfileController(ICandidateProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<CandidateProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetProfileByUserIdAsync(DemoUserId, cancellationToken);
        if (profile == null)
        {
            // Return empty profile template
            return Ok(new CandidateProfileDto
            {
                UserId = DemoUserId,
                Summary = "Software Engineer experienced in .NET, C#, and React.",
                YearsExperience = 5,
                NoticePeriodDays = 30,
                SalaryExpectationMin = 160000,
                Currency = "USD",
                TargetRoles = ["Senior Software Engineer", ".NET Developer", "Full Stack Developer"],
                PreferredLocations = ["Remote", "San Francisco"],
                Skills = [
                    new CandidateSkillDto { Name = ".NET", Category = "Backend", YearsExperience = 5, Proficiency = Domain.Enums.SkillProficiency.Advanced },
                    new CandidateSkillDto { Name = "C#", Category = "Language", YearsExperience = 5, Proficiency = Domain.Enums.SkillProficiency.Advanced },
                    new CandidateSkillDto { Name = "React", Category = "Frontend", YearsExperience = 4, Proficiency = Domain.Enums.SkillProficiency.Advanced },
                    new CandidateSkillDto { Name = "Azure", Category = "Cloud", YearsExperience = 3, Proficiency = Domain.Enums.SkillProficiency.Intermediate }
                ]
            });
        }
        return Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<CandidateProfileDto>> UpsertProfile(
        [FromBody] CandidateProfileDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _profileService.UpsertProfileAsync(DemoUserId, dto, cancellationToken);
        return Ok(updated);
    }
}
