using InterviewPilot.Application.DTOs;

namespace InterviewPilot.Application.Interfaces;

public interface ICandidateProfileService
{
    Task<CandidateProfileDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CandidateProfileDto> UpsertProfileAsync(Guid userId, CandidateProfileDto dto, CancellationToken cancellationToken = default);
}
