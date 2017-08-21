using ClientPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Room
{
    public class Rooms
    {
        public ClientS clientS;
        public Room_Players[] players = new Room_Players[6];

        public string name = "一起玩吧！";
        public int ID = 027;
        public int soutai = 0;
        public int Boss = 0;
        public int mode = 2;
        public int mapTypeID = 6;
        public int mapID = 5;
        public bool IsGame = false;

        public bool Islock = false;
        public bool IsWS = false;
        public bool modeFree = true;
        public string password = "";

        public Rooms()
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

            if (players[Boss].player == null)
            {
                Boss = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (players[i].player != null)
                    {
                        Boss = i;
                        break;
                    }
                }
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
        public int AddPlayer(PlayerData player)
        {
            if (soutai > 1) return -1;
            for (int i = 0; i < 6; i++)
            {
                if (players[i].player == null && players[i].enabled == true)
                {
                    player.roomSit = i;
                    players[i].player = player;
                    players[i].team = 1;
                    players[i].IsReady = false;
                    players[i].player.roomSit = i;
                    return i;
                }
            }
            ResetSoutai();
            return -2;
        }
        public void DelPlayer(int poi)
        {
            players[poi].Reset();
            ResetSoutai();

            //如果退出的是房主，重置房主
            if (poi == Boss)
            {
                Boss = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (players[i].player != null)
                    {
                        Boss = i;
                        break;
                    }
                }
            }
        }
        public string GetMapName()
        {
            /*
            var maps = Global.GetXml().maps;
            if (1 < mapTypeID && mapTypeID < maps.Count)
            {
                if (0 < mapID && mapID < maps[mapTypeID].maps.Count)
                {
                    return maps[mapTypeID].maps[mapID].name;
                }
            }
            if (mapTypeID == 1) return "随机地图";
            */
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

        public ClientData GetSendData_All()
        {
            //传送所有信息
            //类型 + 基础数据
            int len = 4 + ((name.Length) * 2 + 4) + 4 * 6 + 4 + 6 * 6;
            byte[] byt = new byte[len];
            int index = 0;

            GlobalC.AddSendData_Int((int)(ClientData.CLIENT_DATA_TYPE.DATA_ROOM), byt, ref index);
            GlobalC.AddSendData_Str(name, byt, ref index);

            GlobalC.AddSendData_Int(ID, byt, ref index);
            GlobalC.AddSendData_Int(soutai, byt, ref index);
            GlobalC.AddSendData_Int(Boss, byt, ref index);
            GlobalC.AddSendData_Int(mode, byt, ref index);
            GlobalC.AddSendData_Int(mapTypeID, byt, ref index);
            GlobalC.AddSendData_Int(mapID, byt, ref index);

            GlobalC.AddSendData_Bool(IsGame, byt, ref index);
            GlobalC.AddSendData_Bool(Islock, byt, ref index);
            GlobalC.AddSendData_Bool(IsWS, byt, ref index);
            GlobalC.AddSendData_Bool(modeFree, byt, ref index);

            for (int i = 0; i < 6; i++)
            {
                GlobalC.AddSendData_Bool(players[i].enabled, byt, ref index);
                GlobalC.AddSendData_Bool(players[i].IsReady, byt, ref index);
                GlobalC.AddSendData_Int(players[i].team, byt, ref index);
            }

            ClientData dat = new ClientData();
            dat.Type = ClientData.CLIENT_TYPE.SEND;
            dat.Data = byt;

            return dat;
        }

        public void DealSendData(int sit)
        {
            ClientData dat;
            while (true)
            {
                lock (Client.lockRecvList)
                {
                    var list = clientS.GetDataList(sit);
                    if (list == null) return;
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
                DealSendData(dat, sit);
            }
        }
        private void DealSendData(ClientData dat, int sit)
        {
            var byt = dat.Data;
            int index = 0;
            var typ = (ClientData.CLIENT_DATA_TYPE)GlobalC.GetSendData_Int(byt, ref index);
            switch (typ)
            {
                case ClientData.CLIENT_DATA_TYPE.ADD_PLAYER:
                    DealSendData_AddPlayer(byt, sit);
                    break;
                case ClientData.CLIENT_DATA_TYPE.MAP_CHANGE:
                    DealSendData_MapChange(byt, sit);
                    break;
                case ClientData.CLIENT_DATA_TYPE.TEAM_CHANGE:
                    DealSendData_TeamChange(byt, sit);
                    break;
                case ClientData.CLIENT_DATA_TYPE.GAME_START:
                    DealSendData_GameStart(byt, sit);
                    break;
                case ClientData.CLIENT_DATA_TYPE.INPUT:
                    DealSendData_Input(byt, sit);
                    break;
                case ClientData.CLIENT_DATA_TYPE.CHANGE_DELAY:
                    DealSendData_ChangeDelay(dat, sit);
                    break;
                case ClientData.CLIENT_DATA_TYPE.CHANGE_ROOM_PREP:
                    DealSendData_ChangeRoomPrep(byt,sit);
                    break;
                case ClientData.CLIENT_DATA_TYPE.CHANGE_PLAYER_TYPE:
                    DealSendData_ChangePlayerType(byt, sit);
                    break;
                default:
                    break;
            }
        }
        private void DealSendData_AddPlayer(byte[] byt, int sit)
        {
            //玩家接入
            int index = 4;
            var playerR = players[sit];
            playerR.player = new PlayerData();
            playerR.player.SetSendData_All(byt, ref index);
            playerR.player.roomSit = sit;
            playerR.team = 1;
            playerR.IsReady = false;
            ResetSoutai();

            SendData_All();
            SendData_PlayerAll();
        }
        private void DealSendData_MapChange(byte[] byt, int sit)
        {
            //判断是否为房主
            if (sit != Boss) return;

            //读取地图编号
            int index = 4;
            int mapIndex = GlobalC.GetSendData_Int(byt, ref index);

            //更新地图
            SetMapIndex(mapIndex);

            //重新发送房间状态
            var dat = GetSendData_All();
            clientS.AddData(dat);
        }
        private void DealSendData_TeamChange(byte[] byt, int sit)
        {
            //读取队伍编号
            int index = 4;
            int TeamIndex = GlobalC.GetSendData_Int(byt, ref index);

            //更新队伍
            SetRoomTeam(sit,TeamIndex);

            //重新发送房间状态
            var dat = GetSendData_All();
            clientS.AddData(dat);
        }
        private void DealSendData_GameStart(byte[] byt,int sit)
        {
            //游戏开始
            ClientData dat = new ClientData();
            dat.CreateGameStart();
            clientS.AddData(dat);
        }
        private void DealSendData_Input(byte[] byt, int sit)
        {
            //键盘输入
            ClientData dat = new ClientData();
            dat.CreateInput(byt, sit);
            clientS.AddData(dat);
        }
        private void DealSendData_ChangeDelay(ClientData dat,int sit)
        {
            //判断是否为房主
            if (sit != Boss) return;

            clientS.AddData(dat);
        }
        private void DealSendData_ChangeRoomPrep(byte[] byt,int sit)
        {
            //判断是否为房主
            if (sit != Boss) return;

            //读取房间属性
            int index = 4;
            IsWS = GlobalC.GetSendData_Bool(byt, ref index);
            modeFree = GlobalC.GetSendData_Bool(byt, ref index);

            //重新发送房间状态
            var dat = GetSendData_All();
            clientS.AddData(dat);
        }
        private void DealSendData_ChangePlayerType(byte[] byt, int sit)
        {
            //读取玩家类型
            int index = 4;
            int typ = GlobalC.GetSendData_Int(byt, ref index);
            if (typ > 0 && typ <= 6)
            {
                if(players[sit].player != null)
                {
                    players[sit].player.type = typ;
                }
            }

            //重新发送房间玩家状态
            SendData_PlayerAll();
        }

        public void SendData_All()
        {
            var dat = GetSendData_All();
            clientS.AddData(dat);
        }
        public void SendData_PlayerAll()
        {
            for (int i = 0; i < 6; i++)
            {
                if (players[i].player != null)
                {
                    clientS.AddData(players[i].player.GetSendData_All_Change());
                }
                else
                {
                    var dat = new ClientData();
                    dat.CreateDelPlayer(i);
                    clientS.AddData(dat);
                }
            }
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
}
