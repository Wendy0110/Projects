using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace game
{
    public class Nails
    {
        public float x, y;
        public float dy;
        SKBitmap image;

        public Nails(float x, float y, float dy, ContentPage cont)
        {
            this.x = x;
            this.y = 2048 + y;
            this.dy = dy;
            Assembly assembly1 = cont.GetType().GetTypeInfo().Assembly;
            string resourceID = "game.Resources.nails.png";
            using (Stream stream = assembly1.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bmp = SKBitmap.Decode(skStream);
                var dstInfo = new SKImageInfo(280, 40);
                image = bmp.Resize(dstInfo, SKFilterQuality.Medium);
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
