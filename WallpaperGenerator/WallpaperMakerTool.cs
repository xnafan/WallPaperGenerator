using ImageMagick;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WallpaperGenerator
{
    public static class WallpaperMakerTool
    {

        public static void ResizeImagesInFolderWithBlurBackground(string sourceFolderPath, string destinationFolderPath, uint width, uint height)

        {

            var files = Directory.GetFiles(sourceFolderPath);
            DateTime begin = DateTime.Now;

            //Parallel.ForEach(
            //    files,
            //    imageSourcePath => { ResizeImageWithBlurBackgroundToDestinationFolder(destinationFolderPath, width, height, imageSourcePath); });


            files.ToList().ForEach(filePath =>
                    ResizeImageWithBlurBackgroundToDestinationFolder(destinationFolderPath, width, height, filePath));




            DateTime done = DateTime.Now;

            Console.WriteLine($"Time taken: {(done - begin).TotalSeconds}s");
        }

        public static void ResizeImageWithBlurBackgroundToDestinationFolder(string destinationFolderPath, uint width, uint height, string imageSourcePath)
        {
            var sourceImageName = Path.GetFileName(imageSourcePath);
            var destinationImageName = AddResolutionToFileName(sourceImageName, width, height);
            var destinationImagePath = Path.Combine(destinationFolderPath, destinationImageName);
            ResizeImageWithBlurBackgroundToDestinationFile(imageSourcePath, destinationImagePath, width, height);
        }

        private static string AddResolutionToFileName(string imageFileName, uint width, uint height)
        {
            return $"{Path.GetFileNameWithoutExtension(imageFileName)}_{width}x{height}.jpg";
        }

        public static void ResizeImageWithBlurBackgroundToDestinationFile(string sourceImagePath, string destinationImagePath, uint width, uint height)
        {
            try
            {
                using (var image = new MagickImage(new MagickColor("#000000"), width, height))
                {

                    using (var loadImage = new MagickImage(sourceImagePath))
                    {
                        Console.WriteLine($"Converting '{sourceImagePath}'");
                        var geometry = new MagickGeometry(width, height);
                        if (loadImage.Width != width || loadImage.Height != height)
                        {

                            using (var blurImage = loadImage.Clone())
                            {

                                //geometry.IgnoreAspectRatio = true;
                                geometry.FillArea = true;
                                blurImage.Blur(0, 5);
                                blurImage.Resize(geometry);
                                blurImage.BrightnessContrast(new Percentage(-15), new Percentage(1));
                                image.Composite(blurImage, Gravity.Center, CompositeOperator.SrcOver);
                            }
                        }

                        geometry.IgnoreAspectRatio = false;
                        geometry.FillArea = false;

                        loadImage.Resize(geometry);
                        loadImage.Sharpen();
                        image.Composite(loadImage, Gravity.Center, CompositeOperator.SrcOver);

                        image.Format = MagickFormat.Jpeg;
                        image.Quality = 90;
                        EnsureFolderExists(destinationImagePath);
                        image.Write(destinationImagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while converting '{sourceImagePath}' to '{destinationImagePath}'. The error is '{ex.ToString()}'");
            }
        }

        private static void EnsureFolderExists(string outImagePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outImagePath));
        }
    }
}