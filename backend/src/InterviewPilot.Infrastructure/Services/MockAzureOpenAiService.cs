using System.Security.Cryptography;
using System.Text;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Infrastructure.Services;

public class MockAzureOpenAiService : IAzureOpenAiService
{
    private const int EmbeddingDimensions = 384;

    public Task<JobDetails> ExtractJobDetailsAsync(string jobDescription, CancellationToken cancellationToken = default)
    {
        var lower = jobDescription.ToLowerInvariant();
        var required = new List<string>();
        var preferred = new List<string>();

        var commonSkills = new[] { ".NET", "C#", "Azure", "React", "TypeScript", "SQL", "Docker", "Kubernetes", "Microservices" };
        foreach (var s in commonSkills)
        {
            if (lower.Contains(s.ToLowerInvariant()))
                required.Add(s);
        }

        if (required.Count == 0)
        {
            required.AddRange([".NET", "C#", "SQL"]);
            preferred.AddRange(["Azure", "React"]);
        }

        var details = new JobDetails
        {
            Description = jobDescription,
            RequiredSkills = required,
            PreferredSkills = preferred,
            ExperienceYears = lower.Contains("senior") ? 5 : 3
        };

        return Task.FromResult(details);
    }

    public Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        // Deterministic pseudo-embedding vector generated from sha256 of text tokens
        var vector = new float[EmbeddingDimensions];
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));

        for (int i = 0; i < EmbeddingDimensions; i++)
        {
            var b = bytes[i % bytes.Length];
            vector[i] = (float)((b - 128) / 128.0);
        }

        // Normalize vector to unit length
        double norm = Math.Sqrt(vector.Sum(v => v * v));
        if (norm > 0)
        {
            for (int i = 0; i < EmbeddingDimensions; i++)
                vector[i] = (float)(vector[i] / norm);
        }

        return Task.FromResult(vector);
    }

    public Task<string> ChatCompletionAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"AI Assistant Response based on prompt: {userPrompt.Take(100)}...");
    }
}
