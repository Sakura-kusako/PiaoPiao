using Data.Globals;
using Data.PPDevices;
using Data.Resources;
using Room;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = SharpDX.Color;

namespace Data.Windows
{
    enum SpriteID : int
    {
        Login_BG = 1,
        Login_Enter,
        Login_Cancle,
    }
    public class Sprite_Static : SpriteBase
    {
        public Sprite_Static(int ID, float x = 0, float y = 0)
        {
            switch (ID)
            {
                case (int)SpriteID.Login_BG:
                    Create_Sprite_Login();
                    break;
                default:
                    break;
            }
        }
        private void Create_Sprite_Login()
        {
            pic = Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"\Medias\登陆界面背景2.bmp");
            width = 800;
            height = 600;
        }
    }
    public class Sprite_Botton : SpriteBase
    {
        public int frame = 0;
        public override void Action()
        {
            if (x <= Input.x && Input.x < x + width &&
                y <= Input.y && Input.y < y + height)
            {
                frame = 3;
            }
            else
            {
                frame = 0;
            }
        }
        public override RectangleF GetPicArea()
        {
            return new RectangleF(frame * width, 0, width, height);
        }
        public Sprite_Botton(int i, float x, float y)
        {
            /*
            switch (ID)
            {
                case (int)SpriteID.Login_Enter:
                    Create_Sprite_Login_Enter();
                    break;
                case (int)SpriteID.Login_Cancle:
                    Create_Sprite_Login_Cancle();
                    break;
                default:
                    break;
            }
            */
        }
        public void Create_Sprite_Login_Enter(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
            this.width = 83;
            this.height = 29;
            pic = Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"\Medias\登陆界面-登陆按钮2.png");
        }
        public void Create_Sprite_Login_Cancle(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
            this.height = 26;
            this.width = 63;
            pic = Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"\Medias\登陆界面-离开按钮2.png");
        }
    }
    public class Sprite_Text : SpriteBase
    {
        public string str = "文字";
        public float size = 12;
        public new string TextType = "宋体";
        public Color color = new Color(0xff, 0xff, 0xff);
        public Sprite_Text(float x = 0, float y = 0, string str = "文字", int alpha = 255, int color = 0xffffff, float width = -1, float size = 12, string Type = "宋体")
        {
            if (width < 0)
            {
                this.width = size * str.Length;
            }
            else
            {
                this.width = width;
            }
            height = size;
            type = (int)SpriteType.String;
            this.alpha = 1.0f;
            this.color = new Color(color);
            this.color.A = (byte)alpha;
            this.TextType = Type;
            this.str = str;
            this.x = x;
            this.y = y;
        }
        public override void Draw(PPDevice ppDevice)
        {
            /*
            var saved = g.Save();
            g.TranslateTransform(x, y);
            //g.ScaleTransform(e.scale_x, e.scale_y);
            g.TranslateTransform(pic_dx, pic_dy);
            g.DrawString(str, new Font(TextType, 12), new SolidBrush(color), new PointF(0, 0));
            g.Restore(saved);
            */
        }
    }
    public class Sprite_Bar : SpriteBase
    {
        public float pt = 0;//百分比
        public override float GetWidth()
        {
            return width * pt;
        }
    }
    public class Sprite_Bar_Server : Sprite_Bar
    {
        public int index = 0;
        public int frame = 8;
        public override void Action()
        {
            /*
            var ser = Ser_Data.servers[index];
            int ptr = (int)(((ser.players.Count / ser.PlayerMax * (frame - 1))) + 1);
            pt = 1.0f * ptr / frame;
            */
        }
        public Sprite_Bar_Server(string src, int index = 0, float x = 0, float y = 0, float width = 56, float height = 18, int frame = 8)
        {
            this.pic = Load_Bitmap_FromFile(src);
            this.index = index;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.frame = frame;
        }
    }
    public class Sprite_WinAdd : SpriteBase
    {
        public int frame = 0;
        public int line = 0;
        public int count = 0;
        public new BalloonItemPic_Base pic = null;
        public override void Draw(PPDevice ppDevice)
        {
            ppDevice.BitBlt(pic, GetShowArea(), GetPicArea(), alpha);
        }
        public override RectangleF GetPicAABB()
        { //在图层上的坐标
            return new RectangleF(x + pic_dx, y + pic_dy, width, height);
        }
        public override RectangleF GetPicArea()
        {
            if(pic == null)return new RectangleF(0, 0, 1, 1);
            if (pic.frameNum <= 0) pic.frameNum = 1;
            if (pic.line <= 0) pic.line = 1;
            float Width = pic.width;
            float Height = pic.height;
            //           return new RectangleF(0, 0, 1, 1);
            return new RectangleF(frame * Width, line * Height, Width, Height);
        }
        public override RectangleF GetShowArea()
        { //显示在屏幕上的实际位置
            return new RectangleF(x + pic_dx, y + pic_dy, width, height);
        }
        public override void Action()
        {
            count++;
            if(pic != null)
            {
                if (pic.frameNum > 1)
                {
                    if (pic.frameSpan == 0)
                    {
                        pic.frameSpan = 80;
                    }
                    frame = (count * 16 / pic.frameSpan) % pic.frameNum;
                }
            }
        }
        public virtual void Update()
        {

        }
        public new BalloonItemPic_Base GetBitmap()
        {
            return pic;
        }
        public void BitBlt(PPDevice ppDevice, BalloonItemPic_Base pic, RectangleF pos)
        {
            if (pic.frameNum <= 0) pic.frameNum = 1;
            if (pic.line <= 0) pic.line = 1;
            float Width = pic.width;
            float Height = pic.height;

            if (pic.frameNum == 1)
            {
                ppDevice.BitBlt(pic, pos, new RectangleF(0, 0, Width, Height), alpha);
            }

            int frame = 0;
            if (pic.frameNum > 1)
            {
                if (pic.frameSpan == 0)
                {
                    pic.frameSpan = 80;
                }
                frame = (this.count * 16 / pic.frameSpan) % pic.frameNum;
            }
            ppDevice.BitBlt(pic, pos, new RectangleF(frame * Width, 0, Width, Height), alpha);
        }
        public void BitBlt(PPDevice ppDevice, BalloonItemPic_Base pic, float x, float y)
        {
            BitBlt(ppDevice, pic, new RectangleF(x, y, pic.width, pic.height));
        }
        public override void Clear()
        {
            base.Clear();
            Clear(pic);
        }
    }
    public class Sprite_UserInfo : Sprite_WinAdd
    {
        public int index = 0;
        public int select = -1;
        public Texture upimg = null;
        public Texture downimg = null;
        public Texture levelimg = null;
        public Sprite_UserInfo()
        {
            type = (int)SpriteType.WinAdd;
            x = 588;
            y = 223;
            width = 200;
            height = 25;
            upimg = GlobalB.Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"\Medias\UserInfoBk.bmp");
            downimg = GlobalB.Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"\Medias\UserInfoBkOn.bmp");
        }
        public override void Draw(PPDevice ppDevice)
        {
            /*
            var font = new Font(TextType, 9);
            var brush = new SolidBrush(color_white2);
            var players = Ser_Data.servers[Config.server].players;
            var format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Far;

            for (int i = 0; i < 9; i++)
            {
                var saved = g.Save();
                g.TranslateTransform(x, y + i * height);
                g.RotateTransform(rotate);
                g.TranslateTransform(pic_dx, pic_dy);
                if (i == select)
                {
                    g.DrawImage(downimg, GetShowArea(), GetPicArea(), GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(upimg, GetShowArea(), GetPicArea(), GraphicsUnit.Pixel);
                }
                if (i + index < players.Count)
                {
                    var player = players[i + index];
                    g.DrawString(player.GetLevel(), font, brush, new PointF(13, 5));
                    if (player.roomID > 0)
                    {
                        g.DrawString(player.GetRoom(), font, brush, new PointF(43, 5));
                    }
                    g.DrawString(player.GetName(), font, brush, new RectangleF(80, 0, 113, height), format);
                }

                g.Restore(saved);
            }
            */
        }
        public override void Clear()
        {
            base.Clear();
            Clear(upimg);
            Clear(downimg);
            Clear(levelimg);
        }
    }
    public class Sprite_Room : Sprite_WinAdd
    {
        //随机，对抗，竞速，竞技，夺宝，夺旗
        public Rooms.Room room = null;
    }
    public class Sprite_Rooms : Sprite_WinAdd
    {
        public int index = 0;
        public int select = 0;
        public Sprite_Room[] rooms = new Sprite_Room[9];
        public Sprite_Rooms()
        {
            x = 19;
            y = 92;
            pic_dx = 0;
            pic_dy = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rooms[i * 3 + j] = new Sprite_Room_Out();
                    rooms[i * 3 + j].x = x + pic_dx + j * 183;
                    rooms[i * 3 + j].y = y + pic_dy + i * 92;
                }
            }
        }
        public override void Action()
        {
            for (int i = 0; i < 9; i++)
            {
                rooms[i].pic_dx = pic_dx;
                rooms[i].pic_dy = pic_dy;
                //rooms[i].room = Ser_Data.servers[Config.server].rooms[index + i];
                rooms[i].Action();
            }
        }
        public override void Draw(PPDevice ppDevice)
        {
            for (int i = 0; i < 9; i++)
            {
                rooms[i].Draw(ppDevice);
            }
        }
    }
    public class Sprite_Room_Out : Sprite_Room
    {
        public override void Action()
        {
            if (room == null) return;
        }
        public override void Draw(PPDevice ppDevice)
        {
            if (room == null) return;
            Draw_bg(ppDevice);
            Draw_soutai(ppDevice);
            Draw_mode(ppDevice);
            DrawNum(ppDevice, room_Num18x18, room.GetRoomMax(), x + pic_dx + 156, y + pic_dy + 69, 18, 18, 1);
            DrawNum(ppDevice, room_Num18x18, room.GetPlayerNum(), x + pic_dx + 125, y + pic_dy + 69, 18, 18, 1);
            DrawNum(ppDevice, Num_Room_ID_17x16, room.ID, x + pic_dx + 54, y + pic_dy + 4, 17, 16, 3);
            Draw_ModeString(ppDevice, room.GetModeString(), 70, 33);
        }
        public void Draw_bg(PPDevice ppDevice)
        {
            BitBlt(ppDevice, mode_out_bg, x + pic_dx, y + pic_dy);
        }
        public void Draw_soutai(PPDevice ppDevice)
        {
            BitBlt(ppDevice, mode_soutai[room.soutai], x + pic_dx + 60, y + pic_dy + 60);
        }
        public void Draw_mode(PPDevice ppDevice)
        {
            if (room.GetPlayerNum() == 0) return;
            BitBlt(ppDevice, mode_out[room.mode], x + pic_dx + 60, y + pic_dy + 60);
        }
        public void Draw_ModeString(PPDevice ppDevice, string str, float dx, float dy, float size = 10)
        {
            /*
            var saved = g.Save();
            g.TranslateTransform(x + pic_dx, y + pic_dy);
            g.DrawString(str, new Font(TextType, size), new SolidBrush(color_blue), new PointF(dx, dy));
            g.Restore(saved);
            */
        }
    }
    public class Sprite_Room_In2 : Sprite_Room
    {
        //负责绘制房间内部的基本信息
        //房主，游戏地图，房间名字等
        public Sprite_Room_In2()
        {
            //var ser = Ser_Data.servers[Config.server];//读取当前服务器;
            //room = ser.rooms[Config.player.roomID - 1];
            room = Global.GetRoom();
        }
        public override void Action()
        {
            count++;
            if (room == null) return;
        }
        public override void Draw(PPDevice ppDevice)
        {
            if (room == null) return;
            DrawMode(ppDevice);
            DrawRoomNum(ppDevice);
            DrawTxt(ppDevice);
            DrawBoss(ppDevice);
            DrawOther(ppDevice);
        }
        private void DrawMode(PPDevice ppDevice)
        {
            BitBlt(ppDevice, mode[room.mode], 6, 407);
            if (room.mode == 0)
            {
                BitBlt(ppDevice, mode_suijitubiao, 6 + (190 - 157) / 2, 407 + 25);
                BitBlt(ppDevice, mode_txt[room.mode], 6 + (190 - 111) / 2, 407 + 105);
            }
            else if (room.mode == 1)
            {
                BitBlt(ppDevice, mode_quantao, 6 + (190 - 140) / 2, 415);
                BitBlt(ppDevice, mode_txt[room.mode], 6 + (190 - 101) / 2, 407 + 117);
            }
            else if (room.mode == 2)
            {
                BitBlt(ppDevice, mode_qizi, 6 + (190 - 140) / 2, 417);
                BitBlt(ppDevice, mode_txt[room.mode], 6 + (190 - 101) / 2, 407 + 117);
            }
            BitBlt(ppDevice, mode_gaoguang, 9, 407 + 6);
        }
        private void DrawRoomNum(PPDevice ppDevice)
        {
            DrawNum(ppDevice, Num_Room_In_ID_16x16, room.ID, 67, 6, 16, 16, 3);
        }
        private void DrawTxt(PPDevice ppDevice)
        {
            /*
            g.DrawString(str, new Font(TextType, 10), new SolidBrush(color_yellow), new PointF(80, 9));
            */
            Global.BitBlt_Str("飘飘" + ": " + room.name, new Rectangle(80, 10, 300, 30), color_yellow, 6, 12);
            Global.BitBlt_Str(room.GetRoomType(), new Rectangle(337, 10, 600, 100), color_yellow, 6, 12);
            Global.BitBlt_Str(room.GetMapName(), new Rectangle(98, 578, 800, 600), color_blue, 6, 12);
        }
        private void DrawOther(PPDevice ppDevice)
        {
            //g.DrawString(room.GetRoomType(), new Font(TextType, 10), new SolidBrush(color_yellow), new PointF(334, 10));
            if (room.IsWS) BitBlt(ppDevice, room_WS, 305, 7);
        }
        private void DrawBoss(PPDevice ppDevice)
        {
            float dx = 188 + (room.Boss % 2) * 406;
            float dy = 40 + (room.Boss / 2) * 120;
            BitBlt(ppDevice, room_Boss, dx, dy);
        }
    }
    public class Sprite_Room_In : Sprite_Room
    {
        //负责绘制房间内部的玩家
        //大图片，小图片，玩家个人数据等
        public Sprite_Room_In()
        {
            room = Global.GetRoom();
        }
        public override void Action()
        {
            count++;
        }
        public override void Draw(PPDevice ppDevice)
        {
            if (room == null) return;
            DrawPlayer(ppDevice);
        }
        private void DrawPlayer(PPDevice ppDevice)
        {
            for (int i = 0; i < 6; i++)
            {
                Draw_bg(ppDevice, i);
                if (room.GetPlayer(i) != null)
                {
                    DrawBig(ppDevice, i);
                    DrawPlayerData(ppDevice, i);
                }
            }
        }
        public void Draw_bg(PPDevice ppDevice, int i)
        {
            //绘制大角色背景底板
            float bg_x = 10; //左上角坐标
            float bg_y = 36;
            float dx = 600; //横向间隔
            float dy = 120; //纵向间隔

            bool IsLeft = (i % 2 == 0);
            int index = room.GetTeam(i) + (IsLeft ? 0 : 5);
            float x = bg_x + (IsLeft ? 0 : dx);
            float y = bg_y + (i / 2) * dy;

            BitBlt(ppDevice, room_player_bg[index], x, y);
        }
        public void DrawBig(PPDevice ppDevice, int j)
        {
            //绘制大角色
            float x_Left = -54; //左边起始位置
            float x_Right = 654; //右边起始位置
            float y_Begin = 14; //上方起始位置
            float dy = 120; //人物间隔

            bool IsLeft = (j % 2 == 0);
            float x = (IsLeft ? x_Left : x_Right);
            float y = y_Begin + (j / 2) * dy;
            int line = IsLeft ? 1 : 0;

            //绘制顺序
            int[] pic = new[]
            {
                //42, //背景特效（大角色不显示）
                10, //座舱
                18, //身体
                 7, //脸型
                 8, //脸装饰
                20, //嘴巴
                 9, //胡子
                 4, //眼睛
                 5, //眼镜
                 2, //头发
                 3, //发饰
                19, //手
                //12, //舰首像(暂时无图？)
                //13, //舱尾旗（暂时无图？）
                // 1, //气球(大角色不显示)
                //21, //座舱气垫(不显示？)
                //33, //气泡图片(不显示？)
            };
            var res = Global.GetResManager().itemPic.itemPic1;
            for (int i = 0; i < pic.Length; i++)
            {
                var index = room.GetPlayer(j).items[room.GetPlayer(j).type][pic[i]];
                var item2 = res[pic[i]].itemPic2[index];
                if (item2 == null) continue;
                var pic_poi = item2.itemPic_Base[1];
                if (pic_poi == null) continue;
                if (pic_poi.bitmap == null) continue;
                int frame = 0;
                float w = pic_poi.width;
                float h = pic_poi.height;

                if (pic_poi.frameNum > 1)
                {
                    if (pic_poi.frameSpan < 100)
                        pic_poi.frameSpan = 100;
                    if (pic[i] == 4)
                    {
                        //控制眨眼频率
                        int len = pic_poi.frameSpan * pic_poi.frameNum;
                        int t = (count * 16) % 5000; //5000ms一次
                        if (t < len)
                        {
                            //5秒一次
                            frame = t / pic_poi.frameSpan;
                        }
                    }
                    else
                    {
                        frame = (count * 16 / pic_poi.frameSpan) % pic_poi.frameNum;
                    }
                }
                ppDevice.BitBlt(pic_poi,
                                new RectangleF(x + pic_poi.x, y + pic_poi.y, pic_poi.width, pic_poi.height),
                                new RectangleF(frame * w, line * h, w, h), alpha);

            }
        }
        public void DrawPlayerData(PPDevice ppDevice, int i)
        {
            bool IsLeft = (i % 2 == 0);
            var player = room.GetPlayer(i);

            if (player.type != 0)
            {
                int t = room.GetTeam(i) - 1;
                if (t < 0) t = 0;
                BitBlt(ppDevice, room_player_type[player.type][t], IsLeft ? 105 : 636, 86 + (i / 2) * 120);
            }
            BitBlt(ppDevice, room_Hit, IsLeft ? 105 : 636, 69 + (i / 2) * 120);
            DrawNum(ppDevice, Num_Hit_8x10, player.hit, IsLeft ? 137 : 668, 74 + (i / 2) * 120, 8, 10, 0, 1);
            
            //Global.BitBlt_Str(player.GetName(), new Rectangle(17, 47 + (i/2) * 120, 121, 15), color_yellow, 6, 12);
        }
    }
    public class Sprite_Shop : Sprite_WinAdd
    {
        //负责绘制商店内部的玩家面板
        //大图片，玩家个人数据等
        public Sprite_Shop()
        {
        }
        public override void Action()
        {
            count++;
        }
        public override void Draw(PPDevice ppDevice)
        {
            DrawPlayer(ppDevice);
        }
        private void DrawPlayer(PPDevice ppDevice)
        {
            /*
            var format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            */
            Draw_bg(ppDevice);
            DrawBig(ppDevice);
            DrawPlayerData(ppDevice);
            //g.DrawString(player.GetName(), new Font(TextType, 9), new SolidBrush(color_yellow), new RectangleF(17, 47 + i * 120, 121, 20), format);
        }
        public void Draw_bg(PPDevice ppDevice)
        {
            //绘制大角色背景底板
            BitBlt(ppDevice, room_player_bg[2], x, y);
        }
        public void DrawBig(PPDevice ppDevice)
        {
            //绘制大角色
            float x_Left = -54 - 10; //左边起始位置
            float y_Begin = 14 - 45; //上方起始位置

            float x = x_Left;
            float y = y_Begin;
            int line = 1;

            //绘制顺序
            int[] pic = new[]
            {
                //42, //背景特效（大角色不显示）
                10, //座舱
                18, //身体
                 7, //脸型
                 8, //脸装饰
                20, //嘴巴
                 9, //胡子
                 4, //眼睛
                 5, //眼镜
                 2, //头发
                 3, //发饰
                19, //手
                //12, //舰首像(暂时无图？)
                //13, //舱尾旗（暂时无图？）
                // 1, //气球(大角色不显示)
                //21, //座舱气垫(不显示？)
                //33, //气泡图片(不显示？)
            };
            var res = Global.GetResManager().itemPic.itemPic1;
            var player = Global.GetPlayer();
            for (int i = 0; i < pic.Length; i++)
            {
                var index = player.items[player.type][pic[i]];
                var item2 = res[pic[i]].itemPic2[index];
                if (item2 == null) continue;
                var pic_poi = item2.itemPic_Base[1];
                if (pic_poi == null) continue;
                if (pic_poi.bitmap == null) continue;
                int frame = 0;
                float w = pic_poi.width;
                float h = pic_poi.height;

                if (pic_poi.frameNum > 1)
                {
                    if (pic_poi.frameSpan < 100)
                        pic_poi.frameSpan = 100;
                    if (pic[i] == 4)
                    {
                        //控制眨眼频率
                        int len = pic_poi.frameSpan * pic_poi.frameNum;
                        int t = (count * 16) % 5000; //5000ms一次
                        if (t < len)
                        {
                            //5秒一次
                            frame = t / pic_poi.frameSpan;
                        }
                    }
                    else
                    {
                        frame = (count * 16 / pic_poi.frameSpan) % pic_poi.frameNum;
                    }
                }
                ppDevice.BitBlt(pic_poi,
                                new RectangleF(x + pic_poi.x, y + pic_poi.y, pic_poi.width, pic_poi.height),
                                new RectangleF(frame * w, line * h, w, h), alpha);

            }
        }
        public void DrawPlayerData(PPDevice ppDevice)
        {
            var player = Global.GetPlayer();

            if (player.type != 0)
            {
                BitBlt(ppDevice, room_player_type[player.type][1], 95, 50);
            }
            BitBlt(ppDevice, room_Hit, 95, 33);
            DrawNum(ppDevice, Num_Hit_8x10, player.hit, 127, 38, 8, 10, 0, 1);
        }
    }
    public class Sprite_Room_In_bg : Sprite_WinAdd
    {
        public override void Action()
        {
            count++;
        }
        public override void Draw(PPDevice ppDevice)
        {
            Drawbg(ppDevice);
            DrawKumo(ppDevice);
        }
        private void Drawbg(PPDevice ppDevice)
        {
            BitBlt(ppDevice, room_in_bg[0], 0, 0);
        }
        private void DrawKumo(PPDevice ppDevice)
        {
            float dx = count % 4000 / 5;
            BitBlt(ppDevice, room_in_bg[1], dx, 287);
            BitBlt(ppDevice, room_in_bg[1], dx - 800, 287);
        }
    }
    public class Sprite_Room_Player : Sprite_WinAdd
    {
        PlayerData player = Global.GetPlayer();
        public Sprite_Room_Player()
        {
        }

        public override void Action()
        {
            count++;
        }

        public override void Draw(PPDevice ppDevice)
        {
            /*
                var font = new Font(TextType, 9);
                var brush = new SolidBrush(color_black);
                var saved = g.Save();
                g.DrawString(player.GetName(), font, brush, new PointF(629, 118));
                g.DrawString(player.GetQQ(), font, brush, new PointF(629, 134));
                g.DrawString(player.GetLevel(), font, brush, new PointF(629, 150));
                g.DrawString(player.GetHit(), font, brush, new PointF(629, 166));
                g.DrawString(player.GetExp(), font, brush, new PointF(629 + 92, 166));
                g.DrawString(player.GetVS(), font, brush, new PointF(629, 182));
                g.Restore(saved);
            */
        }
    }
    public class Sprite_Shop_ShangDian : Sprite_WinAdd
    {
        /*
        public BalloonItemPic_Base pic_bg = null;
        public BalloonItemPic_Base pic_bg_s = null;

        public Sprite_Shop_ShangDian()
        {
            typeID = 110015;
            x = 24;
            y = 98;
            width = 177 + 8;
            height = 91 + 8;
            val_A = 0;
            poi_A = 0;
            pic = Load_Bitmap_FromFile(path + @"balloon\Pic\Shop\商店-选定框.tga", 185, 99);
            pic_bg = Load_Bitmap_FromFile(path + @"balloon\Pic\Shop\商品货架.tga", 177, 91);
            pic_bg_s = Load_Bitmap_FromFile(path + @"balloon\Pic\Shop\商品架-选中.tga", 177, 91);
        }
        public ShopManager GetShopData()
        {
            return Form1.data.shop;
        }
        public override void Action()
        {
            count++;
        }
        public override void Draw(PPDevice ppDevice)
        {
            for (int i = 0; i < 8; i++)
            {
                Drawbg(ppDevice, i);
                DrawItem(ppDevice, i);
                if (GetShopData().select_ID == i + 1)
                {
                    DrawSelectBox(ppDevice, i);
                }
            }
            DrawPage(ppDevice);
            DrawMoney(ppDevice);
        }
        private RectangleF GetShowArea(int index)
        {
            float dx = (index % 2) * width;
            float dy = (index / 2) * height;
            return new RectangleF(x + pic_dx + dx, y + pic_dy + dy, pic_bg.width, pic_bg.height);
        }
        private RectangleF GetShowArea_s(int index)
        {
            float dx = (index % 2) * width - 4;
            float dy = (index / 2) * height - 4;
            return new RectangleF(x + pic_dx + dx, y + pic_dy + dy, pic.width, pic.height);
        }
        private BalloonItemPic_Base GetBitmap(int index)
        {
            if (index + 1 == poi_A)
            {
                return pic_bg_s;
            }
            return pic_bg;
        }
        private void Drawbg(PPDevice ppDevice, int index)
        {
            BitBlt(ppDevice, GetBitmap(index), GetShowArea(index));
        }
        private void DrawItem(PPDevice ppDevice, int index)
        {
            var shop = GetShopData();
            var shopitem = shop.GetShopItem(shop.mulu_shangdian, (shop.page - 1) * 8 + index);
            if (shopitem == null) return;
            var pic = Form1.data.res.itemPic.itemPic1[shopitem.pic_type].itemPic2[shopitem.pic_ID].shopbitmap;
            float poi_x = (index % 2) * width + x + pic_dx;
            float poi_y = (index / 2) * height + y + pic_dy;
            if (pic.bitmap != null)
            {
                float dx = poi_x + 40 - pic.width / 2;
                float dy = poi_y + 53 - pic.height / 2;
                BitBlt(ppDevice, pic, new RectangleF(dx, dy, pic.width, pic.height));
            }
            DrawNum(ppDevice, Num_Shop_9x17, shopitem.price_Q, poi_x + 110, poi_y + 33, 9, 17, 1, 3);
            DrawNum(ppDevice, Num_Shop_9x17, shopitem.price_Piao, poi_x + 110, poi_y + 63, 9, 17, 1, 3);
        }
        private void DrawPage(PPDevice ppDevice)
        {
            var shop = GetShopData();
            int len = shop.GetShopPageLen(shop.mulu_shangdian);
            float px = pic_dx + 425;
            float py = pic_dy + 217;
            float dx = 9;

            DrawNum(ppDevice, Num_Shop_9x17, shop.page, px, py, 9, 17, 2, 0);
            DrawNum(ppDevice, Num_Shop_9x17, 10, px, py, 9, 17, 1, 10);
            DrawNum(ppDevice, Num_Shop_9x17, len, px + dx, py, 9, 17, 2, 1);
        }
        private void DrawSelectBox(PPDevice ppDevice, int index)
        {
            BitBlt(ppDevice, pic, GetShowArea_s(index));
        }
        private void DrawMoney(PPDevice ppDevice)
        {
            var player = Global.GetPlayer();
            float px = pic_dx + 74;
            float py = pic_dy + 536;

            DrawNum(ppDevice, Num_Shop_9x17, player.piaobi, px, py, 9, 17, 0, 3);
            DrawNum(ppDevice, Num_Shop_9x17, player.Qbi, px, py + 27, 9, 17, 0, 3);
        }
        */
    }
    public class Sprite_Room_MapSelect : Sprite_Room
    {
        public List<Sprite_Room_MapSelect_Root> roots;

        public Sprite_Room_MapSelect()
        {
            type = (int)SpriteType.WinAdd;
            typeID = 210;
            room = Global.GetRoom();
            roots = new List<Sprite_Room_MapSelect_Root>();
            //rect = 7,41,150,271
            x = 7;
            y = 41;
            width = 150;
            height = 271;
            pic_dx = 0;
            pic_dy = 0;
            val_A = 0;
            pic = Global.GetWindowsList().windows[Global.GetWindowsList().GetIndex(20)].pic;

            var maps = Global.GetXmlManager().maps;
            int len = maps.Count - 2; //-2 地图元素 新手教程地图
            for (int i = 1; i < len; i++)
            {
                Sprite_Room_MapSelect_Root u = new Sprite_Room_MapSelect_Root();
                u.enable = 1;
                u.alpha = 255;
                u.soutai = 0;
                u.thisID = ui_ID;
                u.type = (int)SpriteType.UI_CheckBox;
                u.typeID = 210 + i;
                ui_ID++;
                u.pic_dx = 0;
                u.pic_dy = 0;
                u.x = 6;
                u.y = 22 + 18 * i;
                u.width = 150;
                u.height = 18;
                u.soutai = 0;
                u.upimg = SpriteBase.Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"\balloon\" + maps[i].fileIn);
                u.downimg = SpriteBase.Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"\balloon\" + maps[i].fileOut);
                if (i == 1)
                {
                    Sprite_Room_MapSelect_Son son = new Sprite_Room_MapSelect_Son();
                    son.name = "随机地图";
                    u.sons.Add(son);
                }
                for (int j = 1; j < maps[i].maps.Count; j++)
                {
                    Sprite_Room_MapSelect_Son son = new Sprite_Room_MapSelect_Son();
                    son.name = maps[i].maps[j].name;
                    u.sons.Add(son);
                }
                roots.Add(u);
            }
        }

        public override void Action()
        {
            count++;
            for (int i = 0; i < roots.Count; i++)
            {
                var root = roots[i];
                root.pic_dx = this.pic_dx;
                root.pic_dy = this.pic_dy;
            }
            float ox = 6;
            float oy = 40 - val_A;
            if (val_A > GetHeightAll() - height)
            {
                val_A = GetHeightAll() - height;
            }
            if (val_A < 0) val_A = 0;
            for (int i = 0; i < roots.Count; i++)
            {
                var root = roots[i];
                root.y = oy;
                root.x = ox;
                oy += root.GetShowHeight();
            }

        }
        public override void Draw(PPDevice ppDevice)
        {
            for (int i = 0; i < roots.Count; i++)
            {
                var root = roots[i];
                root.Draw(ppDevice);
            }
            if (pic != null)
            {
                //贴图修正
                ppDevice.BitBlt(pic, x + pic_dx, y + pic_dy + height, new RectangleF(x, y + height, width, 20));
                ppDevice.BitBlt(pic, x + pic_dx, y + pic_dy - 20, new RectangleF(x, y - 20, width, 19));
            }
        }
        public override void Update()
        {
            float ox = 6;
            float oy = 40 - val_A;

            for (int i = 0; i < roots.Count; i++)
            {
                var root = roots[i];
                var index = root.Get_MouseY_Index();
                root.rawRect = GetPicAABB();
                if (index == -1)
                {
                    root.soutai = 1 - root.soutai;
                }
                else if (index >= 0)
                {
                    for (int j = 0; j < roots.Count; j++)
                    {
                        roots[j].selectedIndex = -1;
                    }
                    root.selectedIndex = index;
                    poi_A = 100 * (i + 1) + index + 1;
                }
                root.y = oy;
                root.x = ox;
                oy += root.GetShowHeight();
            }
        }
        public float GetHeightAll()
        {
            float h = 0;
            for (int i = 0; i < roots.Count; i++)
            {
                var root = roots[i];
                h += root.GetShowHeight();
            }
            return h;
        }
        public override void Clear()
        {
            foreach (var t in roots)
            {
                if (t != null)
                    t.Clear();
            }
            roots.Clear();
        }
    }
    public class Sprite_Map_Base : Sprite_WinAdd
    {
        /*
        public BalloonItemPic_Base name_bg; //名字背景框
        public BalloonItemPic_Base player_sx; //（生命、速度、攻击)文字
        public BalloonItemPic_Base nu_kong; //空怒槽
        public BalloonItemPic_Base nu_man; //满怒槽
        public BalloonItemPic_Base nu_dong; //冒泡动怒槽
        public BalloonItemPic_Base my_atk; //攻击力显示
        public BalloonItemPic_Base my_hp; //生命显示
        public BalloonItemPic_Base my_v; //速度显示
        public BalloonItemPic_Base player_hp; //其他玩家生命显示
        public BalloonItemPic_Base time1; //时间数字1
        public BalloonItemPic_Base player_daqi; //呼吸条
        public BalloonItemPic_Base player_daqi_time; //呼吸血条
        public BalloonItemPic_Base time_fuhuo; //复活时间

        public int playerID = 0;
        public Sprite_Map_Base()
        {
            Load_pic();
            playerID = 1;
        }
        public void Load_pic()
        {
            string path = GlobalB.GetRootPath() + "\\";
            name_bg = Load_Bitmap_FromFile(path + @"balloon\Pic\Moving\玩家信息栏.tga", 648, 23, 6);
            player_sx = Load_Bitmap_FromFile(path + @"balloon\Pic\Static\玩家属性.tga", 28, 43);
            nu_kong = Load_Bitmap_FromFile(path + @"balloon\Pic\Entropy\怒槽A.tga", 43, 43);
            nu_man = Load_Bitmap_FromFile(path + @"balloon\Pic\Entropy\怒槽B.tga", 43, 43);
            nu_dong = Load_Bitmap_FromFile(path + @"balloon\Pic\Entropy\怒槽C.tga", 60, 60, 4);
            my_atk = Load_Bitmap_FromFile(path + @"balloon\Pic\Static\我的攻击.tga", 9, 13);
            my_hp = Load_Bitmap_FromFile(path + @"balloon\Pic\Static\我的生命.tga", 9, 13);
            my_v = Load_Bitmap_FromFile(path + @"balloon\Pic\Static\我的速度.tga", 9, 13);
            player_hp = Load_Bitmap_FromFile(path + @"balloon\Pic\Static\我的速度.tga", 7, 13);
            time1 = Load_Bitmap_FromFile(path + @"balloon\Pic\Moving\时间数字1.tga", 15, 14);
            player_daqi = Load_Bitmap_FromFile(path + @"balloon\Pic\Static\呼吸条.tga", 335, 34);
            player_daqi_time = Load_Bitmap_FromFile(path + @"balloon\Pic\Static\呼吸血条.tga", 299, 11);
            time_fuhuo = Load_Bitmap_FromFile(path + @"balloon\Pic\Moving\复活倒计时数字.tga", 1700, 159, 10);
        }
        public override void Action()
        {
            count++;
        }
        public override void Draw(PPDevice ppDevice)
        {
            ppDevice.BitBlt(name_bg, new RectangleF(5, 5, 108, 23), new RectangleF(3 * 108, 0, 108, 23));
            Draw_sx(ppDevice);
            Draw_time(ppDevice);
            Draw_dead(ppDevice);
        }
        private void Draw_sx(PPDevice ppDevice)
        {
            //角色属性
            float ox = 40;
            float oy = 32;
            float dx = 10;
            float dy = 14;
            BitBlt(ppDevice, player_sx, new RectangleF(10, 30, 28, 43));
            var player = GetPlayer();
            for (int i = 0; i < player.hp; i++)
            {
                BitBlt(ppDevice, my_hp, new RectangleF(ox + i * dx, oy, 9, 13));
            }
            oy += dy;
            for (int i = 0; i < player.v; i++)
            {
                BitBlt(ppDevice, my_v, new RectangleF(ox + i * dx, oy, 9, 13));
            }
            oy += dy;
            for (int i = 0; i < player.atk; i++)
            {
                BitBlt(ppDevice, my_atk, new RectangleF(ox + i * dx, oy, 9, 13));
            }
        }
        private void Draw_time(PPDevice ppDevice)
        {
            //当局倒计时
            //320,20
            var time = Poi.GetManager().time / 60;
            int min = time / 60 + 1;
            int sec_l = time % 60 / 10 + 1;
            int sec_r = time % 10 + 1;
            ppDevice.BitBlt(time1, new RectangleF(320, 20, 15, 14), new RectangleF(min * 15, 0, 15, 14));
            ppDevice.BitBlt(time1, new RectangleF(335, 20, 15, 14), new RectangleF(0, 0, 15, 14));
            ppDevice.BitBlt(time1, new RectangleF(350, 20, 15, 14), new RectangleF(sec_l * 15, 0, 15, 14));
            ppDevice.BitBlt(time1, new RectangleF(365, 20, 15, 14), new RectangleF(sec_r * 15, 0, 15, 14));
        }
        private void Draw_dead(PPDevice ppDevice)
        {
            var player = GetPlayer();
            var camera = Global.GetCamara();
            if (player.Is_DaQi())
            {
                float time = 1 - player.GetBuffTime() / 480.0f;
                if (time < 0) time = 0;
                ppDevice.BitBlt(player_daqi,
                                new RectangleF(player.x - camera.left - 4, player.y - camera.top, 102, 11),
                                new RectangleF(0, 0, 335, 34));
                ppDevice.BitBlt(player_daqi_time,
                                new RectangleF(player.x - camera.left + 5, player.y - camera.top + 6, 90 * time, 3),
                                new RectangleF(0, 0, 299, 11));
            }
            if (player.Is_ShangTian())
            {
                int time = (player.GetBuffTime() + 59) / 60;
                if (time <= 9)
                {
                    DrawNum(ppDevice, time_fuhuo, time, 485, 226, 170, 159, 1);
                }
            }
        }
        private SpritePlayer GetPlayer()
        {
            var man = Poi.GetManager();
            int index = man.camera_type - 1;
            return man.pla.list[index];
        }
        */
    }
    public class Sprite_Map_JingSu : Sprite_WinAdd
    {
        /*
        public MapManager map;
        public int playerID = 0;

        public BalloonItemPic_Base jdt; //进度条
        public BalloonItemPic_Base dbs; //进度大标识
        public BalloonItemPic_Base name_big; //名字_大

        public Sprite_Map_JingSu()
        {
            OnCreate();
        }
        public void OnCreate()
        {
            map = Poi.GetManager();
            playerID = 1;
            Load_pic();
        }
        public void Load_pic()
        {
            dbs = Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"balloon\Pic\Moving\玩家进度大标识.tga", 228, 46);
            jdt = Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"balloon\Pic\Static\进度条.tga", 472, 34);
            name_big = Load_Bitmap_FromFile(GlobalB.GetRootPath() + @"balloon\Pic\Moving\玩家大标识数字.tga", 108, 9);
        }
        public override void Action()
        {
            count++;
        }
        public override void Draw(PPDevice ppDevice)
        {
            Draw_jd(ppDevice);
        }
        private void Draw_jd(PPDevice ppDevice)
        {
            var player = GetPlayer();
            float dx = (player.x / map.width) * 448 - 19;
            ppDevice.BitBlt(jdt, new RectangleF(164, 560, 472, 34), new RectangleF(0, 0, 472, 34));
            ppDevice.BitBlt(dbs, new RectangleF(184 + dx, 560 - 25, 38, 46), new RectangleF(114, 0, 38, 46));
            ppDevice.BitBlt(name_big, new RectangleF(194 + dx, 560 - 10, 18, 9), new RectangleF(0, 0, 18, 9));
        }

        private SpritePlayer GetPlayer()
        {
            var man = Poi.GetManager();
            int index = map.camera_type - 1;
            return man.pla.list[index];
        }
        */
    }
    public class Sprite_Room_MapSelect_Root : UI_CheckBox2
    {
        public int selectedIndex = -1;
        public List<Sprite_Room_MapSelect_Son> sons;
        public RectangleF rawRect;
        public Sprite_Room_MapSelect_Root()
        {
            sons = new List<Sprite_Room_MapSelect_Son>();
            rawRect = new RectangleF(0, 0, 800, 600);
            width = 150;
            poi_A = 16; //文字高度
        }

        public override void Draw(PPDevice ppDevice)
        {
            var pic = GetBitmap();
            if (pic != null && ((y + pic_dy) < rawRect.Bottom && y + pic_dy + height > rawRect.Top))
                ppDevice.BitBlt(GetBitmap(), GetShowArea(), GetPicArea(), alpha);

            if (soutai == 1)
            {
                for (int i = 0; i < sons.Count; i++)
                {
                    float y_poi = y + pic_dy + height + poi_A * i + 2;
                    if (y_poi < rawRect.Bottom && y_poi + poi_A > rawRect.Top)
                    {
                        if (selectedIndex == i)
                        {
                            Global.BitBlt_Str(sons[i].name,
                                        new Rectangle((int)(x + pic_dx), (int)(y_poi), 800, 600),
                                        new Color(0, 255, 0), 6, 12);
                        }
                        else if (Physics.XY_Rect(Input.x, Input.y, new RectangleF(x + pic_dx, y_poi - 1, width, poi_A - 0.0001f)))
                        {
                            Global.BitBlt_Str(sons[i].name,
                                        new Rectangle((int)(x + pic_dx), (int)(y_poi), 800, 600),
                                        new Color(254, 139, 139), 6, 12);
                        }
                        else
                        {
                            Global.BitBlt_Str(sons[i].name,
                                        new Rectangle((int)(x + pic_dx), (int)(y_poi), 800, 600),
                                        color_blue, 6, 12);
                        }
                    }
                }
            }
        }

        public float GetShowHeight()
        {
            if (soutai == 0)
            {
                return this.height;
            }
            else
            {
                return sons.Count * poi_A + this.height + 2;
            }
        }
        public override RectangleF GetPicAABB()
        {
            if (soutai == 0)
            {
                return base.GetPicAABB();
            }
            else
            {
                return new RectangleF(x + pic_dx, y + pic_dy, width, GetShowHeight());
            }
        }
        public int Get_MouseY_Index()
        {
            if (Physics.Is_X_In_Xs(Input.y, y + pic_dy, y + pic_dy + height))
            {
                return -1;
            }
            if (Physics.Is_X_In_Xs(Input.y, y + pic_dy, y + pic_dy + GetShowHeight()))
            {
                return (int)((Input.y - (y + pic_dy + height + 1)) / poi_A);
            }
            return -10;
        }
    }
    public class Sprite_Room_MapSelect_Son
    {
        public string name = "poi";
    }
}
