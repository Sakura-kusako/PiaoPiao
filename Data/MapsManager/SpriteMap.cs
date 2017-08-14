using Data.Globals;
using Data.PPDevices;
using Data.Resources;
using Room;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Texture = SharpDX.Direct3D9.Texture;

namespace Data.MapsManager
{
    public class SpriteMap
    {
        public static int SpriteID = 0;

        // 实际放在图层的sprite
        public BalloonPic2 pic = null;
        public float x = 0;
        public float y = 0;
        public float width = 0;
        public float height = 0;
        public float vx = 0;
        public float vy = 0;
        public float pic_dx = 0;
        public float pic_dy = 0;
        public float alpha = 1.0f;
        public int type = 0;
        public int typeID = 0;
        public int ID = 0;
        public int soutai = 1;
        public int count = 0;
        public int property = 0; //碰撞组
        public int line = 0; //图片行
        public int frame = 0; //图片帧
        public int layer = 0; //所在图层
        public int Poi_A = 0;

        public List<RectangleF> phyList = null;

        public virtual void BaseCreate()
        {
            ID = SpriteID;
            SpriteID++;
            if (pic != null)
            {
                if (pic.frameNum == 0)
                {
                    pic.frameNum = 1;
                }
                if (pic.frameSpan == 0)
                {
                    pic.frameSpan = 80;
                }
            }
        }
        public virtual void OnCreate()
        {
            BaseCreate();
            pic_dx = 0;
            pic_dy = 0;
            if (pic != null)
            {
                width = pic.width;
                height = pic.height;
            }
        }
        public virtual void OnCreateCenter()
        {
            BaseCreate();
            width = pic.width;
            height = pic.height;
            pic_dx = width / -2.0f;
            pic_dy = height / -2.0f;
        }
        public virtual void CheckError()
        {

        }

        public virtual Layer GetLayer()
        {
            return Global.GetMapManager().layers[layer];
        }
        public virtual void Draw(PPDevice ppDevice)
        {
            ppDevice.BitBlt(pic, GetShowArea(), GetPicArea(), alpha);
            Draw_RectList();
        }
        public virtual RectangleF GetPicAABB()
        { //在图层上的坐标
            var layer = GetLayer();
            return new RectangleF(x + pic_dx, y + pic_dy, width, height);
        }
        public virtual RectangleF GetPicArea()
        {
            float Width = pic.width;
            float Height = pic.height;
            //           return new RectangleF(0, 0, 1, 1);
            return new RectangleF(frame * Width, line * Height, Width, Height);
        }
        public virtual RectangleF GetShowArea()
        { //显示在屏幕上的实际位置
            var rect = GetLayer().GetLayerAABB();
            return new RectangleF(x + pic_dx - rect.Left, y + pic_dy - rect.Top, width, height);
        }
        public virtual void Action()
        {
            count++;
            if (pic.frameNum > 1)
            {
                if (pic.frameSpan == 0)
                {
                    pic.frameSpan = 80;
                }
                frame = (count * 16 / pic.frameSpan) % pic.frameNum;
            }
        }
        public virtual Texture GetBitmap()
        {
            return pic.bitmap;
        }
        public virtual List<RectangleF> GetPhyList()
        {
            if (phyList == null) return null;
            if (phyList.Count == 0) return null;
            List<RectangleF> rects = new List<RectangleF>();
            rects.Add(new RectangleF(x + pic_dx + phyList[0].X, y + pic_dy + phyList[0].Y, phyList[0].Width, phyList[0].Height));
            return rects;
        }
        public virtual void Draw_RectList()
        {
            var rects = GetLayer().GetLayerAABB();
            var list = GetPhyList();
            if (list == null) return;
            foreach (var rect in list)
            {
                switch (property)
                {
                    case 0:
                    case 20:
                    case 2180:
                        Global.BitBlt_Rect_Green(new RectangleF(rect.X - rects.X, rect.Y - rects.Y, rect.Width, rect.Height));
                        break;
                    case 258:
                    case 2306:
                    case 1282:
                        Global.BitBlt_Rect_Red(new RectangleF(rect.X - rects.X, rect.Y - rects.Y, rect.Width, rect.Height));
                        break;
                    case 4096:
                        Global.BitBlt_Rect_Pink(new RectangleF(rect.X - rects.X, rect.Y - rects.Y, rect.Width, rect.Height));
                        break;
                    default:
                        break;
                }
            }
        }
        public virtual void Do_Judgement()
        {

        }

        public virtual void Del()
        {
            soutai = -1;
        }
        public bool IsDel()
        {
            return (soutai == -1);
        }
    }
    public class SpriteStatic : SpriteMap
    {
        public override List<RectangleF> GetPhyList()
        {
            if (phyList == null) return null;
            List<RectangleF> rects = new List<RectangleF>();
            foreach (var rect in phyList)
            {
                rects.Add(new RectangleF(x + pic_dx + rect.X, y + pic_dy + rect.Y, rect.Width, rect.Height));
            }
            return rects;
        }
    }
    public class SpriteFloat : SpriteMap
    {
        public override void Action()
        {
            base.Action();
            x += vx;
        }
        public override RectangleF GetPicArea()
        {
            float Width, Height;
            Width = pic.width;
            Height = pic.height;
            if (pic.ID == 49)
            {
                //float里面的id = 49，那张水.tga居然是竖着的......
                return new RectangleF(0, frame * Height, Width, Height);
            }
            else
            {
                return new RectangleF(frame * Width, line * Height, Width, Height);
            }
        }
    }
    public class SpriteFly : SpriteMap
    {
        public int flyType = 1;
        public int time = 1;
        public int pastTime = 0;
        public int interVal1 = 0;
        public int interVal2 = 0;
        public float left = 0;
        public float top = 0;
        public float right = 0;
        public float bottom = 0;

