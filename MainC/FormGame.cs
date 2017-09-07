using ClientPublic;
using Data.Cameras;
using Data.Globals;
using Data.Inputs;
using Data.MapsManager;
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
        private static int IsReadyClose = 0; 
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
            {
                if (Global.GetSoundManager() != null)
                {
                    Global.GetSoundManager().DelRes();
                }
                var s = new Data.Sounds.Sound(this);
                Global.SetSoundManager(s);

                ppDevice = new PPDevice(this, this.Draw);
                Global.SetPPDevice(ppDevice);

                xml = Global.GetXmlManager();
                xml.Load();
                input = Global.GetInput();
                camera = Global.GetCamara();
                res = Global.GetResManager();
                res.LoadItemPic();
                res.LoadPic();
                windowList = Global.GetWindowsList();
                //Global.Getconfig = new Config();
                //Global.Getshop = new ShopManager();


                windowList.Load();
                windowList.CloseAll();
                windowList.ActiveWindow(19);
                Global.GetRoom().AddPlayer(Global.GetPlayer());
                Global.GetRoom().clientC = Global.GetClientC();
                sw.Start();

                thread = new Thread(new ThreadStart(Action));
                thread.Start();
                Global.IsFormGameOpen = true;
            }
        }
        public void Action()
        {
            try
            {
                while (IsReadyClose == 0)
                {
                    if (this.IsDisposed) break;

                    ppDevice.RenderAll();

                    input.UpdateMousePoint(this.PointToClient(MousePosition));
                    input.UpdateMouseBottons(MouseButtons);

                    if (input.GetFlyUp() > 0 && input.GetBiSha() > 0 && input.GetNum4() == 1)
                    {
                        //↑ + space + 4  锁定/解锁窗口
                        Global.IsCameraFree = !Global.IsCameraFree;
                    }
                    if (input.GetFlyUp() > 0 && input.GetBiSha() > 0 && input.GetNum3() == 1)
                    {
                        //↑ + space + 3  开启/关闭调试模式
                        Global.IsDebug = !Global.IsDebug;
                    }

                    if (Global.GetRoom().UpdateInput()==0)
                    {
                        if (Global.GetMapManager() != null)
                            Global.GetMapManager().Update();
                        camera.Update();
                    }

                    windowList.Action();
                    input.UpdateKey();
                    {
                        var clientC = Global.GetClientC();
                        if(clientC != null && clientC.IsConnect())
                        {
                            clientC.SendAll();
                            clientC.UpdateTime();
                            if (clientC.IsConnect() == false)
                            {
                                Global.GetRoom().Reset();
                            }
                            else
                            {
                                Global.GetRoom().DealSendData();
                            }
                        }
                    }

                    Other();
                }
            }
            catch (Exception ex)
            {
                FormParent.AddTextGame(ex.ToString());
                MessageBox.Show(ex.ToString());
                this.Close();
            }
        }

        private void Draw()
        {
            var map = Global.GetMapManager();
            if (map != null)
            {
                map.Draw(ppDevice);
            }
            windowList.Draw(ppDevice);
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
            FormParent.ShowText9("Y: " + input.y);

            FormParent.ShowText11("Eve: " + Window_List.eve);
            if (Window_List.mouseIndex >= 0)
            {
                FormParent.ShowText10("Win: " + windowList.windows[Window_List.mouseIndex].typeID);
            }
            else
            {
                FormParent.ShowText10("Win: " + "null");
            }

            if(Global.GetClientC() != null)
            {
                int delay = Global.GetClientC().GetDelay();
                FormParent.ShowText12("延时" + " : " + delay + " ms");
            }

            var clientC = Global.GetClientC();
            if(clientC!=null)
            {
                FormParent.ShowText13("发送" + " : " + clientC.SendNum);
                clientC.SendNum = 0;
                FormParent.ShowText14("接收" + " : " + clientC.RecvNum);
                clientC.RecvNum = 0;
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
            Global.GetReplayManager().End();
            XmlPlayer.WritePlayer(GlobalB.GetRootPath()+@"\Setting\", Global.GetPlayer());
            FormParent.AddTextGame("玩家数据保存成功");
            Global.GetSoundManager().DelRes();
            Global.SetSoundManager(null);
            Global.DelRes();
            FormParent.formGame = null;
            Global.IsFormGameOpen = false;
        }

        private void FormGame_KeyDown(object sender, KeyEventArgs e)
        {            
            input.UpdateKeyDown((byte)e.KeyCode);
            input.keyNum++;
        }
        private void FormGame_KeyUp(object sender, KeyEventArgs e)
        {
            input.UpdateKeyUP((byte)e.KeyCode);
        }

        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
                Thread.Sleep(100);
        }
    }
}
