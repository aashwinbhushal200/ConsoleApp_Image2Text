using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Tesseract;
using System.IO;

public class Program
{
    // ... rest of the code ...
    public static void Main(string[] args)
    {
        var imagePath = @"C:\Users\pc\Pictures\order.jpg"; // Replace with actual image path
        var text = ReadTextFromImage(imagePath);
        Console.WriteLine(text);
    }
    private static string ReadTextFromImage(string imagePath)
    {
        using (var image = Image.Load<Rgba32>(imagePath))
        {
            // Convert to grayscale for better OCR accuracy
            image.Mutate(x => x.Grayscale());

            using (var memoryStream = new MemoryStream())
            {
                // Save the image as BMP format (compatible with Tesseract)
                image.SaveAsBmp(memoryStream);
                memoryStream.Position = 0;

                // Use a temporary file for Tesseract processing (optional)
                string tempFilePath = Path.Combine(Path.GetTempPath(), "image.bmp");
                using (var fileStream = File.OpenWrite(tempFilePath))
                {
                    memoryStream.CopyTo(fileStream);
                }

                using (var pix = Pix.LoadFromFile(tempFilePath)) // Use Tesseract's Pix.FromFile to load the BMP image
                {
                    using (var engine = new TesseractEngine(@".\tessdata", "eng")) // Replace with Tesseract installation path and language
                    {
                        using (var page = engine.Process(pix))
                        {
                            return page.GetText();
                        }
                    }
                }

                // Optionally delete the temporary file after processing
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}
