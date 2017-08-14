using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data.Inputs
{
    public class Input
    {
        public int x = 0;
        public int y = 0;
        public MouseButtons type = 0;
        public int time = 0;
        public int keyNum = 0;

        public void UpdateMousePoint(Point poi)
        {
            x = poi.X;
            y = poi.Y;
        }
        public void UpdateMouseBottons(MouseButtons i)
        {
            if (time == 0)
            {
                type = MouseButtons.None;
            }
            if (i == type)
            {
                time++;
            }
            else if (i == MouseButtons.None)
            {
                time = 0;
            }
            else
            {
                type = i;
                time = 1;
            }
        }

        public bool IsLeftDown()
        {
            if (type == MouseButtons.Left && time == 1) return true;
            return false;
        }
        public bool IsRightDown()
        {
            if (type == MouseButtons.Right && time == 1) return true;
            return false;
        }
        public bool IsLeftUp()
        {
            if (type == MouseButtons.Left && time == 0) return true;
            return false;
        }
        public bool IsRightUp()
        {
            if (type == MouseButtons.Right && time == 0) return true;
            return false;
        }
        public bool IsLeft()
        {
            if (type == MouseButtons.Left && time > 0) return true;
            return false;
        }
        public bool IsRight()
        {
            if (type == MouseButtons.Right && time > 0) return true;
            return false;
        }

        public byte[] key = null; //按键设置
        public int[] allKeys = null; // 实际键盘输入
        public void UpdateKeyDown(byte t)
        {
            if (allKeys[t] == 0)
                allKeys[t] = 1;
        }
        public void UpdateKeyUP(byte t)
        {
            allKeys[t] = 0;
        }
        public void UpdateKey()
        {
            int len = key.Length;
            for (int i = 0; i < len; i++)
            {
                if (allKeys[key[i]] > 0)
                    allKeys[key[i]]++;
            }
            keyNum = 0;
        }

        public int GetFlyLeft()
        {
            return allKeys[key[2]];
        }
        public int GetFlyRight()
        {
            return allKeys[key[3]];
        }
        public int GetBiSha()
        {
            return allKeys[key[0]];
        }
        public int GetFlyDown()
        {
            return allKeys[key[1]];
        }
        public int GetFlyUp()
        {
            return allKeys[key[4]];
        }
        public int GetNum1()
        {
            return allKeys[key[5]];
        }
        public int GetNum2()
        {
            return allKeys[key[6]];
        }
        public int GetNum3()
        {
            return allKeys[key[7]];
        }
        public int GetNum4()
        {
            return allKeys[key[8]];
        }

        public bool[] GetKeyBool()
        {
            return new bool[9]
            {
                GetBiSha() > 0,
                GetFlyDown() > 0,
                GetFlyLeft() > 0,
                GetFlyRight() > 0,
                GetFlyUp() > 0,
                GetNum1() > 0,
                GetNum2() > 0,
                GetNum3() > 0,
                GetNum4() > 0,
            };
        }
        public bool[] GetKeyBoolNull()
        {
            return new bool[9]
            {
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            };
        }
        public int[] GetKeyInt()
        {
            return new int[9]
            {
                (GetBiSha() > 0) ? 1 : 0,
                (GetFlyDown() > 0) ? 1 : 0,
                (GetFlyLeft() > 0) ? 1 : 0,
                (GetFlyRight() > 0) ? 1 : 0,
                (GetFlyUp() > 0) ? 1 : 0,
                (GetNum1() > 0) ? 1 : 0,
                (GetNum2() > 0) ? 1 : 0,
                (GetNum3() > 0) ? 1 : 0,
                (GetNum4() > 0) ? 1 : 0,
            };
        }

        public Input()
        {
            // 默认按键设置
            key = new byte[]
                {
                    (byte)Keys.Up,
                    (byte)Keys.Down,
                    (byte)Keys.Left,
                    (byte)Keys.Right,
                    (byte)Keys.Space,
                    (byte)Keys.D1,
                    (byte)Keys.D2,
                    (byte)Keys.D3,
                    (byte)Keys.D4,
                };

            // 键盘输入
            int len = 256;
            allKeys = new int[len];
            for (int i = 0; i < len; i++)
            {
                allKeys[i] = 0;
            }
        }
    }
}
