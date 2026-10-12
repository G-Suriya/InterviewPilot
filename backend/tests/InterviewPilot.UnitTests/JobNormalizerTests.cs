using InterviewPilot.Domain.Enums;
using InterviewPilot.Domain.Models;
using InterviewPilot.Infrastructure.Services;
using Xunit;

namespace InterviewPilot.UnitTests;

public class JobNormalizerTests
{
    private readonly JobNormalizer _normalizer = new();

    [Fact]
    public void Normalize_CleansTitleCompanyAndIdentifiesRemote()
    {
        var raw = new RawJob
        {
            Source = "Greenhouse",
            SourceJobId = "123",
            Company = "Stripe, Inc.",
            Title = "Senior .NET Engineer (Remote)",
            Location = "Remote - US",
            IsRemote = false
        };

        var job = _normalizer.Normalize(raw);

        Assert.Equal("Stripe", job.CompanyName);
        Assert.Equal("Senior .NET Engineer", job.Title);
        Assert.True(job.IsRemote);
        Assert.Equal(EmploymentType.FullTime, job.EmploymentType);
    }
}
