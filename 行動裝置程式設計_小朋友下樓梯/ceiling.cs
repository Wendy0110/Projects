using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace game
{
    public class ceiling
    {
        SKBitmap image;
        public ceiling(ContentPage cont)
        {
            Assembly assembly = cont.GetType().GetTypeInfo().Assembly;
            string resourceID = "game.Resources.ceiling.png";
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bmp = SKBitmap.Decode(skStream);
                var dstInfo = new SKImageInfo(1080, 30);
                image= bmp.Resize(dstInfo, SKFilterQuality.Medium);
            }
        }
        public void Draw(SKCanvas canvas)
        {
            canvas.DrawBitmap(image, 0, 0);

        }
    }
}
