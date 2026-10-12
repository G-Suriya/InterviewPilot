using InterviewPilot.Application.Interfaces;

namespace InterviewPilot.Infrastructure.Services;

public class LocalStorageBlobService : IBlobStorageService
{
    private readonly string _storageDir;

    public LocalStorageBlobService()
    {
        _storageDir = Path.Combine(Directory.GetCurrentDirectory(), "storage", "resumes");
        Directory.CreateDirectory(_storageDir);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(_storageDir, safeFileName);

        using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return $"/storage/resumes/{safeFileName}";
    }

    public Task<Stream?> DownloadAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(blobUrl);
        var filePath = Path.Combine(_storageDir, fileName);

        if (!File.Exists(filePath))
            return Task.FromResult<Stream?>(null);

        Stream stream = File.OpenRead(filePath);
        return Task.FromResult<Stream?>(stream);
    }
}
