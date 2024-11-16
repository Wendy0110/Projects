using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace game
{
    public class conveyor_left
    {
        public float x, y;
        public float dy;
        SKBitmap image;

        public conveyor_left(float x, float y, float dy, ContentPage cont)
        {
            this.x = x;
            this.y = 2048 + y;
            this.dy = dy;
            Assembly assembly1 = cont.GetType().GetTypeInfo().Assembly;
            string resourceID = "game.Resources.conveyor_left.png";
            using (Stream stream = assembly1.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bmp = SKBitmap.Decode(skStream);
                for (int j = 0; j < 4; j++)
                {
                    SKBitmap subimg = new SKBitmap(97, 22);
                    SKRectI rectI = new SKRectI(0, j * 16, 96, (j + 1) * 16 - 1);
                    bmp.ExtractSubset(subimg, rectI);
                    var dstInfo = new SKImageInfo(280, 40);
                    subimg = subimg.Resize(dstInfo, SKFilterQuality.Medium);
                    image = subimg;
                }
            }
        }
        public bool Move()
        {
            y += dy;
            if (y < 0)
            {
                return false;
            }
            return true;
        }

        public void Draw(SKCanvas canvas)
        {
            if (x > 0)
            {
                canvas.DrawBitmap(image, x, y);
            }
        }
    }
}
