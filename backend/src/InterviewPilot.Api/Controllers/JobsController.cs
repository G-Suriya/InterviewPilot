using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Enums;
using InterviewPilot.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<JobDto>>> GetJobs(
        [FromQuery] string? role,
        [FromQuery] string? location,
        [FromQuery] RemotePreference remote = RemotePreference.Any,
        [FromQuery] decimal? minSalary = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var criteria = new JobSearchCriteria
        {
            RoleQuery = role,
            Location = location,
            Remote = remote,
            MinSalary = minSalary,
            Page = page,
            PageSize = pageSize
        };

        var jobs = await _jobService.SearchJobsAsync(criteria, cancellationToken);
        return Ok(jobs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobDto>> GetJobById(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobService.GetJobByIdAsync(id, cancellationToken);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<object>> SyncFromSource(
        [FromQuery] string source = "Greenhouse",
        CancellationToken cancellationToken = default)
    {
        var criteria = new JobSearchCriteria();
        var count = await _jobService.SyncFromSourceAsync(source, criteria, cancellationToken);
        return Ok(new { source, newJobsIngested = count });
    }
}
