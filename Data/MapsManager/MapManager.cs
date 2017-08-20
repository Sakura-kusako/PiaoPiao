using Data.Globals;
using Data.PPDevices;
using Data.Resources;
using Data.XML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//pic.xml
// 1.backGround     大背景
// 2.Static_Element 不动的图片（平台、背景、建筑……）
// 3.Fly_Element    会动的图片（竞速电球、钉板、平台、气球、探照灯……）
// 4.Npcs           NPC的飞行和死亡，共6张图
// 5.Propertys      游戏过程中可以捡的道具( 随机生成 )
// 6.Vanes          竞速地图中那个会转的风车，共1张图
// 7.Traps          陷阱（冰霜、蜘蛛、火焰）
// 8.Players        玩家（图片使用itemPic）
// 9.balloons       ( 空的 )
// 10.Build_Slave   鱼齿轮，共1张图（啥？）
// 11.Primers       终点钥匙、终点圣杯，共2张图
// 12.Floats        大小图片都有（大中小云、飞船……）（都是装饰）
// 13.Gates         竞速赛的电门，共6张图
// 14.Stabbers      陷阱（大锤、大炮、激光）
// 15.Player_Bases  夺旗战大本营，共2张图（坦克、章鱼艇）

//Element.xml
//
// element文件，加载图片文件pic.xml
//
// 1.BackGrounds    大背景（backGround x1）
// 2.Static_Element 不动的东西（Static_Element x1）（坐标链表:通常没有、有的有若干个 X Y 信息）（57号的写法有点奇怪）
// 3.Fly_Element    会动的东西（Fly_Element x1）（判定框链表:有若干个不同的判定框信息、有的没有）
// 4.NPCs           速度和生命值分别为123的3种NPC，使用图片链表包含图片为12、34、56
// 5.Propertys      道具（Propertys x1）
// 6.Vanes          风车（Vanes X1）
// 7.Traps          3种陷阱（Traps x图片链表）
// 8.Players        （Players x1）
// 9.balloons       （空的）
// 10.Build_Slave   （空的）
// 11.Primers       （记忆中游戏里面没有这个东西）（共3个）
// 12.Floats        （装饰，除了第一个有点奇怪，其他都是Float x1）
// 13.Gates         只有一个（四个判定框，7张图）
// 14.Stabbers      3种陷阱（Stabbers x图片链表）
// 15.Spouts        2个夺宝产生宝石的地方，4个夺旗水晶产生/消失的地方
// 16.Barrier       1个，激光陷阱专用，（Stabbers x图片链表）
// 17.Player_Bases  2个（坐标 x4，Player_Bases x1）
//
// 注:只有电门和部分 Fly_Element 设置特别设置了判定框
//

//map.xml
//
// 地图文件，加载element.xml
//


namespace Data.MapsManager
{
    public class MapManager
    {
        public string name = "";
        public int soutai = 1;
        public int situation = 0; //0正常，1九秒倒计时，2结束准备，3结算，4退出
        public int situation_time = 0;
        public float x = 0;
        public float y = 0;
        public float width = 800;
        public float height = 1000;
        public int type = 0;
        public int camera_type = 1;
        public int time = 0;
        public int count = 0;
        public RectangleF PhyRect = new RectangleF(0, 0, 800, 600);
        public BalloonItemPic_Base pic = null;

        public Element_Propertys property = new Element_Propertys();
        public Element_Spouts spout = new Element_Spouts();
        public Element_Static_Elements statics = new Element_Static_Elements();
        public Element_Floats floats = new Element_Floats();
        public Element_Traps traps = new Element_Traps();
        public Element_Stabbers stabbers = new Element_Stabbers();
        public Element_Players players = new Element_Players();
        public Element_Maps maps = new Element_Maps();

