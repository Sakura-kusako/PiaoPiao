using Data.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MapsManager
{
    public class SpriteMap_Other : SpriteMap
    {
        public override void Action()
        {
            base.Action();
            x += vx;
            y += vy;
        }
    }
    public class SpriteMaoyan : SpriteMap_Other
    {
        //玩家死亡冒烟
        public SpriteMaoyan(float x, float y)
        {
            count = 0;
            line = 1;
            this.x = x;
            this.y = y;
            vx = 10;
            vy = -17f;
            layer = 3;
            pic = new Resources.BalloonPic2(SpriteBase.PlayerMaoYan_240x240x2x2);
            base.OnCreate();
        }
        public override void Action()
        {
            base.Action();
            if (count > 500)
            {
                this.Del();
            }
        }
    }
}
