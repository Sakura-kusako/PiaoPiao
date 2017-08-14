using Data.Cameras;
using Data.Globals;
using Data.Inputs;
using Data.PPDevices;
using Data.Resources;
using Data.Windows;
using Data.XML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MainC
{
    public partial class FormGame : Form
    {
        private Form1 FormParent;
        private Stopwatch sw = new Stopwatch();
        private int fps = 0;
        private long t0 = 0;
        private Thread thread;

        public string msg = "";
        public PPDevice ppDevice;
        public XmlManager xml;
        public Input input;
        public Camera camera;
        public ResManager res;
        //public MapManager map;
        public Window_List windowList;
        //public Config config;
        //public ShopManager shop;
        //public Sound sound;

        public FormGame(Form1 FormParent)
        {
            InitializeComponent();
            this.FormParent = FormParent;
            this.ClientSize = new Size(800, 600);
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void FormGame_Load(object sender, EventArgs e)
        {
            try
            {
                ppDevice = new PPDevice(this,this.Draw);
                Global.SetPPDevice(ppDevice);
                xml = Global.GetXmlManager();
                xml.Load();
                input = Global.GetInput();
                camera = Global.GetCamara();
                res = Global.GetResManager();
                res.LoadItemPic();
                windowList = Global.GetWindowsList();
                //Global.Getconfig = new Config();
                //Global.Getshop = new ShopManager();
                //Global.Getsound = new Sound(this);

                windowList.Load();
                windowList.CloseAll();
                windowList.ActiveWindow(19);
                Global.GetRoom().AddPlayer(Global.GetPlayer());
                sw.Start();

                thread = new Thread(new ThreadStart(Action));
                thread.Start();
            }
            catch (Exception ex)
            {
                 FormParent.AddTextGame(ex.ToString());
            }
        }

        public void Action()
        {
            try
            {
                while (true)
                {
                    if (this.IsDisposed) break;
                    input.UpdateMousePoint(this.PointToClient(MousePosition));
                    input.UpdateMouseBottons(MouseButtons);

                    ppDevice.RenderAll();
                    camera.Update();
                    windowList.Action();
                    windowList.Draw(ppDevice);

                    input.UpdateKey();
                    Other();
                }
            }
            catch (Exception ex)
            {
                FormParent.AddTextGame(ex.ToString());
            }
        }

        private void Draw()
        {
            //ppDevice.BeginDraw();
            //if (map != null) map.Draw(ppDevice);
            windowList.Draw(ppDevice);
            //ppDevice.EndDraw();
        }

        private void Other()
        {
            fps++;
            long t = sw.ElapsedMilliseconds;
            if (t - t0 > 1000)
            {
                t0 += 1000;
                FormParent.ShowText7("FPS: " + fps);
                fps = 0;
            }

            FormParent.ShowText8("X: " + input.x);
            FormParent.ShowText9("X: " + input.y);

            FormParent.ShowText11("Eve: " + Window_List.eve);
            if (Window_List.mouseIndex >= 0)
            {
                FormParent.ShowText10("Win: " + windowList.windows[Window_List.mouseIndex].typeID);
            }
            else
            {
                FormParent.ShowText10("Win: " + "null");
            }

            /*
            if (data.input.GetFlyDown() > 0 && data.input.GetFlyUp() > 0 && data.input.GetNum4() == 2)
            {
                IsDebug = IsDebug ? false : true;
            }

            textBox1.Text = "Key: " + data.input.type;
            textBox2.Text = "Time: " + data.input.time;
            textBox3.Text = "X: " + data.input.x;
            textBox4.Text = "Y: " + data.input.y;

            fps++;
            long t = sw.ElapsedMilliseconds;
            if (t - t0 > 1000)
            {
                t0 += 1000;
                textBox5.Text = "FPS: " + fps;
                fps = 0;
            }
            textBox6.Text = "Eve: " + Window_List.eve;
            if (Window_List.mouseIndex >= 0)
            {
                textBox7.Text = "Win: " + data.windowList.windows[Window_List.mouseIndex].typeID;
            }
            else
            {
                textBox7.Text = "Win: " + "null";
            }


            if (data.map != null && data.map.pla != null)
            {
                //var player = data.map.pla.list[data.map.camera_type-1];
                //textBox8.Text = "X=" + player.x + "Y=" + player.y + "vx=" + player.vx + "vy=" + player.vy;
            }

            textBox9.Text = data.msg;
            textBox10.Text = "Cx : " + data.camera.x;
            textBox11.Text = "Cy : " + data.camera.y;

            var input = data.input;
            textBox13.Text = "↑ : " + input.GetBiSha();
            textBox14.Text = "↓ : " + input.GetFlyDown();
            textBox15.Text = "← : " + input.GetFlyLeft();
            textBox16.Text = "→ : " + input.GetFlyRight();
            textBox17.Text = "   : " + input.GetFlyUp();
            textBox18.Text = " 1 : " + input.GetNum1();
            textBox19.Text = " 2 : " + input.GetNum2();
            textBox20.Text = " 3 : " + input.GetNum3();
            textBox21.Text = " 4 : " + input.GetNum4();
            */
        }

        private void FormGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.DelRes();
            FormParent.formGame = null;
        }
    }
}