        public Layer[] layers;
        public Layer layer_poi;
        public List_Other oth;
        public List_Static sta;
        public List_Float flo;
        public List_Player pla;
        public List_Map map;
        public MapManager(Maps map)
        {
            Init_Map_Base(map);
            Init_Map_XML(map);
            Init_Layer();
            Init_Camara();
        }
        public void Init_Map_Base(Maps map)
        {
            SpriteMap.SpriteID = 0;
            name = map.name;
            x = 0;
            y = 0;
            type = map.gameMode;
            width = map.width;
            height = map.height;
            time = map.gameTime * 60;
            if (map.name == "test")
            {
                camera_type = 0;
                PhyRect = new RectangleF(0, 0, 330, 350);
            }
            else
            {
                camera_type = Global.GetPlayer().roomSit + 1;
                PhyRect = new RectangleF(0, 0, width, height);
            }

            pic = Global.GetResManager().GetMapPic2(1, map.backGround);
            layers = new Layer[7]
            {
                new Layer(),
                new Layer(),
                new Layer(),
                new Layer(),
                new Layer(),
                new Layer(),
                new Layer()
            };
            layer_poi = new Layer();
            oth = new List_Other();
        }
        public void Init_Map_XML(Maps map)
        {
            foreach (var e in map.elements)
            {
                if (e.name == "Map")
                {
                    maps.Init(e.id);
                    continue;
                }
                switch (e.id)
                {
                    case 2:
                        statics.Init(e);
                        break;
                    case 5:
                        property.Init(e);
                        break;
                    case 7:
                        traps.Init(e);
                        break;
                    case 8:
                        players.Init(e);
                        break;
                    case 12:
                        floats.Init(e);
                        break;
                    case 14:
                        stabbers.Init(e);
                        break;
                    case 15:
                        spout.Init(e);
                        break;
                    default:
                        break;
                }
            }

        }
        public void Init_Camara()
        {
            var player = Global.GetPlayer();
            if (pla != null)
            {
                if (pla.list != null)
                {
                    for (int i = 0; i < pla.list.Count; i++)
                    {
                        var pp = pla.list[i];
                        if (pp.player != null)
                        {
                            var p = pp.player;
                            if (p.GetExID() == player.GetExID() && p.GetQQ() == player.GetQQ())
                            {
                                camera_type = i+1;
                                pp.IsCamaraFocus = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (camera_type >= 1 && camera_type <= 6)
            {
                Set_Camera(pla.list[camera_type - 1].x - 50, pla.list[camera_type - 1].y - 50);
            }

        }
        public void Init_Layer()
        {
            {
                if (type == 3)
                {
                    Init_Layer_DuoBao();
                }
                else if (type == 1)
                {
                    Init_Layer_JingSu();
                }
                else if (type == 0)
                {
                    Init_Layer_Shop();
                }
                else
                {
                    //Data.msg = "ERROR：不存在的游戏模式";
                }
            }
        }
        public void Init_Layer_DuoBao()
        {
            Init_Layer_Static();
            Init_Layer_Floats();
            Init_Layer_Spout();
            //Init_Layer_Traps();
            //Init_Layer_Stabbers();
        }
        public void Init_Layer_JingSu()
        {
            Init_Layer_Players();
            //Init_Layer_Static();
            //Init_Layer_Floats();
            Init_Layer_Maps();
            Global.PlayBgm(Sounds.Sound.BGM_ID.JINGSU);
        }
        public void Init_Layer_Shop()
        {
            Init_Layer_Players();
        }

        public void Init_Layer_Static()
        {
            this.sta = new List_Static();
            var list = this.sta.list;
            foreach (var ele in statics.elements.elements_base)
            {
                var sprites = layers[ele.layer].sprites;
                var sta = Global.GetXmlManager().elements.static_Elements[ele.id - 1];
                foreach (var pic in sta.pics)
                {
                    var pic2 = Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);
                    var sp = new SpriteStatic();
                    sp.type = 2;
                    sp.typeID = ele.id;
                    sp.x = ele.x + pic.x;
                    sp.y = ele.y + pic.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    sp.OnCreate();
                    sprites.Add(sp);
                    list.Add(sp);
                }
                /*
                if (sta.points.Count == 0)
                {
                    var sp = new SpriteStatic();
                    sp.x = ele.x;
                    sp.y = ele.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    sp.OnCreate();
                    sprites.Add(sp);
                    list.Add(sp);
                }
                else
                {
                    foreach (var point in sta.points)
                    {
                        var sp = new SpriteStatic();
                        sp.x = ele.x + point.X;
                        sp.y = ele.y + point.Y;
                        sp.pic = pic2;
                        sp.layer = ele.layer;
                        sp.OnCreateCenter();
                        sprites.Add(sp);
                        list.Add(sp);
                    }
                }
                */
            }
        }
        public void Init_Layer_Floats()
        {
            this.flo = new List_Float();
            var list = this.flo.list;
            if (floats.elements == null) return;
            foreach (var ele in floats.elements.elements_base)
            {
                var sprites = layers[ele.layer].sprites;
                var sta = Global.GetXmlManager().elements.floats[ele.id - 1];
                foreach (var pic in sta.pics)
                {
                    var pic2 =Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);
                    var sp = new SpriteFloat();
                    sp.type = 12;
                    sp.vx = ele.speed / 60.0f;
                    sp.x = ele.x + pic2.x;
                    sp.y = ele.y + pic2.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    sp.OnCreate();
                    sprites.Add(sp);
                    list.Add(sp);
                }
            }
        }
        public void Init_Layer_Spout()
        {
            if (spout.elements == null) return;
            foreach (var ele in spout.elements.elements_base)
            {
                var sprites = layers[ele.layer].sprites;
                var sta = Global.GetXmlManager().elements.spouts[ele.id - 1];
                foreach (var pic in sta.pics)
                {
                    var pic2 =Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);
                    var sp = new SpriteMap();
                    sp.type = 15;
                    sp.x = ele.x + pic2.x;
                    sp.y = ele.y + pic2.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    sp.OnCreate();
                    sprites.Add(sp);
                }
            }
        }
        public void Init_Layer_Traps()
        {
            if (traps.elements == null) return;
            foreach (var ele in traps.elements.elements_base)
            {
                var sprites = layers[ele.layer].sprites;
                var sta = Global.GetXmlManager().elements.traps[ele.id - 1];
                foreach (var pic in sta.pics)
                {
                    var pic2 =Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);
                    var sp = new SpriteMap();
                    sp.x = ele.x + pic2.x;
                    sp.y = ele.y + pic2.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    sp.OnCreate();
                    sprites.Add(sp);
                    //break;
                }
            }
        }
        public void Init_Layer_Players()
        {
            this.pla = new List_Player();
            var room = Global.GetRoom();
            var list = this.pla.list;

            if (players.elements == null)
            {
                if (this.name == "test")
                {
                    var sprites = layers[3].sprites;
                    var sp = new SpritePlayer();
                    sp.x = 0;
                    sp.y = 0;
                    sp.type = 8;
                    sp.width = 100;
                    sp.height = 100;
                    sp.pic_dx = 0;
                    sp.pic_dy = 0;
                    sp.layer = 3;
                    sp.player = Global.GetPlayer();
                    sp.BaseCreate();
                    list.Add(sp);
                    sprites.Add(sp);
                }
                return;
            }

            var eles = players.elements.elements_base;

            for (int i = 0; i < eles.Count; i++)
            {
                if (room.GetPlayer(i) == null) continue;

                var ele = eles[i];
                var sprites = layers[ele.layer].sprites;
                var sp = new SpritePlayer();
                sp.x = ele.x;
                sp.y = ele.y;
                sp.type = 8;
                //sp.x = 0;
                //sp.y = 0;
                sp.width = 100;
                sp.height = 100;
                sp.pic_dx = 0;
                sp.pic_dy = 0;
                sp.layer = ele.layer;
                sp.player = room.GetPlayer(i);
                sp.BaseCreate();
                list.Add(sp);
                sprites.Add(sp);
            }
        }
        public void Init_Layer_Stabbers()
        {
            if (stabbers.elements == null) return;
            foreach (var ele in stabbers.elements.elements_base)
            {
                var sprites = layers[ele.layer].sprites;
                var sta = Global.GetXmlManager().elements.stabbers[ele.id - 1];
                foreach (var pic in sta.pics)
                {
                    var pic2 =Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);
                    var sp = new SpriteMap();
                    sp.x = ele.x + pic2.x;
                    sp.y = ele.y + pic2.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    sp.OnCreate();
                    sprites.Add(sp);
                    break;
                }
            }
        }
        public void Init_Layer_Maps()
        {
            float dst_x = 0; //竞速地图偏移，每800像素一张小地图
            var list = Global.GetXmlManager().maps; //获取地图列表
            this.map = new List_Map(); //新建地图
            foreach (var m in maps.elements)
            {
                //每800像素遍历加载小地图
                map.Add_Map(list[8].maps[m], dst_x);
                dst_x += 800;
            }

            //将地图sprite添加到图层列表
            foreach (var l in map.list)
            {
                foreach (var sp in l.sta)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
                foreach (var sp in l.flo)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
                foreach (var sp in l.fly)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
                foreach (var sp in l.bar)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
                foreach (var sp in l.pro)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
                foreach (var sp in l.van)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
                foreach (var sp in l.gat)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
                foreach (var sp in l.pri)
                {
                    layers[sp.layer].sprites.Add(sp);
                }
            }
        }
        public void Update_Static()
        {
            if (sta != null)
            {
                if (sta.list != null)
                {
                    var list = sta.list;
                    foreach (var sp in list)
                    {
                        sp.Action();
                    }
                }
            }
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.sta)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Float()
        {
            if (flo != null)
            {
                if (flo.list != null)
                {
                    var list = flo.list;
                    foreach (var sp in list)
                    {
                        sp.Action();
                    }
                }
            }
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.flo)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Fly()
        {
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.fly)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Barrier()
        {
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.bar)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Player()
        {
            if (pla != null)
            {
                if (pla.list != null)
                {
                    var list = pla.list;
                    foreach (var sp in list)
                    {
                        sp.Action();
                    }
                }
            }
        }
        public void Update_Property()
        {
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.pro)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Vane()
        {
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.van)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Gate()
        {
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.gat)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Primer()
        {
            if (map != null)
            {
                if (map.list != null)
                {
                    var list = map.list;
                    foreach (var l in list)
                    {
                        foreach (var sp in l.pri)
                        {
                            sp.Action();
                        }
                    }
                }
            }
        }
        public void Update_Camera()
        {
            if (camera_type >= 1 && camera_type <= 6)
            {
                var rect = pla.list[camera_type - 1].GetPhyAABB();
                Set_Camera_Target((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2);
            }
        }
        public void Update_Other()
        {
            if (oth != null)
            {
                if (oth.list != null)
                {
                    var list = oth.list;
                    foreach (var sp in list)
                    {
                        sp.Action();
                    }
                }
            }
        }
        public RectangleF GetPicArea()
        { //源图片范围
            return new RectangleF(0, 0, width, height);
        }
        public RectangleF GetShowArea()
        { //显示在屏幕上的实际位置
            var rect = Global.GetCamara();
            if (type == 1)
            {
                return new RectangleF(x, y, 800, 1000);
            }
            else
            {
                return new RectangleF(x - rect.left, y - rect.top, width, height);
            }
        }
        public void Draw(PPDevice ppDevice)
        {
            ppDevice.BitBlt(pic, GetShowArea().X, GetShowArea().Y, GetPicArea(), 1.0f);
            foreach (Layer layer in layers)
            {
                layer.Draw(ppDevice);
            }
            if (Global.IsDebug)
            {
                //Draw_debug_poi(ppDevice);
            }
        }
        public void Update()
        {
            if (soutai == 0) return;
            if (situation == 1)
            {
                //十秒倒计时
                if (situation_time > 0)
                {
                    situation_time--;
                }
                else
                {
                    ChangeToEndWait();
                }
            }
            else if (situation == 2)
            {
                //等待结算
                if (situation_time > 0)
                {
                    situation_time--;
                }
                else
                {
                    ChangeToJieSuan();
                }
                return;
            }
            else if(situation == 3)
            {
                if (situation_time > 0)
                {
                    situation_time--;
                }
                else
                {
                    ChangeToDel();
                }
                return;
            }
            else if(situation == 4)
            {
                Global.SetMapManager(null);
                Global.GetXmlManager().OnCreate();
                Global.GetXmlManager().Load();
                Global.GetResManager().Clear();
                Global.GetResManager().OnCreate();
                Global.GetResManager().LoadItemPic();
                Global.GetResManager().LoadPic();
                Global.GetWindowsList().CloseAll();
                Global.GetWindowsList().ActiveWindow(19);
                return;
            }

            Update_Static();
            Update_Float();
            Update_Fly();
            Update_Player();
            Update_Camera();
            Update_Barrier();
            Update_Property();
            Update_Vane();
            Update_Gate();
            Update_Primer();
            Update_Other();

            switch (type)
            {
                case 1:
                    UpdateJudgementJingSu();
                    UpdateJingSu();
                    break;
                default:
                    break;
            }

            Delete_Sprite();
            
            count++;
            if (time > 0)
            {
                time--;
                if (time == 600 && situation==0)
                {
                    ChangeToEnd();
                }
            }
        }
        public void UpdateJudgementJingSu()
        {
            bool check = true;
            while (check)
            {
                while (check)
                {
                    Do_Judgement_Player();
                    check = Judgement_Player_Player();
                }
                Judgement_Player_Map();
                Do_Judgement_Player();
                Judgement_Player_Map();
                Do_Judgement_Player();

                Judgement_Player_MapRect();
                Do_Judgement_Player();
                Judgement_Player_MapRect();
                Do_Judgement_Player();
                check = Judgement_Player_Player();
            }
        }
        public void UpdateJingSu()
        {
            //终点右坐标为410
            //超过400结束
            if (pla != null)
            {
                if (pla.list != null)
                {
                    var list = pla.list;
                    foreach (var sp in list)
                    {
                        if(sp.x <= 350)
                        {
                            //有人到达终点开始读秒
                            sp.IsCtrlAble = false;
                            if(situation == 0)
                            {
                                ChangeToEnd();
                            }
                        }
                    }
                }
            }
        }
        public void ChangeToEnd()
        {
            situation = 1;
            situation_time = 600;
        }
        public void ChangeToEndWait()
        {
            situation = 2;
            situation_time = 400;
            if (pla != null)
            {
                if (pla.list != null)
                {
                    var list = pla.list;
                    foreach (var sp in list)
                    {
                        if (sp.x <= 400)
                        {
                            sp.IsMoveAble = false;
                        }
                    }
                }
            }
            Global.GetReplayManager().End();
            Global.GetRoom().inputManager.Reset();
        }
        public void ChangeToJieSuan()
        {
            situation = 3;
            situation_time = 0;
        }
        public void ChangeToDel()
        {
            situation = 4;
        }
                       
        public void Judgement_Player_MapRect()
        {
            if (type == 0)
            {
                Judgement_Player_MapRect_Shop();
            }
            else
            {
                Judgememt_Player_MapRect_JingSu();
            }
        }
        private void Judgement_Player_MapRect_Shop()
        {
            var phyRect = PhyRect;
            foreach (var player in pla.list)
            {
                var list = player.GetPhyList();
                foreach (var playerRect in list)
                {
                    var dLeft = playerRect.Left - phyRect.Left;
                    var dRight = playerRect.Right - phyRect.Right;
                    var dTop = playerRect.Top - phyRect.Top;
                    var dBottom = playerRect.Bottom - phyRect.Bottom;

                    if (dLeft < 0)
                    {
                        player.x -= 2 * dLeft;
                        player.vx = Math.Abs(player.vx);
                    }
                    if (dRight > 0)
                    {
                        player.x -= 2 * dRight;
                        player.vx = -Math.Abs(player.vx);
                    }
                    if (dTop < 0)
                    {
                        player.y -= 2 * dTop;
                        player.vy = Math.Abs(player.vy);
                    }
                    if (dBottom > 0)
                    {
                        var dx = 0.08f;
                        if (Math.Abs(player.vx) < dx)
                        {
                            player.vx = 0;
                        }
                        else if (player.vx < 0)
                        {
                            player.vx += dx;
                        }
                        else
                        {
                            player.vx -= dx;
                        }

                        var dy = -0.2f + 1.3f * dBottom;
                        if (dy < dBottom) dy = dBottom;
                        player.y -= dy;
                        player.vy = -0.3f * Math.Abs(player.vy);
                    }
                }
                if (player.y < 0)
                {

                }
            }
        }
        private void Judgememt_Player_MapRect_JingSu()
        {
            var MapRect = new RectangleF[4]
            {
                new RectangleF(PhyRect.Left,PhyRect.Top,0,PhyRect.Height),
                new RectangleF(PhyRect.Right,PhyRect.Top,0,PhyRect.Height),
                new RectangleF(PhyRect.Left,PhyRect.Top,PhyRect.Width,0),
                new RectangleF(PhyRect.Left,PhyRect.Bottom,PhyRect.Width,0),
            };
            foreach (var player in pla.list)
            {
                var list = player.GetPhyList();
                foreach (var playerRect in list)
                {
                    foreach (var phyRect in MapRect)
                    {
                        var results = Physics.Rect_Rect_V(phyRect, playerRect, player.vx, player.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到列表
                            player.judgeList.Add(new SpritePlayerJudgement(results, new SpriteMap()));
                        }
                    }
                }
            }
        }
        private void Judgement_Player_Map()
        {
            if (map == null) return;
            foreach (var player in pla.list)
            {
                if (player.pyhEnable == false) continue;
                var pla_AABB = player.GetPhyAABB();
                foreach (var m in map.list)
                {
                    var map_AABB = m.GetAABB();
                    map_AABB.Width = 1600;
                    if (Physics.Rect_Rect(pla_AABB, map_AABB))
                    {
                        if (player.player.roomSit == camera_type - 1)
                        {
                            //Poi.MessageBox(m.name);
                        }
                        Judgement_Player_Staitc(player, m.sta);
                        Judgement_Player_Fly(player, m.fly);
                        Judgement_Player_Barrier(player, m.bar);
                        Judgement_Player_Property(player, m.pro);
                        Judgement_Player_Vane(player, m.van);
                        Judgement_Player_Gate(player, m.gat);
                        Judgement_Player_Primer(player, m.pri);
                    }
                }
            }
        }
        private void Judgement_Player_Staitc(SpritePlayer player, List<SpriteStatic> ele_sta)
        {
            var pla_AABB_list = player.GetPhyList();
            List<SpritePlayerJudgement> list_j = new List<SpritePlayerJudgement>();

            foreach (var sta in ele_sta)
            {
                var sta_AABB_list = sta.GetPhyList();
                foreach (var pla_AABB in pla_AABB_list)
                {
                    if (sta_AABB_list == null) continue;
                    foreach (var sta_AABB in sta_AABB_list)
                    {
                        //获取碰撞信息
                        var results = Physics.Rect_Rect_V(sta_AABB, pla_AABB, player.vx, player.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到临时列表
                            list_j.Add(new SpritePlayerJudgement(results, sta));
                        }
                    }
                }
            }
            if (list_j.Count > 0)
                player.judgeList.Add(Global.GetMin(list_j));
        }
        private void Judgement_Player_Fly(SpritePlayer player, List<SpriteFly> ele_fly)
        {
            var pla_AABB_list = player.GetPhyList();
            List<SpritePlayerJudgement> list_j = new List<SpritePlayerJudgement>();

            foreach (var fly in ele_fly)
            {
                var fly_AABB_list = fly.GetPhyList();
                foreach (var pla_AABB in pla_AABB_list)
                {
                    if (fly_AABB_list == null) continue;
                    foreach (var fly_AABB in fly_AABB_list)
                    {
                        //获取碰撞信息
                        var results = Physics.Rect_Rect_V_V(fly_AABB, pla_AABB, player.vx, player.vy, fly.vx, fly.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到临时列表
                            list_j.Add(new SpritePlayerJudgement(results, fly));
                        }
                    }
                }
            }
            if (list_j.Count > 0)
                player.judgeList.Add(Global.GetMin(list_j));
        }
        private void Judgement_Player_Barrier(SpritePlayer player, List<SpriteBarrier> ele_fly)
        {
            if (player.atkEnable == false) return;
            var pla_AABB_list = player.GetPhyList();
            List<SpritePlayerJudgement> list_j = new List<SpritePlayerJudgement>();

            foreach (var fly in ele_fly)
            {
                var fly_AABB_list = fly.GetPhyList();
                foreach (var pla_AABB in pla_AABB_list)
                {
                    if (fly_AABB_list == null) continue;
                    foreach (var fly_AABB in fly_AABB_list)
                    {
                        //获取碰撞信息
                        var results = Physics.Rect_Rect_V_V(fly_AABB, pla_AABB, player.vx, player.vy, fly.vx, fly.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到列表
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                        else if (Physics.Rect_Rect(fly_AABB, pla_AABB))
                        {
                            //添加到碰撞信息到列表
                            results = new JudgeData(5, 0, 0, 0, 0);
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                    }
                }
            }
        }
        private void Judgement_Player_Property(SpritePlayer player, List<SpriteProperty> ele_fly)
        {
            if (player.atkEnable == false) return;
            var pla_AABB_list = player.GetPhyList();
            List<SpritePlayerJudgement> list_j = new List<SpritePlayerJudgement>();

            foreach (var fly in ele_fly)
            {
                var fly_AABB_list = fly.GetPhyList();
                foreach (var pla_AABB in pla_AABB_list)
                {
                    if (fly_AABB_list == null) continue;
                    foreach (var fly_AABB in fly_AABB_list)
                    {
                        //获取碰撞信息
                        var results = Physics.Rect_Rect_V_V(fly_AABB, pla_AABB, player.vx, player.vy, fly.vx, fly.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到列表
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                        else if (Physics.Rect_Rect(fly_AABB, pla_AABB))
                        {
                            //添加到碰撞信息到列表
                            results = new JudgeData(5, 0, 0, 0, 0);
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                    }
                }
            }
        }
        private void Judgement_Player_Vane(SpritePlayer player, List<SpriteVane> ele_fly)
        {
            if (player.atkEnable == false) return;
            var pla_AABB_list = player.GetPhyList();
            List<SpritePlayerJudgement> list_j = new List<SpritePlayerJudgement>();

            foreach (var fly in ele_fly)
            {
                var fly_AABB_list = fly.GetPhyList();
                foreach (var pla_AABB in pla_AABB_list)
                {
                    if (fly_AABB_list == null) continue;
                    foreach (var fly_AABB in fly_AABB_list)
                    {
                        //获取碰撞信息
                        var results = Physics.Rect_Rect_V_V(fly_AABB, pla_AABB, player.vx, player.vy, fly.vx, fly.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到列表
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                        else if (Physics.Rect_Rect(fly_AABB, pla_AABB))
                        {
                            //添加到碰撞信息到列表
                            results = new JudgeData(5, 0, 0, 0, 0);
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                    }
                }
            }
        }
        private void Judgement_Player_Gate(SpritePlayer player, List<SpriteGate> ele_fly)
        {
            var pla_AABB_list = player.GetPhyList();
            List<SpritePlayerJudgement> list_j = new List<SpritePlayerJudgement>();

            foreach (var fly in ele_fly)
            {
                var fly_AABB_list = fly.GetPhyList();
                foreach (var pla_AABB in pla_AABB_list)
                {
                    if (fly_AABB_list == null) continue;
                    foreach (var fly_AABB in fly_AABB_list)
                    {
                        //获取碰撞信息
                        var results = Physics.Rect_Rect_V_V(fly_AABB, pla_AABB, player.vx, player.vy, fly.vx, fly.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到列表
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                        else if (Physics.Rect_Rect(fly_AABB, pla_AABB))
                        {
                            //添加到碰撞信息到列表
                            if ((fly.property & 0x100) > 0 && player.atkEnable)
                            {
                                results = new JudgeData(5, 0, 0, 0, 0);
                                player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                            }
                        }
                    }
                }
            }
        }
        private void Judgement_Player_Primer(SpritePlayer player, List<SpritePrimer> ele_fly)
        {
            if (player.atkEnable == false) return;
            var pla_AABB_list = player.GetPhyList();
            List<SpritePlayerJudgement> list_j = new List<SpritePlayerJudgement>();

            foreach (var fly in ele_fly)
            {
                var fly_AABB_list = fly.GetPhyList();
                foreach (var pla_AABB in pla_AABB_list)
                {
                    if (fly_AABB_list == null) continue;
                    foreach (var fly_AABB in fly_AABB_list)
                    {
                        //获取碰撞信息
                        var results = Physics.Rect_Rect_V_V(fly_AABB, pla_AABB, player.vx, player.vy, fly.vx, fly.vy);
                        if (results != null)
                        {
                            //添加到碰撞信息到列表
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                        else if (Physics.Rect_Rect(fly_AABB, pla_AABB))
                        {
                            //添加到碰撞信息到列表
                            results = new JudgeData(5, 0, 0, 0, 0);
                            player.judgeList.Add(new SpritePlayerJudgement(results, fly));
                        }
                    }
                }
            }
        }
        private bool Judgement_Player_Player()
        {
            if(pla != null)
            {
                if(pla.list != null)
                {
                    for (int i = 0; i < pla.list.Count-1; i++)
                    {
                        var playerA = pla.list[i];
                        for (int j = i; j < pla.list.Count; j++)
                        {
                            var playerB = pla.list[j];
                            if (playerA.Is_DaQi() && playerB.IsNormal())
                            {
                                //B撞飞A打气
                                if (Physics.Rect_Rect(playerA.GetRectBody(), playerB.GetRectBody()))
                                {
                                    playerA.Change_To_ShangTian();
                                    break;
                                }
                            }
                            else if (playerA.IsNormal() && playerB.Is_DaQi())
                            {
                                //A撞飞B打气
                                if(Physics.Rect_Rect(playerA.GetRectBody(), playerB.GetRectBody()))
                                {
                                    playerB.Change_To_ShangTian();                                    
                                }
                            }
                            else if(playerA.IsNormal() && playerB.IsNormal())
                            {
                                //A攻击对B受击
                                var ARectAtk = playerA.GetAttackList();
                                var BRectBalloon = playerB.GetBalloonList();
                                foreach (var rectA in ARectAtk)
                                {
                                    foreach (var rectB in BRectBalloon)
                                    {
                                        //获取碰撞信息
                                        var results = Physics.Rect_Rect_V_V(rectB, rectA, playerA.vx, playerA.vy, playerB.vx, playerB.vy);
                                        if (results != null)
                                        {
                                            //添加到碰撞信息到列表
                                            results.type |= 128;
                                            playerA.judgeList.Add(new SpritePlayerJudgement(results, playerB));
                                            return true;
                                        }
                                    }
                                }

                                //A受击对B攻击
                                var BRectAtk = playerB.GetAttackList();
                                var ARectBalloon = playerA.GetBalloonList();
                                foreach (var rectB in BRectAtk)
                                {
                                    foreach (var rectA in ARectBalloon)
                                    {
                                        //获取碰撞信息
                                        var results = Physics.Rect_Rect_V_V(rectA, rectB, playerB.vx, playerB.vy, playerA.vx, playerA.vy);
                                        if (results != null)
                                        {
                                            //添加到碰撞信息到列表
                                            results.type |= 128;
                                            playerB.judgeList.Add(new SpritePlayerJudgement(results, playerA));
                                            return true;
                                        }
                                    }
                                }

                                //A物理对B物理
                                var APhyList = playerA.GetPhyList();
                                var BPhyList = playerB.GetPhyList();
                                foreach (var rectA in APhyList)
                                {
                                    foreach (var rectB in BPhyList)
                                    {
                                        //获取碰撞信息
                                        var results = Physics.Rect_Rect_V_V(rectB, rectA, playerA.vx, playerA.vy, playerB.vx, playerB.vy);
                                        if (results != null)
                                        {
                                            //添加到碰撞信息到列表
                                            playerA.judgeList.Add(new SpritePlayerJudgement(results, playerB));
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void Delete_Sprite()
        {
            Delete_Sprite_Map();

        }
        private void Delete_Sprite_Map()
        {
            if (map == null) return;
            foreach (var m in map.list)
            {
                for (int i = 0; i < m.fly.Count; i++)
                {
                    if (m.fly[i].IsDel())
                    {
                        //删除图层中的sprite
                        var sp = m.fly[i];
                        var list = this.layers[sp.layer].sprites;
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].ID == sp.ID)
                            {
                                list.RemoveAt(j);
                                break;
                            }
                        }

                        //删除行动链表中的sprite
                        m.fly.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < m.pro.Count; i++)
                {
                    if (m.pro[i].IsDel())
                    {
                        //删除图层中的sprite
                        var sp = m.pro[i];
                        var list = this.layers[sp.layer].sprites;
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].ID == sp.ID)
                            {
                                list.RemoveAt(j);
                                break;
                            }
                        }

                        //删除行动链表中的sprite
                        m.pro.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < m.gat.Count; i++)
                {
                    if (m.gat[i].IsDel())
                    {
                        //删除图层中的sprite
                        var sp = m.gat[i];
                        var list = this.layers[sp.layer].sprites;
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].ID == sp.ID)
                            {
                                list.RemoveAt(j);
                                break;
                            }
                        }

                        //删除行动链表中的sprite
                        m.gat.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < m.pri.Count; i++)
                {
                    if (m.pri[i].IsDel())
                    {
                        //删除图层中的sprite
                        var sp = m.pri[i];
                        var list = this.layers[sp.layer].sprites;
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].ID == sp.ID)
                            {
                                list.RemoveAt(j);
                                break;
                            }
                        }

                        //删除行动链表中的sprite
                        m.pri.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        private void Delete_Sprite_Other()
        {
            if (oth == null) return;
            for (int i = 0; i < oth.list.Count; i++)
            {
                if (oth.list[i].IsDel())
                {
                    //删除图层中的sprite
                    var sp = oth.list[i];
                    var list = this.layers[sp.layer].sprites;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].ID == sp.ID)
                        {
                            list.RemoveAt(j);
                            break;
                        }
                    }

                    //删除行动链表中的sprite
                    oth.list.RemoveAt(i);
                    i--;
                }
            }
        }
        private void Do_Judgement_Player()
        {
            if (pla != null)
            {
                if (pla.list != null)
                {
                    var list = pla.list;
                    foreach (var sp in list)
                    {
                        sp.Do_Judgement();
                    }
                }
            }
        }
        private void Set_Camera_Target(float x, float y)
        {
            var camera = Global.GetCamara();
            RectangleF rect = new RectangleF(
                this.x + camera.width / 2,
                this.y + camera.height / 2,
                this.width - camera.width,
                this.height - camera.height
                );

            if (x < rect.Left)
            {
                x = rect.Left;
            }
            if (x > rect.Right)
            {
                x = rect.Right;
            }
            if (y < rect.Top)
            {
                y = rect.Top;
            }
            if (y > rect.Bottom)
            {
                y = rect.Bottom;
            }
            camera.target_x = x;
            camera.target_y = y;
        }
        private void Set_Camera(float x, float y)
        {
            var camera = Global.GetCamara();
            RectangleF rect = new RectangleF(
                this.x + camera.width / 2,
                this.y + camera.height / 2,
                this.width - camera.width,
                this.height - camera.height
                );

            if (x < rect.Left)
            {
                x = rect.Left;
            }
            if (x > rect.Right)
            {
                x = rect.Right;
            }
            if (y < rect.Top)
            {
                y = rect.Top;
            }
            if (y > rect.Bottom)
            {
                y = rect.Bottom;
            }
            camera.x = x;
            camera.y = y;
        }

        public void AddSprite(SpriteMap sp)
        {
            oth.list.Add(sp);
            layers[sp.layer].sprites.Add(sp);
        }
    }
    public class Element_Propertys
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Spouts
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Static_Elements
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Fly_Elements
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Floats
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Traps
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Stabbers
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Players
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Barrier
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Vanes
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Gates
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }
    public class Element_Primers
    {
        public Maps_Element elements = null;
        public void Init(Maps_Element e)
        {
            elements = e;
        }
    }

    public class Element_Maps
    {
        public List<int> elements = null;
        public void Init(int e)
        {
            if (elements == null) elements = new List<int>();
            elements.Add(e);
        }
    }

    public class List_Static
    {
        public List<SpriteStatic> list;
        public List_Static()
        {
            list = new List<SpriteStatic>();
        }
    }
    public class List_Float
    {
        public List<SpriteFloat> list;
        public List_Float()
        {
            list = new List<SpriteFloat>();
        }
    }
    public class List_Player
    {
        public List<SpritePlayer> list;
        public List_Player()
        {
            list = new List<SpritePlayer>();
        }
    }
    public class List_Other
    {
        public List<SpriteMap> list;
        public List_Other()
        {
            list = new List<SpriteMap>();
        }
    }
    public class List_Map
    {
        public List<Map_Single> list;
        public Element_Static_Elements statics;
        public Element_Floats floats;
        public Element_Fly_Elements flys;
        public Element_Barrier barriers;
        public Element_Propertys propertys;
        public Element_Vanes vanes;
        public Element_Gates gates;
        public Element_Primers primers;

        public List_Map()
        {
            list = new List<Map_Single>();
            statics = new Element_Static_Elements();
            floats = new Element_Floats();
            flys = new Element_Fly_Elements();
            barriers = new Element_Barrier();
            propertys = new Element_Propertys();
            vanes = new Element_Vanes();
            gates = new Element_Gates();
            primers = new Element_Primers();
        }
        public void Add_Map(Maps map, float dst_x)
        {
            Load_Element(map);
            var m = new Map_Single();
            m.id = map.id;
            m.name = map.name;
            m.dst_x = dst_x;
            Init_Layer_Static(m);
            Init_Layer_Float(m);
            Init_Layer_Fly(m);
            Init_Layer_Barrier(m);
            Init_Layer_Property(m);
            Init_Layer_Vane(m);
            Init_Layer_Gate(m);
            Init_Layer_Primer(m);
            Check_Gate_Primer(m);
            list.Add(m);
            Ele_Clear();
        }
        private void Ele_Clear()
        {
            if (statics.elements != null)
                statics.elements.elements_base.Clear();
            if (floats.elements != null)
                floats.elements.elements_base.Clear();
            if (flys.elements != null)
                flys.elements.elements_base.Clear();
            if (barriers.elements != null)
                barriers.elements.elements_base.Clear();
            if (propertys.elements != null)
                propertys.elements.elements_base.Clear();
            if (vanes.elements != null)
                vanes.elements.elements_base.Clear();
            if (gates.elements != null)
                gates.elements.elements_base.Clear();
            if (primers.elements != null)
                primers.elements.elements_base.Clear();
        }
        public void Load_Element(Maps map)
        {
            foreach (var e in map.elements)
            {
                switch (e.id)
                {
                    case 2:
                        statics.Init(e);
                        break;
                    case 3:
                        flys.Init(e);
                        break;
                    case 5:
                        propertys.Init(e);
                        break;
                    case 6:
                        vanes.Init(e);
                        break;
                    case 11:
                        primers.Init(e);
                        break;
                    case 12:
                        floats.Init(e);
                        break;
                    case 13:
                        gates.Init(e);
                        break;
                    case 16:
                        barriers.Init(e);
                        break;
                    default:
                        break;
                }
            }
        }
        public void Init_Layer_Static(Map_Single m)
        {
            var list = m.sta;

            foreach (var ele in statics.elements.elements_base)
            {
                var sta = Global.GetXmlManager().elements.static_Elements[ele.id - 1];
                bool poi = true;

                foreach (var pic in sta.pics)
                {
                    var pic2 =Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);
                    var sp = new SpriteStatic();
                    sp.type = 2;
                    sp.typeID = ele.id;
                    sp.x = ele.x + pic.x + m.dst_x;
                    sp.y = ele.y + pic.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    if (poi)
                    {
                        poi = false;
                        sp.phyList = Global.GetRectList_StaticElement(sta.type, sta.points);
                    }
                    sp.OnCreate();
                    list.Add(sp);
                }
            }
        }
        public void Init_Layer_Float(Map_Single m)
        {
            var list = m.flo;
            if (floats.elements == null) return;
            foreach (var ele in floats.elements.elements_base)
            {
                var flo = Global.GetXmlManager().elements.floats[ele.id - 1];
                foreach (var pic in flo.pics)
                {
                    var pic2 =Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);
                    var sp = new SpriteFloat();
                    sp.type = 12;
                    sp.vx = ele.speed / 60.0f;
                    sp.x = ele.x + pic2.x + m.dst_x;
                    sp.y = ele.y + pic2.y;
                    sp.pic = pic2;
                    sp.layer = ele.layer;
                    sp.OnCreate();
                    list.Add(sp);
                }
            }
        }
        public void Init_Layer_Fly(Map_Single m)
        {
            var list = m.fly;
            if (flys.elements == null) return;
            foreach (var ele in flys.elements.elements_base)
            {
                var fly = Global.GetXmlManager().elements.fly_Elements[ele.id - 1];
                var pic = fly.pic;
                List<RectangleF> rects1 = new List<RectangleF>();
                List<RectangleF> rects2 = new List<RectangleF>();
                int rect_pro1 = 0;
                int rect_pro2 = 0;
                int rect_len = 0;

                foreach (var rect in fly.rects)
                {
                    if (rect_len == 0)
                    {
                        rect_pro1 = rect.property;
                        rects1.Add(rect.rect);
                        rect_len++;
                    }
                    else if (rect_len == 1)
                    {
                        if (rect_pro1 == rect.property)
                        {
                            rects1.Add(rect.rect);
                        }
                        else
                        {
                            rect_pro2 = rect.property;
                            rects2.Add(rect.rect);
                            rect_len++;
                        }
                    }
                    else if (rect_len == 2)
                    {
                        if (rect_pro1 == rect.property)
                        {
                            rects1.Add(rect.rect);
                        }
                        else if (rect_pro2 == rect.property)
                        {
                            rects2.Add(rect.rect);
                        }
                    }
                }

                if (pic != null)
                {
                    var pic2 =Global.GetResManager().GetMapPic2(pic.typeID, pic.ID);

                    for (int i = 0; i < rect_len || i == 0; i++)
                    {
                        var sp = new SpriteFly();
                        if (rect_len == 0)
                        {
                            sp.phyList = null;
                        }
                        else if (i == 0)
                        {
                            sp.phyList = rects1;
                            sp.property = rect_pro1;
                        }
                        else if (i == 1)
                        {
                            sp.phyList = rects2;
                            sp.property = rect_pro2;
                            sp.Poi_A = 1;
                        }
                        sp.flyType = ele.flyType;
                        sp.type = 3;
                        sp.x = ele.x + pic2.x + m.dst_x;
                        sp.y = ele.y + pic2.y;
                        sp.vrad = ele.speed / 60.0f;
                        sp.centerX = ele.centerX + pic2.x + m.dst_x;
                        sp.centerY = ele.centerY + pic2.y;
                        sp.radian = ele.radian;
                        sp.radius = ele.radius;
                        sp.left = ele.left + m.dst_x;
                        sp.right = ele.right + m.dst_x;
                        sp.top = ele.top;
                        sp.bottom = ele.bottom;
                        sp.time = ele.time * 60;
                        sp.pastTime = ele.pastTime * 60;
                        sp.interVal1 = ele.interVal1 * 60;
                        sp.interVal2 = ele.interVal2 * 60;
                        sp.layer = ele.layer;
                        sp.pic = pic2;
                        sp.OnCreate();
                        sp.CheckError();
                        list.Add(sp);
                    }
                }
            }
        }
        public void Init_Layer_Barrier(Map_Single m)
        {
            var list = m.bar;
            if (barriers.elements == null) return;
            foreach (var ele in barriers.elements.elements_base)
            {
                var bar = Global.GetXmlManager().elements.barriers[ele.id - 1];
                var sp = new SpriteBarrier();
                sp.type = 16;
                sp.flyType = ele.flyType;
                sp.x = ele.x + m.dst_x;
                sp.y = ele.y;
                sp.layer = ele.layer;
                sp.left = ele.left + m.dst_x;
                sp.right = ele.right + m.dst_x;
                sp.top = ele.top;
                sp.bottom = ele.bottom;
                sp.dirLeft = ele.dirLeft;
                sp.dirRight = ele.dirRight;
                sp.dirTop = ele.dirTop;
                sp.dirBottom = ele.dirBottom;
                sp.time = ele.time * 60;
                sp.pastTime = ele.pastTime * 60;
                sp.bstTime = ele.bstTime * 60;
                sp.sprTime = ele.sprTime * 60;
                sp.offsetTime = ele.offsetTime * 60;

                foreach (var pic in bar.pics)
                {
                    sp.picList.Add(Global.GetResManager().GetMapPic2(pic));
                }
                sp.OnCreate();
                sp.CheckError();
                list.Add(sp);
            }
        }
        public void Init_Layer_Property(Map_Single m)
        {
            var list = m.pro;
            if (propertys.elements == null) return;
            foreach (var ele in propertys.elements.elements_base)
            {
                var pro = Global.GetXmlManager().elements.propertys[ele.id - 1];
                var pic2 =Global.GetResManager().GetMapPic2(pro.pic);
                var sp = new SpriteProperty();
                sp.type = 5;
                sp.vy = ele.speed / 60.0f;
                sp.vx = 0;
                sp.flyAreaX = ele.flyAreaX;
                sp.x = ele.x + pic2.x + m.dst_x;
                sp.y = ele.y + pic2.y;
                sp.pic = pic2;
                sp.layer = ele.layer;
                sp.OnCreate();
                sp.CheckError();
                list.Add(sp);
            }
        }
        public void Init_Layer_Vane(Map_Single m)
        {
            var list = m.van;
            if (vanes.elements == null) return;

            foreach (var ele in vanes.elements.elements_base)
            {
                var van = Global.GetXmlManager().elements.vanes[ele.id - 1];
                var pic =Global.GetResManager().GetMapPic2(van.pic);
                var sp = new SpriteVane();
                sp.type = 6;
                sp.typeID = ele.id;
                sp.x = ele.x + pic.x + m.dst_x;
                sp.y = ele.y + pic.y;
                sp.pic = pic;
                sp.layer = ele.layer;
                sp.OnCreate();
                sp.CheckError();
                list.Add(sp);
            }
        }
        public void Init_Layer_Gate(Map_Single m)
        {
            var list = m.gat;
            if (gates.elements == null) return;
            foreach (var ele in gates.elements.elements_base)
            {
                var gat = Global.GetXmlManager().elements.gates[ele.id - 1];
                var pics = gat.pics;
                List<RectangleF> rects1 = new List<RectangleF>();
                List<RectangleF> rects2 = new List<RectangleF>();
                int rect_pro1 = 0;
                int rect_pro2 = 0;
                int rect_len = 0;

                foreach (var rect in gat.rects)
                {
                    if (rect_len == 0)
                    {
                        rect_pro1 = rect.property;
                        rects1.Add(rect.rect);
                        rect_len++;
                    }
                    else if (rect_len == 1)
                    {
                        if (rect_pro1 == rect.property)
                        {
                            rects1.Add(rect.rect);
                        }
                        else
                        {
                            rect_pro2 = rect.property;
                            rects2.Add(rect.rect);
                            rect_len++;
                        }
                    }
                    else if (rect_len == 2)
                    {
                        if (rect_pro1 == rect.property)
                        {
                            rects1.Add(rect.rect);
                        }
                        else if (rect_pro2 == rect.property)
                        {
                            rects2.Add(rect.rect);
                        }
                    }
                }

                if (pics != null)
                {
                    var picList = new List<BalloonItemPic_Base>();
                    foreach (var pic in pics)
                    {
                        picList.Add(Global.GetResManager().GetMapPic2(pic));
                    }

                    for (int i = 0; i < rect_len || i == 0; i++)
                    {
                        var sp = new SpriteGate();
                        if (rect_len == 0)
                        {
                            sp.phyList = null;
                        }
                        else if (i == 0)
                        {
                            sp.phyList = rects1;
                            sp.property = rect_pro1;
                        }
                        else if (i == 1)
                        {
                            sp.phyList = rects2;
                            sp.property = rect_pro2;
                            sp.Poi_A = 1;
                        }
                        sp.type = 13;
                        sp.x = ele.x + m.dst_x;
                        sp.y = ele.y;
                        sp.layer = ele.layer;
                        sp.picList = picList;
                        sp.OnCreate();
                        sp.CheckError();
                        list.Add(sp);
                    }
                }
            }
        }
        public void Init_Layer_Primer(Map_Single m)
        {
            var list = m.pri;
            if (primers.elements == null) return;
            foreach (var ele in primers.elements.elements_base)
            {
                var pri = Global.GetXmlManager().elements.primers[ele.id - 1];
                var pic2 =Global.GetResManager().GetMapPic2(pri.pic);
                var sp = new SpritePrimer();
                sp.type = 11;
                sp.vy = 0;
                sp.vx = 0;
                sp.x = ele.x + m.dst_x;
                sp.y = ele.y;
                sp.pic = pic2;
                sp.layer = ele.layer;
                sp.OnCreate();
                sp.CheckError();
                list.Add(sp);
            }
        }
        public void Check_Gate_Primer(Map_Single m)
        {
            if (m.gat == null) return;
            if (m.gat.Count < 2) return;
            if (m.pri == null) return;
            if (m.pri.Count == 0) return;
            foreach (var pri in m.pri)
            {
                pri.gate1 = m.gat[0];
                pri.gate2 = m.gat[1];
            }
        }
    }
    public class Map_Single
    {
        public int id = 0;
        public float dst_x = 0;
        public string name = "Error";

        public List<SpriteFloat> flo;
        public List<SpriteStatic> sta;
        public List<SpriteFly> fly;
        public List<SpriteBarrier> bar;
        public List<SpriteProperty> pro;
        public List<SpriteVane> van;
        public List<SpriteGate> gat;
        public List<SpritePrimer> pri;

        public RectangleF GetAABB()
        {
            return new RectangleF(dst_x, 0, 800, 600);
        }
        public Map_Single()
        {
            flo = new List<SpriteFloat>();
            sta = new List<SpriteStatic>();
            fly = new List<SpriteFly>();
            bar = new List<SpriteBarrier>();
            pro = new List<SpriteProperty>();
            van = new List<SpriteVane>();
            gat = new List<SpriteGate>();
            pri = new List<SpritePrimer>();
        }
    }
}
