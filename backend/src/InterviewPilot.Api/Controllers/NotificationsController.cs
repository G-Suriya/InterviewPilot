using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notifService;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public NotificationsController(INotificationService notifService)
    {
        _notifService = notifService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetNotifications(CancellationToken cancellationToken)
    {
        var notifs = await _notifService.GetNotificationsAsync(DemoUserId, cancellationToken);
        if (notifs.Count == 0)
        {
            // Seed initial notifications for UI demonstration
            return Ok(new List<NotificationDto>
            {
                new NotificationDto { Id = Guid.NewGuid(), Title = "High Match Job Discovered", Message = "Senior .NET Core Engineer at Stripe matches 94% of your skills.", Type = "Match", IsRead = false, CreatedAt = DateTime.UtcNow.AddMinutes(-20) },
                new NotificationDto { Id = Guid.NewGuid(), Title = "Mock Interview Ready", Message = "AI Mock Interview for Ramp Full-Stack Developer is prepared.", Type = "Interview", IsRead = false, CreatedAt = DateTime.UtcNow.AddHours(-1) }
            });
        }
        return Ok(notifs);
    }

    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        await _notifService.MarkAsReadAsync(id, cancellationToken);
        return Ok(new { success = true });
    }
}
