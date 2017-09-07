using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Cameras
{
    public class Camera
    {
        public float x = 400; //摄像机中心
        public float y = 300;
        public float cx = 0; //缩放中心(相对相机中心)
        public float cy = 0;
        public float width = 800; //相机大小 
        public float height = 600;
        public float target_x = 400;
        public float target_y = 300;

        //下面变量只读
        public float left = 0;
        public float right = 800;
        public float top = 0;
        public float bottom = 600;

        private void Update_lrtb()
        {
            /*
            //计算缩放中心
            float csx = x + cx;
            float csy = y + cy;

            //缩放中心到四边的距离
            float dleft = width / 2 + cx;
            float dright = width - dleft;
            float dtop = height / 2 + cy;
            float dbottom = height - dright;

            //重设四边 lrtb 的值
            left = csx - dleft;
            right = csx + dright;
            top = csy - dtop;
            bottom = csy + dbottom;
            */
            x = (int)x;
            y = (int)y;
            left = x - width / 2;
            right = x + width;
            top = y - height / 2;
            bottom = y + height;
        }
        public RectangleF GetArea()
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }
        public Rectangle GetSize()
        {
            return new Rectangle(0, 0, (int)width, (int)height);
        }
        public void Update()
        {
            if(Globals.Global.IsCameraFree)
            {
                Debug();
            }
            else
            {
                Update_By_Target();
            }
            Update_lrtb();
        }
        public void Update_By_Target()
        {
            float dx = target_x - x;
            float dy = target_y - y;
            float face_x = (dx > 0) ? 1 : -1;
            float face_y = (dy > 0) ? 1 : -1;
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);
            if (dx < 10)
            {
                x = target_x;
            }
            else if (dx < 100)
            {
                x += 10 * face_x;
            }
            else if (dx < 190)
            {
                x = target_x - 90 * face_x;
            }
            else
            {
                x += 100 * face_x;
            }

            if (dy < 10)
            {
                y = target_y;
            }
            else if (dy < 100)
            {
                y += 10 * face_y;
            }
            else if (dx < 190)
            {
                y = target_y - 90 * face_y;
            }
            else
            {
                y += 100 * face_y;
            }
        }
        public void Debug()
        {
            var input = Globals.Global.GetInput();
            if (input.GetFlyDown() == 1 || input.GetFlyDown() > 20)
            {
                y++;
                if (input.GetFlyUp() >= 1)
                {
                    y += 9;
                }
            }
            if (input.GetBiSha() == 1 || input.GetBiSha() > 20)
            {
                y--;
                if (input.GetFlyUp() >= 1)
                {
                    y -= 9;
                }
            }
            if (input.GetFlyLeft() == 1 || input.GetFlyLeft() > 20)
            {
                x--;
                if (input.GetFlyUp() >= 1)
                {
                    x -= 9;
                }
            }
            if (input.GetFlyRight() == 1 || input.GetFlyRight() > 20)
            {
                x++;
                if (input.GetFlyUp() >= 1)
                {
                    x += 9;
                }
            }
        }
    }
}
