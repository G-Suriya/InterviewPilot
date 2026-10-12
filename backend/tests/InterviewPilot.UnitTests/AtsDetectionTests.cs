using InterviewPilot.Domain.Enums;
using InterviewPilot.Infrastructure.Services;
using Xunit;

namespace InterviewPilot.UnitTests;

public class AtsDetectionTests
{
    private readonly AtsDetectionService _service = new();

    [Theory]
    [InlineData("https://boards.greenhouse.io/stripe/jobs/101", ATSProvider.Greenhouse)]
    [InlineData("https://jobs.lever.co/spotify/201", ATSProvider.Lever)]
    [InlineData("https://company.myworkdayjobs.com/careers/job/1", ATSProvider.Workday)]
    [InlineData("https://jobs.ashbyhq.com/linear/42", ATSProvider.Ashby)]
    [InlineData("https://careers.smartrecruiters.com/AcmeCorp", ATSProvider.SmartRecruiters)]
    [InlineData("https://example.com/careers", ATSProvider.Custom)]
    public async Task DetectAtsProvider_ReturnsCorrectProvider(string url, ATSProvider expected)
    {
        var result = await _service.DetectAtsProviderAsync(url);
        Assert.Equal(expected, result);
    }
}
