using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Data.XML
{
    enum ElementID
    {
        NULL,
        BackGrounds,
        Static_Element,
        Fly_Element,
        NPCs,
        Propertys,
        Vanes,
        Traps,
        Players,
        Balloons,
        Build_Slave,
        Primers,
        Floats,
        Gates,
        Stabbers,
        Spouts,
        Barrier,
        Player_Bases
    }
    public class Elements
    {
        public int elementID = 0;
    }
    public class BackGround : Elements
    {
        public int ID = 0;
        public int index = 0;
        public Pic pic = new Pic();
    }
    public class Static_Element : Elements
    {
        public int ID = 0;
        public int type = 0;
        public List<Point> points = new List<Point>();
        public List<Pic> pics = new List<Pic>();
    }
    public class Fly_Element : Elements
    {
        //<Element ID = "1" Type="3" FloatType="1">
        public int ID = 0;
        public int type = 0;
        public int floatType = 0;
        public List<Rect> rects = new List<Rect>();
        public Pic pic = new Pic();
    }
    public class NPC : Elements
    {
        /*
           <NPCs ID = "4" >
           < NPC ID="1" Type="1" Weight="150" Life="1" Attack="1" Speed="1">
               <Pic TypeID = "4" ID="1"/>
               <Pic TypeID = "4" ID="2"/>
        */
        public int ID = 0;
        public int type = 0;
        public int weight = 0;
        public int life = 0;
        public int attack = 0;
        public int speed = 0;
        public List<Pic> pics = new List<Pic>();
    }
    public class Property : Elements
    {
        /*
            <Property ID="38" Type="72">
                <Pic TypeID="5" ID="38"/>
        */
        public int ID = 0;
        public int type = 0;
        public Pic pic = new Pic();
    }
    public class Vane : Elements
    {
        public int ID = 0;
        public Pic pic = new Pic();
    }
    public class Trap : Elements
    {
        /*
            <Traps ID="7">
                <Trap ID="1" Type="1" Radius="150" BstRepeat="1" BstActionNum="19" SprTime="2" BstTime="2">
                AngleBegin="20" AngleEnd="160"
        */
        public int ID = 0;
        public int type = 0;
        public int radius = 0;
        public int bstRepeat = 0;
        public int bstActionNum = 0;
        public int sprTime = 0;
        public int bstTime = 0;
        public int angleBegin = 0;
        public int angleEnd = 0;
        public List<Pic> pics = new List<Pic>();
    }
    public class Player : Elements
    {
        /*
            <Player ID="1" Type="1">
                <Pic TypeID="8" ID="1"/>
        */
        public int ID = 0;
        public int type = 0;
        public Pic pic = new Pic();
    }
    public class Balloon : Elements
    {

    }
    public class Build_Slave : Elements
    {

    }
    public class Primer : Elements
    {
        /*
            <Primer ID="1" Type="1">
                <Pic TypeID="11" ID="1"/>
        */
        public int ID = 0;
        public int type = 0;
        public Pic pic = new Pic();
    }
    public class Float : Elements
    {
        /*
            <Float ID = "1" Type="1">
            <Pic TypeID = "12" ID="1"/>
            <Pic TypeID = "10" ID="1" X="34" Y="50"/>
        */
        public int ID = 0;
        public int type = 0;
        public List<Pic> pics = new List<Pic>();
    }
    public class Gate : Elements
    {
        public int ID = 0;
        public int type = 0;
        public List<Rect> rects = new List<Rect>();
        public List<Pic> pics = new List<Pic>();
    }
    public class Stabber : Elements
    {
        public int ID = 0;
        public int type = 0;
        public int weight = 0;
        public int sprTime = 0;
        public int bstTime = 0;
        public int interval = 0;
        public int speed = 0;
        public List<Pic> pics = new List<Pic>();
    }
    public class Spout : Elements
    {
        public int ID = 0;
        public int type = 0;
        public List<Pic> pics = new List<Pic>();
    }
    public class Barrier : Elements
    {
        public int ID = 0;
        public int type = 0;
        public int sprTime = 0;
        public int bstTime = 0;
        public int pastTime = 0;
        public List<Pic> pics = new List<Pic>();
    }
    public class Player_Base : Elements
    {
        public int ID = 0;
        public List<Point> points = new List<Point>();
        public Pic pic = new Pic();
    }
    public class Pic
    {
        public int typeID = 0;
        public int ID = 0;
        public int x = 0;
        public int y = 0;
    }
    public class Rect
    {
        public RectangleF rect;
        public int property = 0;
    }
}
