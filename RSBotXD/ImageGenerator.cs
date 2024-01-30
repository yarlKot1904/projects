using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RSBotXD
{
        public class ImageGenerator{
        
        private static string resultPath = @"D:\output.jpg";
        public static string Generate(string imagePath, string text)
        {
            

            var background = Brushes.Black;
            var textColor = Brushes.White;

            var gap = 20;
            var fontSize = 70;

            var dpi = 96;

            var font =
                new Typeface(
                    new FontFamily("Segoe UI"), FontStyles.Normal,
                    FontWeights.Bold, FontStretches.SemiExpanded);
            // <--

            var image = BitmapFrame.Create(new Uri("file://" + imagePath));
            var imageWidth = (double)image.PixelWidth;
            var imageHeight = (double)image.PixelHeight;

            var formattedText =
                new FormattedText(
                    text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    font, fontSize, textColor, dpi)
                {
                    MaxTextWidth = imageWidth,
                    TextAlignment = TextAlignment.Center
                };

            var textWidth = formattedText.Width;
            var textHeight = formattedText.Height;

            var totalWidth = (int)Math.Ceiling(imageWidth + 2 * gap);
            var totalHeight = (int)Math.Ceiling(imageHeight + 3 * gap + textHeight);

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    background, null,
                    new Rect(0, 0, totalWidth, totalHeight));

                drawingContext.DrawImage(
                    image,
                    new Rect(gap, gap, imageWidth, imageHeight));
                drawingContext.DrawText(
                    formattedText,
                    new Point(gap, imageHeight + 2 * gap));
            }

            var bmp =
                new RenderTargetBitmap(
                    totalWidth, totalHeight, dpi, dpi,
                    PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (var stream = File.Create(resultPath))
                encoder.Save(stream);
            return resultPath;
        }
    }
}