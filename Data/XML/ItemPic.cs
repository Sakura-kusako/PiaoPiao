using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.XML
{
    public class ItemPics
    {
        //X1="34" Y1="0" File1="Pic\\角色\\气球\\娃娃.tga" Width1="29" Height1="24" Line1="2" FrameNum1="1" FrameSpan1="200"
        //X2="60" Y2="0" File2="Pic\\大角色\\气球\\娃娃.tga" Width2="58" Height2="48" Line2="2" FrameNum2="1" FrameSpan2="200" 
        //ShopFile="Pic\\Shop\\气球\\娃娃.tga"/>
        public int ID = 0;
        public string name = "ERROR";
        public ItemPic_Base[] item = new ItemPic_Base[2] { new ItemPic_Base(), new ItemPic_Base() };
        public string shopFile = "";
    }
    public class ItemPic_Base
    {
        public int x = 0;
        public int y = 0;
        public int width = 0;
        public int height = 0;
        public int line = 0;
        public int frameNum = 0;
        public int frameSpan = 0;
        public string file = "";
    }
}
