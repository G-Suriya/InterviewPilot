using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Models;

namespace InterviewPilot.Application.Interfaces;

public interface IJobDeduplicationService
{
    Task<bool> IsDuplicateAsync(RawJob job, CancellationToken cancellationToken = default);
    double CalculateSemanticSimilarity(string text1, string text2);
}
