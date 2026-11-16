using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace TangyAzureFunc;

public class ResizeImageOnBlobUpload
{
    private readonly ILogger<ResizeImageOnBlobUpload> _logger;

    public ResizeImageOnBlobUpload(ILogger<ResizeImageOnBlobUpload> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ResizeImageOnBlobUpload))]
    [BlobOutput("salesrep-resized/{name}")]
    public async Task<Byte[]> Run([BlobTrigger("salesrep/{name}")] Byte[] bytes, string name)
    {
        using var memoryStream = new MemoryStream(bytes);
        using var image = Image.Load(memoryStream);
        image.Mutate(x => x.Resize(100, 100));
        using var outputStream = new MemoryStream();
        image.SaveAsJpeg(outputStream);
        outputStream.Position = 0;
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name}", name);

        return outputStream.ToArray();      
    }
}