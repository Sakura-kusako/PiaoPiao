using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Data.Globals
{
    public static class GlobalB
    {
        //当前工作路径
        private static string RootPath = "";

        public static string GetRootPath()
        {
            if (RootPath != "")
            {
                //不为空直接返回
                return RootPath;
            }

            //获取当前工作路径
            string path = System.Environment.CurrentDirectory;

            //判断balloon资源文件夹是否存在
            if (Directory.Exists(path + @"\balloon"))
            {
                return path;
            }

            //再次查找
            string[] temp = path.Split('\\');
            if (temp.Length < 4)
            {
                return "";
            }

            string a = "";
            for (int i = 0; i < temp.Length - 3; i++)
            {
                if (i != 0)
                    a += "\\";
                a += temp[i];
            }
            RootPath = a;
            return a;
        }
        public static string IntToString(int x,int zeroNum = 0)
        {
            //int转string，不够补0，补0位数zeroNum
            string ret = x + "";

            int n = 10;
            for (int i = 0; i < zeroNum; i++)
            {
                if(x<n)
                {
                    ret = "0" + ret;
                }
                n *= 10;
            }

            return ret;
        }
        public static int GetIntLen(int i)
        {
            int num = 1;
            int t = Math.Abs(i);
            while (t >= 10)
            {
                t /= 10;
                num++;
            }
            return num;
        }
        public static int Get2Min(int x)
        {
            //返回最小的2的整数幂
            int n = 1;
            while (n < x)
            {
                n *= 2;
            }
            return n;
        }

        public static Texture Load_Bitmap_FromFile(string path, string file)
        {
            return Load_Bitmap_FromFile(path + file);
        }
        public static Texture Load_Bitmap_FromFile(string file)
        {
            file = file.Replace("tga", "png");
            file = file.Replace("Tga", "png");
            if (file == null)
            {
                return null;
            }
            if (File.Exists(file))
            {
                return Global.GetPPDevice().LoadBitmapFromFile(file);
                //return Texture.FromFile(Form1.data.ppDevice.device, file);
            }
            //MessageBox("加载图片失败：" + file);
            return null;
        }

    }

    public static class Physics
    {
        public static bool Rect_Rect(RectangleF r1, RectangleF r2)
        {
            if (r1 == null || r2 == null) return false;
            if (r1.Left > r2.Right) return false;
            if (r1.Right < r2.Left) return false;
            if (r1.Top > r2.Bottom) return false;
            if (r1.Bottom < r2.Top) return false;
            return true;
        }
        public static bool XY_Rect(float x, float y, RectangleF r)
        {
            if (x < r.Left) return false;
            if (x > r.Right) return false;
            if (y < r.Top) return false;
            if (y > r.Bottom) return false;
            return true;
        }
        public static bool XY_Rect(int x, int y, RectangleF r)
        {
            if (x < r.Left) return false;
            if (x > r.Right) return false;
            if (y < r.Top) return false;
            if (y > r.Bottom) return false;
            return true;
        }

        //点系列
        public static Line Point2_To_Line(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return new Line(dy, -dx, dx * y1 - dy * x1);
        }
        public static bool Is_X_In_Xs(float x1, float x2, float x3)
        {
            //值是否在区间内
            if (x1 < x2 && x1 < x3) return false;
            if (x1 > x2 && x1 > x3) return false;
            return true;
        }
        public static void Swap_Poi(ref float x, ref float y)
        {
            //交换XY值
            float t = x;
            x = y;
            y = t;
        }

        //线系列
        public static bool Line_YLine(float x1, float y1, float x2, float y2, float x3, float y3, float y4, ref float y0)
        {
            if (Is_X_In_Xs(x3, x2, x1))
            {
                var line = Point2_To_Line(x1, y1, x2, y2);
                if (line.b == 0)
                    return false;
                else
                {
                    y0 = (line.c + line.a * x3) / -line.b;
                    if (Is_X_In_Xs(y0, y3, y4)) return true;
                    else return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool Line_XLine(float x1, float y1, float x2, float y2, float x3, float y3, float x4, ref float x0)
        {
            return Line_YLine(y1, x1, y2, x2, y3, x3, x4, ref x0);
        }

        //矩形系列
        private static JudgeData Rect_Rect_Vy(RectangleF rect1, RectangleF rect2, float vx, float vy)
        {
            JudgeData judgeData = new JudgeData();
            float poi1 = 0;
            float poi2 = 0;
            bool j1 = false;
            bool j2 = false;
            if (rect2.Bottom >= rect1.Top && vy > 0)
            {
                if (j1 = Line_XLine(rect2.Left, rect2.Bottom, rect2.Left - vx, rect2.Bottom - vy,
                                rect1.Left, rect1.Top, rect1.Right, ref poi1))
                {

                }
                else if (j1 = Line_XLine(rect1.Left, rect1.Top, rect1.Left + vx, rect1.Top + vy,
                                rect2.Left, rect2.Bottom, rect2.Right, ref poi1))
                {
                    poi1 = rect1.Left;
                }
                if (j2 = Line_XLine(rect2.Right, rect2.Bottom, rect2.Right - vx, rect2.Bottom - vy,
                                rect1.Left, rect1.Top, rect1.Right, ref poi2))
                {

                }
                else if (j2 = Line_XLine(rect1.Right, rect1.Top, rect1.Right + vx, rect1.Top + vy,
                rect2.Left, rect2.Bottom, rect2.Right, ref poi2))
                {
                    poi2 = rect1.Right;
                }
                if (j1 && j2)
                {
                    //if (j1 && !j2) poi2 = rect1.Right;
                    //if (j2 && !j1) poi1 = rect1.Left;
                    judgeData.x = (poi1 + poi2) / 2;
                    judgeData.y = rect1.Top;
                    judgeData.type = 1;
                    judgeData.depth = rect2.Bottom - rect1.Top;
                    judgeData.time = 1.0f - judgeData.depth / vy;
                    return judgeData;
                }
            }
            if (rect2.Top <= rect1.Bottom && vy < 0)
            {
                if (j1 = Line_XLine(rect2.Left, rect2.Top, rect2.Left - vx, rect2.Top - vy,
                                rect1.Left, rect1.Bottom, rect1.Right, ref poi1))
                {

                }
                else if (j1 = Line_XLine(rect1.Left, rect1.Bottom, rect1.Left + vx, rect1.Bottom + vy,
                                rect2.Left, rect2.Top, rect2.Right, ref poi1))
                {
                    poi1 = rect1.Left;
                }
                if (j2 = Line_XLine(rect2.Right, rect2.Top, rect2.Right - vx, rect2.Top - vy,
                                rect1.Left, rect1.Bottom, rect1.Right, ref poi2))
                {

                }
                else if (j2 = Line_XLine(rect1.Right, rect1.Bottom, rect1.Right + vx, rect1.Bottom + vy,
                                rect2.Left, rect2.Top, rect2.Right, ref poi2))
                {
                    poi2 = rect1.Right;
                }
                if (j1 && j2)
                {
                    //if (j1 && !j2) poi2 = rect1.Right;
                    //if (j2 && !j1) poi1 = rect1.Left;
                    judgeData.x = (poi1 + poi2) / 2;
                    judgeData.y = rect1.Top;
                    judgeData.type = 3;
                    judgeData.depth = rect1.Bottom - rect2.Top;
                    judgeData.time = 1.0f - judgeData.depth / -vy;
                    return judgeData;
                }
            }
            return null;
        }
        private static JudgeData Rect_Rect_Vx(RectangleF rect1, RectangleF rect2, float vx, float vy)
        {
            var judgedata = Rect_Rect_Vy(new RectangleF(rect1.Y, rect1.X, rect1.Height, rect1.Width),
                                         new RectangleF(rect2.Y, rect2.X, rect2.Height, rect2.Width), vy, vx);
            if (judgedata == null) return null;
            judgedata.type++;
            Swap_Poi(ref judgedata.x, ref judgedata.y);
            return judgedata;
        }
        public static JudgeData Rect_Rect_V(RectangleF rect1, RectangleF rect2, float vx, float vy)
        {
            var j1 = Rect_Rect_Vx(rect1, rect2, vx, vy);
            var j2 = Rect_Rect_Vy(rect1, rect2, vx, vy);
            if (j1 == null && j2 == null) return null;
            if (j1 == null && j2 != null) return j2;
            if (j1 != null && j2 == null) return j1;
            if (j1.time <= j2.time) return j1;
            return j2;
        }
        public static JudgeData Rect_Rect_V_V(RectangleF rect1, RectangleF rect2, float vx2, float vy2, float vx1, float vy1)
        {
            var j = Rect_Rect_V(rect1, rect2, vx2 - vx1, vy2 - vy1);
            if (j == null) return null;
            j.x -= (1 - j.time) * vx1;
            j.y -= (1 - j.time) * vy1;
            return j;
        }

    }
    public class Line
    {
        public float a;
        public float b;
        public float c;
        public Line(float ra, float rb, float rc)
        {
            a = ra;
            b = rb;
            c = rc;
        }
    }
    public class JudgeData
    {
        public int type;
        public float x;
        public float y;
        public float time;
        public float depth;
        public JudgeData()
        {

        }
        public JudgeData(int rtype, float rx, float ry, float rtime, float rdepth)
        {
            type = rtype;
            x = rx;
            y = ry;
            time = rtime;
            depth = rdepth;
        }
    }

}
