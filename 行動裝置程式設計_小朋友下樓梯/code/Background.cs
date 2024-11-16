using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace game
{
    public class Background
    {
        float x1, y1,x2, y2; 
        SKBitmap image1, image2, image3;
        public Background(ContentPage cont)
        {
            x1 = 0; y1 = 0;
            x2 = 1050; y2 = 0;
            Assembly assembly1 = cont.GetType().GetTypeInfo().Assembly;
            string resourceID = "game.Resources.wall.png";
            using (Stream stream = assembly1.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bmp = SKBitmap.Decode(skStream);
                var dstInfo = new SKImageInfo(30, 2340);
                image1 = bmp.Resize(dstInfo, SKFilterQuality.Medium);
                image2 = bmp.Resize(dstInfo, SKFilterQuality.Medium);
            }
        }
        public void Draw(SKCanvas canvas)
        {
            canvas.DrawBitmap(image1, x1, y1);
            canvas.DrawBitmap(image2, x2, y2);
            //canvas.DrawBitmap(image3, 0, 0);

        }

    }
}
