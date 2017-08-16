using ClientPublic;
using Data.Globals;
using Room;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Rooms
{
    public class Room
    {
        public ClientC clientC; //FormGame初始化
        public RoomInputManager inputManager = new RoomInputManager();
        public Room_Players[] players = new Room_Players[6];

        public string name = "大家一起踩~~";
        public int ID = 1;
        public int soutai = 0;
        public int Boss = 0;
        public int mode = 0;
        public int mapTypeID = 1;
        public int mapID = 1;
        public bool IsGame = false;

        public int copySit = 0;
        public bool Islock = false;
        public bool IsWS = false;
        public bool modeFree = true;
        public string password = "";

        public Room()
        {
            for (int i = 0; i < 6; i++)
            {
                players[i] = new Room_Players();
            }
        }
        public void CheckError()
        {
            for (int i = 0; i < 6; i++)
            {
                if (players[i].player != null)
                {
                    players[i].player.roomSit = i;
                    players[i].player.roomID = ID;
                }
            }
            ResetSoutai();
        }
        public void ResetSoutai()
        {
            if (IsAllReady())
            {
                soutai = 3;
                return;
            }
            if (GetPlayerNum() == 0)
            {
                soutai = 0;
                return;
            }
            if (GetPlayerNum() == GetRoomMax())
            {
                soutai = 2;
            }
            else
            {
                soutai = 1;
            }
        }
        public int GetRoomMax()
        {
            //返回房间上限人数
            int max = 0;
            for (int i = 0; i < 6; i++)
            {
                if (players[i].enabled) max++;
            }
            return max;
        }
        public int GetPlayerNum()
        {
            int num = 0;
            for (int i = 0; i < 6; i++)
            {
                if (players[i].player != null) num++;
            }
            return num;
        }
        public string GetModeString()
        {
            string[] names = new string[6]
            {
                "随机地图",
                "机械城",
                "玩具城",
                "竞技场",
                "七号遗迹",
                "夺旗战",
            };
            return names[mode];
        }
        public int GetReadyNum()
        {
            int t = 0;
            for (int i = 0; i < 6; i++)
            {
                if (players[i].IsReady)
                {
                    t++;
                }
            }
            return t;
        }
        public string GetRoomType()
        {
            if (modeFree) return "自由模式";
            return "标准模式";
        }
        public bool IsAllReady()
        {
            return (GetReadyNum() == GetRoomMax());
        }
        public int GetAddPlayerSit()
        {
            int sit = 100;
            if (soutai > 1) return sit;
            for (int i = 0; i < 6; i++)
            {
                if (players[i].player == null && players[i].enabled == true)
                {
                    return i;
                }
            }
            return sit;
        }
        public void AddPlayer(PlayerData player)
        {
            if (soutai > 1) return;
            for (int i = 0; i < 6; i++)
            {
                if (players[i].player == null && players[i].enabled == true)
                {
                    player.roomSit = i;
                    players[i].player = player;
                    players[i].team = 1;
                    players[i].IsReady = false;
                    players[i].player.roomSit = i;
                    break;
                }
            }
            ResetSoutai();
        }
        public void DelPlayer(int poi)
        {
            players[poi].Reset();
            ResetSoutai();
        }
        public string GetMapName()
        {
            
            var maps = Global.GetXmlManager().maps;
            if (1 < mapTypeID && mapTypeID < maps.Count)
            {
                if (0 < mapID && mapID < maps[mapTypeID].maps.Count)
                {
                    return maps[mapTypeID].maps[mapID].name;
                }
            }
            if (mapTypeID == 1) return "随机地图";
            
            return "Er:" + mapTypeID + "," + mapID;
        }
        public void SetMapIndex(int index)
        {
            //随机，对抗，竞速，竞技，夺宝，夺旗
            int[] mode_poi = new int[] { 0, 4, 3, 1, 1, 2, 5 };
            mapTypeID = index / 100;
            mapID = index % 100;
            mode = mode_poi[mapTypeID - 1];
        }
        public void SetRoomTeam(int sit, int team)
        {
            players[sit].team = team;
        }

        public PlayerData GetPlayer(int i)
        {
            return players[i].player;
        }
        public int GetTeam(int i)
        {
            return players[i].team;
        }

        public void DealSendData(List<ClientData> list)
        {
            ClientData dat;
            if (list == null) return;
            while (true)
            {
                lock (list)
                {
                    if (list.Count > 0)
                    {
                        dat = list[0];
                        list.RemoveAt(0);
                    }
                    else
                    {
                        return;
                    }
                }
                DealSendData(dat);
            }
        }
        private void DealSendData(ClientData dat)
        {
            var byt = dat.Data;
            int index = 0;
            var typ = (ClientData.CLIENT_DATA_TYPE)GlobalC.GetSendData_Int(byt, ref index);
            switch (typ)
            {
                case ClientData.CLIENT_DATA_TYPE.DATA_PLAYER:
                    DealSendData_ChangePlayer(byt);
                    break;
                case ClientData.CLIENT_DATA_TYPE.DATA_ROOM:
                    DealSendData_ChangeRoom(byt);
                    break;
                case ClientData.CLIENT_DATA_TYPE.DEL_PLAYER:
                    DealSendData_DelPlayer(byt);
                    break;
                case ClientData.CLIENT_DATA_TYPE.INPUT:
                    DealSendData_Input(byt);
                    break;
                case ClientData.CLIENT_DATA_TYPE.GAME_START:
                    DealSendData_GameStart(byt);
                    break;
                default:
                    break;
            }
        }
        private void DealSendData_ChangePlayer(byte[] byt)
        {
            //接收玩家数据
            int index = 4;
            int sit = GlobalC.GetSendData_Int(byt, ref index);
            var player = new PlayerData();
            player.SetSendData_All(byt,ref index);
            players[sit].player = player;
        }
        private void DealSendData_ChangeRoom(byte[] byt)
        {
            //接收房间数据
            int index = 4;

            name = GlobalC.GetSendData_Str(byt, ref index);

            ID = GlobalC.GetSendData_Int(byt, ref index);
            soutai = GlobalC.GetSendData_Int(byt, ref index);
            Boss = GlobalC.GetSendData_Int(byt, ref index);
            mode = GlobalC.GetSendData_Int(byt, ref index);
            mapTypeID = GlobalC.GetSendData_Int(byt, ref index);
            mapID = GlobalC.GetSendData_Int(byt, ref index);

            IsGame = GlobalC.GetSendData_Bool(byt, ref index);
            Islock = GlobalC.GetSendData_Bool(byt, ref index);
            IsWS = GlobalC.GetSendData_Bool(byt, ref index);
            modeFree = GlobalC.GetSendData_Bool(byt, ref index);

            for (int i = 0; i < 6; i++)
            {
                players[i].enabled = GlobalC.GetSendData_Bool(byt, ref index);
                players[i].IsReady = GlobalC.GetSendData_Bool(byt, ref index);
                players[i].team = GlobalC.GetSendData_Int(byt, ref index);
            }

        }
        private void DealSendData_DelPlayer(byte[] byt)
        {
            //玩家退出
            int index = 4;
            int sit = GlobalC.GetSendData_Int(byt, ref index);
            players[sit].player = null;
        }
        private void DealSendData_Input(byte[] byt)
        {
            //键盘输入
            //type + sit + 帧ID + bool[9]
            int index = 4;
            int sit = GlobalC.GetSendData_Int(byt, ref index);
            int ID = GlobalC.GetSendData_Int(byt, ref index);
            bool[] dat = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                dat[i] = GlobalC.GetSendData_Bool(byt, ref index);
            }
            inputManager.SetInput(ID, sit, dat);
        }
        private void DealSendData_GameStart(byte[] byt)
        {
            //游戏开始
            Global.GetWindowsList().CloseAll();
            Global.GetWindowsList().ActiveWindow(1003);
        }

        public int UpdateInput()
        {
            if(Global.IsConnect())
            {
                //游戏是否开始
                if(Global.GetMapManager()!=null)
                {
                    if(inputManager.IsComplete())
                    {
                        //发送到服务端
                        {
                            var dat = new ClientData();
                            dat.CreateInput(inputManager.GetTime(),Global.GetInput().GetKeyBool());
                            Global.GetClientC().AddData(dat);
                        }
                        
                        var rep = Global.GetReplayManager();
                        var sw = rep.GetSw();
                        if(inputManager.IsBegin())
                        {
                            var player = Global.GetPlayer();
                            string path = GlobalB.GetRootPath() + @"\Setting\PlayerData\" + (player.GetQQ() + "_" + player.GetExID() + @"\Replay");
                            if(Directory.Exists(path) == false)
                            {
                                Directory.CreateDirectory(path);
                            }
                            var time = DateTime.Now;
                            string timeStr = "" + time.Year + time.Month + time.Day + "_" + time.Hour + time.Minute + time.Second;
                            path = path + "\\" + timeStr + ".rep";
                            rep.Start(path);
                            sw = rep.GetSw();
                            sw.WriteLine("" + GetPlayerNum());

                            //
                            bool[] dat = new bool[6];
                            for (int i = 0; i < 6; i++)
                            {
                                dat[i] = (players[i].player != null);
                            }
                            inputManager.SetPlayer(dat);
                        }
                        int index = 0;
                        for (int i = 0; i < 6; i++)
                        {
                            if(inputManager.IsIn(i))
                            {
                                var player = players[i].player;
                                if (player != null)
                                {
                                    var inp = inputManager.GetInput(i);
                                    {
                                        //player.input.UpdateKey(inp);
                                        //？？？？
                                        //创建的player有误？
                                        Global.GetMapManager().pla.list[index].player.input.UpdateKey(inp);
                                        index++;
                                    }
                                    for (int j = 0; j < 9; j++)
                                    {
                                        if(inp[j])
                                        {
                                            sw.Write('1');
                                        }
                                        else
                                        {
                                            sw.Write('0');
                                        }
                                    }
                                    sw.Write(' ');
                                }
                            }
                        }
                        sw.WriteLine();
                        //sw.Flush();
                        inputManager.Update();
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else
            {
                //本地更新
                Global.GetPlayer().input.UpdateKey(Global.GetInput().GetKeyBool());
            }
            return 0;
        }
    }
    public class Room_Players
    {
        public PlayerData player = null;
        public bool enabled = true;
        public bool IsReady = false;
        public int team = 0;

        public void Reset()
        {
            player = null;
            enabled = true;
            IsReady = false;
            team = 0;
        }
    }
    public class RoomInputManager
    {
        const int delayFps = 5; //延迟帧
        const int LenMax = 10; //最大储存量
        private int timeFps = 0; //第0帧开始
        public List<InputFps> inputFps;
        public static bool[] player;

        public RoomInputManager()
        {
            Reset();
        }
        public void Reset()
        {
            timeFps = 0;
            if (inputFps != null)
                inputFps.Clear();
            inputFps = new List<InputFps>();
            for (int i = 0; i < delayFps; i++)
            {
                inputFps.Add(new InputFps(false));
            }
            for (int i = delayFps; i < LenMax; i++)
            {
                inputFps.Add(new InputFps());
            }
            player = new bool[6] { false, false, false, false, false, false };
        }
        public bool IsIn(int sit)
        {
            return player[sit];
        }
        public bool IsComplete()
        {
            if (inputFps.Count == 0) return false;
            return inputFps[0].IsComplete();
        }
        public void SetInput(int ID,int sit,bool[] dat)
        {
            ID = ID - timeFps + delayFps;
            if(ID<0&&ID>=LenMax)
            {
                return;
            }
            lock(inputFps)
            {
                inputFps[ID].SetInput(sit, dat);
            }
        }
        public bool[] GetInput(int sit)
        {
            if(inputFps.Count>0)
            {
                return inputFps[0].GetInput(sit);
            }
            else
            {
                return new bool[9]
                {
                    false,false,false,
                    false,false,false,
                    false,false,false
                };
            }
        }
        public bool IsBegin()
        {
            return timeFps == 0;
        }
        public int GetPlayerNum()
        {
            int ret = 0;
            for (int i = 0; i < 6; i++)
            {
                if (player[i])
                    ret++;
            }
            return ret;
        }
        public void Update()
        {
            timeFps++;
            lock(inputFps)
            {
                inputFps.RemoveAt(0);
                inputFps.Add(new InputFps());
            }
        }           
        public void SetPlayer(bool[] dat)
        {
            player = dat;
        }
        public int GetTime()
        {
            return timeFps;
        }

        public class InputFps
        {
            private bool[][] input;

            public InputFps()
            {
                input = new bool[6][];
            }
            public InputFps(bool x)
            {
                input = new bool[6][];
                for (int i = 0; i < 6; i++)
                {
                    input[i] = new bool[9];
                    for (int j = 0; j < 9; j++)
                    {
                        input[i][j] = false;
                    }
                }
            }

            public void SetInput(int sit, bool[] dat)
            {
                input[sit] = dat;
            }
            public bool IsComplete()
            {
                for (int i = 0; i < 6; i++)
                {
                    if(player[i] && input[i] == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            public bool[] GetInput(int sit)
            {
                return input[sit];
            }
        }
    }
}
