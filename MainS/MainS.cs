using ClientPublic;
using Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MainS
{
    class MainS
    {
        static void Main(string[] args)
        {
            Rooms room = new Rooms();
            ClientS clientS = new ClientS();
            clientS.OpenClient();
            Console.WriteLine("等待客户端接入");

            room.clientS = clientS;
            int t = 0;
            int ret = 0;
            while(true)
            {
                t++;
                Thread.Sleep(5);
                clientS.SendAll();
                ret = clientS.UpdateTime();
                if(ret > 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if ((ret & (1 << i)) > 0)
                        {
                            room.players[i].Reset();
                            room.DelPlayer(i);
                        }
                    }
                    room.ResetSoutai();
                    room.SendData_All();
                    room.SendData_PlayerAll();
                }
                for (int i = 0; i < 6; i++)
                {
                    room.DealSendData(i);

                    if(t>=20)
                    {
                        int delay = clientS.GetDelay(i);
                        if (delay > 0)
                        {
                            //Console.WriteLine("clients " + i + " : " + delay + " ms");
                        }
                    }
                }
                if(t>=20)
                {
                    t = 0;
                    clientS.AddDataImpulseAll();
                }
                //Console.WriteLine("Send : " + clientS.SendNum + "   Recv : " + clientS.RecvNum);
                clientS.SendNum = 0;
                clientS.RecvNum = 0;
            }
        }
    }
}
