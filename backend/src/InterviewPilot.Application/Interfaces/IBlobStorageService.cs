namespace InterviewPilot.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string blobUrl, CancellationToken cancellationToken = default);
}
