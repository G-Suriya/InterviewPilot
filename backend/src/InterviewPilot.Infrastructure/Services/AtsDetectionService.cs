using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Enums;

namespace InterviewPilot.Infrastructure.Services;

public class AtsDetectionService : IAtsDetectionService
{
    public Task<ATSProvider> DetectAtsProviderAsync(string careerUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(careerUrl)) return Task.FromResult(ATSProvider.Unknown);

        var lower = careerUrl.ToLowerInvariant();

        if (lower.Contains("greenhouse.io") || lower.Contains("gh_src"))
            return Task.FromResult(ATSProvider.Greenhouse);

        if (lower.Contains("lever.co"))
            return Task.FromResult(ATSProvider.Lever);

        if (lower.Contains("myworkdayjobs.com") || lower.Contains("workday"))
            return Task.FromResult(ATSProvider.Workday);

        if (lower.Contains("ashbyhq.com") || lower.Contains("ashby"))
            return Task.FromResult(ATSProvider.Ashby);

        if (lower.Contains("smartrecruiters.com"))
            return Task.FromResult(ATSProvider.SmartRecruiters);

        return Task.FromResult(ATSProvider.Custom);
    }
}
