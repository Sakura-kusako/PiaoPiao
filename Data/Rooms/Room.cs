using ClientPublic;
using Data.Globals;
using Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Rooms
{
    public class Room
    {
        public ClientC clientC; //FormGame初始化
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
