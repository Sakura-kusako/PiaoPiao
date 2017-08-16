using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientPublic
{
    public class ClientS
    {
        /**服务器传输类
         * 
         * 实现方法
         * ·获取连接状态
         * ·开启服务器
         * ·关闭服务器
         * ·发送所有数据
         * ·将数据添加到数据发送列表
         * ·获取数据接收列表
         * ·获取连接延时
         * ·更新延时
         */
        private bool isConnect = false;
        private int Port = 10800;
        private UdpClient Client; 
        private IPEndPoint EndPoint; //临时变量
        private Client[] clients;
        public int SendNum = 0;
        public int RecvNum = 0;

        public ClientS()
        {
            //构造函数
            clients = new Client[6] { null, null, null, null, null, null };
        }
        public bool IsConnect()
        {
            //获取连接状态
            return isConnect;
        }
        public void OpenClient()
        {
            //开启服务器

            //状态初始化
            isConnect = true;
            for (int i = 0; i < 6; i++)
            {
                if(clients[i] == null)
                {
                    clients[i] = new Client();
                    clients[i].SetQQ(i + "");
                    clients[i].SetExID(i + "");
                }
                else
                {
                    clients[i].LostConnect();
                }
            }

            //创建一个UdpClient对象，10800为接收端口
            IPEndPoint ep = new IPEndPoint(IPAddress.Any,Port);
            Client = new UdpClient(ep);

            //消除远程错误
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            Client.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

            //设置远程主机，(IPAddress.Any, 0)代表接收所有IP所有端口发送的数据
            EndPoint = new IPEndPoint(IPAddress.Any, 0);

            //开始接收
            Client.BeginReceive(new AsyncCallback(Callback), null);
        }
        public void CloseClient()
        {
            //关闭服务器
            isConnect = false;
            Client.Close();
        }
        public void SendAll()
        {
            //发送所有数据
            if (isConnect == false) return;

            foreach (var client in clients)
            {
                if(client.IsConnect())
                {
                    var ep = client.GetIPEndPoint();
                    var list = client.GetSendList();
                    lock(list)
                    {
                        int i = 0;
                        while(i < list.Count)
                        {
                            var dat = list[i];
                            var byt = dat.CreateSendData();
                            Client.BeginSend(byt, byt.Length,ep, CallbackSend, null);
                            switch(dat.Type)
                            {
                                case ClientData.CLIENT_TYPE.SEND:
                                    {
                                        i++;
                                        break;
                                    }
                                case ClientData.CLIENT_TYPE.RECV_SEND:
                                case ClientData.CLIENT_TYPE.SEND_ONCE:
                                case ClientData.CLIENT_TYPE.RECV_RECV_SEND:
                                case ClientData.CLIENT_TYPE.IMPULSE:
                                case ClientData.CLIENT_TYPE.IMPULSE_RECV:
                                case ClientData.CLIENT_TYPE.ERROR:
                                default:
                                    {
                                        list.RemoveAt(0);
                                        break;
                                    }
                            }
                            //Console.WriteLine("Send"+ep.ToString()+"  "+dat.Type.ToString());
                        }
                    }
                }
            }
        }
        public void AddData(ClientData dat,int poi)
        {
            //将数据添加到发送列表
            if (poi <= 5 && poi >= 0)
            {
                if (clients[poi].IsConnect())
                {
                    clients[poi].AddSendData(dat);
                }
            }
        }
        public void AddData(ClientData dat)
        {
            //将数据添加到发送列表
            for (int i = 0; i < 6; i++)
            {
                if (clients[i].IsConnect())
                {
                    clients[i].AddSendData(dat);
                }
            }
        }
        public List<ClientData> GetDataList(int poi)
        {
            //获取数据列表
            if (poi <= 5 && poi >= 0)
            {
                if (clients[poi].IsConnect())
                {
                    return clients[poi].GetRecvList();
                }
            }
            return null;
        }
        public int GetDelay(int poi)
        {
            //获取连接延时
            if (poi <= 5 && poi >= 0)
            {
                if (clients[poi].IsConnect())
                {
                    return clients[poi].GetDelay();
                }
            }
            return 0;
        }
        public int UpdateTime()
        {
            Client client;
            int ret = 0;
            for (int i = 0; i < 6; i++)
            {
                client = clients[i];
                if(client.IsConnect())
                {
                    client.UpdateTime();
                    if(client.GetDelay()>10000)
                    {
                        client.LostConnect();
                        ret += 1<<0;
                    }
                }
            }
            return ret;
        }

        private void Callback(IAsyncResult ar)
        {
            //接收回调函数

            if (isConnect == false) return;

            var ep = new IPEndPoint(IPAddress.Any, 0);

            //读取数据
            byte[] bytes = Client.EndReceive(ar, ref ep);
            var dat = new ClientData(ep, bytes);


            //Console.WriteLine(dat.ToString());

            //重新开始接收
            Client.BeginReceive(new AsyncCallback(Callback), null);

            //是否为登录消息
            if (dat.IsSignIn())
            {
                int i = 0;
                int t = -1;
                bool IsReConnect = false;
                int index = 4;
                string qq = GlobalC.GetSendData_Str(dat.Data, ref index);
                string exid = GlobalC.GetSendData_Str(dat.Data, ref index);
                foreach (var client in clients)
                {
                    if (client.EqualQQ(qq,exid))
                    {
                        client.ReConnect(ep);
                        IsReConnect = true;
                        client.AddRecvData(dat);
                        client.SetQQ(qq);
                        client.SetExID(exid);
                        break;
                    }
                    else
                    {
                        if (t == -1 && client.IsConnect() == false)
                        {
                            t = i;
                        }
                    }
                    i++;
                }
                if (IsReConnect == false && t != -1)
                {
                    clients[t].AddConnect(ep);
                    clients[t].AddRecvData(dat);
                    clients[t].SetQQ(qq);
                    clients[t].SetExID(exid);
                }
            }
            else
            {
                foreach (var client in clients)
                {
                    //判断ip
                    if (client.EqualIP(ep))
                    {
                        //添加到消息列表
                        client.AddRecvData(dat);
                    }
                }
            }
            RecvNum++;
        }
        private void CallbackSend(IAsyncResult ar)
        {
            //发送回调函数
            try
            {
                if (isConnect == false) return;
                Client.EndSend(ar);
                SendNum++;
            }
            catch(Exception)
            {

            }
        }

        public void AddDataImpulseAll()
        {
            var dat = new ClientData();
            dat.CreateImpulse();
            AddData(dat);
        }
    }
}
