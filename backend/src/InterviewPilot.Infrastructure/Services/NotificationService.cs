using InterviewPilot.Application.DTOs;
using InterviewPilot.Application.Interfaces;
using InterviewPilot.Domain.Entities;
using InterviewPilot.Domain.Interfaces;

namespace InterviewPilot.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notifRepo;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IRepository<Notification> notifRepo, IUnitOfWork unitOfWork)
    {
        _notifRepo = notifRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task SendNotificationAsync(Guid userId, string title, string message, string type = "Info", CancellationToken cancellationToken = default)
    {
        var notif = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notifRepo.AddAsync(notif, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifs = await _notifRepo.FindAsync(n => n.UserId == userId, cancellationToken);
        return notifs.OrderByDescending(n => n.CreatedAt).Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notif = await _notifRepo.GetByIdAsync(notificationId, cancellationToken);
        if (notif != null)
        {
            notif.IsRead = true;
            _notifRepo.Update(notif);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
