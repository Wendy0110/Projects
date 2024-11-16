using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;

namespace game
{
    public class player
    {
        public float x, y;
        public float dx, dy;
        public int live;
        int delay = 15;
        int g = 0;
        int frame = 0;
        List<SKBitmap> images;

        public player(float x, float y, int live, ContentPage cont)
        {
            this.x = x;
            this.y = y;
            this.live = live;
            frame = 8;
            images = new List<SKBitmap>();
            dx = 0;
            dy = 10;
            Assembly assembly = cont.GetType().GetTypeInfo().Assembly;
            string resourceID = "game.Resources.player.png";

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                SKBitmap bmp = SKBitmap.Decode(skStream);
                for (int i = 0; i < 5; i++)
                {
                    if (i <= 1)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            SKBitmap subimg = new SKBitmap(32, 32);
                            SKRectI rectI = new SKRectI(j * 32, i * 32, (j + 1) * 32 - 1, (i + 1) * 32 - 1);
                            bmp.ExtractSubset(subimg, rectI);
                            //圖片擴展到140*140
                            var dstInfo = new SKImageInfo(140, 140);
                            subimg = subimg.Resize(dstInfo, SKFilterQuality.Medium);
                            images.Add(subimg);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            SKBitmap subimg = new SKBitmap(32, 32);
                            SKRectI rectI = new SKRectI(j * 32, i * 32, (j + 1) * 32 - 1, (i + 1) * 32 - 1);
                            bmp.ExtractSubset(subimg, rectI);
                            var dstInfo = new SKImageInfo(140, 140);
                            subimg = subimg.Resize(dstInfo, SKFilterQuality.Medium);
                            images.Add(subimg);
                        }
                    }
                }
            }
        }
        public int Move(float mx, float my, int conveyor_type, int iii)
        {
            touch(mx, my, g, conveyor_type, iii);
            delay--;
            if (delay == 0)
            {
                g++;
                delay = 15;
            }
            if (g > 1)
            {
                g = 0;
            }
            this.x += dx;
            this.y += dy;
            if (this.x < 0)
            {
                this.x = 0;
                dy = 10;
                return 1;
            }
            else if (this.x > 1050 - images[0].Width)
            {
                this.x = 1050 - images[0].Width;
                return 0;
            }
            if (this.y < 0)
            {
                this.y = 0;
                return 1;
            }
            else if (this.y > 2048 - images[0].Height)
            {
                live = 0;
                return 2;
            }
            return 0;
        }


        public bool Coll(Stone s) //使用Rect做藍色平台的碰撞偵測
        {
            Rect rect1 = new Rect(s.x, s.y, 280, 20);
            Rect rec2 = new Rect(x, y + images[0].Height, images[0].Width, 1);
            if (rect1.IntersectsWith(rec2))
            {
                return true;
            }
            return false;
        }
        public bool Coll_nails(Nails s) //使用Rect做尖刺的碰撞偵測
        {
            Rect rect1 = new Rect(s.x, s.y, 280, 20);
            Rect rec2 = new Rect(x, y + images[0].Height, images[0].Width, 1);
            if (rect1.IntersectsWith(rec2))
            {
                return true;
            }
            return false;
        }
        public bool Coll_Trampoline(Trampoline s) //使用Rect做彈簧板的碰撞偵測
        {
            Rect rect1 = new Rect(s.x, s.y, 280, 20);
            Rect rec2 = new Rect(x, y + images[0].Height, images[0].Width, 1);
            if (rect1.IntersectsWith(rec2))
            {
                return true;
            }
            return false;
        }
        public bool Coll_conveyor_right(conveyor_right s) //使用Rect做往左傳送帶的碰撞偵測
        {
            Rect rect1 = new Rect(s.x, s.y, 280, 20);
            Rect rec2 = new Rect(x, y + images[0].Height, images[0].Width, 1);
            if (rect1.IntersectsWith(rec2))
            {
                return true;
            }
            return false;
        }
        public bool Coll_conveyor_left(conveyor_left s) //使用Rect做往右傳送帶的碰撞偵測
        {
            Rect rect1 = new Rect(s.x, s.y, 280, 20);
            Rect rec2 = new Rect(x, y + images[0].Height, images[0].Width, 1);
            if (rect1.IntersectsWith(rec2))
            {
                return true;
            }
            return false;
        }

        bool sss = false;
        public void touch(float mx, float my, int g, int conveyor_type, int iii)
        {
            if (iii == 1)
            {
                sss = false;
            }
            if (conveyor_type == 1)
            {
                if (mx < this.x - 20)  //滑鼠點在人物左邊向左移
                {
                    dx = -15;
                    frame = 8;
                }
                else if (mx > this.x + 20)  //滑鼠點在人物右邊,向右移
                {
                    dx = 5;
                    frame = 8;
                    if (sss == true)
                    {
                        dx = -10;
                        frame = 8;
                    }
                }
                else  //滑鼠在人物附近就停止
                {
                    dx = -10;
                    frame = 8;
                    sss = true;
                }
            }
            else if (conveyor_type == 0)
            {
                if (mx < this.x - 20)  //滑鼠點在人物左邊向左移
                {
                    dx = -5;
                    frame = 8;
                }
                else if (mx > this.x + 20)  //滑鼠點在人物右邊,向右移
                {
                    dx = 15;
                    frame = 8;
                    if (sss == true)
                    {
                        dx = 10;
                        frame = 8;
                    }
                }
                else  //滑鼠在人物附近就停止
                {
                    dx = 10;
                    frame = 8;
                    sss = true;
                }
            }
            else
            {
                if (mx < this.x - 20)  //滑鼠點在人物左邊向左移
                {
                    dx = -10;
                    frame = 0 + g;
                }
                else if (mx > this.x + 20)  //滑鼠點在人物右邊,向右移
                {
                    dx = 10;
                    frame = 9 + g;
                }
                else  //滑鼠在人物附近就停止
                {
                    dx = 0;
                    frame = 8;
                }
            }

        }
        public void Draw(SKCanvas canvas)
        {
            if (live > 0)
                canvas.DrawBitmap(images[frame], x, y);
        }
    }
}
