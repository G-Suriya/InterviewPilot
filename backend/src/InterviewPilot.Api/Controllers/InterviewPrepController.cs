using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/interview")]
public class InterviewPrepController : ControllerBase
{
    private readonly IInterviewPrepService _interviewService;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public InterviewPrepController(IInterviewPrepService interviewService)
    {
        _interviewService = interviewService;
    }

    [HttpPost("generate/{jobId:guid}")]
    public async Task<ActionResult<InterviewSessionDto>> GenerateMockInterview(Guid jobId, CancellationToken cancellationToken)
    {
        var session = await _interviewService.GenerateMockInterviewAsync(DemoUserId, jobId, cancellationToken);
        return Ok(session);
    }

    [HttpPost("evaluate/{questionId:guid}")]
    public async Task<ActionResult<AnswerEvaluationDto>> EvaluateAnswer(Guid questionId, [FromBody] EvaluateAnswerRequest request, CancellationToken cancellationToken)
    {
        var eval = await _interviewService.EvaluateAnswerAsync(questionId, request.CandidateAnswer, cancellationToken);
        return Ok(eval);
    }
}

public class EvaluateAnswerRequest
{
    public string CandidateAnswer { get; set; } = string.Empty;
}
