using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ICandidateProfileService _profileService;
    private readonly IBlobStorageService _storageService;
    private readonly IResumeParserService _parserService;
    private readonly ICandidateRagService _ragService;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public ProfileController(
        ICandidateProfileService profileService,
        IBlobStorageService storageService,
        IResumeParserService parserService,
        ICandidateRagService ragService)
    {
        _profileService = profileService;
        _storageService = storageService;
        _parserService = parserService;
        _ragService = ragService;
    }

    [HttpGet]
    public async Task<ActionResult<CandidateProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetProfileByUserIdAsync(DemoUserId, cancellationToken);
        if (profile == null)
        {
            return Ok(new CandidateProfileDto
            {
                UserId = DemoUserId,
                Summary = "Accomplished Software Engineer specializing in .NET 10, C#, Azure microservices, and React.",
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
        await _ragService.IndexCandidateAsync(DemoUserId, cancellationToken);
        return Ok(updated);
    }

    [HttpPost("resume")]
    public async Task<ActionResult<ParsedResumeDto>> UploadResume(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        var blobUrl = await _storageService.UploadAsync(stream, file.FileName, file.ContentType, cancellationToken);

        stream.Position = 0;
        var parsed = await _parserService.ParseResumeAsync(stream, file.FileName, cancellationToken);

        // Update profile summary and auto-index into RAG
        var profileDto = new CandidateProfileDto
        {
            UserId = DemoUserId,
            Summary = parsed.Summary,
            YearsExperience = 5,
            PreferredLocations = ["Remote"],
            TargetRoles = ["Senior Software Engineer"],
            Skills = parsed.ExtractedSkills.Select(s => new CandidateSkillDto
            {
                Name = s,
                Category = "Extracted",
                YearsExperience = 3,
                Proficiency = Domain.Enums.SkillProficiency.Intermediate
            }).ToList()
        };

        await _profileService.UpsertProfileAsync(DemoUserId, profileDto, cancellationToken);
        await _ragService.IndexCandidateAsync(DemoUserId, cancellationToken);

        return Ok(parsed);
    }
}
