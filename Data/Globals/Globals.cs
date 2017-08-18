using Data.Rooms;
using Data.Cameras;
using Data.Inputs;
using Data.PPDevices;
using Data.Resources;
using Data.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Room;
using Data.Windows;
using System.Drawing;
using SharpDX.Direct3D9;
using Data.MapsManager;
using ClientPublic;
using Data.Replays;
using Data.Sounds;

namespace Data.Globals
{
    public static class Global
    {
        public static bool IsFormGameOpen = false;
        public static bool IsDebug = false;
        public static bool IsConnect()
        {
            if (clientC != null)
                return clientC.IsConnect();
            return false;
        }

        private static ResManager resManager = new ResManager();
        private static XmlManager xmlManager = new XmlManager();
        private static Input inputManager = new Input();
        private static Camera camara = new Camera();
        private static Rooms.Room room = new Rooms.Room();
        private static PlayerData player = new PlayerData();
        private static PPDevice ppDevice;
        private static Window_List windowsList = new Window_List();
        private static MapManager mapManager = null;
        private static ClientC clientC ;
        private static Replay replayManager = new Replay();
        private static Sound soundManager = new Sound();

        public static ResManager GetResManager()
        {
            return resManager;
        }
        public static XmlManager GetXmlManager()
        {
            return xmlManager;
        }
        public static Input GetInput()
        {
            return inputManager;
        }
        public static PPDevice GetPPDevice()
        {
            return ppDevice;
        }
        public static Camera GetCamara()
        {
            return camara;
        }
        public static Rooms.Room GetRoom()
        {
            return room;
        }
        public static PlayerData GetPlayer()
        {
            return player;
        }
        public static Window_List GetWindowsList()
        {
            return windowsList;
        }
        public static void SetPPDevice(PPDevice dv)
        {
            ppDevice = dv;
        }
        public static MapManager GetMapManager()
        {
            return mapManager;
        }
        public static void SetMapManager(MapManager mm)
        {
            mapManager = mm;
        }
        public static ClientC GetClientC()
        {
            return clientC;
        }
        public static void SetClientC(ClientC c)
        {
            clientC = c;
        }
        public static Replay GetReplayManager()
        {
            return replayManager;
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
                return ppDevice.LoadBitmapFromFile(file);
                //return Texture.FromFile(Form1.data.ppDevice.device, file);
            }
            //MessageBox("加载图片失败：" + file);
            return null;
        }

        public static Sound GetSoundManager()
        {
            return soundManager;
        }
        public static void PlayBgm(int id,bool isLoop = true)
        {
            soundManager.PlayBgm(id,isLoop);
        }
        public static void PlayBgm(Sound.BGM_ID id, bool isLoop = true)
        {
            soundManager.PlayBgm(id, isLoop);
        }
        public static void PlayEffect(int id)
        {
            soundManager.PlayEffect(id);
        }
        public static void PlayEffect(Sound.EFFECT_ID id)
        {
            soundManager.PlayEffect(id);
        }

        public static void BitBlt_Rect_Green(RectangleF pos)
        {
            if (IsDebug)
                GetPPDevice().BitBlt_Rect_Green(pos, 1, 1);
        }
        public static void BitBlt_Rect_Red(RectangleF pos)
        {
            if (IsDebug)
                GetPPDevice().BitBlt_Rect_Red(pos, 1, 1);
        }
        public static void BitBlt_Rect_Blue1(RectangleF pos)
        {
            if (IsDebug)
                GetPPDevice().BitBlt_Rect_Blue1(pos, 1, 1);
        }
        public static void BitBlt_Rect_Blue2(RectangleF pos)
        {
            if (IsDebug)
                GetPPDevice().BitBlt_Rect_Blue2(pos, 1, 1);
        }
        public static void BitBlt_Rect_Pink(RectangleF pos)
        {
            if (IsDebug)
                GetPPDevice().BitBlt_Rect_Pink(pos, 1, 1);
        }
        public static void BitBlt_Str(string str, Rectangle rect, SharpDX.Color color, int width, int height, string ZiTi = "宋体")
        {
            var p = ppDevice;
            p.BitBlt(str, rect, color, width, height, ZiTi);
        }

        public static void MessageBox(string str)
        {

        }
        public static void DelRes()
        {
            //释放所有资源
            resManager.Clear();
            room = new Rooms.Room();
            player = new PlayerData();
            inputManager = new Input();
            camara = new Camera();
            ppDevice.Clear();
            ppDevice = null;
            windowsList.Clear();
            windowsList = new Window_List();
            SpriteBase.ClearAll();
        }
        public static SpritePlayerJudgement GetMin(List<SpritePlayerJudgement> list)
        {
            if (list == null) return null;
            if (list.Count == 0) return null;
            int t = 0;
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].judgeData.time < list[t].judgeData.time)
                {
                    t = i;
                }
            }
            return list[t];
        }
        public static List<RectangleF> GetRectList_StaticElement(int type, List<Point> points)
        {
            if (points == null) return null;
            if (points.Count == 0) return null;
            List<RectangleF> rects = new List<RectangleF>();

            if (points.Count == 4)
            {
                rects.Add(new RectangleF(
                                points[3].X,
                                points[0].Y,
                                points[1].X - points[3].X,
                                points[2].Y - points[0].Y));
            }
            else if (points.Count == 12)
            {
                rects.Add(new RectangleF(
                                points[0].X,
                                points[0].Y,
                                points[6].X - points[0].X,
                                points[6].Y - points[0].Y));
                rects.Add(new RectangleF(
                                points[2].X,
                                points[2].Y,
                                points[8].X - points[2].X,
                                points[8].Y - points[2].Y));
            }
            return rects;
        }

        public static void Init_Map(int typeID, int mapID)
        {
            if(mapManager == null)
            {
                mapManager = new MapManager(xmlManager.maps[typeID].maps[mapID]);
            }
        }

    }
}
