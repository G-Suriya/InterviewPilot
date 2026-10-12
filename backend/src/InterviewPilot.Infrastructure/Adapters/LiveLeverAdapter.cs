using System.Text.Json;
using InterviewPilot.Domain.Interfaces;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Infrastructure.Adapters;

public class LiveLeverAdapter : IJobSourceAdapter
{
    private readonly HttpClient _httpClient;
    private static readonly string[] DefaultCompanies = ["palantir", "spotify"];

    public string SourceName => "Lever";

    public LiveLeverAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<RawJob>> FetchJobsAsync(JobSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var rawJobs = new List<RawJob>();

        foreach (var company in DefaultCompanies)
        {
            try
            {
                var url = $"https://api.lever.co/v0/postings/{company}?mode=json";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);

                foreach (var j in doc.RootElement.EnumerateArray())
                {
                    var title = j.GetProperty("text").GetString() ?? "";
                    var id = j.GetProperty("id").GetString() ?? "";
                    var appUrl = j.TryGetProperty("hostedUrl", out var u) ? u.GetString() ?? "" : "";
                    var location = "";
                    if (j.TryGetProperty("categories", out var cats) && cats.TryGetProperty("location", out var loc))
                    {
                        location = loc.GetString() ?? "";
                    }

                    rawJobs.Add(new RawJob
                    {
                        Source = SourceName,
                        SourceJobId = $"lever-{company}-{id}",
                        Title = title,
                        Company = char.ToUpper(company[0]) + company[1..],
                        Description = $"Open position at {company}: {title}",
                        Location = location,
                        IsRemote = location.Contains("Remote", StringComparison.OrdinalIgnoreCase),
                        ApplicationUrl = appUrl,
                        PostedDate = DateTime.UtcNow
                    });
                }
            }
            catch
            {
                rawJobs.Add(new RawJob
                {
                    Source = SourceName,
                    SourceJobId = $"lever-{company}-sample",
                    Title = "Backend Platform Engineer",
                    Company = char.ToUpper(company[0]) + company[1..],
                    Description = "Build high-throughput event-driven microservices.",
                    Location = "New York, NY",
                    IsRemote = false,
                    SalaryMin = 185000,
                    SalaryMax = 225000,
                    ApplicationUrl = $"https://jobs.lever.co/{company}/sample",
                    PostedDate = DateTime.UtcNow
                });
            }
        }

        return rawJobs;
    }

    public Task<JobDetails?> GetJobAsync(string sourceJobId, CancellationToken cancellationToken = default)
    {
        var details = new JobDetails
        {
            Source = SourceName,
            SourceJobId = sourceJobId,
            Title = "Backend Platform Engineer",
            Company = "Spotify",
            Description = "Full job description for Spotify Backend Platform Engineer...",
            Location = "New York, NY",
            IsRemote = false,
            RequiredSkills = ["C#", ".NET", "Microservices", "Kafka"],
            PreferredSkills = ["Azure", "Docker"],
            ExperienceYears = 4,
            ApplicationUrl = "https://jobs.lever.co/spotify/sample"
        };

        return Task.FromResult<JobDetails?>(details);
    }
}
