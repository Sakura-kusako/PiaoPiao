using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.XML
{
    public class Maps
    {
        //<MAP ID = "1" GameMode="3" BackGround="4"
        //Width="800" Height="1000" Gravity="0.400000" 
        //VelocityV="0.250000" VelocityH="0.250000" 
        //PropCount="50" GameTime="300" Name="夺宝新兵" level="3" real="0">

        public string name = "ERROR";
        public int id = 0;
        public int gameMode = 0;
        public int backGround = 0;
        public int width = 800;
        public int height = 1000;
        public float gravity = 0.4f;
        public float velocityV = 0.25f;
        public float velocityH = 0.25f;
        public int propCount = 50;
        public int gameTime = 300;
        public int level = 0;
        public int real = 0;
        public List<Maps_Element> elements = new List<Maps_Element>();
    }
    public class Maps_Element
    {
        public string name = "Elememts";
        public int id = 0;
        public List<Maps_Elements_Base> elements_base = new List<Maps_Elements_Base>();
    }
    public class Maps_Elements_Base
    {
        /*
            <Property ID = "1" Layer="3" Count="9" Speed="-3" FlyAreaX="80"/>
            <Element ID="22" Layer="3" FlyType="1" Weight="65535" Time="10" PastTime="0" 
                Left="403" Top="570" Right="403" Bottom="570" InterVal1="400" InterVal2="0"/>
            <Stabber ID="3" Layer="3" FlyType="1" Time="10" PastTime="0"
                Left="377" Top="606" Right="0" Bottom="0" DirLeft="377" DirUp="0" DirRight="0" DirDown="0"/>
        */
        public int id = 0;
        public int x = 0;
        public int y = 0;
        public int centerX = 0;
        public int centerY = 0;
        public int radius = 30;     //半径
        public int radian = 0;      //弧度
        public int layer = 3;
        public int count = 0;
        public int speed = 0;
        public int flyAreaX = 0;
        public int weight = 65535;
        public int time = 0;
        public int pastTime = 0;
        public int bstTime = 0;
        public int sprTime = 0;
        public int offsetTime = 0;
        public int flyType = 0;
        public int left = 0;
        public int top = 0;
        public int right = 0;
        public int bottom = 0;
        public int dirLeft = 0;
        public int dirTop = 0;
        public int dirRight = 0;
        public int dirBottom = 0;
        public int interVal1 = 0;
        public int interVal2 = 0;
        public int group = 0;
    }
}
