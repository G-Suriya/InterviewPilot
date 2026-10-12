using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Enums;
using InterviewPilot.Domain.Interfaces;
using InterviewPilot.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IRepository<SavedJob> _savedRepo;
    private readonly IRepository<Job> _jobRepo;
    private readonly IJobDeduplicationService _dedupService;
    private readonly IJobNormalizer _normalizer;
    private readonly IEnumerable<IJobSourceAdapter> _adapters;
    private readonly IUnitOfWork _unitOfWork;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public JobsController(
        IJobService jobService,
        IRepository<SavedJob> savedRepo,
        IRepository<Job> jobRepo,
        IJobDeduplicationService dedupService,
        IJobNormalizer normalizer,
        IEnumerable<IJobSourceAdapter> adapters,
        IUnitOfWork unitOfWork)
    {
        _jobService = jobService;
        _savedRepo = savedRepo;
        _jobRepo = jobRepo;
        _dedupService = dedupService;
        _normalizer = normalizer;
        _adapters = adapters;
        _unitOfWork = unitOfWork;
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

    [HttpPost("{id:guid}/save")]
    public async Task<ActionResult> SaveJob(Guid id, [FromQuery] string? notes, CancellationToken cancellationToken)
    {
        var existing = await _savedRepo.FindAsync(s => s.UserId == DemoUserId && s.JobId == id, cancellationToken);
        if (existing.Count == 0)
        {
            await _savedRepo.AddAsync(new SavedJob { UserId = DemoUserId, JobId = id, Notes = notes }, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Ok(new { saved = true, jobId = id });
    }

    [HttpDelete("{id:guid}/save")]
    public async Task<ActionResult> UnsaveJob(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _savedRepo.FindAsync(s => s.UserId == DemoUserId && s.JobId == id, cancellationToken);
        foreach (var s in existing) _savedRepo.Remove(s);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok(new { saved = false, jobId = id });
    }

    [HttpGet("saved")]
    public async Task<ActionResult<IReadOnlyList<JobDto>>> GetSavedJobs(CancellationToken cancellationToken)
    {
        var saved = await _savedRepo.FindAsync(s => s.UserId == DemoUserId, cancellationToken);
        var jobIds = saved.Select(s => s.JobId).ToHashSet();
        var allJobs = await _jobRepo.GetAllAsync(cancellationToken);
        var result = allJobs.Where(j => jobIds.Contains(j.Id)).Select(j => new JobDto
        {
            Id = j.Id,
            Source = j.Source,
            Title = j.Title,
            CompanyName = j.CompanyName,
            Description = j.Description,
            Location = j.Location,
            IsRemote = j.IsRemote,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            ApplicationUrl = j.ApplicationUrl
        }).ToList();
        return Ok(result);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<object>> SyncFromAllSources(CancellationToken cancellationToken = default)
    {
        int totalIngested = 0;
        int duplicatesSkipped = 0;
        var criteria = new JobSearchCriteria();

        foreach (var adapter in _adapters)
        {
            var rawJobs = await adapter.FetchJobsAsync(criteria, cancellationToken);
            foreach (var raw in rawJobs)
            {
                if (await _dedupService.IsDuplicateAsync(raw, cancellationToken))
                {
                    duplicatesSkipped++;
                    continue;
                }

                var job = _normalizer.Normalize(raw);
                await _jobRepo.AddAsync(job, cancellationToken);
                totalIngested++;
            }
        }

        if (totalIngested > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new { newJobsIngested = totalIngested, duplicatesSkipped });
    }
}
