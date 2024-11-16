using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using OpenTK.Input;
using OpenTK;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Xamarin.CommunityToolkit;
using Xamarin.CommunityToolkit.UI.Views;
using Plugin.SimpleAudioPlayer;

namespace game
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        string blood = "||||||||||";//血條跟位置
        string textScore = "0";
        string filename = "分數.txt";

        float mx, my;
        bool pageIsActive = false;
        Background bg;
        Background2 bg2;
        ceiling c;
        List<Stone> s;//一般地板
        List<Nails> nails;//有刺地坂
        List<Trampoline> trampoline;//彈跳地板
        Trampoline tmusic;
        List<conveyor_right> conveyor_right;
        List<conveyor_left> conveyor_left;

        player p;
        Random r;
        SKCanvasView canvas;
        List<IEmeny> emenies;
        private MediaElement jumpSound;

        public GamePage()
        {
            InitializeComponent();
            mx = 400;
            my = 30;
            r = new Random();
            bg = new Background(this);
            bg2 = new Background2(this);
            c = new ceiling(this);

            s = new List<Stone>();
            nails = new List<Nails>();
            trampoline = new List<Trampoline>();
            tmusic = new Trampoline(0, 0, 0, this);
            conveyor_right = new List<conveyor_right>();
            conveyor_left = new List<conveyor_left>();

            emenies = new List<IEmeny>();
            p = new player(mx, my, 3, this);
            canvas = new SKCanvasView() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            canvas.PaintSurface += OnCanvasPaintSurface;
            stacklayout.Children.Add(canvas);
            jumpSound = new MediaElement
            {
                Source = "jump.mp3",
                AutoPlay = false,
                Volume = 1.0, // 音量 (0.0 到 1.0 之間)
                IsLooping = false
            };


        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            pageIsActive = true;
            GameLoop();
            AnimationLoop();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pageIsActive = false;
        }

        void Clear()
        {

            Nails nails_2 = null;
            Trampoline trampoline_2 = null;
            conveyor_right conveyor_right_2 = null;
            conveyor_left conveyor_left_2 = null;


            Stone rs = null;
            foreach (Stone b in s)//一般
            {
                if (!b.Move())
                    rs = b;
            }
            if (rs != null)//一般
                s.Remove(rs);


            foreach (Nails b in nails)//刺
            {
                if (!b.Move())
                    nails_2 = b;
            }
            foreach (Trampoline b in trampoline)//跳
            {
                if (!b.Move())
                {
                    trampoline_2 = b;
                }
            }
            foreach (conveyor_right b in conveyor_right)//碰到磚塊右移
            {
                if (!b.Move())
                {
                    conveyor_right_2 = b;
                }
            }
            foreach (conveyor_left b in conveyor_left)//碰到磚塊右移
            {
                if (!b.Move())
                {
                    conveyor_left_2 = b;
                }
            }



            if (nails_2 != null)//刺
                nails.Remove(nails_2);
            if (trampoline_2 != null)//跳
                trampoline.Remove(trampoline_2);
            if (conveyor_right_2 != null)//碰到磚塊右移
                conveyor_right.Remove(conveyor_right_2);
            if (conveyor_left_2 != null)//碰到磚塊右移
                conveyor_left.Remove(conveyor_left_2);

        }

        int generate = 0; //出現磚塊時間
        int Attack_twice_in_a_row = 0; //不要一直都出現刺
        int add_tpye = 0; //新增的磚塊類型
        void add_brick()
        {
            if (generate <= 0) //隨機時間產生新的平台
            {
                float xx, yy;
                xx = r.Next(770); //亂數產生座標及位移向量
                yy = 0;
                int temp = s.Count();
                int nauls_count = nails.Count();
                int trampoline_count = trampoline.Count();
                int conveyor_right_count = conveyor_right.Count();
                int conveyor_left_count = conveyor_left.Count();

                if (add_tpye == 0) //產生藍色普通平台
                {
                    if (temp != 0) //將新產生的平台加入串列
                    {
                        if (Math.Abs(s[temp - 1].x - xx) < 140)
                            xx = r.Next(770);
                    }
                    Stone S = new Stone(xx, yy, -4, this);
                    s.Add(S);
                    add_tpye = r.Next(0, 5);
                    if (int.TryParse(textScore, out int result))
                    {
                        //轉換成功，result 中為轉換後的整數值
                        result = result + 1;
                        textScore = result.ToString();
                    }
                }
                else if (add_tpye == 1) //產生彈簧板
                {
                    if (trampoline_count != 0)
                    {
                        if (Math.Abs(trampoline[trampoline_count - 1].x - xx) < 140)
                            xx = r.Next(770);
                    }
                    Trampoline t = new Trampoline(xx, yy, -4, this);
                    trampoline.Add(t);
                    add_tpye = r.Next(0, 5);
                    if (int.TryParse(textScore, out int result))
                    {
                        //轉換成功，result 中為轉換後的整數值
                        result = result + 2;
                        textScore = result.ToString();
                    }
                }
                else if (add_tpye == 2) //產生尖刺平台
                {
                    if (nauls_count != 0)
                    {
                        if (Math.Abs(nails[nauls_count - 1].x - xx) < 140)
                            xx = r.Next(770);
                    }
                    Nails n = new Nails(xx, yy, -4, this);
                    nails.Add(n);
                    add_tpye = r.Next(0, 5);
                    if (int.TryParse(textScore, out int result))
                    {
                        //轉換成功，result 中為轉換後的整數值
                        result = result + 3;
                        textScore = result.ToString();
                    }
                }
                else if (add_tpye == 3) //產生往右傳送帶
                {
                    if (conveyor_right_count != 0)
                    {
                        if (Math.Abs(conveyor_right[conveyor_right_count - 1].x - xx) < 140)
                            xx = r.Next(770);
                    }
                    conveyor_right n = new conveyor_right(xx, yy, -4, this);
                    conveyor_right.Add(n);
                    add_tpye = r.Next(0, 5);
                    if (int.TryParse(textScore, out int result))
                    {
                        //轉換成功，result 中為轉換後的整數值
                        result = result + 2;
                        textScore = result.ToString();
                    }
                }
                else if (add_tpye == 4) //產生往左傳送帶
                {
                    if (conveyor_left_count != 0)
                    {
                        if (Math.Abs(conveyor_left[conveyor_left_count - 1].x - xx) < 140)
                            xx = r.Next(770);
                    }
                    conveyor_left n = new conveyor_left(xx, yy, -4, this);
                    conveyor_left.Add(n);
                    add_tpye = r.Next(0, 5);
                    if (int.TryParse(textScore, out int result))
                    {
                        //轉換成功，result 中為轉換後的整數值
                        result = result + 2;
                        textScore = result.ToString();
                    }
                }
                generate = r.Next(50, 70);
            }
            else
            {
                generate--;
            }
        }

        async Task GameLoop()
        {

            int delay = 0;
            int stt = 0; //彈跳磚塊判斷
            int jump = 0; //彈跳磚塊時間
            bool touch_nails = false;//是否在尖刺上
            int touch_tpye = -1;
            int jump_voice = 0;
            int conveyor_type = -1;
            int drop = 1;

            while (pageIsActive)
            {
                int decide = p.Move(mx, my, conveyor_type, iii);

                if ((decide == 1 || decide == 2) && delay <= 0)//當角色碰到天花板的時候的時候
                {
                    if (decide == 2) //掉落到底部
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
                        // 使用 StreamWriter 將字串寫入檔案
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.Write(textScore);
                        }
                        pageIsActive = false;
                        media.Stop();
                        await Navigation.PushAsync(new EndPage());
                        drop = 0;
                    }
                    int blood_count = blood.Count();
                    blood_count = blood_count - 5; //扣5滴
                    if (blood_count <= 0 && drop != 0) //碰到天花板
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
                        // 使用 StreamWriter 將字串寫入檔案
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.Write(textScore);
                        }
                        pageIsActive = false;
                        media.Stop();
                        await Navigation.PushAsync(new EndPage());
                    }
                    else
                    {
                        string ex = "";
                        for (int i = 0; i < blood_count; i++)
                        {
                            ex += "|";
                        }
                        blood = ex;
                    }
                    delay = 30;

                }
                else
                {
                    delay--;
                }


                Clear();
                add_brick();//新增磚塊

                int st = 0;

                foreach (Stone b in s) //判斷接觸藍色平台
                {
                    if (p.Coll(b))
                    {
                        p.dy = -4;
                        touch_tpye = 0;
                        st = 1;
                        break;
                    }
                }
                if (st != 1)
                {
                    foreach (Nails n in nails)
                    {
                        if (p.Coll_nails(n)) //判斷接觸尖刺平台停下
                        {
                            p.dy = -4;
                            touch_tpye = 1;
                            st = 2;
                            break;
                        }
                    }
                }
                if (st != 1 && st != 2)
                {
                    foreach (Trampoline t in trampoline) //判斷接觸彈簧板
                    {
                        if (p.Coll_Trampoline(t))
                        {
                            p.dy = -4;
                            touch_tpye = 0;
                            st = 3;
                            break;
                        }
                    }
                }
                if (st != 1 && st != 2 && st != 3)
                {
                    foreach (conveyor_right c_r in conveyor_right) //判斷接觸往右傳送帶
                    {
                        if (p.Coll_conveyor_right(c_r))
                        {
                            p.dy = -4;
                            touch_tpye = 0;
                            st = 4;
                            break;
                        }
                    }
                }
                if (st != 1 && st != 2 && st != 3 && st != 4)
                {
                    foreach (conveyor_left c_l in conveyor_left) //判斷接觸往左傳送帶
                    {
                        if (p.Coll_conveyor_left(c_l))
                        {
                            p.dy = -4;
                            touch_tpye = 0;
                            st = 5;
                            break;
                        }
                    }
                }
                if (st != 1 && st != 2 && st != 3 && st != 4 && st != 5 && jump == 0)
                {
                    p.dy = 10;
                }

                if (p.dy == -4 && touch_nails == false && touch_tpye == 1)//碰到尖刺扣血 touch_nails是否在同一個磚塊 touch_tpye站著的磚塊類型
                {
                    touch_nails = true;
                    int blood_count = blood.Count();
                    blood_count = blood_count - 3;//扣三滴
                    if (blood_count <= 0)
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
                        // 使用 StreamWriter 將字串寫入檔案
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.Write(textScore);
                        }
                        pageIsActive = false;
                        media.Stop();
                        await Navigation.PushAsync(new EndPage());

                    }
                    else
                    {
                        string ex = "";
                        for (int i = 0; i < blood_count; i++)
                        {
                            ex += "|";
                        }
                        blood = ex;
                    }
                }

                if (p.dy == 10)
                {
                    touch_nails = false;
                }
                else if (p.dy == -4)
                {
                    touch_nails = true;
                }

                if (st == 3) //碰到彈跳
                {
                    if (jump_voice == 0)
                    {
                        tmusic.playSound();
                        jump_voice = 1;
                    }
                    p.dy = -20;
                    if (stt == 0)
                    {
                        jump = 10;
                        stt = 1;
                    }
                }
                if (jump > 0)
                {
                    jump_voice = 0;
                    jump--;
                    if (jump == 0)
                    {
                        p.dy = 18;
                        stt = 0;
                    }
                }
                if (st == 4) //右移
                {
                    conveyor_type = 0;
                }
                else if (st == 5)
                {
                    conveyor_type = 1;//左
                }
                else
                {
                    conveyor_type = -1;
                }
                await Task.Delay(TimeSpan.FromSeconds(1.0 / 60));
            }
        }
        async Task AnimationLoop()
        {
            while (pageIsActive)
            {
                canvas.InvalidateSurface();
                await Task.Delay(TimeSpan.FromSeconds(1.0 / 60));
            }
        }
        void OnCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            bg2.Draw(canvas);
            bg.Draw(canvas);
            c.Draw(canvas);
            foreach (Stone S in s)
            {
                S.Draw(canvas);
            }
            foreach (Nails n in nails)
            {
                n.Draw(canvas);
            }
            foreach (Trampoline t in trampoline)
            {
                t.Draw(canvas);
            }
            foreach (conveyor_right c_r in conveyor_right)
            {
                c_r.Draw(canvas);
            }
            foreach (conveyor_left c_l in conveyor_left)
            {
                c_l.Draw(canvas);
            }

            p.Draw(canvas);

            float xPosition = 50;
            float yPosition = 100;

            //分數
            SKPaint textPaint_Score = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 120,
                FakeBoldText = true, // 使文字看起來更加粗體
                IsAntialias = true, // 啟用抗鋸齒，使邊緣更平滑
                Style = SKPaintStyle.Fill, // 填充文字
                TextAlign = SKTextAlign.Left, // 文字對齊方式
                StrokeWidth = 2, // 邊框的寬度
                StrokeCap = SKStrokeCap.Round, // 邊框會在每個端點形成圓形
                StrokeJoin = SKStrokeJoin.Round // 邊框的連接處形成圓形
            };

            float xScore = 820;
            float yScore = 200;
            canvas.DrawText(textScore, xScore, yScore, textPaint_Score);

            // 在畫布上繪製文字
            SKPaint textPaint = new SKPaint
            {
                Color = SKColors.Red,

                TextSize = 80,
                FakeBoldText = true, // 使文字看起來更加粗體
                IsAntialias = true, // 啟用抗鋸齒，使邊緣更平滑
                Style = SKPaintStyle.Fill, // 填充文字
                TextAlign = SKTextAlign.Left, // 文字對齊方式
                StrokeWidth = 2, // 邊框的寬度
                StrokeCap = SKStrokeCap.Round, // 邊框會在每個端點形成圓形
                StrokeJoin = SKStrokeJoin.Round // 邊框的連接處形成圓形
            };
            SKRect textBounds = new SKRect();
            textPaint.MeasureText(blood, ref textBounds);
            // 設置背景矩形的大小
            SKRect backgroundRect = new SKRect(xPosition, yPosition, xPosition + textBounds.Width + 20, yPosition + textBounds.Height + 20);
            // 繪製背景矩形
            SKPaint backgroundPaint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(backgroundRect, backgroundPaint);
            // 繪製文字
            canvas.DrawText(blood, xPosition + 5, yPosition + textBounds.Height, textPaint);
        }

        private float prevX, prevY;
        int iii;
        private void TouchEffect_TouchAction(object sender, TouchTracking.TouchActionEventArgs args)
        {
            iii = 0;
            mx = args.Location.X * 2.75f;
            my = args.Location.Y * 2.75f;
            // 判斷觸摸位置是否改變
            if (mx != prevX || my != prevY)
            {
                iii = 1;
            }

            // 更新前一個觸摸位置
            prevX = mx;
            prevY = my;
        }

        private void PlayJumpSound()
        {
            jumpSound.Play();
        }
    }
}