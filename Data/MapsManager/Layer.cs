using Data.Globals;
using Data.PPDevices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MapsManager
{
    public class Layer
    {
        public string names = "ERROR";
        public int ID = 0;

        //相对镜头图层偏移坐标
        public float dst_x = 0;
        public float dst_y = 0;

        public float mx = 0; //相对镜头移动比例
        public float my = 0;

        public float width = 800;
        public float height = 600;
        public List<SpriteMap> sprites = new List<SpriteMap>();

        public void SetSize(float x, float y)
        {
            width = x;
            height = y;
        }
        public RectangleF GetLayerAABB()
        {
            var camera = Global.GetCamara();
            return new RectangleF(camera.left - dst_x, camera.top - dst_y, width, height);
        }
        public void Update_dst()
        {
            var camera = Global.GetCamara();
            dst_x = camera.left * mx;
            dst_y = camera.top * my;
        }
        public void Draw(PPDevice ppDevice)
        {
            foreach (SpriteMap sp in sprites)
            {
                if (Physics.Rect_Rect(GetLayerAABB(), sp.GetPicAABB()))
                {
                    sp.Draw(ppDevice);
                }
            }
        }
        public void Action()
        {
            foreach (SpriteMap sp in sprites)
            {
                sp.Action();
            }
        }
    }
}
