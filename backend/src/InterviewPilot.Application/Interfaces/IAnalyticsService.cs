using InterviewPilot.Application.DTOs;

namespace InterviewPilot.Application.Interfaces;

public interface IAnalyticsService
{
    Task<CandidateAnalyticsDto> GetAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default);
}
