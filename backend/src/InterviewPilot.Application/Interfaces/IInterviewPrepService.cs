using InterviewPilot.Application.DTOs;

namespace InterviewPilot.Application.Interfaces;

public interface IInterviewPrepService
{
    Task<InterviewSessionDto> GenerateMockInterviewAsync(Guid userId, Guid jobId, CancellationToken cancellationToken = default);
    Task<AnswerEvaluationDto> EvaluateAnswerAsync(Guid questionId, string candidateAnswer, CancellationToken cancellationToken = default);
}
