using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchingController : ControllerBase
{
    private readonly IMatchingEngine _matchingEngine;
    private readonly IRepository<Job> _jobRepo;
    private readonly IRepository<CandidateProfile> _profileRepo;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public MatchingController(
        IMatchingEngine matchingEngine,
        IRepository<Job> jobRepo,
        IRepository<CandidateProfile> profileRepo)
    {
        _matchingEngine = matchingEngine;
        _jobRepo = jobRepo;
        _profileRepo = profileRepo;
    }

    [HttpPost("evaluate/{jobId:guid}")]
    public async Task<ActionResult<MatchingResultDto>> EvaluateMatch(Guid jobId, CancellationToken cancellationToken)
    {
        var job = await _jobRepo.GetByIdAsync(jobId, cancellationToken);
        if (job == null) return NotFound("Job not found");

        var profiles = await _profileRepo.FindAsync(p => p.UserId == DemoUserId, cancellationToken);
        var candidate = profiles.FirstOrDefault() ?? new CandidateProfile
        {
            UserId = DemoUserId,
            YearsExperience = 5,
            TargetRoles = "Senior .NET Developer,Full Stack Engineer",
            PreferredLocations = "Remote,San Francisco",
            RemotePreference = Domain.Enums.RemotePreference.Any,
            SalaryExpectationMin = 150000
        };

        var match = _matchingEngine.CalculateMatch(candidate, job);
        return Ok(match);
    }
}
