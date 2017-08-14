using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Room
{
    public class Rooms
    {
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

        public List<byte[]> GetSendData_Player(int poi)
        {
            var list = players[poi].GetSendData_All();
            list.Insert(0, GetSendData_CopySit(poi));
            return list;
        }
        public byte[] GetSendData_CopySit(int poi)
        {
            //设置复制位置
            //5
            byte[] bye = new byte[5 + 10];
            int index = 9;

            System.Buffer.BlockCopy(BitConverter.GetBytes(5), 0, bye, index, 4);
            index += 4;

            bye[index] = (byte)poi;
            return bye;
        }
        public byte[] GetSendData_Prop()
        {
            //传送房间属性
            //1005
            byte[] bye = new byte[10 + 10];
            int index = 9;

            System.Buffer.BlockCopy(BitConverter.GetBytes(1005), 0, bye, index, 4);
            index += 4;

            bye[index + 0] = (byte)ID;
            bye[index + 1] = (byte)soutai;
            bye[index + 2] = (byte)Boss;
            bye[index + 3] = (byte)mode;
            bye[index + 4] = (byte)mapTypeID;
            bye[index + 5] = (byte)mapID;
            return bye;
        }

        public void SetSendData(byte[] bye, int poi = -1)
        {
            if (poi == -1) poi = copySit;
            switch (System.BitConverter.ToInt32(bye, 9))
            {
                case 1000:
                case 1001:
                case 1002:
                case 1003:
                case 1004:
                    SetPlayer(bye, poi);
                    break;
                case 1005:
                    SetSendData_Prop(bye);
                    break;
                default:
                    break;
            }
        }
        public void SetSendData_Prop(byte[] bye)
        {
            //传送房间属性
            //1005
            int index = 9;

            //校验指令
            //t = System.BitConverter.ToInt32(bye, index);
            index += 4;

            //读数据
            ID = bye[index + 0];
            soutai = bye[index + 1];
            Boss = bye[index + 2];
            mode = bye[index + 3];
            mapTypeID = bye[index + 4];
            mapID = bye[index + 5];
        }
        public void SetPlayer(byte[] bye, int poi = 0)
        {
            if (0 <= poi && poi <= 5)
                players[poi].SetSendData(bye);
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
        public List<byte[]> GetSendData_All()
        {
            if (player == null)
            {
                var rlist = new List<byte[]>();
                rlist.Add(GetSendData_Prop());
                return rlist;
            }
            var list = player.GetSendData_All();
            list.Insert(0, GetSendData_Prop());
            return list;
        }
        public byte[] GetSendData_Prop()
        {
            //传送玩家房间属性
            //1004
            byte[] bye = new byte[8 + 10];
            int index = 9;

            System.Buffer.BlockCopy(BitConverter.GetBytes(1004), 0, bye, index, 4);
            index += 4;

            bye[index] = (player == null) ? (byte)0 : (byte)1;
            bye[index + 1] = (enabled) ? (byte)0 : (byte)1;
            bye[index + 2] = (IsReady) ? (byte)0 : (byte)1;
            bye[index + 3] = (byte)team;
            return bye;
        }

        public void SetSendData(byte[] bye)
        {
            switch (System.BitConverter.ToInt32(bye, 9))
            {
                case 1000:
                case 1001:
                case 1002:
                case 1003:
                    SetPlayer(bye);
                    break;
                case 1004:
                    SetSendData_Prop(bye);
                    break;
                default:
                    break;
            }
        }
        public void SetSendData_Prop(byte[] bye)
        {
            //传送玩家房间属性
            //1004
            int index = 9;

            //校验指令
            //t = System.BitConverter.ToInt32(bye, index);
            index += 4;

            //读数据
            if (bye[index] == 0) player = null;
            enabled = (bye[index + 1] == 1) ? true : false;
            IsReady = (bye[index + 2] == 1) ? true : false;
            team = bye[index + 3];
        }
        public void SetPlayer(byte[] bye)
        {
            if (player == null) player = new PlayerData();
            player.SetSendData(bye);
        }
    }
}
