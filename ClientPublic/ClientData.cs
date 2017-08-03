using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ClientPublic
{
    public class ClientData
    {
        //data格式
        //数据头15（byte）+ 长度（int32）+ id（int32）+ type（int32）+ data（byte[]）+ 数据尾部16（byte）

        //type
        //0为只发送，1为发送，2为接收，3为确认接收。
        //2取消1，3取消2，3只发送一次

        public enum CLIENT_TYPE : int
        {
            //传输类型
            SEND_ONCE,
            SEND,
            RECV_SEND,
            RECV_RECV_SEND,
            IMPULSE,
            IMPULSE_RECV,
            ERROR,
        };
        public enum CLIENT_DATA_TYPE : int
        {
           //数据类型
           SIGN_IN = 1, //登录
           IMPULSE, //脉冲信号
           RECV_SEND, //接收信号
        };
        
        public IPAddress IP;
        public int Port = 0;
        public int ID = 0;
        public CLIENT_TYPE Type = CLIENT_TYPE.ERROR; //传输类型
        public int Time = 1; //生存时间（帧）
        public byte[] Data = null;

        public ClientData()
        {
        }

        public ClientData(IPEndPoint ep,byte[] byt)
        {
            //构造函数
            IP = ep.Address;
            Port = ep.Port;
            ID = BitConverter.ToInt32(byt, 5);
            Type = (CLIENT_TYPE)(BitConverter.ToInt32(byt, 9));
            Data = new byte[byt.Length - 14];
            Buffer.BlockCopy(byt, 13, Data, 0, byt.Length - 14);
        }
        public byte[] CreateSendData()
        {
            //转变为发送的byte[]

            //创建
            byte[] byt = new byte[Data.Length + 14];

            //数据头
            byt[0] = 15;

            //长度
            Buffer.BlockCopy(BitConverter.GetBytes(Data.Length + 14), 0, byt, 1, 4);

            //ID
            Buffer.BlockCopy(BitConverter.GetBytes(ID), 0, byt, 5, 4);

            //Type
            Buffer.BlockCopy(BitConverter.GetBytes((int)(Type)), 0, byt, 9, 4);

            //数据data
            Buffer.BlockCopy(Data, 0, byt, 13, Data.Length);

            //数据尾
            byt[0] = 16;
            
            return byt;
        }
        public void CreateImpulse()
        {
            //转变为脉冲信号
            Type = CLIENT_TYPE.IMPULSE;

            //数据类型+当前时间
            Data = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes((int)(CLIENT_DATA_TYPE.IMPULSE)), 0, Data, 0, 4);

            var time = DateTime.Now;
            int dataTime = (time.Minute * 60000) + (time.Second * 1000) + (time.Millisecond);
            Buffer.BlockCopy(BitConverter.GetBytes(dataTime), 0, Data, 4, 4);
        }
        public void CreateImpulseRecv()
        {
            //转变为脉冲信号回复
            Type = CLIENT_TYPE.IMPULSE_RECV;
        }
        public void CreateSignIn()
        {
            //转变为登录信息
            Type = CLIENT_TYPE.SEND;
            Data = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes((int)(CLIENT_DATA_TYPE.SIGN_IN)), 0, Data, 0, 4);
        }
        public ClientData NewRecvSend()
        {
            //根据发送信号创建接收信号
            ClientData newDat = new ClientData();
            newDat.ID = ID;
            newDat.Type = CLIENT_TYPE.RECV_SEND;
            newDat.Time = 5000;
            newDat.Data = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes((int)(CLIENT_DATA_TYPE.RECV_SEND)), 0, newDat.Data, 0, 4);
            return newDat;
        }

        public int GetImpulseTime()
        {
            //获取脉冲信号记录的时间
            return BitConverter.ToInt32(Data, 4);
        }
        public bool Equal(ClientData dat)
        {
            return (dat.ID == ID && dat.Type == Type);
        }
        public bool IsRecvSend(ClientData dat)
        {
            //判断是否为对应的RECV
            return (dat.Type == CLIENT_TYPE.RECV_SEND && ID == dat.ID);
        }
        public bool IsCancleSend(ClientData dat)
        {
            //判断是否为对应取消的SEND
            return (dat.Type == CLIENT_TYPE.SEND && ID == dat.ID);
        }
        public bool IsSignIn()
        {
            //判断是否为重连消息
            return ((CLIENT_DATA_TYPE)(BitConverter.ToInt32(Data, 0)) == CLIENT_DATA_TYPE.SIGN_IN);
        }
    }
}
