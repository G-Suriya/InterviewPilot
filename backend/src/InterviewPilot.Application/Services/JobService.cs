using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Interfaces;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Application.Services;

public class JobService : IJobService
{
    private readonly IRepository<Job> _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumerable<IJobSourceAdapter> _adapters;

    public JobService(IRepository<Job> jobRepository, IUnitOfWork unitOfWork, IEnumerable<IJobSourceAdapter> adapters)
    {
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _adapters = adapters;
    }

    public async Task<IReadOnlyList<JobDto>> SearchJobsAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var allJobs = await _jobRepository.GetAllAsync(cancellationToken);
        var query = allJobs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(criteria.RoleQuery))
        {
            var role = criteria.RoleQuery.ToLowerInvariant();
            query = query.Where(j => j.Title.ToLowerInvariant().Contains(role) || j.Description.ToLowerInvariant().Contains(role));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Location))
        {
            var loc = criteria.Location.ToLowerInvariant();
            query = query.Where(j => j.Location.ToLowerInvariant().Contains(loc));
        }

        if (criteria.Remote == Domain.Enums.RemotePreference.RemoteOnly)
        {
            query = query.Where(j => j.IsRemote);
        }

        if (criteria.MinSalary.HasValue)
        {
            query = query.Where(j => (j.SalaryMax ?? j.SalaryMin ?? 0) >= criteria.MinSalary.Value);
        }

        return query
            .OrderByDescending(j => j.PostedDate)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<JobDto?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByIdAsync(id, cancellationToken);
        return job != null ? MapToDto(job) : null;
    }

    public async Task<JobDto> CreateJobAsync(Job job, CancellationToken cancellationToken = default)
    {
        await _jobRepository.AddAsync(job, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(job);
    }

    public async Task<int> SyncFromSourceAsync(string sourceName, JobSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var adapter = _adapters.FirstOrDefault(a => a.SourceName.Equals(sourceName, StringComparison.OrdinalIgnoreCase));
        if (adapter == null) return 0;

        var rawJobs = await adapter.FetchJobsAsync(criteria, cancellationToken);
        int added = 0;

        foreach (var raw in rawJobs)
        {
            var existing = await _jobRepository.FindAsync(j => j.Source == raw.Source && j.SourceJobId == raw.SourceJobId, cancellationToken);
            if (existing.Count == 0)
            {
                var job = new Job
                {
                    Source = raw.Source,
                    SourceJobId = raw.SourceJobId,
                    Title = raw.Title,
                    CompanyName = raw.Company,
                    Description = raw.Description,
                    Location = raw.Location,
                    IsRemote = raw.IsRemote,
                    SalaryMin = raw.SalaryMin,
                    SalaryMax = raw.SalaryMax,
                    PostedDate = raw.PostedDate,
                    ApplicationUrl = raw.ApplicationUrl
                };
                await _jobRepository.AddAsync(job, cancellationToken);
                added++;
            }
        }

        if (added > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return added;
    }

    private static JobDto MapToDto(Job job)
    {
        return new JobDto
        {
            Id = job.Id,
            Source = job.Source,
            Title = job.Title,
            CompanyName = job.CompanyName,
            Description = job.Description,
            Location = job.Location,
            IsRemote = job.IsRemote,
            EmploymentType = job.EmploymentType,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            PostedDate = job.PostedDate,
            ApplicationUrl = job.ApplicationUrl,
            ApplicationMethod = job.ApplicationMethod,
            Skills = job.Skills.Select(s => s.Skill?.Name ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList()
        };
    }
}