        public float centerX = 0;
        public float centerY = 0;
        public float radius = 0; //旋转半径
        public float radian = 0; //初相位
        public float vrad = 0;

        public override void CheckError()
        {
            if (bottom == 0 && interVal1 == 0 && interVal2 == 0 && time == 0)
            {
                interVal1 = 999;
                right = left;
                bottom = top;
            }
            if (time == 0)
            {
                time = 60;
            }
            if (pic.frameSpan == 0) pic.frameSpan = 80;

        }
        public override void Action()
        {
            if (flyType == 1)
            {
                MoveType1();
            }
            else if (flyType == 2)
            {
                MoveType2();
            }
            base.Action();
        }
        public override void Draw(PPDevice ppDevice)
        {
            if (Poi_A == 0) //防止重复显示
                ppDevice.BitBlt(pic, GetShowArea(), GetPicArea(), alpha);
            Draw_RectList();
        }
        public override List<RectangleF> GetPhyList()
        {
            if (phyList == null) return null;
            List<RectangleF> rects = new List<RectangleF>();
            foreach (var rect in phyList)
            {
                rects.Add(new RectangleF(x + pic_dx + rect.X, y + pic_dy + rect.Y, rect.Width, rect.Height));
            }
            return rects;
        }
        public override void Draw_RectList()
        {
            var rects = GetLayer().GetLayerAABB();
            var list = GetPhyList();
            if (list == null) return;
            foreach (var rect in list)
            {
                switch (property)
                {
                    case 0:
                    case 20:
                    case 2180:
                        Global.BitBlt_Rect_Green(new RectangleF(rect.X - rects.X, rect.Y - rects.Y, rect.Width, rect.Height));
                        break;
                    case 258:
                    case 2306:
                    case 1282:
                        Global.BitBlt_Rect_Red(new RectangleF(rect.X - rects.X, rect.Y - rects.Y, rect.Width, rect.Height));
                        break;
                    case 4096:
                        Global.BitBlt_Rect_Pink(new RectangleF(rect.X - rects.X, rect.Y - rects.Y, rect.Width, rect.Height));
                        break;
                    default:
                        break;
                }
            }
        }
        private int GetLoopTime()
        {
            return 2 * time + interVal1 + interVal2;
        }
        private void MoveType1()
        {
            int t = (count + pastTime) % (GetLoopTime());
            float p = 0;
            if (t < interVal1)
            {
                p = 0;
            }
            else if (t < (time + interVal1))
            {
                p = 1.0f * (t - interVal1) / time;
            }
            else if (t < time + interVal1 + interVal2)
            {
                p = 1.0f;
            }
            else
            {
                p = 1 - (t - (time + interVal2 + interVal1)) / (1.0f * time);
            }
            float tx = left * (1 - p) + right * p;
            float ty = top * (1 - p) + bottom * p;
            vx = tx - x;
            vy = ty - y;
            x = tx;
            y = ty;
        }
        private void MoveType2()
        {
            radian += vrad;
            if (radian >= 360)
            {
                radian -= 360;
            }
            float tx = centerX + radius * (float)(Math.Cos(radian / 180 * 3.141593));
            float ty = centerY + radius * (float)(Math.Sin(radian / 180 * 3.141593));
            vx = tx - x;
            vy = ty - y;
            x = tx;
            y = ty;
        }
    }
    public class SpriteBarrier : SpriteMap
    {
        public int flyType = 1;
        public int time = 1;
        public int pastTime = 0;
        public int bstTime = 0;
        public int sprTime = 0;
        public int offsetTime = 0;
        public float left = 0;
        public float top = 0;
        public float right = 0;
        public float bottom = 0;
        public float dirLeft = 0;
        public float dirTop = 0;
        public float dirRight = 0;
        public float dirBottom = 0;
        public List<BalloonItemPic_Base> picList = new List<BalloonItemPic_Base>();

        public override void OnCreate()
        {
            BaseCreate();
            pic_dx = 0;
            pic_dy = 0;
            property = 258;
        }
        public override void Action()
        {
            MoveType1();
            Poi_A = ((count - offsetTime) % (bstTime + sprTime) < bstTime) ? 1 : 2;

            count++;
        }
        public override void CheckError()
        {
            width = 60;
            height = 60;
            dirTop -= 14;
            dirBottom -= 14;
            picList[5].line = 2;
            picList[7].line = 1;
        }
        private void MoveType1()
        {
            int t = (count + pastTime) % (GetLoopTime());
            float p = 0;

            if (t < (time))
            {
                p = 1.0f * t / time;
            }
            else
            {
                p = 1 - (t - time) / (1.0f * time);
            }
            float tx = left * (1 - p) + right * p;
            float ty = top * (1 - p) + bottom * p;
            vx = tx - x;
            vy = ty - y;
            x = tx;
            y = ty;
        }
        private int GetLoopTime()
        {
            return 2 * time;
        }
        public override void Draw(PPDevice ppDevice)
        {
            var rect = GetLayer().GetLayerAABB();
            for (int i = 0; i < 1; i++)
            {
                int line = 0;
                int frame = 0;
                var pic = picList[i];
                if (pic.frameNum <= 0) pic.frameNum = 1;
                if (pic.line <= 0) pic.line = 1;

                float Width = pic.width;
                float Height = pic.height;
                if (pic.frameNum > 1)
                {
                    if (pic.frameSpan == 0)
                    {
                        pic.frameSpan = 80;
                    }
                    frame = (count * 16 / pic.frameSpan) % pic.frameNum;
                }

                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + pic.x - rect.Left, y + pic_dy + pic.y - rect.Top, pic.width, pic.height),
                                new RectangleF(frame * Width, line * Height, Width, Height),
                                alpha);
            }

