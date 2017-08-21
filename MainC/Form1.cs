using Data.Globals;
using Data.XML;
using Room;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace MainC
{
    public partial class Form1 : Form
    {
        List<PlayerData> players = new List<PlayerData>();
        public FormGame formGame;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var xmlManager = Global.GetXmlManager();
            xmlManager.Load();
            Form1_Load_Player();
            Form1_Load_Config();
            if(Global.GetClientC() == null)
            {
                Global.SetClientC(new ClientPublic.ClientC());
            }
        }
        private void Form1_Load_Player()
        {
            var rootPath = GlobalB.GetRootPath() + @"\Setting\PlayerData";
            if (Directory.Exists(rootPath) == false) return;
            players.Clear();

            DirectoryInfo dirs = new DirectoryInfo(rootPath);
            foreach (var dir in dirs.GetDirectories())
            {
                var player = new PlayerData();
                bool poi = Data.XML.XmlPlayer.ReadPlayer(dir.FullName, player);
                if(poi == true)
                {
                    players.Add(player);
                    comboBox1.Items.Add(player.GetQQ() + " - " + player.GetName());
                }
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }
        private void Form1_Load_Config()
        {
            //加载窗口设置
            string path = GlobalB.GetRootPath() + @"\Setting\config.xml";
            if (File.Exists(path) == false) return;

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            foreach (XElement item1 in root.Elements())
            {
                var name = item1.Name.LocalName;
                if (name == "选择玩家")
                {
                    int t = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                    if (t >= 0 && t < players.Count)
                        comboBox1.SelectedIndex = t;
                }
                else if (name == "音乐音量")
                {
                    int t = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                    if (t >= 0 && t <= 100)
                        trackBar1.Value = t;
                }
                else if (name == "音效音量")
                {
                    int t = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                    if (t >= 0 && t <= 100)
                        trackBar2.Value = t;
                }
                else if (name == "窗口解锁")
                {
                    string t = XmlManager.Try_Get_Attribute_Value_Str(item1.Attribute("Value"));
                    checkBox2.Checked = (t == "是");
                    Global.IsCameraFree = checkBox2.Checked;
                }
                else if (name == "调试模式")
                {
                    string t = XmlManager.Try_Get_Attribute_Value_Str(item1.Attribute("Value"));
                    checkBox1.Checked = (t == "是");
                    Global.IsDebug = checkBox1.Checked;
                }
                else if (name == "IP地址")
                {
                    string t = XmlManager.Try_Get_Attribute_Value_Str(item1.Attribute("Value"));
                    textBox3.Text = t;
                }
                else if (name == "端口")
                {
                    string t = XmlManager.Try_Get_Attribute_Value_Str(item1.Attribute("Value"));
                    textBox4.Text = t;
                }
            }
        }
        private void Form1_Save_Config()
        {
            //保存窗口设置
            string path = GlobalB.GetRootPath() + @"\Setting";
            if(Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            path = path + @"\config.xml";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string[,] str = new string[,]
            {
                { "选择玩家",""+comboBox1.SelectedIndex },
                { "音乐音量",""+trackBar1.Value },
                { "音效音量",""+trackBar2.Value },
                { "窗口解锁",checkBox2.Checked?"是":"否" },
                { "调试模式",checkBox1.Checked?"是":"否" },
                { "IP地址",textBox3.Text },
                { "端口",textBox4.Text },
            };
            XmlDocument xmlDoc = new XmlDocument();
            //创建Xml声明部分，即<?xml version="1.0" encoding="utf-8" ?>
            XmlDeclaration Declaration = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);

            //创建根节点
            XmlNode rootNode = xmlDoc.CreateElement("Root");
            for (int i = 0; i < str.Length / 2; i++)
            {
                //创建student子节点
                XmlNode childNode = xmlDoc.CreateElement(str[i, 0]);

                //创建一个属性
                XmlAttribute value = xmlDoc.CreateAttribute("Value");
                value.Value = str[i, 1];

                //xml节点附件属性
                childNode.Attributes.Append(value);

                //附加子节点到根节点
                rootNode.AppendChild(childNode);
            }

            //附加根节点
            xmlDoc.AppendChild(rootNode);
            xmlDoc.InsertBefore(Declaration, xmlDoc.DocumentElement);

            //保存Xml文档
            xmlDoc.Save(path);

        }

        private void Save()
        {
            //保存所有数据
            SavePlayer();
            Form1_Save_Config();
        }
        private void SavePlayer()
        {
            //保存玩家数据
            string path = GlobalB.GetRootPath() + @"\Setting\";
            foreach (var player in players)
            {
                Data.XML.XmlPlayer.WritePlayer(path, player);
            }
        }            

        private void button1_Click(object sender, EventArgs e)
        {
            var formCreatePlayer = new FormCreatePlayer();
            formCreatePlayer.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(comboBox1.Items.Count == 0)
            {
                AddTextLine(textBox6, "当前没有玩家数据，需要创建一个玩家");
                return;
            }
            if(comboBox1.SelectedIndex < 0 || comboBox1.SelectedIndex >= players.Count)
            {
                comboBox1.SelectedIndex = 0;
                AddTextLine(textBox6, "Error : 超出引索，按第一个玩家启动");
            }

            //复制数据
            var player = Global.GetPlayer();
            player.Clone(players[comboBox1.SelectedIndex]);
            AddTextLine(textBox6, "玩家加载成功");

            if ((formGame == null || formGame.IsDisposed) && Global.IsFormGameOpen == false)
            {
                AddTextLine(textBox6, "飘飘正在启动，可能需要几十秒的时间");
                formGame = new FormGame(this);
                formGame.Show();
                if (Global.GetSoundManager() != null)
                {
                    Global.GetSoundManager().SetVolume(trackBar1.Value);
                    Global.GetSoundManager().SetEffVol(trackBar2.Value);
                }
                AddTextLine(textBox6, "飘飘启动成功");
            }
            else
            {
                AddTextLine(textBox6, "飘飘已启动");
            }
        }
        private void AddTextLine(TextBox tx,string str)
        {
            tx.AppendText(str + System.Environment.NewLine);
        }
        public void AddTextGame(string str)
        {
            AddTextLine(textBox5, str);
        }
        public void ShowText7(string str)
        {
            textBox7.Text = str;
        }
        public void ShowText8(string str)
        {
            textBox8.Text = str;
        }
        public void ShowText9(string str)
        {
            textBox9.Text = str;
        }
        public void ShowText10(string str)
        {
            textBox10.Text = str;
        }
        public void ShowText11(string str)
        {
            textBox11.Text = str;
        }
        public void ShowText12(string str)
        {
            textBox12.Text = str;
        }
        public void ShowText13(string str)
        {
            textBox13.Text = str;
        }
        public void ShowText14(string str)
        {
            textBox14.Text = str;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.IsFormGameOpen)
                {
                    string port = textBox4.Text;
                    string ip = textBox3.Text;
                    int ret = 0;
                    byte[] ipbyte = new byte[4];

                    var strs = ip.Split('.');
                    if (strs.Length != 4)
                    {
                        AddTextLine(textBox6, "IP地址格式有误");
                        return;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        bool checks = byte.TryParse(strs[i],out ipbyte[i]);
                        if(checks == false)
                        {
                            AddTextLine(textBox6, "IP地址格式有误");
                            return;
                        }
                    }

                    bool check =  int.TryParse(port, out ret);
                    if(check == false || ret>65535)
                    {
                        AddTextLine(textBox6, "端口格式有误");
                        return;
                    }

                    var clientC = Global.GetClientC();
                    if (clientC == null || true)
                    {
                        clientC = new ClientPublic.ClientC(Global.GetPlayer().GetQQ(), Global.GetPlayer().GetExID());
                        Global.SetClientC(clientC);
                    }
                    if (clientC.IsConnect() == false)
                    {
                        {
                            clientC.IP = new System.Net.IPAddress(ipbyte);
                            clientC.Port = ret;
                        }

                        clientC.OpenClient();
                        clientC.AddData(Global.GetPlayer().GetSendData_All());
                        AddTextLine(textBox6, "尝试连接到 " + clientC.IP.ToString() + ":" + clientC.Port);
                    }
                    else
                    {
                        AddTextLine(textBox6, "连接已经打开");
                    }
                }
                else
                {
                    AddTextLine(textBox6, "飘飘尚未启动");
                }
            }
            catch (Exception ex)
            {
                AddTextLine(textBox6, ex.ToString());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Global.IsFormGameOpen)
            {
                formGame.Close();
                e.Cancel = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool c = checkBox1.Checked;
            Global.IsDebug = c;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            var clientC = Global.GetClientC();
            if(clientC == null || clientC.IsConnect() == false)
            {
                AddTextLine(textBox5, "Error : 还没有连接到服务端");
                return;
            }
            if(Global.GetMapManager() != null)
            {
                AddTextLine(textBox5, "Error : 游戏已经开始，不能修改延时");
                return;
            }

            int delay = 5;
            bool check = int.TryParse(textBox15.Text,out delay);
            if (check == false || delay > 30 || delay < 1)
            {
                AddTextLine(textBox5, "Error : 延迟的取值范围是1-30帧");
                return;
            }

            var dat = new ClientPublic.ClientData();
            dat.CreateChangeDelay(delay);
            clientC.AddData(dat);

            AddTextLine(textBox5, "延迟修改信息已发送");
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (Global.GetSoundManager() != null)
            {
                Global.GetSoundManager().SetVolume(trackBar1.Value);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Global.IsCameraFree = checkBox2.Checked;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            if (Global.GetSoundManager() != null)
            {
                Global.GetSoundManager().SetEffVol(trackBar2.Value);
            }
        }
    }
}
