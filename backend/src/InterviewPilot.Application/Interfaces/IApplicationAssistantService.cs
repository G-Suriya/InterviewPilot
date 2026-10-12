using InterviewPilot.Application.DTOs;

namespace InterviewPilot.Application.Interfaces;

public interface IApplicationAssistantService
{
    Task<TailoredResumeDto> TailorResumeAsync(Guid userId, Guid jobId, CancellationToken cancellationToken = default);
    Task<CoverLetterDto> GenerateCoverLetterAsync(Guid userId, Guid jobId, string tone = "Professional", CancellationToken cancellationToken = default);
    Task<string> AnswerQuestionAsync(Guid userId, Guid jobId, string question, CancellationToken cancellationToken = default);
}
