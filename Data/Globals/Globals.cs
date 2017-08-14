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

namespace Data.Globals
{
    public static class Global
    {
        public static bool IsDebug = false; 
        private static ResManager resManager = new ResManager();
        private static XmlManager xmlManager = new XmlManager();
        private static Input inputManager = new Input();
        private static Camera camara = new Camera();
        private static Rooms.Room room = new Rooms.Room();
        private static PlayerData player = new PlayerData();
        private static PPDevice ppDevice;
        private static Window_List windowsList = new Window_List(); 

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
        public static void PlayBgm(int id)
        {

        }
        public static Window_List GetWindowsList()
        {
            return windowsList;
        }
        public static void SetPPDevice(PPDevice dv)
        {
            ppDevice = dv;
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
    }
}
