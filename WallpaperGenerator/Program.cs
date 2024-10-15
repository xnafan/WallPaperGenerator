using System;
using System.IO;

namespace WallpaperGenerator
{
    class Program
    {

        private const string SUBFOLDER_NAME = "wallpaper";
        static void Main(string[] args)
        {
            DateTime begin = DateTime.Now;
            if (args.Length == 0 || !PathIsValid(args[0]))
            {
                WriteUsage();
                return;
            }

            string path = args[0];
            uint width = GetWidth(args);
            uint height = GetHeight(args);

            if (Directory.Exists(path))
            {
                var wallpaperSubfolder = AddSubfolderToPath(path, SUBFOLDER_NAME);
                WallpaperMakerTool.ResizeImagesInFolderWithBlurBackground(path, wallpaperSubfolder, width, height);
            }
            else if (File.Exists(path))
            {
                var wallpaperSubfolder = AddSubfolderToPath(Path.GetDirectoryName(path), SUBFOLDER_NAME);
                WallpaperMakerTool.ResizeImageWithBlurBackgroundToDestinationFolder(wallpaperSubfolder, width, height, path);
            }
            else
            {
                Console.WriteLine($"Unable to verify path '{path}' as either directory or file. Exiting program.");
                return;
            }
            DateTime end = DateTime.Now;
            TimeSpan timeTaken = end - begin;
            Console.WriteLine($"Time taken: {timeTaken.TotalMinutes:00}m {timeTaken.TotalSeconds % 60:00}s");
        }

        private static uint GetHeight(string[] args)
        {
            int height = 0;
            if (args.Length < 3 || !int.TryParse(args[2], out height))
            {
                height = DisplayPropertiesTool.GetPrimaryDisplaySize().Height;
                Console.WriteLine($"unable to parse argumentlist to integer value. Going with monitor's height: {height}px");
            }
            return (uint)height;
        }

        private static uint GetWidth(string[] args)
        {
            int width = 0;
            if (args.Length < 2 || !int.TryParse(args[1], out width))
            {
                width = DisplayPropertiesTool.GetPrimaryDisplaySize().Width;
                Console.WriteLine($"unable to parse argumentlist to integer value. Going with monitor's width: {width}px");
            }
            return (uint)width;
        }

        private static bool PathIsValid(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }

        private static void WriteUsage()
        {
            Console.WriteLine($"Wallpaper generator");
            Console.WriteLine($"USAGE:");
            Console.WriteLine($"WallpaperGenerator.exe [imagepath or folderpath] [width] [height]");
        }

        private static string AddSubfolderToPath(string directoryPath, string subfolderName)
        {
            return Path.Combine(directoryPath, "wallpaper");
        }
    }
}