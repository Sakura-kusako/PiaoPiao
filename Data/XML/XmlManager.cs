using Data.Globals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Data.XML
{
    public class XmlManager
    {
        //读取所有xml数据

        public Element elements;
        public List<ItemPic> itemPics;
        public List<SpritePic> pics;
        public List<Map> maps;
        public List<ShopItem> shopItem;

        public string rootPath = GlobalB.GetRootPath() + "\\";

        public XmlManager()
        {
            OnCreate();
        }
        public void OnCreate()
        {
            elements = new Element();
            itemPics = new List<ItemPic>();
            pics = new List<SpritePic>();
            maps = new List<Map>();
            shopItem = new List<ShopItem>();

            maps.Add(null); //第一个为空
            shopItem.Add(null);
        }

        public void Load()
        {
            elements.Load();
            Load_ItemPic();
            Load_Pic();
            Load_Map();
            Load_ShopItem();
        }
        public void Load_ItemPic()
        {
            string path = rootPath + @"balloon\BalloonItemPic.xml";
            if (!File.Exists(path))
            {
                MessageBox.Show("无法打开: " + path, "ERROR");
                return;
            }

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            ItemPic t1;
            ItemPics t;
            foreach (XElement item1 in root.Elements())
            {
                t1 = new ItemPic();
                t1.ID = int.Parse(item1.Attribute("ID").Value);
                t1.name = item1.Name.LocalName;
                foreach (XElement item2 in item1.Elements())
                {
                    t = new ItemPics();
                    t.ID = int.Parse(item2.Attribute("ID").Value);
                    t.name = item2.Name.LocalName;
                    t.shopFile = Try_Get_Attribute_Value_File(item2.Attribute("ShopFile"));
                    for (int i = 1; i <= 2; i++)
                    {
                        t.item[i - 1].x = Try_Get_Attribute_Value(item2.Attribute("X" + i));
                        t.item[i - 1].y = Try_Get_Attribute_Value(item2.Attribute("Y" + i));
                        t.item[i - 1].file = Try_Get_Attribute_Value_File(item2.Attribute("File" + i));
                        t.item[i - 1].width = Try_Get_Attribute_Value(item2.Attribute("Width" + i));
                        t.item[i - 1].height = Try_Get_Attribute_Value(item2.Attribute("Height" + i));
                        t.item[i - 1].line = Try_Get_Attribute_Value(item2.Attribute("Line" + i));
                        t.item[i - 1].frameNum = Try_Get_Attribute_Value(item2.Attribute("FrameNum" + i));
                        t.item[i - 1].frameSpan = Try_Get_Attribute_Value(item2.Attribute("FrameSpan" + i));
                    }
                    t1.itemPics.Add(t);
                }
                itemPics.Add(t1);
            }
        }
        public void Load_Pic()
        {
            string path = rootPath + @"balloon\BalloonPic.xml";
            if (!File.Exists(path))
            {
                MessageBox.Show("无法打开: " + path, "ERROR");
                return;
            }

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            SpritePic t1;
            SpritePics t;
            foreach (XElement item1 in root.Elements())
            {
                t1 = new SpritePic();
                t1.ID = int.Parse(item1.Attribute("ID").Value);
                t1.name = item1.Name.LocalName;
                foreach (XElement item2 in item1.Elements())
                {
                    t = new SpritePics();
                    t.ID = int.Parse(item2.Attribute("ID").Value);
                    t.name = item2.Name.LocalName;
                    t.file = Try_Get_Attribute_Value_File(item2.Attribute("File"));
                    t.width = Try_Get_Attribute_Value(item2.Attribute("Width"));
                    t.height = Try_Get_Attribute_Value(item2.Attribute("Height"));
                    t.frameNum = Try_Get_Attribute_Value(item2.Attribute("FrameNum"));
                    t.frameSpan = Try_Get_Attribute_Value(item2.Attribute("FrameSpan"));
                    t.line = Try_Get_Attribute_Value(item2.Attribute("Line"));
                    t1.itemPics.Add(t);
                }
                pics.Add(t1);
            }
        }
        public void Load_Map()
        {
            string path = rootPath + @"balloon\BalloonMap.xml";
            if (!File.Exists(path))
            {
                MessageBox.Show("无法打开: " + path, "ERROR");
                return;
            }

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            Map t1;
            Maps t2;
            Maps_Element t3;
            Maps_Elements_Base t4;

            foreach (XElement item1 in root.Elements())
            {
                t1 = new Map();
                t1.ID = int.Parse(item1.Attribute("ID").Value);
                t1.name = item1.Name.LocalName;
                t1.fileIn = Try_Get_Attribute_Value_File(item1.Attribute("FileIn"));
                t1.fileOut = Try_Get_Attribute_Value_File(item1.Attribute("FileOut"));
                foreach (XElement item2 in item1.Elements())
                {
                    t2 = new Maps();
                    t2.id = int.Parse(item2.Attribute("ID").Value);
                    t2.name = Try_Get_Attribute_Value_Str(item2.Attribute("Name"));
                    t2.gameMode = Try_Get_Attribute_Value(item2.Attribute("GameMode"));
                    t2.backGround = Try_Get_Attribute_Value(item2.Attribute("BackGround"));
                    t2.width = Try_Get_Attribute_Value(item2.Attribute("Width"));
                    t2.height = Try_Get_Attribute_Value(item2.Attribute("Height"));
                    t2.gravity = Try_Get_Attribute_Value_F(item2.Attribute("Gravity"));
                    t2.velocityH = Try_Get_Attribute_Value_F(item2.Attribute("VelocityH"));
                    t2.velocityV = Try_Get_Attribute_Value_F(item2.Attribute("VelocityV"));
                    t2.gameTime = Try_Get_Attribute_Value(item2.Attribute("GameTime"));
                    t2.level = Try_Get_Attribute_Value(item2.Attribute("Level"));
                    t2.real = Try_Get_Attribute_Value(item2.Attribute("Real"));
                    t2.propCount = Try_Get_Attribute_Value(item2.Attribute("PropCount"));
                    foreach (XElement item3 in item2.Elements())
                    {
                        t3 = new Maps_Element();
                        t3.id = int.Parse(item3.Attribute("ID").Value);
                        t3.name = item3.Name.LocalName;
                        foreach (XElement item4 in item3.Elements())
                        {
                            t4 = new Maps_Elements_Base();
                            t4.id = Try_Get_Attribute_Value(item4.Attribute("ID"));
                            t4.x = Try_Get_Attribute_Value(item4.Attribute("X"));
                            t4.y = Try_Get_Attribute_Value(item4.Attribute("Y"));
                            t4.centerX = Try_Get_Attribute_Value(item4.Attribute("CenterX"));
                            t4.centerY = Try_Get_Attribute_Value(item4.Attribute("CenterY"));
                            t4.radian = Try_Get_Attribute_Value(item4.Attribute("Radian"));
                            t4.radius = Try_Get_Attribute_Value(item4.Attribute("Radius"));
                            t4.layer = Try_Get_Attribute_Value(item4.Attribute("Layer"));
                            t4.count = Try_Get_Attribute_Value(item4.Attribute("Count"));
                            t4.speed = Try_Get_Attribute_Value(item4.Attribute("Speed"));
                            t4.flyAreaX = Try_Get_Attribute_Value(item4.Attribute("FlyAreaX"));
                            t4.weight = Try_Get_Attribute_Value(item4.Attribute("Weight"));
                            t4.time = Try_Get_Attribute_Value(item4.Attribute("Time"));
                            t4.pastTime = Try_Get_Attribute_Value(item4.Attribute("PastTime"));
                            t4.bstTime = Try_Get_Attribute_Value(item4.Attribute("BstTime"));
                            t4.sprTime = Try_Get_Attribute_Value(item4.Attribute("SprTime"));
                            t4.offsetTime = Try_Get_Attribute_Value(item4.Attribute("OffsetTime"));
                            t4.flyType = Try_Get_Attribute_Value(item4.Attribute("FlyType"));
                            t4.left = Try_Get_Attribute_Value(item4.Attribute("Left"));
                            t4.top = Try_Get_Attribute_Value(item4.Attribute("Top"));
                            t4.right = Try_Get_Attribute_Value(item4.Attribute("Right"));
                            t4.bottom = Try_Get_Attribute_Value(item4.Attribute("Bottom"));
                            t4.dirLeft = Try_Get_Attribute_Value(item4.Attribute("DirLeft"));
                            t4.dirTop = Try_Get_Attribute_Value(item4.Attribute("DirUp"));
                            t4.dirRight = Try_Get_Attribute_Value(item4.Attribute("DirRight"));
                            t4.dirBottom = Try_Get_Attribute_Value(item4.Attribute("DirDown"));
                            t4.interVal1 = Try_Get_Attribute_Value(item4.Attribute("InterVal1"));
                            t4.interVal2 = Try_Get_Attribute_Value(item4.Attribute("InterVal2"));
                            t4.group = Try_Get_Attribute_Value(item4.Attribute("Group"));
                            t3.elements_base.Add(t4);
                        }
                        t2.elements.Add(t3);
                    }
                    t1.maps.Add(t2);
                }
                maps.Add(t1);
            }
        }
        public void Load_ShopItem()
        {
            string path = rootPath + @"balloon\BalloonShopItem.xml";
            if (!File.Exists(path))
            {
                MessageBox.Show("商店列表加载失败 无法打开: " + path);
                return;
            }

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            ShopItem t;
            foreach (XElement item1 in root.Elements())
            {
                t = new ShopItem();
                t.equip_ID = int.Parse(item1.Attribute("ID").Value);
                t.equip_name = item1.Name.LocalName;
                t.equip_type = Try_Get_Attribute_Value(item1.Attribute("装备类型"));
                t.flag = Try_Get_Attribute_Value(item1.Attribute("附加标志"));
                t.use_time = Try_Get_Attribute_Value(item1.Attribute("使用时限"));
                t.pic_type = Try_Get_Attribute_Value(item1.Attribute("图片类型"));
                t.pic_ID = Try_Get_Attribute_Value(item1.Attribute("图片ID"));
                t.price_Piao = Try_Get_Attribute_Value(item1.Attribute("飘币售价"));
                t.price_Q = (int)(Try_Get_Attribute_Value_F(item1.Attribute("Q币售价")) * 100);
                t.str_ShuoMing = Try_Get_Attribute_Value_Str(item1.Attribute("秀说明文字"));
                t.Is_TuiJian = Try_Get_Attribute_Value_Bool(item1.Attribute("出售类型"));
                t.Is_XuFei = Try_Get_Attribute_Value_Bool(item1.Attribute("只能续费"));
                this.shopItem.Add(t);
            }
        }
        public static int Try_Get_Attribute_Value(XAttribute t)
        {
            if (t == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(t.Value);
            }
        }
        public static bool Try_Get_Attribute_Value_Bool(XAttribute t)
        {
            if (t == null)
            {
                return false;
            }
            else
            {
                if (int.Parse(t.Value) == 0)
                {
                    return false;
                }
                return true;
            }
        }
        public static float Try_Get_Attribute_Value_F(XAttribute t)
        {
            if (t == null)
            {
                return 0;
            }
            else
            {
                try
                {
                    return float.Parse(t.Value);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public static string Try_Get_Attribute_Value_Str(XAttribute t)
        {
            if (t == null)
            {
                return "NULL";
            }
            else
            {
                return t.Value;
            }
        }
        public static string Try_Get_Attribute_Value_File(XAttribute t)
        {
            if (t == null)
            {
                return "NULL";
            }
            else
            {
                string poi = t.Value;
                poi = poi.Replace("\\\\", "\\");
                poi = poi.Replace("sfp", "tga");
                poi = poi.Replace("tga", "png");
                return poi;
            }
        }
    }

    public class Element
    {
        public List<BackGround> backGrounds;
        public List<Static_Element> static_Elements;
        public List<Fly_Element> fly_Elements;
        public List<NPC> npcs;
        public List<Property> propertys;
        public List<Vane> vanes;
        public List<Trap> traps;
        public List<Player> players;
        public List<Balloon> balloons;
        public List<Build_Slave> build_Slaves;
        public List<Primer> primers;
        public List<Float> floats;
        public List<Gate> gates;
        public List<Stabber> stabbers;
        public List<Spout> spouts;
        public List<Barrier> barriers;
        public List<Player_Base> player_Bases;

        public Element()
        {
            OnCreate();
            Init();
        }
        public void OnCreate()
        {
            backGrounds = new List<BackGround>();
            static_Elements = new List<Static_Element>();
            fly_Elements = new List<Fly_Element>();
            npcs = new List<NPC>();
            propertys = new List<Property>();
            vanes = new List<Vane>();
            traps = new List<Trap>();
            players = new List<Player>();
            balloons = new List<Balloon>();
            build_Slaves = new List<Build_Slave>();
            primers = new List<Primer>();
            floats = new List<Float>();
            gates = new List<Gate>();
            stabbers = new List<Stabber>();
            spouts = new List<Spout>();
            barriers = new List<Barrier>();
            player_Bases = new List<Player_Base>();
        }
        public void Init()
        {
            Clear();
        }

        public void Clear()
        {
            backGrounds.Clear();
            static_Elements.Clear();
            fly_Elements.Clear();
            npcs.Clear();
            propertys.Clear();
            vanes.Clear();
            traps.Clear();
            players.Clear();
            balloons.Clear();
            build_Slaves.Clear();
            primers.Clear();
            floats.Clear();
            gates.Clear();
            stabbers.Clear();
            spouts.Clear();
            barriers.Clear();
            player_Bases.Clear();
        }
        public void Load()
        {
            string path = @"D:\vs2015.data\PiaoPiao\PiaoPiao\balloon\BalloonElemnet.xml";
            if (!File.Exists(path))
            {
                MessageBox.Show("无法打开: " + path, "ERROR");
                return;
            }

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            Load_BackGrounds(root.Element("BackGrounds"));
            Load_Static_Element(root.Element("Static_Element"));
            Load_Fly_Element(root.Element("Fly_Element"));
            Load_NPCs(root.Element("NPCs"));
            Load_Propertys(root.Element("Propertys"));
            Load_Vanes(root.Element("Vanes"));
            Load_Traps(root.Element("Traps"));
            Load_Players(root.Element("Players"));
            Load_Balloons(root.Element("Balloons"));
            Load_Build_Slave(root.Element("Build_Slave"));
            Load_Primers(root.Element("Primers"));
            Load_Floats(root.Element("Floats"));
            Load_Gates(root.Element("Gates"));
            Load_Stabbers(root.Element("Stabbers"));
            Load_Spouts(root.Element("Spouts"));
            Load_Barrier(root.Element("Barrier"));
            Load_Player_Bases(root.Element("Player_Bases"));

        }
        private void Load_BackGrounds(XElement items)
        {
            BackGround t;
            foreach (XElement item1 in items.Elements())
            {
                t = new BackGround();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.index = int.Parse(item1.Attribute("Index").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    t.pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                    t.pic.ID = int.Parse(item2.Attribute("ID").Value);
                }
                backGrounds.Add(t);
            }
        }
        private void Load_Static_Element(XElement items)
        {
            Static_Element t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Static_Element();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.x = Try_Get_Attribute_Value(item2.Attribute("x"));
                        pic.y = Try_Get_Attribute_Value(item2.Attribute("y"));
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        t.pics.Add(pic);
                    }
                    else if (item2.Name == "Point")
                    {
                        Point point = new Point();
                        point.X = int.Parse(item2.Attribute("X").Value);
                        point.Y = int.Parse(item2.Attribute("Y").Value);
                        t.points.Add(point);
                    }
                }
                static_Elements.Add(t);
            }
        }
        private void Load_Fly_Element(XElement items)
        {
            Fly_Element t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Fly_Element();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                t.floatType = int.Parse(item1.Attribute("FloatType").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        t.pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        t.pic.ID = int.Parse(item2.Attribute("ID").Value);
                    }
                    else if (item2.Name == "Rect")
                    {
                        Rect rect = new Rect();
                        int left = int.Parse(item2.Attribute("Left").Value);
                        int top = int.Parse(item2.Attribute("Top").Value);
                        int right = int.Parse(item2.Attribute("Right").Value);
                        int bottom = int.Parse(item2.Attribute("Bottom").Value);
                        rect.rect = new RectangleF(left, top, right - left, bottom - top);
                        rect.property = int.Parse(item2.Attribute("Property").Value);
                        t.rects.Add(rect);
                    }
                }
                fly_Elements.Add(t);
            }
        }
        private void Load_NPCs(XElement items)
        {
            NPC t;
            foreach (XElement item1 in items.Elements())
            {
                t = new NPC();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                t.weight = int.Parse(item1.Attribute("Weight").Value);
                t.life = int.Parse(item1.Attribute("Life").Value);
                t.attack = int.Parse(item1.Attribute("Attack").Value);
                t.speed = int.Parse(item1.Attribute("Speed").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        t.pics.Add(pic);
                    }
                    else
                    {
                        MessageBox.Show("Value" + item2.Name, "ERROR_LOAD_NPC");
                    }
                }
                npcs.Add(t);
            }
        }
        private void Load_Propertys(XElement items)
        {
            Property t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Property();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    t.pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                    t.pic.ID = int.Parse(item2.Attribute("ID").Value);
                }
                propertys.Add(t);
            }
        }
        private void Load_Vanes(XElement items)
        {
            Vane t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Vane();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    t.pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                    t.pic.ID = int.Parse(item2.Attribute("ID").Value);
                }
                vanes.Add(t);
            }
        }
        private void Load_Traps(XElement items)
        {
            Trap t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Trap();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                t.radius = int.Parse(item1.Attribute("Radius").Value);
                t.angleBegin = Try_Get_Attribute_Value(item1.Attribute("AngleBegin"));
                t.angleEnd = Try_Get_Attribute_Value(item1.Attribute("AngleEnd"));
                t.bstRepeat = int.Parse(item1.Attribute("BstRepeat").Value);
                t.bstActionNum = int.Parse(item1.Attribute("BstActionNum").Value);
                t.sprTime = int.Parse(item1.Attribute("SprTime").Value);
                t.bstTime = int.Parse(item1.Attribute("BstTime").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        t.pics.Add(pic);
                    }
                    else
                    {
                        MessageBox.Show("Value:" + item2.Name, "ERROR_LOAD_Traps");
                    }
                }
                traps.Add(t);
            }
        }
        private void Load_Players(XElement items)
        {
            Player t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Player();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    t.pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                    t.pic.ID = int.Parse(item2.Attribute("ID").Value);
                }
                players.Add(t);
            }
        }
        private void Load_Balloons(XElement items)
        {
        }
        private void Load_Build_Slave(XElement items)
        {
        }
        private void Load_Primers(XElement items)
        {
            Primer t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Primer();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    t.pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                    t.pic.ID = int.Parse(item2.Attribute("ID").Value);
                }
                primers.Add(t);
            }

        }
        private void Load_Floats(XElement items)
        {
            Float t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Float();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        pic.x = Try_Get_Attribute_Value(item2.Attribute("X"));
                        pic.y = Try_Get_Attribute_Value(item2.Attribute("Y"));
                        t.pics.Add(pic);
                    }
                    else
                    {
                        MessageBox.Show("Value" + item2.Name, "ERROR_LOAD_Float");
                    }
                }
                floats.Add(t);
            }
        }
        private void Load_Gates(XElement items)
        {
            Gate t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Gate();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        pic.x = Try_Get_Attribute_Value(item2.Attribute("X"));
                        pic.y = Try_Get_Attribute_Value(item2.Attribute("Y"));
                        t.pics.Add(pic);
                    }
                    else if (item2.Name == "Rect")
                    {
                        Rect rect = new Rect();
                        int left = int.Parse(item2.Attribute("Left").Value);
                        int top = int.Parse(item2.Attribute("Top").Value);
                        int right = int.Parse(item2.Attribute("Right").Value);
                        int bottom = int.Parse(item2.Attribute("Bottom").Value);
                        rect.rect = new RectangleF(left, top, right - left + 1, bottom - top + 1);
                        rect.property = int.Parse(item2.Attribute("Property").Value);
                        t.rects.Add(rect);
                    }
                    else
                    {
                        MessageBox.Show("Value:" + item2.Name, "ERROR_LOAD_Gate");
                    }
                }
                gates.Add(t);
            }
        }
        private void Load_Stabbers(XElement items)
        {
            Stabber t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Stabber();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                t.weight = Try_Get_Attribute_Value(item1.Attribute("Weight"));
                t.sprTime = int.Parse(item1.Attribute("SprTime").Value);
                t.bstTime = int.Parse(item1.Attribute("BstTime").Value);
                t.interval = Try_Get_Attribute_Value(item1.Attribute("Interval"));
                t.speed = Try_Get_Attribute_Value(item1.Attribute("Speed"));
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        t.pics.Add(pic);
                    }
                    else
                    {
                        MessageBox.Show("Value:" + item2.Name, "ERROR_LOAD_Stabber");
                    }
                }
                stabbers.Add(t);
            }
        }
        private void Load_Spouts(XElement items)
        {
            Spout t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Spout();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        pic.x = Try_Get_Attribute_Value(item2.Attribute("X"));
                        pic.y = Try_Get_Attribute_Value(item2.Attribute("Y"));
                        t.pics.Add(pic);
                    }
                    else
                    {
                        MessageBox.Show("Value:" + item2.Name, "ERROR_LOAD_Gate");
                    }
                }
                spouts.Add(t);
            }
        }
        private void Load_Barrier(XElement items)
        {
            Barrier t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Barrier();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                t.type = int.Parse(item1.Attribute("Type").Value);
                t.sprTime = int.Parse(item1.Attribute("SprTime").Value);
                t.bstTime = int.Parse(item1.Attribute("BstTime").Value);
                t.pastTime = int.Parse(item1.Attribute("PastTime").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        Pic pic = new Pic();
                        pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        pic.ID = int.Parse(item2.Attribute("ID").Value);
                        pic.x = Try_Get_Attribute_Value(item2.Attribute("X"));
                        pic.y = Try_Get_Attribute_Value(item2.Attribute("Y"));
                        t.pics.Add(pic);
                    }
                    else
                    {
                        MessageBox.Show("Value:" + item2.Name, "ERROR_LOAD_Traps");
                    }
                }
                barriers.Add(t);
            }
        }
        private void Load_Player_Bases(XElement items)
        {
            Player_Base t;
            foreach (XElement item1 in items.Elements())
            {
                t = new Player_Base();
                t.elementID = int.Parse(items.Attribute("ID").Value);
                t.ID = int.Parse(item1.Attribute("ID").Value);
                foreach (XElement item2 in item1.Elements())
                {
                    if (item2.Name == "Pic")
                    {
                        t.pic.typeID = int.Parse(item2.Attribute("TypeID").Value);
                        t.pic.ID = int.Parse(item2.Attribute("ID").Value);
                    }
                    else if (item2.Name == "Point")
                    {
                        Point point = new Point();
                        point.X = int.Parse(item2.Attribute("X").Value);
                        point.Y = int.Parse(item2.Attribute("Y").Value);
                        t.points.Add(point);
                    }
                    else
                    {
                        MessageBox.Show("Value:", "ERROR_Load_Player_Base");
                    }
                }
                player_Bases.Add(t);
            }
        }
        private int Try_Get_Attribute_Value(XAttribute t)
        {
            if (t == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(t.Value);
            }
        }
    }
    public class ItemPic
    {
        public int ID = 0;
        public string name = "ERROR";
        public List<ItemPics> itemPics;

        public ItemPic()
        {
            OnCreate();
        }
        public void OnCreate()
        {
            itemPics = new List<ItemPics>();
        }
        public void Init()
        {
            ID = 0;
            name = "ERROR";
            itemPics.Clear();
        }
    }
    public class SpritePic
    {
        public int ID = 0;
        public string name = "ERROR";
        public List<SpritePics> itemPics;

        public SpritePic()
        {
            OnCreate();
        }
        public void OnCreate()
        {
            itemPics = new List<SpritePics>();
        }
        public void Init()
        {
            ID = 0;
            name = "ERROR";
            itemPics.Clear();
        }
    }
    public class Map
    {
        //<夺宝地图 ID="2" Show="1" FileIn="Pic\\Prepare\\夺宝地图栏+.bmp" FileOut="Pic\\Prepare\\夺宝地图栏-.bmp">

        public string name = "";
        public int ID = 0;
        public int show = 1;
        public string fileIn = "";
        public string fileOut = "";
        public List<Maps> maps;

        public Map()
        {
            OnCreate();
        }
        public void OnCreate()
        {
            maps = new List<Maps>();
            maps.Add(null); //第一个为空
        }
    }
    public class ShopItem
    {
        public string equip_name = "Error";
        public string str_ShuoMing = "null";
        public int equip_ID = 0;
        public int equip_type = 0;
        public int use_time = 0;
        public int price_Q = 0;
        public int price_Piao = 0;
        public int pic_type = 0;
        public int pic_ID = 0;

        public int flag = 0;
        public bool Is_XuFei = false;
        public bool Is_TuiJian = false;
    }

}
