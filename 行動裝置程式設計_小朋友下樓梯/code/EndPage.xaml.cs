using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Xamarin.Essentials;
using Plugin.SimpleAudioPlayer;
using System.Diagnostics;

namespace game
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EndPage : ContentPage
    {

        public EndPage()
        {
            InitializeComponent();

            string filename = "分數.txt";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);

            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string content = reader.ReadToEnd();
                        myLabel.Text = content;
                    }
                }
            }
            catch (Exception ex)
            {
                // 處理可能的例外情況
                Console.WriteLine("讀取檔案時發生錯誤：" + ex.Message);
            }
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            // 跳到 MainPage
            Navigation.PushAsync(new MainPage());
            media.Stop();
        }
    }
}