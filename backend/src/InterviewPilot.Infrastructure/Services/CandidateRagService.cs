using System.Text.Json;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewPilot.Infrastructure.Services;

public class CandidateRagService : ICandidateRagService
{
    private readonly IRepository<CandidateResumeChunk> _chunkRepo;
    private readonly IRepository<CandidateProfile> _profileRepo;
    private readonly IAzureOpenAiService _openAiService;
    private readonly IUnitOfWork _unitOfWork;

    public CandidateRagService(
        IRepository<CandidateResumeChunk> chunkRepo,
        IRepository<CandidateProfile> profileRepo,
        IAzureOpenAiService openAiService,
        IUnitOfWork unitOfWork)
    {
        _chunkRepo = chunkRepo;
        _profileRepo = profileRepo;
        _openAiService = openAiService;
        _unitOfWork = unitOfWork;
    }

    public async Task IndexCandidateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profiles = await _profileRepo.FindAsync(p => p.UserId == userId, cancellationToken);
        var profile = profiles.FirstOrDefault();
        if (profile == null) return;

        // Clear existing chunks for clean re-indexing
        var existingChunks = await _chunkRepo.FindAsync(c => c.UserId == userId, cancellationToken);
        foreach (var c in existingChunks) _chunkRepo.Remove(c);

        var chunksToCreate = new List<(string category, string text)>();

        if (!string.IsNullOrWhiteSpace(profile.Summary))
            chunksToCreate.Add(("Summary", profile.Summary));

        foreach (var exp in profile.Experiences)
        {
            chunksToCreate.Add(("Experience", $"{exp.Role} at {exp.Company} ({exp.StartDate:yyyy} - {(exp.IsCurrent ? "Present" : exp.EndDate?.ToString("yyyy"))}): {exp.Description}. Tech: {exp.TechnologiesUsed}"));
        }

        foreach (var sk in profile.Skills)
        {
            chunksToCreate.Add(("Skill", $"{sk.Skill?.Name}: {sk.YearsExperience} years experience, level {sk.Proficiency}"));
        }

        foreach (var item in chunksToCreate)
        {
            var embedding = await _openAiService.GenerateEmbeddingAsync(item.text, cancellationToken);
            var chunk = new CandidateResumeChunk
            {
                UserId = userId,
                Category = item.category,
                Content = item.text,
                EmbeddingJson = JsonSerializer.Serialize(embedding)
            };
            await _chunkRepo.AddAsync(chunk, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> QueryCandidateContextAsync(Guid userId, string query, int topK = 3, CancellationToken cancellationToken = default)
    {
        var queryVector = await _openAiService.GenerateEmbeddingAsync(query, cancellationToken);
        var chunks = await _chunkRepo.FindAsync(c => c.UserId == userId, cancellationToken);

        if (chunks.Count == 0) return [];

        var scored = chunks.Select(c =>
        {
            var chunkVector = JsonSerializer.Deserialize<float[]>(c.EmbeddingJson) ?? [];
            double similarity = CosineSimilarity(queryVector, chunkVector);
            return (chunk: c, similarity);
        })
        .OrderByDescending(x => x.similarity)
        .Take(topK)
        .Select(x => x.chunk.Content)
        .ToList();

        return scored;
    }

    private static double CosineSimilarity(float[] vecA, float[] vecB)
    {
        if (vecA.Length != vecB.Length || vecA.Length == 0) return 0.0;
        double dot = 0.0, normA = 0.0, normB = 0.0;
        for (int i = 0; i < vecA.Length; i++)
        {
            dot += vecA[i] * vecB[i];
            normA += vecA[i] * vecA[i];
            normB += vecB[i] * vecB[i];
        }
        return (normA == 0 || normB == 0) ? 0.0 : dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
