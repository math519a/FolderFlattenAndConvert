using System;
using System.Net.Mime;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Webp;

namespace FolderFlattenAndConvert;
// Note: actual namespace depends on the project name.

internal static class Program
{
    private static async Task Main(string[] args)
    {
        //Remember nullable args
        if (args.Length == 0) return;
        Console.WriteLine("Path: " + args[0]);
        if (!Directory.Exists(args[0])) throw new DirectoryNotFoundException();

        //Get directories
        var directories = Directory.GetDirectories(args[0]);
        var files = directories.Select(Directory.GetFiles);

        var outputPath = args[0] + "\\output";

        //Convert
        Console.WriteLine("Creating directory: " + outputPath);
        Directory.CreateDirectory(outputPath);
        if (args.Contains("-flatten"))
        {
            foreach (var folder in files)
            {
                foreach (var s in folder)
                {
                    if (s.Contains(".jpg") || s.Contains(".jpeg") || s.Contains(".png"))
                    {
                        await ConvertToWebp(s, outputPath);
                    }
                    else if (s.Contains(".gif") || s.Contains(".mp4"))
                    {
                        //Not yet implemented
                        continue;
                    }
                }
            }
        }
        else
        {
            foreach (var folder in files)
            {
                foreach (var s in folder)
                {
                    if (s.Contains(".jpg") || s.Contains(".jpeg") || s.Contains(".png"))
                    {
                        var parent = Directory.GetParent(s) ??
                                     throw new Exception("Could not find parent directory of: " + s);
                        Directory.CreateDirectory(outputPath + "/" + parent.Name);
                        await ConvertToWebp(s, outputPath + "/" + parent.Name);
                    }
                    else if (s.Contains(".gif") || s.Contains(".mp4"))
                    {
                        //Not yet implemented
                        continue;
                    }
                }
            }
        }
    }

    private static async Task ConvertToWebp(string imagePath, string outputPath)
    {
        var imageBytes = await File.ReadAllBytesAsync(imagePath);
        var stream = new MemoryStream(imageBytes);
        var image = await Image.LoadAsync(stream);

        var outputstream = new MemoryStream();
        await image.SaveAsWebpAsync(outputstream);
        var filePath = outputPath + "\\" + Path.GetFileNameWithoutExtension(imagePath) + ".webp";
        await File.WriteAllBytesAsync(filePath, outputstream.ToArray());
        Console.WriteLine("Converted file "+Path.GetFileNameWithoutExtension(imagePath).PadRight(50)
                                           +"| "+stream.Length.ToString().PadRight(10)
                                           +"-->".PadRight(5)
                                           +outputstream.Length.ToString().PadRight(10)
                                           +" bytes");
    }

    private static async Task ConvertToWebm(string mediaPath, string outputPath)
    {
        var mediaBytes = await File.ReadAllBytesAsync(mediaPath);
        using var stream = new MemoryStream(mediaBytes);
    }
}