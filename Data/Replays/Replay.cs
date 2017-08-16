using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Replays
{
    public class Replay
    {
        FileStream fs;
        StreamWriter sw;

        public Replay()
        {
        }
        public void Start(string path)
        { 
            End();
            fs = new FileStream(path, FileMode.Create);
            sw = new StreamWriter(fs);
        }
        public void End()
        {
            if(sw != null)
            {
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                sw = null;
            }
            if(fs != null)
            {
                fs.Close();
                fs = null;
            }
        }
        public StreamWriter GetSw()
        {
            return sw;
        }
    }
}
