using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ClientPublic
{
    public class Client
    {
        /**服务器传输类
         * 
         * 实现方法
         * ·获取连接状态
         * ·添加连接
         * ·重连
         * ·连接断开
         * ·将数据添加到数据发送列表
         * ·将数据添加到数据接收列表
         * ·获取数据发送列表
         * ·获取数据接收列表
         * ·获取连接延时
         * ·获取IPEndPoint
         */

        private IPAddress IP;
        private int Port = 0;
        private bool isConnect = false; //连接状态
        private int delay = 0; //延时
        private int ID = 0; //发送编号

        private List<ClientData> SendList = new List<ClientData>();
        private List<ClientData> RecvList = new List<ClientData>();

        public Client()
        {

        }
        public Client(IPEndPoint ep)
        {
            IP = ep.Address;
            Port = ep.Port;
        }
        public bool IsConnect()
        {
            //获取连接状态
            return isConnect;
        }
        public void AddConnect(IPEndPoint ep)
        {
            //添加连接
            IP = ep.Address;
            Port = ep.Port;
            isConnect = true;
            delay = 0;
            ID = 0;
            SendList.Clear();
            RecvList.Clear();
        }
        public void ReConnect(IPEndPoint ep)
        {
            //重连
            IP = ep.Address;
            Port = ep.Port;
            isConnect = true;
            delay = 0;
        }
        public void LostConnect()
        {
            //连接断开
            isConnect = false;
            SendList.Clear();
            RecvList.Clear();
        }
        public void AddSendData(ClientData dat)
        {
            //将数据添加到数据发送列表
            lock (SendList)
            {
                SendList.Add(dat);
            }
        }
        public void AddRecvData(ClientData dat)
        {
            //判断数据
            switch (dat.Type)
            {
                case ClientData.CLIENT_TYPE.IMPULSE:
                    {
                        //脉冲信号
                        dat.CreateImpulseRecv(); //转换为回复信号
                        AddSendData(dat); //添加到发送列表
                        return;
                    }
                case ClientData.CLIENT_TYPE.IMPULSE_RECV:
                    {
                        //脉冲回复信号
                        //计算延迟
                        var time = DateTime.Now;
                        int dataTime = (time.Minute * 60000) + (time.Second * 1000) + (time.Millisecond) + 3600000;
                        delay = (dataTime - dat.GetImpulseTime()) % 3600000;
                        return;
                    }
                case ClientData.CLIENT_TYPE.SEND_ONCE:
                    {
                        break;
                    }
                case ClientData.CLIENT_TYPE.SEND:
                    {
                        //检查发送列表是否有RECV_SEND
                        if(AddRecvSend(dat) == false)
                        {
                            //已经存在
                            return;
                        }
                        else
                        {
                            //不存在
                            //检查是否在接收列表
                            if(CheckRecvExist(dat))
                            {
                                return;
                            }
                        }
                        break;
                    }
                case ClientData.CLIENT_TYPE.RECV_SEND:
                    {
                        CancleSend(dat);
                        return;
                    }
                case ClientData.CLIENT_TYPE.RECV_RECV_SEND:
                    {
                        return;
                    }
                default:
                    return;
            }

            //将数据添加到数据接收列表
            lock (RecvList)
            {
                RecvList.Add(dat);
            }
        }
        public List<ClientData> GetRecvList()
        {
            //获取数据接收列表
            return RecvList;
        }
        public List<ClientData> GetSendList()
        {
            //获取数据接收列表
            return SendList;
        }
        public int GetDelay()
        {
            //获取连接延时
            return delay;
        }
        public bool EqualIP(IPEndPoint ep)
        {
            return (ep.Address.Equals(IP) && ep.Port == Port);
        }
        public IPEndPoint GetIPEndPoint()
        {
            return new IPEndPoint(IP, Port);
        }

        private bool AddRecvSend(ClientData dat)
        {
            //已经存在返回false
            lock(SendList)
            {
                foreach (var data in SendList)
                {
                    if(dat.IsRecvSend(data))
                    {
                        return false;
                    }
                }
                SendList.Add(dat.NewRecvSend());
            }
            return true;
        }
        private bool CheckRecvExist(ClientData dat)
        {
            lock(RecvList)
            {
                foreach (var data in RecvList)
                {
                    if(dat.Equal(data))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void CancleSend(ClientData dat)
        {
            //2取消1
            int i = 0;
            lock (SendList)
            {
                foreach (var data in SendList)
                {
                    if (dat.IsCancleSend(data))
                    {
                        SendList.RemoveAt(i);
                        return;
                    }
                    i++;
                }
            }
        }
    }
}
