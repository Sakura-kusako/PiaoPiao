using ClientPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Room
{
    public class PlayerData
    {
        const int itemsChaNum = 7;
        const int itemsNum = 44;
        public PlayerInput input = new PlayerInput();

        public string QQ = "88888888";
        public string ExID = "0";
        public string name = "NPC-9972";
        public int exp = 0;
        public int hit = 0;
        public int piaobi = 0;
        public int Qbi = 0;
        public int level = 1;

        public int vs_win = 0;
        public int vs_lose = 0;
        public int vs_peace = 0;
        public int roomID = 0;
        public int roomSit = 0; //0-5
        public int server = 0;

        public int type = 2;
        public int[][] items;

        public PlayerData()
        {
            //构造函数
            items = new int[itemsChaNum][];
            for (int i = 0; i < itemsChaNum; i++)
            {
                items[i] = new int[itemsNum]
                {
                    0,i,i,i,i,    0,0,1,0,0,
                    i,i,0,0,0,    0,1,1,i,3,
                    1,1,1,1,1,    1,1,1,1,1,
                    1,1,1,1,1,    1,1,1,1,1,
                    0,0,0,0
                };
            }
        }

        public string GetQQ()
        {
            return QQ;
        }
        public string GetExID()
        {
            return ExID;
        }
        public string GetName()
        {
            return name;
        }
        public string GetLevel()
        {
            return "" + level;
        }
        public string GetHit()
        {
            return "" + hit;
        }
        public string GetExp()
        {
            return "" + exp;
        }
        public int GetPiaoBi()
        {
            return piaobi;
        }
        public int GetQBi()
        {
            return Qbi;
        }
        public string GetVS()
        {
            return vs_win + "胜 " + vs_lose + "负 " + vs_peace + "平";
        }
        public string GetVS_Win()
        {
            return vs_win + "";
        }
        public string GetVS_Lose()
        {
            return vs_lose + "";
        }
        public string GetVS_Peace()
        {
            return vs_peace + "";
        }
        public string GetTypes()
        {
            return type + "";
        }
        public string GetRoom()
        {
            if (roomID < 0) return "";
            else if (roomID < 10) return "00" + roomID;
            else if (roomID < 100) return "0" + roomID;
            else if (roomID < 1000) return "" + roomID;
            else return "";
        }

        public int GetItemIndex(int id)
        {
            return items[type][id];
        }

        public void Clone(PlayerData player)
        {
            QQ = player.QQ;
            ExID = player.ExID;
            name = player.name;
            exp = player.exp;
            hit = player.hit;
            piaobi = player.piaobi;
            Qbi = player.Qbi;
            level = player.level;
            vs_win = player.vs_win;
            vs_lose = player.vs_lose;
            vs_peace = player.vs_peace;
            roomID = player.roomID;
            roomSit = player.roomSit;
            type = player.type;
            server = player.server;

            for (int i = 0; i < itemsChaNum; i++)
            {
                for (int j = 0; j < itemsNum; j++)
                {
                    items[i][j] = player.items[i][j];
                }
            }
        }

        //传送函数
        public ClientData GetSendData_All()
        {
            //传送所有信息
            //类型 + 基础数据 + 道具
            int len = ((QQ.Length + ExID.Length + name.Length) * 2 + 12) + 4 * 11 + itemsChaNum * itemsNum + 4;
            byte[] byt = new byte[len];
            int index = 0;

            AddSendData_Int((int)(ClientData.CLIENT_DATA_TYPE.ADD_PLAYER), byt, ref index);

            AddSendData_Str(QQ, byt, ref index);
            AddSendData_Str(ExID, byt, ref index);
            AddSendData_Str(name, byt, ref index);

            AddSendData_Int(exp, byt, ref index);
            AddSendData_Int(hit, byt, ref index);
            AddSendData_Int(piaobi, byt, ref index);
            AddSendData_Int(Qbi, byt, ref index);
            AddSendData_Int(level, byt, ref index);

            AddSendData_Int(vs_win, byt, ref index);
            AddSendData_Int(vs_lose, byt, ref index);
            AddSendData_Int(vs_peace, byt, ref index);
            AddSendData_Int(roomID, byt, ref index);
            AddSendData_Int(roomSit, byt, ref index);
            AddSendData_Int(type, byt, ref index);

            AddSendData_Item(byt, ref index);

            ClientData dat = new ClientData();
            dat.Type = ClientData.CLIENT_TYPE.SEND;
            dat.Data = byt;

            return dat;
        }
        public ClientData GetSendData_All_Change()
        {
            //传送所有信息
            //类型 + 位置 +基础数据 + 道具
            int len = ((QQ.Length + ExID.Length + name.Length) * 2 + 12) + 4 * 11 + itemsChaNum * itemsNum + 4+4;
            byte[] byt = new byte[len];
            int index = 0;

            AddSendData_Int((int)(ClientData.CLIENT_DATA_TYPE.DATA_PLAYER), byt, ref index);
            AddSendData_Int(roomSit, byt, ref index);

            AddSendData_Str(QQ, byt, ref index);
            AddSendData_Str(ExID, byt, ref index);
            AddSendData_Str(name, byt, ref index);

            AddSendData_Int(exp, byt, ref index);
            AddSendData_Int(hit, byt, ref index);
            AddSendData_Int(piaobi, byt, ref index);
            AddSendData_Int(Qbi, byt, ref index);
            AddSendData_Int(level, byt, ref index);

            AddSendData_Int(vs_win, byt, ref index);
            AddSendData_Int(vs_lose, byt, ref index);
            AddSendData_Int(vs_peace, byt, ref index);
            AddSendData_Int(roomID, byt, ref index);
            AddSendData_Int(roomSit, byt, ref index);
            AddSendData_Int(type, byt, ref index);

            AddSendData_Item(byt, ref index);

            ClientData dat = new ClientData();
            dat.Type = ClientData.CLIENT_TYPE.SEND;
            dat.Data = byt;

            return dat;
        }
        private void AddSendData_Str(string str,byte[] byt, ref int index)
        {
            //拼接长度信息
            System.Buffer.BlockCopy(BitConverter.GetBytes(str.Length * 2), 0, byt, index, 4);
            index += 4;

            //拼接字符串
            Encoding.Unicode.GetBytes(str, 0, str.Length, byt, index);
            index += str.Length * 2;
        }
        private void AddSendData_Int(int data,byte[] byt,ref int index)
        {
            System.Buffer.BlockCopy(BitConverter.GetBytes(data), 0, byt, index, 4);
            index += 4;
        }
        private void AddSendData_Item(byte[]byt,ref int index)
        {
            for (int i = 0; i < itemsChaNum; i++)
            {
                for (int j = 0; j < itemsNum; j++)
                {
                    byt[index] = (byte)(items[i][j]);
                    index++;
                }
            }
        }

        //接收函数
        public void SetSendData_All(byte[] byt,ref int index)
        {
            QQ = GetSendData_Str(byt, ref index);
            ExID = GetSendData_Str(byt, ref index);
            name = GetSendData_Str(byt, ref index);

            exp = GetSendData_Int(byt, ref index);
            hit = GetSendData_Int(byt, ref index);
            piaobi = GetSendData_Int(byt, ref index);
            Qbi = GetSendData_Int(byt, ref index);
            level = GetSendData_Int(byt, ref index);

            vs_win = GetSendData_Int(byt, ref index);
            vs_lose = GetSendData_Int(byt, ref index);
            vs_peace = GetSendData_Int(byt, ref index);
            roomID = GetSendData_Int(byt, ref index);
            roomSit = GetSendData_Int(byt, ref index);
            type = GetSendData_Int(byt, ref index);

            SetSendData_Items(byt, ref index);
        }
        private string GetSendData_Str(byte[] byt, ref int index)
        {
            //读长度
            int t = System.BitConverter.ToInt32(byt, index);
            index += 4;

            //读字符
            string str = Encoding.Unicode.GetString(byt, index, t);
            index += t;

            return str;
        }
        private int GetSendData_Int(byte[] byt,ref int index)
        {
            int t = System.BitConverter.ToInt32(byt, index);
            index += 4;
            return t;
        }
        private void SetSendData_Items(byte[] byt,ref int index)
        {
            for (int i = 0; i < itemsChaNum; i++)
            {
                for (int j = 0; j < itemsNum; j++)
                {
                    items[i][j] = byt[index];
                    index++;
                }
            }
        }
    }

    public class PlayerInput
    {
        public int[] key = new int[9];

        public PlayerInput()
        {
            Init();
        }
        public void Init()
        {
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = 0;
            }
        }
        public void UpdateKey(bool[] input)
        {
            for (int i = 0; i < 9; i++)
            {
                if (input[i])
                {
                    key[i]++;
                }
                else
                {
                    key[i] = 0;
                }
            }
        }

        public int GetFlyUp()
        {
            return key[4];
        }
        public int GetFlyLeft()
        {
            return key[2];
        }
        public int GetFlyRight()
        {
            return key[3];
        }
        public int GetNum1()
        {
            return key[5];
        }
        public int GetNum2()
        {
            return key[6];
        }
        public int GetNum3()
        {
            return key[7];
        }
        public int GetNum4()
        {
            return key[8];
        }
    }

}
