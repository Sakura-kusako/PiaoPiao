using Data.Globals;
using Data.XML;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Resources
{
    public class BalloonPic
    {
        public string path = "null";
        public List<BalloonPic1> pic1 = new List<BalloonPic1>();

        public void Init(List<SpritePic> pics, string rootPath)
        {
            Clear();
            pic1.Clear();
            pic1.Add(null);
            Load(pics, rootPath);
        }
        public void Clear()
        {
            foreach (var t in pic1)
            {
                if (t != null)
                    t.Clear();
            }
        }
        public void Load(List<SpritePic> pics, string rootPath)
        {
            path = rootPath + @"balloon\";

            BalloonPic1 t1;
            BalloonPic2 t2;

            SpritePic item1;
            SpritePics item2;
            int len = 0;

            for (int i = 0; i < pics.Count; i++)
            {
                item1 = pics[i];
                t1 = new BalloonPic1();
                t1.ID = item1.ID;
                t1.name = item1.name;

                len = item1.itemPics.Count;
                for (int j = 0; j < len; j++)
                {
                    item2 = item1.itemPics[j];
                    t2 = new BalloonPic2();
                    t2.ID = item2.ID;
                    t2.name = item2.name;
                    t2.x = item2.x;
                    t2.y = item2.y;
                    t2.width = item2.width;
                    t2.height = item2.height;
                    t2.frameNum = item2.frameNum;
                    t2.frameSpan = item2.frameSpan;
                    t2.line = item2.line;
                    //t2.bitmap = Load_Bitmap_FromFile(path, item2.file);
                    t2.bitmap = null;
                    t1.pic2.Add(t2);
                }
                pic1.Add(t1);
            }
        }
        public Texture Load_Bitmap_FromFile(string path, string file)
        {
            if (file == null)
            {
                return null;
            }
            if (File.Exists(path + file))
            {
                return Global.Load_Bitmap_FromFile(path + file);
            }
            return null;
        }
    }
    public class BalloonPic1
    {
        public int ID;
        public string name;
        public List<BalloonPic2> pic2;

        public BalloonPic1()
        {
            OnCreate();
            Init();
        }
        public void OnCreate()
        {
            ID = 0;
            name = "ERROR";
            pic2 = new List<BalloonPic2>();
        }
        public void Init()
        {
            pic2.Add(null);
        }
        public void Clear()
        {
            foreach (var t in pic2)
            {
                if (t != null)
                    t.Clear();
            }
        }
    }
    public class BalloonPic2 : BalloonItemPic_Base
    {
        public int ID = 0;
        public string name = "ERROR";
        public bool isLoad = false;
        public override void Clear()
        {
            base.Clear();
            isLoad = false;
        }
    }
}
