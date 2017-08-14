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
            count++;
            x += vx;
            y += vy;
        }
    }
    public class SpritePlayerFly : SpriteMap_Other
    {
        //玩家死亡冒烟黑人
        public SpritePlayerFly(SpritePlayer player)
        {

        }
    }
}
