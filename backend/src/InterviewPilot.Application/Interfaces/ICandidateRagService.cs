namespace InterviewPilot.Application.Interfaces;

public interface ICandidateRagService
{
    Task IndexCandidateAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> QueryCandidateContextAsync(Guid userId, string query, int topK = 3, CancellationToken cancellationToken = default);
}
