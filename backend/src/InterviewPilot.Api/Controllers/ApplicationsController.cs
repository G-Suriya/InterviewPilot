using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _appService;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public ApplicationsController(IApplicationService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ApplicationDto>>> GetApplications(
        [FromQuery] ApplicationStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var apps = await _appService.GetApplicationsByUserIdAsync(DemoUserId, status, cancellationToken);
        return Ok(apps);
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> CreateApplication(
        [FromQuery] Guid jobId,
        [FromQuery] ApplicationMethod method = ApplicationMethod.Manual,
        CancellationToken cancellationToken = default)
    {
        var app = await _appService.CreateApplicationAsync(DemoUserId, jobId, method, cancellationToken);
        return Ok(app);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApplicationDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateApplicationStatusDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _appService.UpdateStatusAsync(id, dto.Status, dto.Notes, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }
}
