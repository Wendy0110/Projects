using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace game
{
        interface IEmeny    //建立一個敵人的interface, 底下為需要的函式
        {
            bool Move();
            void Draw(SKCanvas canvas);
            void Coll(Stone s);
        }

}
