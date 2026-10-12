using InterviewPilot.Application.DTOs;

namespace InterviewPilot.Application.Interfaces;

public interface IResumeParserService
{
    Task<ParsedResumeDto> ParseResumeAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
}
