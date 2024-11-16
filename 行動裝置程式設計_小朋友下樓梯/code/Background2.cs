using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace game
{
    public class Background2
    {
        SKBitmap image;
        public Background2(ContentPage cont)
        {
            Assembly assembly = cont.GetType().GetTypeInfo().Assembly;
            string resourceID = "game.Resources.bg1.png";
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bmp = SKBitmap.Decode(skStream);
                var dstInfo = new SKImageInfo(1080, 2340);
                image = bmp.Resize(dstInfo, SKFilterQuality.Medium);
            }
        }
        public void Draw(SKCanvas canvas)
        {
            canvas.DrawBitmap(image, 0, 0);

        }
    }
}