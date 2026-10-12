using InterviewPilot.Domain.Models;

namespace InterviewPilot.Domain.Interfaces;

public interface IJobSourceAdapter
{
    string SourceName { get; }
    Task<IReadOnlyList<RawJob>> FetchJobsAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<JobDetails?> GetJobAsync(string sourceJobId, CancellationToken cancellationToken = default);
}
