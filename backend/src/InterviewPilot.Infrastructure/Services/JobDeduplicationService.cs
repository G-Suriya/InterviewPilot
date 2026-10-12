using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Interfaces;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Infrastructure.Services;

public class JobDeduplicationService : IJobDeduplicationService
{
    private readonly IRepository<Job> _jobRepo;

    public JobDeduplicationService(IRepository<Job> jobRepo)
    {
        _jobRepo = jobRepo;
    }

    public async Task<bool> IsDuplicateAsync(RawJob job, CancellationToken cancellationToken = default)
    {
        // Signal 1: Exact Source and SourceJobId
        var matchSourceId = await _jobRepo.FindAsync(j =>
            j.Source == job.Source && j.SourceJobId == job.SourceJobId, cancellationToken);
        if (matchSourceId.Count > 0) return true;

        // Signal 2: Exact Application URL
        if (!string.IsNullOrEmpty(job.ApplicationUrl))
        {
            var matchUrl = await _jobRepo.FindAsync(j => j.ApplicationUrl == job.ApplicationUrl, cancellationToken);
            if (matchUrl.Count > 0) return true;
        }

        // Signal 3: Company + Normalized Title + Location
        var normComp = job.Company.Trim().ToLowerInvariant();
        var normTitle = job.Title.Trim().ToLowerInvariant();
        var normLoc = job.Location.Trim().ToLowerInvariant();

        var matchFuzzy = await _jobRepo.FindAsync(j =>
            j.CompanyName.ToLower() == normComp &&
            j.Title.ToLower() == normTitle &&
            j.Location.ToLower() == normLoc, cancellationToken);
        if (matchFuzzy.Count > 0) return true;

        // Signal 4: High Text Similarity (Jaccard similarity > 0.85)
        var recentJobs = await _jobRepo.GetAllAsync(cancellationToken);
        foreach (var existing in recentJobs.Take(100))
        {
            if (existing.CompanyName.Equals(job.Company, StringComparison.OrdinalIgnoreCase))
            {
                var sim = CalculateSemanticSimilarity(existing.Description, job.Description);
                if (sim >= 0.85) return true;
            }
        }

        return false;
    }

    public double CalculateSemanticSimilarity(string text1, string text2)
    {
        if (string.IsNullOrWhiteSpace(text1) || string.IsNullOrWhiteSpace(text2)) return 0.0;

        var tokens1 = text1.ToLowerInvariant().Split([' ', '\n', '\r', '\t', '.', ','], StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var tokens2 = text2.ToLowerInvariant().Split([' ', '\n', '\r', '\t', '.', ','], StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        int intersection = tokens1.Count(tokens2.Contains);
        int union = tokens1.Count + tokens2.Count - intersection;

        return union == 0 ? 0.0 : (double)intersection / union;
    }
}
