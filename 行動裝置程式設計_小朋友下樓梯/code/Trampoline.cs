using Plugin.SimpleAudioPlayer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace game
{
    public class Trampoline
    {
        public float x, y;
        public float dy;
        SKBitmap image;
        ISimpleAudioPlayer player;

        public Trampoline(float x, float y, float dy, ContentPage cont)
        {
            this.x = x;
            this.y = 2048 + y;
            this.dy = dy;
            Assembly assembly1 = cont.GetType().GetTypeInfo().Assembly;
            string resourceID = "game.Resources.trampoline.png";
            using (Stream stream = assembly1.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bmp = SKBitmap.Decode(skStream);
                for (int j = 0; j < 4; j++)
                {
                    SKBitmap subimg = new SKBitmap(97, 22);
                    SKRectI rectI = new SKRectI(0, j*22, 97,(j+1)* 22 - 1);
                    bmp.ExtractSubset(subimg, rectI);
                    //圖片擴展到140*140
                    var dstInfo = new SKImageInfo(280, 40);
                    subimg = subimg.Resize(dstInfo, SKFilterQuality.Medium);
                    image=subimg;
                }

            }

            player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            Assembly assembly = cont.GetType().GetTypeInfo().Assembly;
            string musicresourceID = "game.Resources.jump.mp3";
            using (Stream stream = assembly.GetManifestResourceStream(musicresourceID))
                player.Load(stream);

        }
        public void playSound()
        {
            player.Play();
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
