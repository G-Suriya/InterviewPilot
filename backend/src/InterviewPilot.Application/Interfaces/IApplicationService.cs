using InterviewPilot.Application.DTOs;
using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Application.Interfaces;

public interface IApplicationService
{
    Task<IReadOnlyList<ApplicationDto>> GetApplicationsByUserIdAsync(Guid userId, ApplicationStatus? status = null, CancellationToken cancellationToken = default);
    Task<ApplicationDto?> GetApplicationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationDto> CreateApplicationAsync(Guid userId, Guid jobId, ApplicationMethod method, CancellationToken cancellationToken = default);
    Task<ApplicationDto?> UpdateStatusAsync(Guid id, ApplicationStatus newStatus, string? notes = null, CancellationToken cancellationToken = default);
}
