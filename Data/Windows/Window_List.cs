using ClientPublic;
using Data.Globals;
using Data.Inputs;
using Data.PPDevices;
using Data.Resources;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace Data.Windows
{
    enum SpriteType : int
    {
        Window = 1,
        UI_Button,
        UI_CheckBox,
        UI_StaticMap,
        UI_Edit,
        UI_Radio,
        UI_Chat,
        UI_Listbox,
        String,
        WinAdd,
    }
    public class Window_List
    {
        public static string rootPath;
        public static int eve = 0;
        public static int mouseIndex = -1;
        public static string msg = "";
        public static int windows_ID = 0;
        public static Input Input = null;

        public List<int> showNum = new List<int>();
        public List<Window> windows = new List<Window>();

        public Window_List()
        {
        }
        public void Load()
        {
            if(showNum==null) showNum = new List<int>();
            if(windows==null) windows = new List<Window>();

            Input = Global.GetInput();
            SpriteBase.Input = Input;
            var Data_rootPath = GlobalB.GetRootPath() + @"\";
            string[] paths = new string[2]
            {
                @"balloon\window.txt",
                @"balloon\window_poi.txt"
            };
            string[] roots = new string[2]
            {
                @"balloon\Wnd\",
                @""
            };
            for (int path_index = 0; path_index < paths.Length; path_index++)
            {
                string path = Data_rootPath + paths[path_index]; //窗口文件路径
                rootPath = Data_rootPath + roots[path_index]; //对应资源文件路径

                if (!File.Exists(path))
                {
                    //MessageBox.Show("Load Windows \n " + path, "ERROR");
                    return;
                }

                StreamReader sr = new StreamReader(path, Encoding.Default);
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] p = Cut_string(line);
                    if (p[0].Length < 1) continue;
                    char poi = p[0][0];
                    if (poi == '/') continue;
                    if (p[0] == "object")
                    {
                        int id = int.Parse(p[2]);
                        Load_Window(sr, p[1], id);
                    }
                }
            }
        }
        public void Draw(PPDevice ppDevice)
        {
            var Camera = Global.GetCamara();
            int len = showNum.Count;
            for (int i = 0; i < len; i++)
            {
                var win = windows[showNum[i]];
                if (win.enabled == 1)
                {
                    if (true || Physics.Rect_Rect(win.GetPicAABB(), Camera.GetArea()))
                    {
                        win.Draw(ppDevice);
                    }
                }
            }
        }
        private string[] Cut_string(string line)
        {
            line = line.Replace(",", " ");
            line = line.Replace("\t", "");
            line = line.Replace("\n", "");
            line = line.Replace("=", " ");
            line = line.Replace("  ", " ");
            line = line.Replace("  ", " ");
            return line.Split(' ');
        }
        private void Load_Window(StreamReader sr, string name, int id)
        {
            bool IsRead = true;
            int Num = 0;
            if (name == "CursorType") IsRead = false;

            Window win = new Window();
            win.thisID = windows_ID;
            windows_ID++;
            win.typeID = id;
            win.index = Window.Index;
            Window.Index++;
            showNum.Add(win.index);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '{')
                {
                    Num++;
                    continue;
                }
                if (poi == '}')
                {
                    Num--;
                    if (Num == 0) break;
                    continue;
                }
                if (IsRead == false) continue;
                if (p[0] == "object")
                {
                    name = p[1];
                    id = int.Parse(p[2]);
                    Load_UI(sr, win, name, id, IsRead);
                }
                else if (p[0] == "type")
                {
                    win.type = (int)SpriteType.Window;
                }
                else if (p[0] == "rect")
                {
                    win.x = int.Parse(p[1]);
                    win.y = int.Parse(p[2]);
                    win.width = int.Parse(p[3]);
                    win.height = int.Parse(p[4]);
                }
                else if (p[0] == "src")
                {
                    win.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    win.alpha = alpha / 255.0f;
                    if (win.alpha < 0.0f) win.alpha = 0;
                    if (win.alpha > 1.0f) win.alpha = 1.0f;
                }
                else if (p[0] == "visible")
                {
                    win.visible = int.Parse(p[1]);
                }
                else if (p[0] == "modal")
                {
                    win.modal = int.Parse(p[1]);
                }
                else if (p[0] == "mask")
                {
                    win.mask = int.Parse(p[1]);
                }
                else if (p[0] == "movable")
                {
                    win.visible = int.Parse(p[1]);
                }
                else if (p[0] == "enabled")
                {
                    win.enabled = int.Parse(p[1]);
                }
                else if (p[0] == "topmast")
                {
                    win.topmost = int.Parse(p[1]);
                }
                else if (p[0] == "fontname")
                {
                    win.fontname = (p[1]);
                }
                else if (p[0] == "fontsize")
                {
                    win.fontsize = float.Parse(p[1]);
                }
                else if (p[0] == "topmost")
                {
                    win.topmost = int.Parse(p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load Window Failed \n name = " + name + " \n key = " + p[0]);
                    continue;
                }
            }
            windows.Add(win);
        }
        private void Load_UI(StreamReader sr, Window win, string name, int id, bool IsRead)
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "type")
                {
                    if (p[1] == "Button" || p[1] == "button")
                    {
                        Load_UI_Button(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "ButtonLine" || p[1] == "buttonLine")
                    {
                        Load_UI_ButtonLine(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "CheckBoxLine" || p[1] == "checkBoxLine")
                    {
                        Load_UI_CheckBoxLine(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "CheckBoxLine_Channel" || p[1] == "checkBoxLine_Channel")
                    {
                        Load_UI_CheckBoxLine_Channel(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "CheckBox2" || p[1] == "checkBox2")
                    {
                        Load_UI_CheckBox2(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "CheckBoxShop")
                    {
                        Load_UI_CheckBoxShop(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "staticbitmap")
                    {
                        Load_UI_StaticBitmap(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "edit")
                    {
                        Load_UI_Edit(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "radio")
                    {
                        Load_UI_Radio(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "chat")
                    {
                        Load_UI_Chat(sr, win, name, id);
                        return;
                    }
                    else if (p[1] == "checkbox")
                    {
                        Load_UI_CheckBox(sr, win, name, id);
                        return;
                    }
                }
            }
        }
        private void Load_UI_Button(StreamReader sr, Window win, string name, int id)
        {
            UI_Button ui = new UI_Button();
            ui.type = (int)SpriteType.UI_Button;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "upimg")
                {
                    ui.upimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "downimg")
                {
                    ui.downimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "Hoverimg" || p[0] == "hoverimg")
                {
                    ui.hoverimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "disableimg")
                {
                    ui.disableimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load Button Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_ButtonLine(StreamReader sr, Window win, string name, int id)
        {
            UI_ButtonLine ui = new UI_ButtonLine();
            ui.type = (int)SpriteType.UI_Button;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "frame")
                {
                    ui.frame = int.Parse(p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load Button Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_CheckBoxLine(StreamReader sr, Window win, string name, int id)
        {
            UI_CheckBoxLine ui = new UI_CheckBoxLine();
            ui.type = (int)SpriteType.UI_CheckBox;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "frame")
                {
                    ui.frame = int.Parse(p[1]);
                }
                else if (p[0] == "index")
                {
                    ui.index = int.Parse(p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load Button Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_StaticBitmap(StreamReader sr, Window win, string name, int id)
        {
            UI_StaticMap ui = new UI_StaticMap();
            ui.type = (int)SpriteType.UI_StaticMap;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "movable")
                {
                }
                else
                {
                    //Poi.MessageBox("Load Button Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_Edit(StreamReader sr, Window win, string name, int id)
        {
            UI_Edit ui = new UI_Edit();
            ui.type = (int)SpriteType.UI_Edit;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "border")
                {
                    //border - 边界
                    ui.border = new Point(int.Parse(p[1]), int.Parse(p[2]));
                }
                else if (p[0] == "maxlen")
                {
                    ui.maxlen = int.Parse(p[1]);
                }
                else if (p[0] == "password")
                {
                    ui.password = p[1];
                }
                else
                {
                    //Poi.MessageBox("Load Edit Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_Radio(StreamReader sr, Window win, string name, int id)
        {
            UI_Radio ui = new UI_Radio();
            ui.type = (int)SpriteType.UI_Radio;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "stateimg")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "forecolor")
                {
                    ui.forecolor = new Color(int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]));
                }
                else
                {
                    //Poi.MessageBox("Load Radio Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_Chat(StreamReader sr, Window win, string name, int id)
        {
            UI_Chat ui = new UI_Chat();
            ui.type = (int)SpriteType.UI_Chat;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "margin")
                {
                    ui.margin = int.Parse(p[1]);
                }
                else if (p[0] == "linespace")
                {
                    ui.linespace = int.Parse(p[1]);
                }
                else if (p[0] == "autoscroll")
                {
                    ui.autoscroll = int.Parse(p[1]);
                }
                else if (p[0] == "maxlines")
                {
                    ui.maxlines = int.Parse(p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load Chat Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_Listbox(StreamReader sr, Window win, string name, int id)
        {
            UI_Listbox ui = new UI_Listbox();
            ui.type = (int)SpriteType.UI_Listbox;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "scrollimg")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "forecolor")
                {
                    ui.forecolor = new Color(int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]));
                }
                else if (p[0] == "context")
                {
                    ui.context = p[1];
                }
                else
                {
                    //Poi.MessageBox("Load Chat Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_CheckBox(StreamReader sr, Window win, string name, int id)
        {
            UI_CheckBox ui = new UI_CheckBox();
            ui.type = (int)SpriteType.UI_Listbox;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "stateimg")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "forecolor")
                {
                    ui.forecolor = new Color(int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]), 1);
                }
                else
                {
                    //Poi.MessageBox("Load CheckBox Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_CheckBox2(StreamReader sr, Window win, string name, int id)
        {
            UI_CheckBox2 ui = new UI_CheckBox2();
            ui.type = (int)SpriteType.UI_CheckBox;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "upimg")
                {
                    ui.upimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "downimg")
                {
                    ui.downimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load CheckBox2 Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_CheckBoxLine_Channel(StreamReader sr, Window win, string name, int id)
        {
            UI_CheckBoxLine_Channel ui = new UI_CheckBoxLine_Channel();
            ui.type = (int)SpriteType.UI_CheckBox;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "frame")
                {
                    ui.frame = int.Parse(p[1]);
                }
                else if (p[0] == "index")
                {
                    ui.index = int.Parse(p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load Button Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }
        private void Load_UI_CheckBoxShop(StreamReader sr, Window win, string name, int id)
        {
            UI_CheckBoxShop ui = new UI_CheckBoxShop();
            ui.type = (int)SpriteType.UI_CheckBox;
            ui.typeID = id;
            ui.thisID = windows_ID;
            windows_ID++;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] p = Cut_string(line);
                if (p.Length < 1) continue;
                if (p[0].Length < 1) continue;
                char poi = p[0][0];
                if (poi == '/') continue;
                if (poi == '}') break;
                if (p[0] == "src")
                {
                    ui.pic = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "rect")
                {
                    ui.x = int.Parse(p[1]);
                    ui.y = int.Parse(p[2]);
                    ui.width = int.Parse(p[3]);
                    ui.height = int.Parse(p[4]);
                }
                else if (p[0] == "pic_rect")
                {
                    ui.pic_Rect.X = int.Parse(p[1]);
                    ui.pic_Rect.Y = int.Parse(p[2]);
                    ui.pic_Rect.Width = int.Parse(p[3]);
                    ui.pic_Rect.Height = int.Parse(p[4]);
                }
                else if (p[0] == "alpha")
                {
                    int alpha = int.Parse(p[1]);
                    ui.alpha = alpha / 255.0f;
                    if (ui.alpha < 0.0f) ui.alpha = 0;
                    if (ui.alpha > 1.0f) ui.alpha = 1;
                }
                else if (p[0] == "upimg")
                {
                    ui.upimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "downimg")
                {
                    ui.downimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "Hoverimg" || p[0] == "hoverimg")
                {
                    ui.hoverimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "disableimg")
                {
                    ui.disableimg = SpriteBase.Load_Bitmap_FromFile(rootPath + p[1]);
                }
                else if (p[0] == "val_A")
                {
                    ui.val_A = int.Parse(p[1]);
                }
                else
                {
                    //Poi.MessageBox("Load CheckBox2 Failed \n name = " + name + " \n key = " + p[0]);
                }
            }
            win.ui.Add(ui);
        }

        public void InitLogin()
        {
            CloseAll();
            ActiveWindow(1000);
            //Ser_Data.CloseAll();
        }
        public void InitServer()
        {
            CloseAll();
            ActiveWindow(1001);
        }
        public void InitRoom()
        {
            CloseAll();
            ActiveWindow(1002);
        }
        public void InitRoomIn()
        {
            CloseAll();
            ActiveWindow(19);
        }
        public void CloseAll()
        {
            foreach (var win in windows)
            {
                win.enabled = 0;
                win.visible = 0;
            }
        }
        public void Action()
        {
            DoEvents();
            int index = GetMouseIndex();
            mouseIndex = index;
            foreach (var win in windows)
            {
                win.Action();
            }
            if (index >= 0)
            {
                windows[index].ui_Action();
                if (Global.GetInput().IsLeftDown())
                {
                    windows[index].ToTop();
                }
            }
        }
        public void ActiveWindow(int typeID)
        {
            int len = windows.Count();
            for (int i = 0; i < len; i++)
            {
                var win = windows[i];
                if (win.typeID == typeID)
                {
                    win.Active();
                    return;
                }
            }
        }
        public void ActiveWindow_IF_enabled(int typeID)
        {
            int len = windows.Count();
            for (int i = 0; i < len; i++)
            {
                var win = windows[i];
                if (win.typeID == typeID && win.enabled == 0)
                {
                    win.Active();
                    return;
                }
            }
        }

        //响应按钮
        public void DoEvents()
        {
            if (eve == 191)
            { //打开地图列表
                Room_Open_Map_Eve();
            }
            else if (195 <= eve && eve <= 198)
            { //选择队伍
                Room_SelectTeam_Eve();
            }
            else if (eve == 199)
            { //开始游戏按钮
                Ready_Go_Eve();
            }
            else if (eve == 201)
            { //选择地图确认按钮
                Room_SelectMap_Yes_Eve();
            }
            else if (eve == 202)
            { //选择地图取消按钮
                Room_SelectMap_ToBottom_Eve();
            }
            else if (eve == 203)
            { //选择地图上按钮
                Room_SelectMap_PageUp_Eve();
            }
            else if (eve == 204)
            { //选择地图下按钮
                Room_SelectMap_PageDown_Eve();
            }
            else if (eve == 205)
            { //选择地图面板区域
                Room_SelectMap_Select_Eve();
            }
            else if (eve == 223)
            {
            }
            else if (eve == 224)
            {
            }
            else if (eve == 225)
            {
                Room_SetFree_Eve();
            }
            else if (eve == 226)
            {
                Room_SetSingle_Eve();
            }
            else if (eve == 227)
            {
                Room_Set_Yes_Eve();
            }
            else if (eve == 228)
            {
                Room_Set_Quit_Eve();
            }
            else if (eve == 1911)
            {
                Into_Shop_Eve();
            }
            else if (eve == 100012)
            {
                Login_CreateServer_Eve();
            }
            else if (eve == 100161)
            {
                Server_Out_Login_Eve();
            }
            else if (100112 <= eve && eve <= 100131)
            {
                Select_Server_Eve();
            }
            else if (eve == 100166)
            {
                GetInRoom_Eve();
            }
            else if (eve == 100167)
            {
                GetQuickInRoom_Eve();
            }
            else if (eve == 100203)
            {
                Server_Out_Server_Eve();
            }
            else if (eve == 100207)
            {
                CreateRoom_Eve();
            }
            else if (eve == 110007)
            {
                Out_Shop_Eve();
            }
            else if (eve == 110001)
            {
                Shop_ShangDian_Page_Up_Eve();
            }
            else if (eve == 110002)
            {
                Shop_ShangDian_Page_Down_Eve();
            }
            else if (eve == 110006)
            {
                Shop_ShangDian_ChongZhi_Eve();
            }
            else if (110010 <= eve && eve <= 110014)
            {
                Shop_ShangDian_CheckBoxShop_Update_Eve();
            }
            else if (111009 <= eve && eve <= 111013)
            {
                Shop_ChangKu_CheckBoxShop_Update_Eve();
            }
            else if (112000 <= eve && eve <= 112006)
            {
                Shop_JueSe_CheckBoxShop_Update_Eve();
            }
            eve = 0;
        }
        private void Login_CreateServer_Eve()
        {
            /*
            if (Input.IsLeftUp())
            {
                var ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
                client = new ServerClient(ip);
                try
                {
                    Ser_Data.CreateServers();
                    InitServer();
                    wait_server = 1;
                    client.SendMessage("100012" + "0000" + "0000");
                    while (wait_server > 0)
                    {

                    }
                }
                catch (Exception ex)
                {
                    Window_List.msg = (ip + ":8500 连接失败 ");
                    return;
                }
            }
            */
            eve = 0;
        }
        private void Server_Out_Login_Eve()
        {
            if (Input.IsLeftUp())
            {
                InitLogin();
            }
            eve = 0;
        }
        private void Server_Out_Server_Eve()
        {
            if (Input.IsLeftUp())
            {
                InitServer();
            }
            eve = 0;
        }
        private void Select_Server_Eve()
        {
            if (Input.IsLeftDown())
            {
                int index = GetIndex(1001);
                var win = windows[index];
                index = win.GetIndex(100112);
                for (int i = 0; i < 20; i++)
                {
                    win.ui[i + index].Reset();
                }
                int index2 = win.GetIndex(eve);
                win.ui[index2].Active();
                //Config.server = index2 - index;
            }
        }
        private void GetInRoom_Eve()
        {
            if (Input.IsLeftUp())//&& Config.server < Ser_Data.servers.Count)
            {
                InitRoom();
            }
            eve = 0;
        }
        private void GetQuickInRoom_Eve()
        {
            if (Input.IsLeftUp())
            {
                /*
                Config.server = 0;
                if (Config.server < Ser_Data.servers.Count)
                {
                    InitRoom();
                }
                */
                InitRoom();
            }
            eve = 0;
        }
        private void CreateRoom_Eve()
        {
            if (Input.IsLeftUp())
            {
                ActiveWindow(22);
            }
        }
        private void Room_SetFree_Eve()
        {
            if (Input.IsLeftDown())
            {
                var win = windows[GetIndex(22)];
                var ui = win.ui[win.GetIndex(226)];
                ui.soutai = 0;
            }
        }
        private void Room_SetSingle_Eve()
        {
            if (Input.IsLeftDown())
            {
                var win = windows[GetIndex(22)];
                var ui = win.ui[win.GetIndex(225)];
                ui.soutai = 0;
            }
        }
        private void Room_Set_Quit_Eve()
        {
            if (Input.IsLeftUp())
            {
                var win = windows[GetIndex(22)];
                win.ToBottom();
            }
        }
        private void Room_Set_Yes_Eve()
        {
            if (Input.IsLeftUp())
            {
                var win = windows[GetIndex(22)];

                UI ui = win.ui[win.GetIndex(223)];
                bool Islock = (ui.soutai == 0) ? false : true;
                ui = win.ui[win.GetIndex(224)];
                bool IsWS = (ui.soutai == 1) ? true : false;
                ui = win.ui[win.GetIndex(225)];
                bool IsFree = (ui.soutai == 1) ? true : false;

                win.ToBottom();
            }
        }
        private void Room_Open_Map_Eve()
        {
            if (Input.IsLeftUp())
            {
                ActiveWindow_IF_enabled(20);
            }
        }
        private void Room_SelectMap_Yes_Eve()
        {
            if (Input.IsLeftUp())
            {
                var win = windows[GetIndex(20)];
                int index = (int)win.addBefore[0].poi_A; //unsafe
                win.ToBottom();
                if (index <= 100) return;

                if (Global.IsConnect())
                {
                    ClientData dat = new ClientData();
                    dat.CreateMapChange(index);
                    Global.GetClientC().AddData(dat);
                }
                else
                {
                    Global.GetRoom().SetMapIndex(index);
                }
            }
        }
        private void Room_SelectMap_ToBottom_Eve()
        {
            if (Input.IsLeftUp())
            {
                var win = windows[GetIndex(20)];
                win.ToBottom();
            }
        }
        private void Room_SelectMap_PageUp_Eve()
        {
            //上翻页
            if (Input.IsLeftUp())
            {
                var win = windows[GetIndex(20)];
                win.addBefore[0].val_A -= 16; //unsafe
                win.addBefore[0].Action();
            }
        }
        private void Room_SelectMap_PageDown_Eve()
        {
            //下翻页
            if (Input.IsLeftUp())
            {
                var win = windows[GetIndex(20)];
                win.addBefore[0].val_A += 16; //unsafe
                win.addBefore[0].Action();
            }
        }
        private void Room_SelectMap_Select_Eve()
        {
            if (Input.IsLeftDown())
            {
                var win = windows[GetIndex(20)];
                win.addBefore[0].Update(); //unsafe
            }
        }
        private void Room_SelectTeam_Eve()
        {
            if (Input.IsLeftUp())
            {
                if (Global.IsConnect())
                {
                    ClientData dat = new ClientData();
                    dat.CreateTeamChange(eve - 194);
                    Global.GetClientC().AddData(dat);
                }
                else
                {
                    Global.GetRoom().SetRoomTeam(Global.GetPlayer().roomSit, eve - 194);
                }
            }
        }
        private void Ready_Go_Eve()
        {
            if (Input.IsLeftUp())
            {
                if (Global.IsConnect())
                {
                    //开始游戏
                    //Poi.GetForm().SendMessage_ReadyAll();
                }
                else
                {
                    CloseAll();
                    ActiveWindow(1003);
                }
            }
        }
        private void Into_Shop_Eve()
        {
            if (Input.IsLeftUp())
            {
                /*
                CloseAll();
                Form1.data.Init_Map(9, 2);
                ActiveWindow(1100);
                ActiveWindow(1110);
                ActiveWindow(1120);
                */
            }
        }
        private void Out_Shop_Eve()
        {
            if (Input.IsLeftUp())
            {
                /*
                CloseAll();
                Form1.data.map = null;
                ActiveWindow(19);
                */
            }
        }
        private void Shop_ShangDian_CheckBoxShop_Update_Eve()
        {
            //更新商店界面的商店复选按钮目录
            if (Input.IsLeft())
            {
                /*
                int index = GetIndex(1100);
                var win = windows[index];
                index = win.GetIndex(110010);
                Form1.data.shop.mulu_shangdian = eve - 110009;
                Form1.data.shop.page = 1;
                for (int i = 0; i < 5; i++)
                {
                    win.ui[i + index].Set_Poi_A(eve - 110009);
                }
                */
            }
        }
        private void Shop_ShangDian_Page_Up_Eve()
        {
            //商店界面向上翻页
            if (Input.IsLeftUp())
            {
                /*
                var shop = Form1.data.shop;
                if (shop.page > 1)
                {
                    shop.select_ID = 0;
                    shop.page--;
                }
                */
            }
        }
        private void Shop_ShangDian_Page_Down_Eve()
        {
            //商店界面向下翻页
            if (Input.IsLeftUp())
            {
                /*
                var shop = Form1.data.shop;
                if (shop.page < shop.GetShopPageLen(shop.mulu_shangdian))
                {
                    shop.select_ID = 0;
                    shop.page++;
                }
                */
            }
        }
        private void Shop_ShangDian_ChongZhi_Eve()
        {
            //点一下加1W Q点
            if (Input.IsLeftUp())
            {
                var player = Global.GetPlayer();
                player.Qbi += 10000;
            }
        }
        private void Shop_ChangKu_CheckBoxShop_Update_Eve()
        {
            //更新商店界面的商店复选按钮目录
            if (Input.IsLeft())
            {
                /*
                int index = GetIndex(1110);
                var win = windows[index];
                index = win.GetIndex(111009);
                Form1.data.shop.mulu_changku = eve - 111008;
                for (int i = 0; i < 5; i++)
                {
                    win.ui[i + index].Set_Poi_A(eve - 111008);
                }
                */
            }
        }
        private void Shop_JueSe_CheckBoxShop_Update_Eve()
        {
            //更新角色选择界面的复选按钮目录
            if (Input.IsLeft())
            {
                /*
                int index = GetIndex(1120);
                var win = windows[index];
                index = win.GetIndex(112000);
                Form1.data.config.player.type = eve - 112000;
                for (int i = 0; i < 7; i++)
                {
                    win.ui[i + index].Set_Poi_A(eve - 112000);
                    if (eve == 112000)
                    {
                        win.ui[i + index].Set_Poi_A(7);
                    }
                }
                */
            }
        }

        //响应服务器
        public void DealMsg(string m)
        {
            /*
            int id = 0;
            id = int.Parse(m.Substring(0, 6));
            if (id == 100012)
            {
                GetServerData(m);
            }
            wait_server = 0;
            */
        }
        public void GetServerData(string m)
        {
            int type = int.Parse(m.Substring(6, 4));
        }

        public int GetIndex(int typeID)
        {
            int len = windows.Count;
            for (int i = 0; i < len; i++)
            {
                if (windows[i].typeID == typeID)
                {
                    return i;
                }
            }
            return -1;
        }
        public int GetMouseIndex()
        {
            int len = windows.Count;
            for (int i = len - 1; i > 0; i--)
            {
                var win = windows[showNum[i]];
                if (win.enabled == 1)
                {
                    if (win.IsMouseInAABB())
                        return showNum[i];
                }
            }
            return -1;
        }
        public void Clear()
        {
            foreach (var t in windows)
            {
                if (t != null)
                    t.Clear();
            }
        }
    }
    public class SpriteBase
    {
        public static string path = GlobalB.GetRootPath() + @"\";
        public static string TextType = "宋体";
        public static BalloonItemPic_Base Load_Bitmap_FromFile(string path, int w, int h, int frame = 1, int frameSqan = 80, int line = 1)
        {
            path = path.Replace("tga", "png");
            path = path.Replace("Tga", "png");

            BalloonItemPic_Base pic = new BalloonItemPic_Base();
            pic.width = w;
            pic.height = h;
            pic.frameNum = frame;
            pic.frameSpan = frameSqan;
            pic.line = line;
            pic.bitmap = GlobalB.Load_Bitmap_FromFile(path);
            if (pic.bitmap == null)
            {
                //Poi.MessageBox("Load Room pic Error , path = " + path);
            }
            return pic;
        }
        public static BalloonItemPic_Base Load_Bitmap_FromFile(string path)
        {
            path = path.Replace("tga", "png");
            path = path.Replace("Tga", "png");

            BalloonItemPic_Base pic = new BalloonItemPic_Base();
            pic.frameNum = 1;
            pic.frameSpan = 80;
            pic.line = 1;
            pic.bitmap = GlobalB.Load_Bitmap_FromFile(path);
            if (pic.bitmap == null)
            {
                //Poi.MessageBox("Load Room pic Error , path = " + path);
            }
            else
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);
                pic.width = bitmap.Size.Width;
                pic.height = bitmap.Size.Height;
                bitmap.Dispose();
            }
            return pic;
        }
        public static void DrawNum(PPDevice ppDevice, BalloonItemPic_Base pic, int value, float x, float y, float width, float height, int num = 3, int type = 0)
        {
            //1.左对齐
            //0.右对齐
            //2.
            //3.左对齐自动长度
            //4.右对齐自动长度
            //10.图片显示
            if (type == 10)
            {
                ppDevice.BitBlt(pic, new RectangleF(x, y, width, height), new RectangleF(value * width, 0, width, height));
                return;
            }
            if (type >= 3 && type <= 5)
            {
                num = GlobalB.GetIntLen(value);
            }
            if (num == 0)
            {
                int val = value;
                while (val != 0)
                {
                    val /= 10;
                    num++;
                }
                if (num == 0) num++;
            }
            if (type == 1 || type == 3)
            {
                //左对齐
                x += width * num;
            }
            int t = 0;
            for (int i = 1; i <= num; i++)
            {
                t = value % 10;
                value /= 10;
                ppDevice.BitBlt(pic, new RectangleF(x - i * width, y, width, height), new RectangleF(t * width, 0, width, height));
            }
        }
        //
        public static Color color_blue = new Color(0x66, 0xCC, 0xff);
        public static Color color_white = new Color(0xff, 0xff, 0xff);
        public static Color color_white2 = new Color(0xdd, 0xdd, 0xff);
        public static Color color_black = new Color(0x00, 0x00, 0x00);
        public static Color color_yellow = new Color(149, 222, 60);
        public static BalloonItemPic_Base Num_Room_ID_17x16 = Load_Bitmap_FromFile(path + @"Medias\Room_ID.tga", 170, 16);
        public static BalloonItemPic_Base Num_PiaoBi_10x14 = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\货币数字.tga", 100, 14);
        public static BalloonItemPic_Base Num_Hit_8x10 = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\击落数字.tga", 80, 10);
        public static BalloonItemPic_Base Num_Room_In_ID_16x16 = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\房间数字.tga", 160, 16);
        public static BalloonItemPic_Base Num_Shop_9x17 = Load_Bitmap_FromFile(path + @"balloon\Pic\Shop\商店数字.tga", 99, 17);

        //随机，对抗，竞速，竞技，夺宝，夺旗
        public static BalloonItemPic_Base[] mode = new BalloonItemPic_Base[6] {
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\随机模式底框.tga",190,161),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\对抗模式底框.tga",190,161),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\竞速模式底框.tga",190,161),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\竞技模式.tga",190,161),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\夺宝模式.tga",190,161),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\夺旗模式.tga",190,161),
        };
        public static BalloonItemPic_Base[] mode_out = new BalloonItemPic_Base[6] {
            Load_Bitmap_FromFile(path + @"Medias\Room_Mode_Free.tga",49,55),
            Load_Bitmap_FromFile(path + @"Medias\Room_Mode_VS.tga",49,55),
            Load_Bitmap_FromFile(path + @"Medias\Room_Mode_Race.tga",49,55),
            Load_Bitmap_FromFile(path + @"Medias\Room_Mode_Death.tga",49,55),
            Load_Bitmap_FromFile(path + @"Medias\Room_Mode_Treasure.tga",49,55),
            Load_Bitmap_FromFile(path + @"Medias\Room_Mode_Target.tga",49,55),
        };
        public static BalloonItemPic_Base[] mode_txt = new BalloonItemPic_Base[3] {
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\随机模式文字.tga",111,49),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\对抗模式文字.tga",101,35),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\竞速模式文字.tga",101,35),
        };
        public static BalloonItemPic_Base mode_suijitubiao = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\随机模式图标.tga", 157, 80);
        public static BalloonItemPic_Base mode_qizi = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\旗子动画.tga", 140, 140, 4, 100);
        public static BalloonItemPic_Base mode_quantao = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\拳套动画.tga", 140, 140, 4, 100);
        public static BalloonItemPic_Base mode_gaoguang = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\模式窗口高光.tga", 185, 20);
        public static BalloonItemPic_Base mode_out_bg = Load_Bitmap_FromFile(path + @"Medias\Room_Bk_On.tga", 180, 89);
        public static BalloonItemPic_Base[] mode_soutai = new BalloonItemPic_Base[4] {
            Load_Bitmap_FromFile(path + @"Medias\Room_Empty.tga",63,23),
            Load_Bitmap_FromFile(path + @"Medias\Room_Waiting.tga",63,23),
            Load_Bitmap_FromFile(path + @"Medias\RoomFull.tga",63,23),
            Load_Bitmap_FromFile(path + @"Medias\Room_Playing.tga",63,23),
        };
        public static BalloonItemPic_Base room_Num18x18 = Load_Bitmap_FromFile(path + @"Medias\Room_Num.tga", 180, 18);
        public static BalloonItemPic_Base[] room_player_bg = new BalloonItemPic_Base[10] {
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\灰色底框.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\蓝色底框.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\绿色底框.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\黄色底框.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\橙色底框.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\灰色底框2.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\蓝色底框2.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\绿色底框2.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\黄色底框2.tga",180,119),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\橙色底框2.tga",180,119),
        };
        public static BalloonItemPic_Base[] room_in_bg = new BalloonItemPic_Base[5]
        {
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\单色背景.tga",800,400),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\单色云.tga",800,113),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\单色云-01.tga",143,68),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\单色云-02.tga",231,106),
            Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\单色云-03.tga",146,69),
        };
        public static BalloonItemPic_Base room_Boss = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\房主标志.tga", 18, 26);
        public static BalloonItemPic_Base room_WS = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\误伤图标.tga", 25, 19);
        public static BalloonItemPic_Base room_Hit = Load_Bitmap_FromFile(path + @"balloon\Pic\Prepare\击落标志.tga", 30, 17);

        //playertype
        public static BalloonItemPic_Base[][] room_player_type = new BalloonItemPic_Base[7][]
        {
            new BalloonItemPic_Base[]{
                null,
            },
            new BalloonItemPic_Base[]{
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\独角兽-A.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\独角兽-B.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\独角兽-C.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\独角兽-自由.tga",58,51),
            },
            new BalloonItemPic_Base[]{
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\四叶草-A.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\四叶草-B.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\四叶草-C.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\四叶草-自由.tga",58,51),
            },
            new BalloonItemPic_Base[]{
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\心-A.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\心-B.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\心-C.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\心-自由.tga",58,51),
            },
            new BalloonItemPic_Base[]{
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\翅膀-A.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\翅膀-B.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\翅膀-C.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\翅膀-自由.tga",58,51),
            },
            new BalloonItemPic_Base[]{
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\糖果-A.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\糖果-B.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\糖果-C.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\糖果-自由.tga",58,51),
            },
            new BalloonItemPic_Base[]{
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\鲸鱼-A.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\鲸鱼-B.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\鲸鱼-C.tga",58,51),
                Load_Bitmap_FromFile(path + @"balloon\Pic\角色\预备\鲸鱼-自由.tga",58,51),
            },
        };

        public static void ClearAll()
        {
            //
            Clear(Num_Hit_8x10);
            Clear(Num_PiaoBi_10x14);
            Clear(Num_Room_In_ID_16x16);
            Clear(Num_Room_ID_17x16);
            Clear(Num_Shop_9x17);

            //随机，对抗，竞速，竞技，夺宝，夺旗
            Clear(mode);
            Clear(mode_gaoguang);
            Clear(mode_out);
            Clear(mode_txt);
            Clear(mode_suijitubiao);
            Clear(mode_qizi);
            Clear(mode_quantao);
            Clear(mode_out_bg);
            Clear(mode_soutai);
            Clear(room_player_bg);
            Clear(room_in_bg);
            Clear(room_Boss);
            Clear(room_WS);
            Clear(room_Hit);

            //playertype
            Clear(room_player_type);
        }
        public static void Clear(BalloonItemPic_Base[][] t)
        {
            if(t !=null)
            {
                foreach (var t1 in t)
                {
                    Clear(t1);
                }
                t = null;
            }
        }
        public static void Clear(BalloonItemPic_Base[] t)
        {
            if(t != null)
            {
                foreach (var t1 in t)
                {
                    Clear(t1);   
                }
                t = null;
            }
        }
        public static void Clear(BalloonItemPic_Base t)
        {
            if (t != null)
                t.Clear();
            t = null;
        }
        public static void Clear(Texture t)
        {
            if (t != null)
                t.Dispose();
            t = null;
        }

        public static Input Input = null;
        public static int ui_ID = 0;

        public int thisID = 0;
        public int typeID = 0;
        public int type = 0;
        public float x = 0;
        public float y = 0;
        public float pic_dx = 0;
        public float pic_dy = 0;
        public float width = 800;
        public float height = 600;
        public float alpha = 1.0f;

        public float val_A = 0;
        public float poi_A = 0;
        public BalloonItemPic_Base pic = null;

        public virtual float GetWidth()
        {
            return width;
        }
        public virtual float GetHeight()
        {
            return height;
        }
        public virtual RectangleF GetPicAABB()
        {
            return new RectangleF(x + pic_dx, y + pic_dy, GetWidth(), GetHeight());
        }
        public virtual RectangleF GetPicArea()
        {
            return new RectangleF(0, 0, width, height);
        }
        public virtual RectangleF GetShowArea()
        {
            return new RectangleF(x + pic_dx, y + pic_dy, GetWidth(), GetHeight());
        }
        public virtual BalloonItemPic_Base GetBitmap()
        {
            return pic;
        }
        public virtual void Action()
        {

        }
        public virtual bool IsMouseInAABB()
        {
            return Physics.XY_Rect(Input.x, Input.y, GetPicAABB());
        }
        public virtual void Draw(PPDevice ppDevice)
        {
            var pic = GetBitmap();
            if (pic != null)
                ppDevice.BitBlt(GetBitmap(), GetShowArea(), GetPicArea(), alpha);
        }
        public virtual void Clear()
        {
            Clear(pic);
        }
    }
    public class Window : SpriteBase
    {
        public static int Index = 0;

        public int visible = 1; //需要显示吗
        public int modal = 0;
        public int mask = 1;    //
        public int movable = 0; //可以移动吗
        public int enabled = 1; //起作用吗
        public int topmost = 0; //

        public int soutai = 0;
        public float move_dx = 0;
        public float move_dy = 0;

        public string fontname = "宋体";
        public float fontsize = 10;

        public int index = 0;
        public void Active()
        {
            WinAddDel();
            visible = 1;
            enabled = 1;
            move_dx = 0;
            move_dy = 0;
            foreach (var u in this.ui)
            {
                u.Reset();
            }
            switch (this.typeID)
            {
                case 19:
                    InitRoomIn();
                    break;
                case 20:
                    OpenMapList();
                    break;
                case 22:
                    SetRoom();
                    break;
                case 1000:
                    Init_Login();
                    break;
                case 1001:
                    Init_Sever();
                    break;
                case 1002:
                    Init_Room();
                    break;
                case 1003:
                    Ready_Go();
                    break;
                case 1100:
                    InitShop();
                    break;
                case 1110:
                    InitShop2();
                    break;
                case 1120:
                    InitShop3();
                    break;
                case 35:
                    InitShop4();
                    break;
                default:
                    break;
            }
            ToTop();
        }

        public void Init_Login()
        {
        }
        public void Init_Sever()
        {
            /*
            int index = GetIndex(100112); //第一个区UI
            for (int i = 0; i < Ser_Data.servers.Count; i++)
            {
                var ui = this.ui[index + i];
                Data.layers[1].sprites.Add(
                    new Sprite_Text(
                        ui.x + 6,
                        ui.y + 10,
                        "浮空巨门" + (i + 1),
                        255,
                        0x66ccff,
                        -1,
                        this.fontsize,
                        this.fontname
                        ));
                Data.layers[1].sprites.Add(
                    new Sprite_Bar_Server(
                        path + @"Medias\Server_Busy_Status.tga",
                        i,
                        ui.x + 100,
                        ui.y + 9
                        ));
            }
            */
            var u = this.ui[GetIndex(100100)]; //新手频道UI
        }
        public void Init_Room()
        {
            /*
            Data.Layer_Clear();
            var ser = Ser_Data.servers[Config.server];//读取当前服务器;
            ser.GetInSever(Config.player);
            addBefore.Add(new Sprite_UserInfo());
            addBefore.Add(new Sprite_Rooms());
            addBefore.Add(new Sprite_Room_Player());
            */
        }
        public void SetRoom()
        {
            movable = 1;
            x = 308;
            y = 163;
            pic_dx = 0;
            pic_dy = 0;
        }
        public void InitShop()
        {
            movable = 0;
            x = 331;
            y = 2;
            pic_dx = 0;
            pic_dy = 0;
            Del_WinAdd();
            addBefore.Add(new Sprite_Shop_ShangDian());
        }
        public void InitShop2()
        {
            movable = 0;
            x = 1;
            y = 350;
            pic_dx = 0;
            pic_dy = 0;
        }
        public void InitShop3()
        {
            movable = 0;
            x = 200;
            y = 0;
            pic_dx = 0;
            pic_dy = 0;
            var poi = Global.GetPlayer().type;
            if (poi == 0) poi = 7;
            for (int i = 0; i < 7; i++)
            {
                ui[i].poi_A = poi;
            }
            addBefore.Add(new Sprite_Shop());
        }
        public void InitShop4()
        {
            movable = 1;
            x = 336;
            y = 62;
            pic_dx = 0;
            pic_dy = 0;
        }
        public void OpenMapList()
        {
            topmost = 1;
            x = 200;
            y = 252;
            pic_dx = 0;
            pic_dy = 0;
            addBefore.Add(new Sprite_Room_MapSelect());
        }
        public void InitRoomIn()
        {
            //var ser = Ser_Data.servers[Config.server];//读取当前服务器;
            Global.PlayBgm(11);
            int index = GetIndex(1918);
            ui[index].enable = 0;
            Del_WinAdd();
            addAfter.Add(new Sprite_Room_In_bg());
            addAfter.Add(new Sprite_Room_In());
            addBefore.Add(new Sprite_Room_In2());
        }
        public void Ready_Go()
        {
            //开始游戏
            var room = Global.GetRoom();
            if (room.mapTypeID == 1)
            {
                //随机地图
                Global.Init_Map(6, 5);
                room.mapTypeID = 6;
            }
            else
            {
                Global.Init_Map(room.mapTypeID, room.mapID);
            }
            if (room.mapTypeID == 6)
            {
                addBefore.Add(new Sprite_Map_JingSu());
            }
            addBefore.Add(new Sprite_Map_Base());
        }

        public int GetIndex(int typeID)
        {
            int len = ui.Count;
            for (int i = 0; i < len; i++)
            {
                if (ui[i].typeID == typeID)
                {
                    return i;
                }
            }
            return -1;
        }
        public int GetShowIndex()
        {
            var list = Global.GetWindowsList();
            var array = list.showNum;
            var win = list.windows;
            for (int i = 0; i < array.Count; i++)
            {
                if (win[array[i]].typeID == this.typeID)
                {
                    return i;
                }
            }
            //Poi.MessageBox("窗口排序错误: " + this.typeID);
            return 99;
        }
        public void ToTop()
        {
            var winl = Global.GetWindowsList();
            var array = winl.showNum;
            int index = GetShowIndex();
            if (topmost == 1)
            {
                array.RemoveAt(index);
                array.Add(winl.GetIndex(typeID));
            }
            else
            {
                array.RemoveAt(index);
                for (int i = 0; i < array.Count; i++)
                {
                    var win = winl.windows[winl.showNum[i]];
                    if (win.enabled == 0) continue;
                    if (win.topmost == 1)
                    {
                        array.Insert(i, winl.GetIndex(typeID));
                        return;
                    }
                }
                array.Add(winl.GetIndex(typeID));
            }
        }
        public void ToBottom()
        {
            enabled = 0;
            var winl = Global.GetWindowsList();
            var array = winl.showNum;
            int index = GetShowIndex();
            if (topmost == 1)
            {
                array.RemoveAt(index);
                array.Insert(0, winl.GetIndex(typeID));
            }
            Del_WinAdd();
        }
        public void WinAddDel()
        {
            addAfter.Clear();
            addBefore.Clear();
        }

        public List<UI> ui = new List<UI>();
        public List<Sprite_WinAdd> addBefore = new List<Sprite_WinAdd>();
        public List<Sprite_WinAdd> addAfter = new List<Sprite_WinAdd>();
        public void Del_WinAdd()
        {
            foreach (var t in addBefore)
            {
                if (t != null)
                    t.Clear();
            }
            foreach (var t in addAfter)
            {
                if (t != null)
                    t.Clear();
            }

            addAfter.Clear();
            addBefore.Clear();
        }
        public override void Draw(PPDevice ppDevice)
        {
            foreach (var adda in addAfter)
            {
                adda.Draw(ppDevice);
            }
            base.Draw(ppDevice);
            foreach (var addb in addBefore)
            {
                addb.Draw(ppDevice);
            }
            foreach (var ui in this.ui)
            {
                if (ui.enable == 0) continue;
                ui.Draw(ppDevice);
            }

        }
        public virtual void Move()
        {
            if (Input.type == MouseButtons.Left && Input.time == 1 && IsMouseInAABB())
            {
                soutai = 1;//被拖动
                move_dx = Input.x - x - pic_dx;
                move_dy = Input.y - y - pic_dy;
            }
            if (Input.type == MouseButtons.Left && soutai == 1)
            {
                pic_dx = Input.x - x - move_dx;
                pic_dy = Input.y - y - move_dy;
            }
            else
            {
                soutai = 0;
            }
        }
        public override void Action()
        {
            if (enabled == 0) return;
            if (movable == 1)
            {
                Move();
            }
            foreach (var u in addAfter)
            {
                u.pic_dx = this.x + this.pic_dx;//重置UI位置
                u.pic_dy = this.y + this.pic_dy;
                u.Action();
            }
            foreach (var u in addBefore)
            {
                u.pic_dx = this.x + this.pic_dx;//重置UI位置
                u.pic_dy = this.y + this.pic_dy;
                u.Action();
            }
            foreach (var u in ui)
            {
                u.pic_dx = this.x + this.pic_dx;//重置UI位置
                u.pic_dy = this.y + this.pic_dy;
            }
        }
        public void ui_Action()
        {
            foreach (var u in ui)
            {
                if (u.enable == 0) continue;
                u.Action();
            }
        }
        public override void Clear()
        {
            base.Clear();
            foreach (var t in ui)
            {
                if (t != null)
                    t.Clear();
            }
            ui.Clear();
            Del_WinAdd();
        }
    }
    public class UI : SpriteBase
    {
        public int soutai = 0;
        public int enable = 1;
        public virtual void Reset()
        {
            soutai = 0;
            enable = 1;
        }
        public virtual void Active()
        {

        }
        public virtual void Set_Val_A(int val)
        {
            val_A = val;
        }
        public virtual void Set_Poi_A(int poi)
        {
            poi_A = poi;
        }
        public virtual void Kill()
        {

        }
    }
    public class UI_Button : UI
    {
        public BalloonItemPic_Base upimg = null;
        public BalloonItemPic_Base hoverimg = null;
        public BalloonItemPic_Base downimg = null;
        public BalloonItemPic_Base disableimg = null;
        public override void Action()
        {
            if (soutai == 3) return; //禁止
            if (IsMouseInAABB())
            {
                Window_List.eve = typeID;
                soutai = 1; //滑过
                if (Input.IsLeft())
                {
                    soutai = 2; //按下
                }
            }
            else
            {
                soutai = 0; //无
            }
        }
        public override BalloonItemPic_Base GetBitmap()
        {
            switch (soutai)
            {
                case 0:
                    return upimg;
                case 1:
                    return hoverimg;
                case 2:
                    return downimg;
                case 3:
                    return disableimg;
                default:
                    return upimg;
            }
        }
        public override void Kill()
        {
            soutai = 3;
        }
        public override void Clear()
        {
            base.Clear();
            Clear(upimg);
            Clear(hoverimg);
            Clear(downimg);
            Clear(disableimg);
        }
    }
    public class UI_ButtonLine : UI
    {
        public int frame = 0;
        public override void Action()
        {
            if (soutai == 3)
            {
                frame = 4;
                return; //禁止
            }
            if (IsMouseInAABB())
            {
                Window_List.eve = typeID;
                soutai = 1; //滑过
                frame = 3;
                if (Input.IsLeft())
                {
                    soutai = 2; //按下
                    frame = 1;
                }
            }
            else
            {
                soutai = 0; //无
                frame = 0;
            }
        }
        public override RectangleF GetPicArea()
        {
            return new RectangleF(frame * width, 0, width, height);
        }
    }
    public class UI_Edit : UI
    {
        public Point border = new Point(1, 1);
        public int maxlen = 0;
        public string password = "NULL";
    }
    public class UI_StaticMap : UI
    {
    }
    public class UI_Listbox : UI
    {
        public Color forecolor = new Color();
        public string context = "ERROR";
    }
    public class UI_Chat : UI
    {
        public int margin = 2;
        public int linespace = 2;
        public int autoscroll = 0;
        public int maxlines = 100;
    }
    public class UI_CheckBox : UI
    {
        public Color forecolor;
        public int frame = 0;
        public override void Action()
        {
            if (soutai == 2)
            {
                frame = 2;
                return; //禁止
            }
            if (soutai == 3)
            {
                frame = 3;
                return; //禁止
            }
            if (IsMouseInAABB())
            {
                Window_List.eve = typeID;
                if (Input.IsLeftDown())
                {
                    soutai = 1 - soutai; //按下
                    frame = 1 - frame;
                }
            }
        }
        public override RectangleF GetPicArea()
        {
            return new RectangleF(frame * width, 0, width, height);
        }
    }
    public class UI_Radio : UI
    {
        public Color forecolor;
        public int frame = 0;
        public override void Action()
        {
            if (soutai == 2)
            {
                frame = 2;
                return; //禁止
            }
            if (soutai == 3)
            {
                frame = 3;
                return; //禁止
            }
            if (IsMouseInAABB())
            {
                Window_List.eve = typeID;
                if (Input.IsLeftDown())
                {
                    soutai = 1; //按下
                }
            }
            frame = soutai;
        }
        public override RectangleF GetPicArea()
        {
            return new RectangleF(frame * width, 0, width, height);
        }
    }

    public class UI_CheckBoxLine : UI
    {
        public int frame = 0;
        public int index = 0;
        public override void Action()
        {
            if (IsMouseInAABB() && Input.IsLeftDown())
            {
                frame = 1 - frame;
            }
        }
        public override RectangleF GetPicArea()
        {
            return new RectangleF(frame * width, 0, width, height);
        }
        public override void Reset()
        {
            frame = 0;
        }
    }
    public class UI_CheckBoxLine_Channel : UI_CheckBoxLine
    {
        public override void Action()
        {
            if (IsMouseInAABB() && Input.IsLeftDown())
            {
                Window_List.eve = typeID;
            }
            /*
            if (index == Config.channel)
            {
                frame = 1;
            }
            else
            {
                Reset();
            }
            */
        }
    }
    public class UI_CheckBox2 : UI
    {
        public BalloonItemPic_Base upimg = null;
        public BalloonItemPic_Base downimg = null;
        public override void Action()
        {
            if (IsMouseInAABB())
            {
                Window_List.eve = typeID;
            }
        }
        public override BalloonItemPic_Base GetBitmap()
        {
            switch (soutai)
            {
                case 0:
                    return upimg;
                case 1:
                    return downimg;
                default:
                    return upimg;
            }
        }
        public override void Active()
        {
            soutai = 1;
        }
        public override void Clear()
        {
            base.Clear();
            Clear(upimg);
            Clear(downimg);
        }
    }
    public class UI_CheckBoxShop : UI
    {
        public BalloonItemPic_Base upimg = null;
        public BalloonItemPic_Base hoverimg = null;
        public BalloonItemPic_Base downimg = null;
        public BalloonItemPic_Base disableimg = null;
        public RectangleF pic_Rect = new RectangleF(0, 0, 200, 200);
        public UI_CheckBoxShop()
        {
            poi_A = 1;
        }
        public override void Action()
        {
            if (soutai == 3) return; //禁止
            if (IsMouseInAABB())
            {
                Window_List.eve = typeID;
                soutai = 1; //滑过
                if (Input.IsLeft())
                {
                    soutai = 2; //按下
                }
            }
            else
            {
                soutai = 0; //无
            }
        }
        public override BalloonItemPic_Base GetBitmap()
        {
            if (poi_A == val_A && val_A != 0)
            {
                return pic;
            }
            switch (soutai)
            {
                case 0:
                    return upimg;
                case 1:
                    return hoverimg;
                case 2:
                    return downimg;
                case 3:
                    return disableimg;
                default:
                    return upimg;
            }
        }
        public override RectangleF GetShowArea()
        {
            if (poi_A == val_A && val_A != 0)
            {
                return new RectangleF(pic_Rect.X + pic_dx, pic_Rect.Y + pic_dy, pic_Rect.Width, pic_Rect.Height);
            }
            return base.GetShowArea();
        }
        public override RectangleF GetPicArea()
        {
            if (poi_A == val_A && val_A != 0)
            {
                return new RectangleF(0, 0, pic_Rect.Width, pic_Rect.Height);
            }
            return base.GetPicArea();
        }
        public override void Clear()
        {
            base.Clear();
            Clear(upimg);
            Clear(hoverimg);
            Clear(downimg);
            Clear(disableimg);
        }
    }

    public class UI_ListBox_MapSelect : Sprite_WinAdd
    { //仅仅供地图选择窗口使用
        public int soutai = 0;
        public List<UI_ListBox_MapSelect_Son> types = new List<UI_ListBox_MapSelect_Son>();
        public UI_ListBox_MapSelect()
        {
            x = 6;
            y = 22;
            width = 150;
            height = 271;
            soutai = 0;
        }
        public override void Action()
        {

        }
        public override void Draw(PPDevice ppDevice)
        {
        }
        public override void Clear()
        {
            base.Clear();
            foreach (var t in types)
            {
                if(t!=null)
                {
                    t.Clear();
                }
            }
            types.Clear();
        }
    }
    public class UI_ListBox_MapSelect_Son : Sprite_WinAdd
    {
        public int soutai = -1;
        public Texture uping = null;
        public Texture downimg = null;
        public List<string> maps = new List<string>();
        public UI_ListBox_MapSelect_Son()
        {
            width = 150;
            height = 18;
        }
        public override void Action()
        {

        }
        public override void Draw(PPDevice ppDevice)
        {
            base.Draw(ppDevice);
        }
        public override void Clear()
        {
            base.Clear();
            Clear(uping);
            Clear(downimg);
        }
    }
}
