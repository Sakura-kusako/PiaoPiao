using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Room
{
    public class PlayerData
    {
        public PlayerInput input = new PlayerInput();

        public string QQ = "88888888";
        public string ExID = "0";
        public string name = "name";
        public int exp = 0;
        public int hit = 0;
        public int piaobi = 0;
        public int Qbi = 0;
        public int level = 1;

        public int vs_win = 0;
        public int vs_lose = 0;
        public int vs_peace = 0;
        public int roomID = 0;
        public int roomSit = 0;
        public int server = 0;

        public int type = 2;
        public int[][] items;

        public PlayerData()
        {
            //构造函数
            items = new int[7][];
            for (int i = 0; i < 7; i++)
            {
                items[i] = new int[44]
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

        //传送函数
        public List<byte[]> GetSendData_All()
        {
            var list = new List<byte[]>();
            list.Add(GetSendData_Name());
            //list.Add(GetSendData_Prop());
            list.Add(GetSendData_Items());
            list.Add(GetSendData_ItemType());
            return list;
        }
        public byte[] GetSendData_Name()
        {
            //传送名字
            byte[] bye = new byte[QQ.Length * 2 + name.Length * 2 + 12 + 10];
            int index = 9;

            System.Buffer.BlockCopy(BitConverter.GetBytes(1000), 0, bye, index, 4);
            index += 4;

            System.Buffer.BlockCopy(BitConverter.GetBytes(QQ.Length * 2), 0, bye, index, 4);
            index += 4;

            Encoding.Unicode.GetBytes(QQ, 0, QQ.Length, bye, index);
            index += QQ.Length * 2;

            System.Buffer.BlockCopy(BitConverter.GetBytes(name.Length * 2), 0, bye, index, 4);
            index += 4;

            Encoding.Unicode.GetBytes(name, 0, name.Length, bye, index);
            return bye;
        }
        public byte[] GetSendData_Prop()
        {
            //传送玩家属性
            byte[] bye = new byte[10];
            //int index = 9;

            return bye;
        }
        public byte[] GetSendData_Items()
        {
            const int type_len = 7;
            const int item_len = 44;

            byte[] bye = new byte[type_len * item_len + 4 + 10];
            int index = 9;

            System.Buffer.BlockCopy(BitConverter.GetBytes(1003), 0, bye, index, 4);
            index += 4;

            for (int i = 0; i < type_len; i++)
            {
                for (int j = 0; j < item_len; j++)
                {
                    bye[index] = (byte)items[i][j];
                    index++;
                }
            }

            return bye;
        }
        public byte[] GetSendData_ItemType()
        {
            //传送当前角色
            byte[] bye = new byte[8 + 10];
            int index = 9;

            System.Buffer.BlockCopy(BitConverter.GetBytes(1002), 0, bye, index, 4);
            index += 4;

            System.Buffer.BlockCopy(BitConverter.GetBytes(type), 0, bye, index, 4);
            index += 4;

            return bye;
        }

        //接收函数
        public void SetSendData(byte[] bye)
        {
            switch (System.BitConverter.ToInt32(bye, 9))
            {
                case 1000:
                    SetSendData_Name(bye);
                    break;
                case 1001:
                    SetSendData_Prop(bye);
                    break;
                case 1002:
                    SetSendData_ItemType(bye);
                    break;
                case 1003:
                    SetSendData_Items(bye);
                    break;
                default:
                    break;
            }
        }
        public void SetSendData_Name(byte[] bye)
        {
            //传送名字
            int index = 9;
            int t = 0;

            //读指令
            //1000
            t = System.BitConverter.ToInt32(bye, index);
            index += 4;

            //QQ长度
            t = System.BitConverter.ToInt32(bye, index);
            index += 4;

            //读QQ
            QQ = Encoding.Unicode.GetString(bye, index, t);
            index += t;

            //名字长度
            t = System.BitConverter.ToInt32(bye, index);
            index += 4;

            //读名字
            name = Encoding.Unicode.GetString(bye, index, t);
            index += t;
        }
        public void SetSendData_Prop(byte[] bye)
        {
            //传送玩家属性

        }
        public void SetSendData_Items(byte[] bye)
        {
            const int type_len = 7;
            const int item_len = 44;
            int index = 9;

            //校验指令
            //            
            index += 4;

            //读item列表
            for (int i = 0; i < type_len; i++)
            {
                for (int j = 0; j < item_len; j++)
                {
                    items[i][j] = bye[index];
                    index++;
                }
            }
        }
        public void SetSendData_ItemType(byte[] bye)
        {
            //传送当前角色
            int index = 9;

            //校验指令
            //
            index += 4;

            //读数据
            type = System.BitConverter.ToInt32(bye, index);
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
    }

}
