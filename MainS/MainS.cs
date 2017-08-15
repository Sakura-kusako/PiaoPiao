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
            while(true)
            {
                Thread.Sleep(5);
                clientS.SendAll();
                clientS.UpdateTime();
                for (int i = 0; i < 6; i++)
                {
                    var list = clientS.GetDataList(i);
                    if (list != null)
                    {
                        room.DealSendData(list,i);
                    }
                    else
                    {
                        room.players[i].Reset();
                        room.DelPlayer(i);
                        room.ResetSoutai();
                        room.SendData_All();
                        room.SendData_PlayerAll();
                    }
                    int delay = clientS.GetDelay(i);
                    if (delay > 0)
                    {
                        Console.WriteLine("clients " + i + " : " + delay + " ms");
                    }
                }
            }
        }
    }
}
