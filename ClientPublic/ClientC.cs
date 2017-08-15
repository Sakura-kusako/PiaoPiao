using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientPublic
{
    public class ClientC
    {
        /**客户端传输类
         * 
         * 实现方法
         * ·获取连接状态
         * ·开启客户端
         * ·关闭客户端
         * ·发送所有数据
         * ·将数据添加到数据发送列表
         * ·获取数据接收列表
         * ·获取连接延时
         * ·更新延时
         */
        private bool isConnect = false;
        private IPAddress IP = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private int Port = 10800;
        private UdpClient Client;
        private IPEndPoint EndPoint; //临时变量
        private Client client;

        public ClientC()
        {
            //构造函数
            client = new Client();
            client.AddConnect(new IPEndPoint(IP, Port));
        }
        public ClientC(string qq,string exid)
        {
            //构造函数
            client = new Client();
            client.AddConnect(new IPEndPoint(IP, Port));
            client.SetQQ(qq);
            client.SetExID(exid);
        }
        public bool IsConnect()
        {
            //获取连接状态
            return isConnect;
        }
        public void OpenClient()
        {
            //开启客户端

            //状态初始化

            //连接到服务端并指定接收端口
            Client = new UdpClient(0);

            //消除远程错误
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            Client.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

            //发送登录消息
            var dat = new ClientData();
            dat.CreateSignIn(client.GetQQ(),client.GetExID());
            client.AddSendData(dat);

            //开始接收
            Client.BeginReceive(new AsyncCallback(Callback), null);
            isConnect = true;
        }
        public void CloseClient()
        {
            //关闭客户端
            isConnect = false;
            Client.Close();
        }
        public void SendAll()
        {
            //发送所有数据
            if (isConnect == false) return;
            IPEndPoint ep = new IPEndPoint(IP, Port);
            //发送脉冲
            AddDataImpulseAll();

            if (client.IsConnect())
            {
                var list = client.GetSendList();
                lock (list)
                {
                    int i = 0;
                    while (i < list.Count)
                    {
                        var dat = list[i];
                        var byt = dat.CreateSendData();
                        Client.BeginSend(byt, byt.Length, ep, CallbackSend, null);
                        switch (dat.Type)
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
                                    list.RemoveAt(i);
                                    break;
                                }
                        }
                    }
                }
            }
        }
        public void AddData(ClientData dat)
        {
            //将数据添加到发送列表
            if (client.IsConnect())
            {
                client.AddSendData(dat);
            }
        }
        public List<ClientData> GetDataList()
        {
            //获取数据列表
            if (client.IsConnect())
            {
                return client.GetRecvList();
            }
            return null;
        }
        public int GetDelay()
        {
            //获取连接延时
            if (client.IsConnect())
            {
                return client.GetDelay();
            }
            return 0;
        }
        public void UpdateTime()
        {
            client.UpdateTime();
            if (client.GetDelay() > 10000)
            {
                client.LostConnect();
                CloseClient();
            }
        }

        private void Callback(IAsyncResult ar)
        {
            //接收回调函数
                if (isConnect == false) return;

                //读取数据
                byte[] bytes = Client.EndReceive(ar, ref EndPoint);
                var dat = new ClientData(EndPoint, bytes);

                //重新开始接收
                Client.BeginReceive(new AsyncCallback(Callback), null);

                //判断ip
                if (client.EqualIP(EndPoint))
                {
                    //添加到消息列表
                    client.AddRecvData(dat);
                }
        }
        private void CallbackSend(IAsyncResult ar)
        {
            //发送回调函数

            if (isConnect == false) return;
            Client.EndSend(ar);
        }
        private void AddDataImpulseAll()
        {
            var dat = new ClientData();
            dat.CreateImpulse();
            AddData(dat);
        }

    }
}