            if (dirTop >= 60)
            {
                BalloonItemPic_Base pic0;
                BalloonItemPic_Base pic;
                float dx, dy;
                if (Poi_A == 1)
                {
                    pic0 = picList[1];
                    pic = picList[7];
                    dx = 7.5f;
                    dy = 14;
                    frame = 5;
                }
                else
                {
                    pic0 = picList[1];
                    pic = picList[5];
                    dx = 7.5f;
                    dy = 14;
                    frame = 0;
                }

                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + dx - rect.Left, y + pic_dy + dy - dirTop - rect.Top, pic.width, pic.height),
                                new RectangleF(0 * pic.width, 0, pic.width, pic.height),
                                alpha);
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + dx - rect.Left, y + pic_dy + dy - dirTop + 30 - rect.Top, pic.width, dirTop - 60),
                                new RectangleF(1 * pic.width, 0, pic.width, pic.height / 2),
                                alpha);
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + dx - rect.Left, y + pic_dy + dy - 30 - rect.Top, pic.width, pic.height),
                                new RectangleF(2 * pic.width, 0, pic.width, pic.height),
                                alpha);
                ppDevice.BitBlt(pic0,
                                new RectangleF(x + pic_dx + 0 - rect.Left, y + pic_dy + 0 - rect.Top, pic0.width, pic0.height),
                                new RectangleF(frame * pic0.width, 0, pic0.width, pic0.height),
                                alpha);
            }
            if (dirBottom >= 60)
            {
                BalloonItemPic_Base pic0;
                BalloonItemPic_Base pic;
                float dx, dy;
                if (Poi_A == 1)
                {
                    pic0 = picList[1];
                    pic = picList[7];
                    dx = 7.5f;
                    dy = 47;
                    frame = 5;
                }
                else
                {
                    pic0 = picList[1];
                    pic = picList[5];
                    dx = 7.5f;
                    dy = 47;
                    frame = 0;
                }
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + dx - rect.Left, y + pic_dy + dy - rect.Top, pic.width, pic.height),
                                new RectangleF(0 * pic.width, 0, pic.width, pic.height),
                                alpha);
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + dx - rect.Left, y + pic_dy + dy + 30 - rect.Top, pic.width, dirBottom - 60),
                                new RectangleF(1 * pic.width, 0, pic.width, pic.height / 2),
                                alpha);
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + dx - rect.Left, y + pic_dy + dy + dirBottom - 30 - rect.Top, pic.width, pic.height),
                                new RectangleF(2 * pic.width, 0, pic.width, pic.height),
                                alpha);
                ppDevice.BitBlt(pic0,
                                new RectangleF(x + pic_dx + 0 - rect.Left, y + pic_dy + 31 - rect.Top, pic0.width, pic0.height),
                                new RectangleF(frame * pic0.width, 0, pic0.width, pic0.height),
                                alpha);
            }
            //没有左右
            if (dirLeft >= 60)
            {
                var pic = picList[1];
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + pic.x - rect.Left, y + pic_dy + pic.y - rect.Top, pic.width, pic.height),
                                new RectangleF(5 * 1 / 6.0f, 2 * 1 / 4.0f, 1 / 6.0f, 1 / 4.0f),
                                alpha);
            }
            if (dirRight >= 60)
            {
                var pic = picList[1];
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + pic.x - rect.Left, y + pic_dy + pic.y - rect.Top, pic.width, pic.height),
                                new RectangleF(5 * 1 / 6.0f, 3 * 1 / 4.0f, 1 / 6.0f, 1 / 4.0f),
                                alpha);
            }

            for (int i = 2; i < 4; i++)
            {
                int line = 0;
                int frame = 0;
                var pic = picList[i];
                if (pic.frameNum <= 0) pic.frameNum = 1;
                if (pic.line <= 0) pic.line = 1;

                float Width = pic.width;
                float Height = pic.height;
                if (pic.frameNum > 1)
                {
                    if (pic.frameSpan == 0)
                    {
                        pic.frameSpan = 80;
                    }
                    frame = (count * 16 / pic.frameSpan) % pic.frameNum;
                }

                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + pic.x - rect.Left, y + pic_dy + pic.y - rect.Top, pic.width, pic.height),
                                new RectangleF(frame * Width, line * Height, Width, Height),
                                alpha);
            }

            Draw_RectList();
        }
        public override List<RectangleF> GetPhyList()
        {
            List<RectangleF> rects = new List<RectangleF>();
            if (Poi_A == 1)
            {
                if (dirTop >= 60)
                {
                    rects.Add(new RectangleF(x + pic_dx + 7.5f, y + pic_dy + 14 - dirTop, 45, dirTop));
                }
                if (dirBottom >= 60)
                {
                    rects.Add(new RectangleF(x + pic_dx + 7.5f, y + pic_dy + 47, 45, dirBottom));
                }
            }
            return rects;
        }
    }
    public class SpritePlayer : SpriteMap
    {
        public bool pyhEnable = true;
        public bool atkEnable = true;
        public PlayerData player = null;
        private RectangleF balloonList;
        private List<RectangleF> fireList = null;

        public List<SpritePlayerJudgement> judgeList = new List<SpritePlayerJudgement>();
        private Buff buff_base = new Buff(1, -1);
        private Buff buff_wudi = new Buff(10, 0); //无敌buff;

        //脸朝向
        public int face = 0;

        //生命、攻击、速度
        public int hp = 3;
        public int hp_max = 5;
        public int atk = 1;
        public int atk_max = 5;
        public int v = 4;
        public int v_max = 5;

        //重力
        private float g = 0.04f;

        public override void Action()
        {
            count++;
            player.input.UpdateKey(Global.GetInput().GetKeyBool());
            Update_Buff_Time();
            Update_Buff_Eff();
            switch (buff_base.type)
            {
                case 0:
                    //移出游戏
                    Action_Del();
                    break;
                case 1:
                    //正常模式
                    Action_Normal();
                    break;
                case 2:
                    //无气球死亡
                    Action_Dead();
                    break;
                case 3:
                    //气球打气
                    Action_DaQi();
                    break;
                case 4:
                    //上天
                    Action_ShangTian();
                    break;
                case 5:
                    //下地
                    Action_XiaDi();
                    break;
                case 6:
                    //被电
                    Action_BeiDian();
                    break;
                default:
                    break;
            }
        }
        private void Update_Buff_Time()
        {
            Update_Buff_Time_Single(buff_base);
            Update_Buff_Time_Single(buff_wudi);
        }
        private void Update_Buff_Eff()
        {
        }
        private void Update_Buff_Time_Single(Buff buff)
        {
            if (buff.time > 0) buff.time--;
        }
        private void Action_Del()
        {

        }
        private void Action_Normal()
        {
            Action_Normal_Input();
            Action_Normal_Gravity();
            Action_Normal_Air_f();
            Action_Normal_Move();
        }
        private void Action_Normal_Input()
        {
            float vx_poi = 0.06f;
            float vy_poi = 0.09f;

            if (player.input.GetFlyLeft() > 0)
            {
                face = -1;
            }
            if (player.input.GetFlyRight() > 0)
            {
                face = 1;
            }
            if (player.input.GetFlyUp() > 0)
            {
                vy -= vy_poi;
                if (player.input.GetFlyLeft() > 0)
                {
                    vx -= vx_poi;
                }
                if (player.input.GetFlyRight() > 0)
                {
                    vx += vx_poi;
                }
            }
        }
        private void Action_Normal_Move()
        {
            x += vx;
            y += vy;
        }
        private void Action_Normal_Gravity()
        {
            //重力
            vy += g;
        }
        private void Action_Normal_Air_f()
        {
            //空气阻力
            const float fx = 4.0f; //速度上限
            const float fy = 4.0f;
            const float df = 0.1f; //每帧-0.1
            float dx, dy;

            if (vx > fx)
            {
                dx = vx - fx;
                if (dx <= df)
                {
                    vx = fx;
                }
                else
                {
                    vx -= df;
                }
            }
            if (vx < -fx)
            {
                dx = -fx - vx;
                if (dx <= df)
                {
                    vx = -fx;
                }
                else
                {
                    vx += df;
                }
            }
            if (vy > fy)
            {
                dy = vy - fy;
                if (dy <= df)
                {
                    vy = fy;
                }
                else
                {
                    vy -= df;
                }
            }
            if (vy < -fy)
            {
                dy = -fy - vy;
                if (dy <= df)
                {
                    vy = -fy;
                }
                else
                {
                    vy += df;
                }
            }
        }
        private void Action_Dead()
        {
            vx = 0;
            vy += 2 * g;
            Action_Normal_Air_f();
            Action_Normal_Move();
        }
        private void Action_DaQi()
        {
            vy += 2 * g;
            Action_Normal_Air_f();
            Action_Normal_Move();
            if (buff_base.time <= 0)
            {
                Change_To_Normal();
            }
        }
        private void Action_ShangTian()
        {
            vy = 0;
            vx = 0;
            if (buff_base.time == 0)
            {
                //复活冷却结束
                if (Global.GetMapManager().type == 1)
                {
                    y = Global.GetMapManager().PhyRect.Bottom - 150;
                    Change_To_Normal();
                }
                else
                {
                    Change_To_Lost();
                }
            }
        }
        private void Action_XiaDi()
        {
            vy += 2 * g;
            Action_Normal_Air_f();
            Action_Normal_Move();
            if (this.y - 10 > Global.GetMapManager().PhyRect.Bottom)
            {
                Change_To_ShangTian();
            }
        }
        private void Action_BeiDian()
        {
            vx = 0;
            vy = 0;
            if (buff_base.time == 0)
            {
                Change_To_Dead();
            }
        }

        public override void Draw(PPDevice ppDevice)
        {
            if (Is_Lost()) return;
            if (Is_BeiDian())
            {
                //被电
                Draw_Single(ppDevice, 31, 0);
            }
            else
            {
                DrawSmall(ppDevice);
                Draw_RectList();
            }
        }
        private void DrawSmall(PPDevice ppDevice)
        {
            DrawBgTx(ppDevice);
            DrawBalloon(ppDevice);
            DrawShip(ppDevice);
            DrawBody(ppDevice);
            DrawFace(ppDevice);
            DrawZhuangShi(ppDevice);
            DrawShou(ppDevice);
        }
        private void DrawBgTx(PPDevice ppDevice)
        {
            //42.背景特效
            Draw_Single(ppDevice, 42, 0);
        }
        private void DrawBalloon(PPDevice ppDevice)
        {
            //1.气球
            //Draw_Single(ppDevice, 1, 0);
            var pic_poi = GetPlayerPic_Big(1);
            if (pic_poi == null) return;
            var rect = GetLayer().GetLayerAABB();
            var line = GetLine();
            if (this.hp == 0) return;
            if (this.hp == 1)
            {
                ppDevice.BitBlt(pic_poi,
                                new RectangleF(x + pic_dx + pic_poi.x - rect.X, y + pic_dy + pic_poi.y - rect.Y, pic_poi.width, pic_poi.height),
                                new RectangleF(0, line * pic_poi.height, pic_poi.width, pic_poi.height), alpha);
            }
            else
            {
                ppDevice.BitBlt(pic_poi,
                                new RectangleF(x + pic_dx + pic_poi.x - rect.X - 10, y + pic_dy + pic_poi.y - rect.Y, pic_poi.width, pic_poi.height),
                                new RectangleF(0, line * pic_poi.height, pic_poi.width, pic_poi.height), alpha);
                ppDevice.BitBlt(pic_poi,
                                new RectangleF(x + pic_dx + pic_poi.x - rect.X + 10, y + pic_dy + pic_poi.y - rect.Y, pic_poi.width, pic_poi.height),
                                new RectangleF(0, line * pic_poi.height, pic_poi.width, pic_poi.height), alpha);
                if (this.hp >= 3)
                {
                    ppDevice.BitBlt(pic_poi,
                                    new RectangleF(x + pic_dx + pic_poi.x - rect.X, y + pic_dy + pic_poi.y - rect.Y + 5, pic_poi.width, pic_poi.height),
                                    new RectangleF(0, line * pic_poi.height, pic_poi.width, pic_poi.height), alpha);
                }
            }
        }
        private void DrawShip(PPDevice ppDevice)
        {
            //10.座舱
            Draw_Single(ppDevice, 10);
        }
        private void DrawBody(PPDevice ppDevice)
        {
            //18.身体
            Draw_Single(ppDevice, 18);
        }
        private void DrawFace(PPDevice ppDevice)
        {
            //脸部
            //7.脸型
            //20.嘴巴
            //4.眼睛
            Draw_Single(ppDevice, 7);
            Draw_Single(ppDevice, 20);

            var pic = GetPlayerPic_Big(4);
            if (pic == null) return;
            //控制眨眼频率
            int frame = 0;
            //if (pic.frameSpan < 100) pic.frameSpan = 100;
            int len = pic.frameSpan * pic.frameNum;
            int t = (count * 16) % 5000; //5000ms一次
            if (t < len)
            {
                //5秒一次
                frame = t / pic.frameSpan;
            }
            BitBlt(ppDevice, pic, frame, GetLine());
        }
        private void DrawZhuangShi(PPDevice ppDevice)
        {
            //8.脸装饰
            //9.胡子
            //5.眼镜
            //2.头发
            //3.发饰
            Draw_Single(ppDevice, 8);
            Draw_Single(ppDevice, 9);
            Draw_Single(ppDevice, 5);
            Draw_Single(ppDevice, 2);
            Draw_Single(ppDevice, 3);
        }
        private void DrawShou(PPDevice ppDevice)
        {
            //19.手
            Draw_Single(ppDevice, 19);
        }
        private void Draw_Single(PPDevice ppDevice, int poi, int draw_type = -1)
        {
            var pic = GetPlayerPic_Big(poi);
            if (pic == null) return;
            BitBlt(ppDevice, pic, GetFrame(pic), ((draw_type < 0) ? GetLine() : draw_type));
        }
        private void BitBlt(PPDevice ppDevice, BalloonItemPic_Base pic_poi, int frame, int line)
        {
            var rect = GetLayer().GetLayerAABB();
            float w = pic_poi.width;
            float h = pic_poi.height;
            if (line == 0)
            {
                ppDevice.BitBlt(pic_poi,
                                new RectangleF(x + pic_dx + pic_poi.x - rect.X, y + pic_dy + pic_poi.y - rect.Y, pic_poi.width, pic_poi.height),
                                new RectangleF(frame * w, line * h, w, h), alpha);
            }
            else
            {
                ppDevice.BitBlt(pic_poi,
                                new RectangleF(x + pic_dx + pic_poi.x - rect.X - 7, y + pic_dy + pic_poi.y - rect.Y, pic_poi.width, pic_poi.height),
                                new RectangleF(frame * w, line * h, w, h), alpha);
            }
        }
        public override List<RectangleF> GetPhyList()
        {
            List<RectangleF> rects = new List<RectangleF>();
            rects.Add(new RectangleF(x + pic_dx + 23, y + pic_dy + 5, 46, 79));
            return rects;
        }
        public RectangleF GetPhyAABB()
        {
            return new RectangleF(x + pic_dx + 23, y + pic_dy + 5, 46, 79);
        }
        private BalloonItemPic_Base GetPlayerPic_Big(int poi)
        {
            return Global.GetResManager().GetItemPic_Big(poi, player.GetItemIndex(poi));
        }
        private int GetLine()
        {
            return (face == 1) ? 1 : 0;
        }
        private int GetFrame(BalloonItemPic_Base pic_poi)
        {
            int frame = 0;
            if (pic_poi.frameNum > 1)
            {
                if (pic_poi.frameSpan < 100) pic_poi.frameSpan = 100;
                frame = (count * 16 / pic_poi.frameSpan) % pic_poi.frameNum;
            }
            return frame;
        }

        public override void Do_Judgement()
        {
            if (pyhEnable == false)
            {
                judgeList.Clear();
                return;
            }
            while (true)
            {
                if (judgeList.Count == 0) return;
                bool judge_check = false;
                int index = Judgement_GetMinIndex();
                var data = judgeList[index];
                switch (data.sprite.type)
                {
                    case 0:
                        judge_check = Judgement_Action_Map(data);
                        break;
                    case 2:
                        judge_check = Judgement_Action_Static(data);
                        break;
                    case 3:
                    case 16:
                    case 13:
                        judge_check = Judgement_Action_Fly(data);
                        break;
                    case 5:
                        judge_check = Judgement_Action_Property(data);
                        break;
                    case 6:
                        judge_check = Judgement_Action_Vane(data);
                        break;
                    case 11:
                        judge_check = Judgement_Action_Primer(data);
                        break;
                    default:
                        break;
                }

                if (judge_check == true)
                {
                    break;
                }
                judgeList.RemoveAt(index);
            }

            judgeList.Clear();
        }
        private int Judgement_GetMinIndex()
        {
            //返回最先碰撞的下标;
            int index = 0;
            float time = 1;
            for (int i = 1; i < judgeList.Count; i++)
            {
                if (judgeList[i].judgeData.time < time)
                {
                    time = judgeList[i].judgeData.time;
                    index = i;
                }
            }
            return index;
        }
        private bool Judgement_Action_Map(SpritePlayerJudgement data)
        {
            switch (Global.GetMapManager().type)
            {
                case 1:
                    return Judgement_Action_Map_JingSu(data);
                default:
                    return false;
            }

        }
        private bool Judgement_Action_Map_JingSu(SpritePlayerJudgement data)
        {
            switch (data.judgeData.type)
            {
                case 1:
                    Change_To_XiaDi();
                    return true;
                case 2:
                    return Judgement_Action_sta_left(data);
                case 3:
                    return Judgement_Action_sta_bottom(data);
                case 4:
                    return Judgement_Action_sta_right(data);
                default:
                    return false;
            }
        }
        private bool Judgement_Action_Static(SpritePlayerJudgement data)
        {
            switch (data.judgeData.type)
            {
                case 1:
                    return Judgement_Action_sta_top(data);
                case 2:
                    return Judgement_Action_sta_left(data);
                case 3:
                    return Judgement_Action_sta_bottom(data);
                case 4:
                    return Judgement_Action_sta_right(data);
                default:
                    return false;
            }
        }
        private bool Judgement_Action_sta_top(SpritePlayerJudgement data)
        {
            if (Is_Dead())
            {
                this.vy = 0;
                this.y -= data.judgeData.depth + 0.0001f;//浮点误差
                this.Change_To_DaQi();
                return true;
            }
            else
            {
                //水平摩擦减速
                var dx = 0.01f + Math.Abs(this.vx) * 0.15f;
                if (Math.Abs(this.vx) < dx)
                {
                    this.vx = 0;
                }
                else if (this.vx < 0)
                {
                    this.vx += dx;
                }
                else
                {
                    this.vx -= dx;
                }

                //竖直半反弹
                var dBottom = data.judgeData.depth + 0.0001f;//浮点误差
                var dy = -0.2f + 1.3f * dBottom;
                if (dy < dBottom) dy = dBottom;
                this.y -= dy;
                this.vy = -0.3f * Math.Abs(this.vy);
                return true;
            }
        }
        private bool Judgement_Action_sta_bottom(SpritePlayerJudgement data)
        {
            this.y += 2 * data.judgeData.depth;
            this.vy = Math.Abs(this.vy);
            return true;
        }
        private bool Judgement_Action_sta_left(SpritePlayerJudgement data)
        {
            this.x -= 2 * data.judgeData.depth;
            this.vx = -0.5f * Math.Abs(this.vx);
            return true;
        }
        private bool Judgement_Action_sta_right(SpritePlayerJudgement data)
        {
            this.x += 2 * data.judgeData.depth;
            this.vx = 0.5f * Math.Abs(this.vx);
            return true;
        }
        private bool Judgement_Action_Fly(SpritePlayerJudgement data)
        {
            bool check = false;
            var pro = data.sprite.property;
            if ((pro & 0x800) > 0 && atkEnable)
            {
                //消失;
                data.sprite.Del();
            }
            if ((pro & 0x100) > 0)
            {
                //电属性;
                if (atkEnable == false)
                {
                    check = false;
                }
                else if (buff_wudi.time == 0)
                {
                    x -= (1 - data.judgeData.time) * vx;
                    y -= (1 - data.judgeData.time) * vy;
                    Change_To_BeiDian();
                    check = true;
                }
                else
                {
                    check = false;
                }
            }
            else
            {
                //不是电必反弹
                switch (data.judgeData.type)
                {
                    case 1:
                        check = Judgement_Action_fly_top(data);
                        break;
                    case 2:
                        check = Judgement_Action_fly_left(data);
                        break;
                    case 3:
                        check = Judgement_Action_fly_bottom(data);
                        break;
                    case 4:
                        check = Judgement_Action_fly_right(data);
                        break;
                    default:
                        check = false;
                        break;
                }
            }

            if ((pro & 0x1000) > 0 && atkEnable && (buff_wudi.time == 0))
            {
                //刺，扣一血
                Dec_hp();
                check = true;
            }
            return check;
        }
        private bool Judgement_Action_fly_top(SpritePlayerJudgement data)
        {
            if (Is_Dead())
            {
                this.vy = data.sprite.vy;
                this.y -= data.judgeData.depth + 0.0001f;//浮点误差
                this.Change_To_DaQi();
                return true;
            }
            else
            {
                //水平摩擦减速
                var dx = 0.01f + Math.Abs(this.vx - data.sprite.vx) * 0.15f;
                if (Math.Abs(this.vx - data.sprite.vx) < dx)
                {
                    this.vx = data.sprite.vx;
                }
                else if (this.vx - data.sprite.vx < 0)
                {
                    this.vx += dx;
                }
                else
                {
                    this.vx -= dx;
                }

                //竖直半反弹
                var dBottom = data.judgeData.depth + 0.0001f;//浮点误差
                var dy = (-0.2f + 1.3f * dBottom);
                if (dy < dBottom) dy = dBottom;
                this.y -= dy;
                this.vy -= 1.3f * Math.Abs(this.vy - data.sprite.vy);
                return true;
            }
        }
        private bool Judgement_Action_fly_bottom(SpritePlayerJudgement data)
        {
            this.y += 2 * data.judgeData.depth;
            this.vy += -2 * (this.vy - data.sprite.vy);
            return true;
        }
        private bool Judgement_Action_fly_left(SpritePlayerJudgement data)
        {
            this.x += -2 * data.judgeData.depth;
            this.vx += -1.5f * (this.vx - data.sprite.vx);
            return true;
        }
        private bool Judgement_Action_fly_right(SpritePlayerJudgement data)
        {
            this.x += 2 * data.judgeData.depth;
            this.vx += -1.5f * (this.vx - data.sprite.vx);
            return true;
        }
        private bool Judgement_Action_Property(SpritePlayerJudgement data)
        {
            if (atkEnable)
            {
                switch (data.sprite.typeID)
                {
                    case 1:
                        Add_Atk(1);
                        break;
                    case 2:
                        Add_Atk(2);
                        break;
                    case 3:
                        Add_Atk(3);
                        break;
                    case 4:
                        Add_V(1);
                        break;
                    case 5:
                        Add_V(2);
                        break;
                    case 6:
                        Add_V(3);
                        break;
                    case 7:
                        Add_Hp(1);
                        break;
                    case 8:
                        Add_Hp(2);
                        break;
                    case 9:
                        Add_Hp(3);
                        break;
                    case 10:
                        //玩家进入10秒轻盈状态，玩家上升速度提高而下坠速度变缓，拾取即时使用。
                        break;
                    case 12:
                        //使玩家进入10秒混乱状态，操作左右反向，拾取即时使用。
                        break;
                    case 20:
                        Add_PiaoBi(1);
                        break;
                    case 21:
                        Add_PiaoBi(2);
                        break;
                    case 22:
                        Add_PiaoBi(5);
                        break;
                    case 23:
                        Add_PiaoBi(10);
                        break;
                    default:
                        break;
                }
                data.sprite.Del();
            }
            return false;
        }
        private bool Judgement_Action_Vane(SpritePlayerJudgement data)
        {
            if (atkEnable)
            {
                vx = -9.0f;
            }
            return false;
        }
        private bool Judgement_Action_Primer(SpritePlayerJudgement data)
        {
            data.sprite.Del();
            return false;
        }

        public bool Is_Lost()
        {
            return (buff_base.type == 0);
        }
        public bool Is_Dead()
        {
            return (buff_base.type == 2);
        }
        public bool Is_BeiDian()
        {
            return (buff_base.type == 6);
        }
        public bool Is_DaQi()
        {
            return (buff_base.type == 3);
        }
        public bool Is_ShangTian()
        {
            return (buff_base.type == 4);
        }
        public int GetBuffTime()
        {
            return buff_base.time;
        }
        private void Change_To_Dead()
        {
            buff_base.type = 2;
            hp = 0;
            atkEnable = false;
            pyhEnable = true;
        }
        private void Change_To_DaQi()
        {
            buff_base.type = 3;
            buff_base.time = 600;
            hp = 0;
            atkEnable = false;
            pyhEnable = true;
        }
        private void Change_To_Normal()
        {
            Buff_Clear();
            buff_base.type = 1;
            buff_base.time = -1;
            hp = 2;
            buff_wudi.time = 180;
            pyhEnable = true;
            atkEnable = true;
        }
        private void Change_To_ShangTian()
        {
            Global.PlayEffect(24);
            buff_base.type = 4;
            buff_base.time = 600;
            atkEnable = false;
            pyhEnable = false;
        }
        private void Change_To_XiaDi()
        {
            buff_base.type = 5;
            buff_base.time = -1;
            pyhEnable = false;
            atkEnable = false;
        }
        private void Change_To_Lost()
        {
            buff_base.type = 0;
            buff_base.time = -1;
        }
        private void Change_To_BeiDian()
        {
            buff_base.type = 6;
            buff_base.time = 60;
            atkEnable = false;
            pyhEnable = false;
            Global.PlayEffect(25);
        }
        private void Dec_hp()
        {
            Global.PlayEffect(43);
            hp--;
            if (hp == 0)
            {
                Change_To_Dead();
            }
        }
        private void Buff_Clear()
        {
            buff_wudi.time = 0;
        }

        private void Add_PiaoBi(int poi)
        {
            this.player.piaobi += poi;
        }
        private void Add_V(int poi)
        {
            v += poi;
            if (v > v_max) v = v_max;
        }
        private void Add_Atk(int poi)
        {
            atk += poi;
            if (atk > atk_max) atk = atk_max;
        }
        private void Add_Hp(int poi)
        {
            hp += poi;
            if (hp > hp_max) hp = hp_max;
        }
    }
    public class SpritePlayerJudgement
    {
        public JudgeData judgeData = null;
        public SpriteMap sprite = null;
        public SpritePlayerJudgement(JudgeData j, SpriteMap s)
        {
            judgeData = j;
            sprite = s;
        }
    }
    public class Buff
    {
        public int type = 1;
        public int time = -1;
        public Buff(int type, int time)
        {
            this.type = type;
            this.time = time;
        }
    }
    public class SpriteProperty : SpriteMap
    {
        public float flyAreaX = 0;
        public override void Action()
        {
            base.Action();
            vx = (float)(flyAreaX * Math.Cos((count / 300.0f) * 2 * 3.14159f));
            y += vy;
            x += vx;
        }
        public override List<RectangleF> GetPhyList()
        {
            List<RectangleF> list = new List<RectangleF>();
            list.Add(new RectangleF(x + pic_dx, y + pic_dy, width, height));
            return list;
        }
    }
    public class SpriteVane : SpriteMap
    {
        public override void CheckError()
        {
            phyList = new List<RectangleF>();
            phyList.Add(new RectangleF(3, 3, width - 6, height - 6));
        }
    }
    public class SpriteGate : SpriteMap
    {
        public List<BalloonItemPic_Base> picList = new List<BalloonItemPic_Base>();

        public override void Action()
        {
            count++;
        }
        public override void CheckError()
        {
            width = 183;
            height = 355;
            pic = null;
            if (property == 0) Poi_A = 0;
            else Poi_A = 1;
            soutai = 1;
        }
        public override void Draw(PPDevice ppDevice)
        {
            var rect = GetLayer().GetLayerAABB();
            int[] picIndex;
            int[] dx = { 25, 68, 107 };//横向修正
            if (Poi_A == 0)
            {
                if (IsOpen())
                {
                    //开门
                    picIndex = new int[] { 6, 7, 2 };
                }
                else
                {
                    //关门
                    picIndex = new int[] { 0, 1, 2 };
                }
            }
            else
            {
                //电
                picIndex = new int[] { 3, 4, 5 };
            }

            for (int i = 0; i < picIndex.Length; i++)
            {
                int line = 0;
                int frame = 0;
                var pic = picList[picIndex[i]];
                if (pic.frameNum <= 0) pic.frameNum = 1;
                if (pic.line <= 0) pic.line = 1;

                float Width = pic.width;
                float Height = pic.height;
                if (pic.frameNum > 1)
                {
                    if (pic.frameSpan == 0)
                    {
                        pic.frameSpan = 80;
                    }
                    frame = (count * 16 / pic.frameSpan) % pic.frameNum;
                }
                if (picIndex[i] >= 6) frame = 1; //鬼畜显示修正
                if (Poi_A == 1)
                {
                    pic.x = dx[i];
                }
                ppDevice.BitBlt(pic,
                                new RectangleF(x + pic_dx + pic.x - rect.Left, y + pic_dy + pic.y - rect.Top, pic.width, pic.height),
                                new RectangleF(frame * Width, line * Height, Width, Height),
                                alpha);
            }
            Draw_RectList();
        }
        public override List<RectangleF> GetPhyList()
        {
            if (phyList == null) return null;
            List<RectangleF> rects = new List<RectangleF>();
            foreach (var rect in phyList)
            {
                rects.Add(new RectangleF(x + pic_dx + rect.X, y + pic_dy + rect.Y, rect.Width, rect.Height));
            }
            return rects;
        }
        public void OpenTheDoor()
        {
            soutai = 2;
        }
        public bool IsOpen()
        {
            return soutai == 2;
        }
    }
    public class SpritePrimer : SpriteMap
    {
        public SpriteGate gate1 = null;
        public SpriteGate gate2 = null;
        public override List<RectangleF> GetPhyList()
        {
            List<RectangleF> list = new List<RectangleF>();
            list.Add(new RectangleF(x + pic_dx, y + pic_dy, width, height));
            return list;
        }
        public override void Del()
        {
            if (gate1 != null)
            {
                gate1.OpenTheDoor();
                gate1 = null;
            }
            if (gate2 != null)
            {
                gate2.Del();
                gate2 = null;
            }
            base.Del();
        }
    }
}
