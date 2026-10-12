using InterviewPilot.Domain.Models;

namespace InterviewPilot.Domain.Interfaces;

public interface IApplicationAdapter
{
    string SourceName { get; }
    Task<ApplicationResult> SubmitAsync(ApplicationRequest request, CancellationToken cancellationToken = default);
}
