using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/ai")]
public class AiAssistantController : ControllerBase
{
    private readonly IApplicationAssistantService _assistantService;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public AiAssistantController(IApplicationAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    [HttpPost("resume-tailor")]
    public async Task<ActionResult<TailoredResumeDto>> TailorResume([FromQuery] Guid jobId, CancellationToken cancellationToken)
    {
        var result = await _assistantService.TailorResumeAsync(DemoUserId, jobId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("cover-letter")]
    public async Task<ActionResult<CoverLetterDto>> GenerateCoverLetter([FromQuery] Guid jobId, [FromQuery] string tone = "Professional", CancellationToken cancellationToken = default)
    {
        var result = await _assistantService.GenerateCoverLetterAsync(DemoUserId, jobId, tone, cancellationToken);
        return Ok(result);
    }

    [HttpPost("application-answer")]
    public async Task<ActionResult<object>> GenerateAnswer([FromQuery] Guid jobId, [FromBody] AnswerQuestionRequest request, CancellationToken cancellationToken)
    {
        var answer = await _assistantService.AnswerQuestionAsync(DemoUserId, jobId, request.Question, cancellationToken);
        return Ok(new { question = request.Question, suggestedAnswer = answer });
    }
}

public class AnswerQuestionRequest
{
    public string Question { get; set; } = string.Empty;
}
