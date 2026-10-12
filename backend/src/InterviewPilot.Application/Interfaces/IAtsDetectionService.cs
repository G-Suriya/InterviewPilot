using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Application.Interfaces;

public interface IAtsDetectionService
{
    Task<ATSProvider> DetectAtsProviderAsync(string careerUrl, CancellationToken cancellationToken = default);
}
