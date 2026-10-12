using InterviewPilot.Domain.Models;

namespace InterviewPilot.Application.Interfaces;

public interface IAzureOpenAiService
{
    Task<JobDetails> ExtractJobDetailsAsync(string jobDescription, CancellationToken cancellationToken = default);
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<string> ChatCompletionAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);
}
