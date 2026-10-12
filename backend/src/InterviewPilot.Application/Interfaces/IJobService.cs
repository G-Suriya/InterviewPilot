using InterviewPilot.Application.DTOs;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Application.Interfaces;

public interface IJobService
{
    Task<IReadOnlyList<JobDto>> SearchJobsAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<JobDto?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JobDto> CreateJobAsync(Job job, CancellationToken cancellationToken = default);
    Task<int> SyncFromSourceAsync(string sourceName, JobSearchCriteria criteria, CancellationToken cancellationToken = default);
}
