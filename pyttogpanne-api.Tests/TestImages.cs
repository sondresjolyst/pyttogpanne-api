using Microsoft.Extensions.DependencyInjection;
using pyttogpanne_api.Models;
using SjolystInnovation.AppKit.Storage.Images;

namespace pyttogpanne_api.Tests;

/// <summary>The package's image service over a test database, with its files in a directory of its own.</summary>
public sealed class TestImages : IDisposable
{
    private readonly ServiceProvider _provider;

    public TestImages(ApplicationDbContext db)
    {
        Directory.CreateDirectory(Root);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(db);
        services.AddAppKitContentImages<ApplicationDbContext>(o => o.ImagesPath = Root);
        _provider = services.BuildServiceProvider();
        Service = _provider.GetRequiredService<IContentImageService>();
    }

    public string Root { get; } = Path.Combine(Path.GetTempPath(), $"pyttogpanne-images-{Guid.NewGuid():N}");

    public IContentImageService Service { get; }

    /// <summary>An image row with a file on disk behind it.</summary>
    public ContentImage Stored(string id)
    {
        File.WriteAllBytes(Path.Combine(Root, $"{id}.png"), [1, 2, 3]);
        return new ContentImage { Id = id, FileName = $"{id}.png", ContentType = "image/png", StoredPath = $"{id}.png" };
    }

    public bool FileExists(string id) => File.Exists(Path.Combine(Root, $"{id}.png"));

    public void Dispose()
    {
        _provider.Dispose();
        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }
}
