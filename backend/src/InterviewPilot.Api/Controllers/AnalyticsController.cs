using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet]
    public async Task<ActionResult<CandidateAnalyticsDto>> GetAnalytics(CancellationToken cancellationToken)
    {
        var data = await _analyticsService.GetAnalyticsAsync(DemoUserId, cancellationToken);
        return Ok(data);
    }
}
