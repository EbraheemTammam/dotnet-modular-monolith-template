using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Base.Utilities;

public static class ImageConverter
{
    public static async Task SaveAsWebP(this IFormFile img, string filenName, string saveTo, int quality = 50)
    {
        try
        {
            using var image = await Image.LoadAsync(img.OpenReadStream());
            var webpEncoder = new WebpEncoder
            {
                Quality = quality,
                FileFormat = WebpFileFormatType.Lossless
            };
            await image.SaveAsync(Path.Combine(saveTo, $"{filenName}.webp"), webpEncoder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting image: {ex.Message}");
        }
    }
}
