using Data.Globals;
using Data.Windows;
using Data.XML;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Resources
{
    public class BalloonItemPic
    {
        public string path = "null";
        public List<BalloonItemPic1> itemPic1 = new List<BalloonItemPic1>();

        public void Init(List<ItemPic> itemPics, string rootPath)
        {
            Clear();
            itemPic1.Add(null);
            Load(itemPics, rootPath);
        }
        public void Clear()
        {
            foreach (var t in itemPic1)
            {
                if (t != null)
                    t.Clear();
            }
            itemPic1.Clear();
        }
        public void Load(List<ItemPic> itemPics, string rootPath)
        {
            path = rootPath + @"balloon\";

            BalloonItemPic1 t1;
            BalloonItemPic2 t2;
            BalloonItemPic_Base tb;

            ItemPic item1;
            ItemPics item2;
            ItemPic_Base itemb;
            int len = 0;

            for (int i = 0; i < itemPics.Count; i++)
            {
                item1 = itemPics[i];
                t1 = new BalloonItemPic1();
                t1.ID = item1.ID;
                t1.name = item1.name;

                len = item1.itemPics.Count;
                for (int j = 0; j < len; j++)
                {
                    item2 = item1.itemPics[j];
                    t2 = new BalloonItemPic2();
                    t2.ID = item2.ID;
                    t2.name = item2.name;
                    t2.shopbitmap = SpriteBase.Load_Bitmap_FromFile(path + item2.shopFile);
                    for (int k = 0; k < 2; k++)
                    {
                        itemb = item2.item[k];
                        tb = t2.itemPic_Base[k];

                        tb.x = itemb.x;
                        tb.y = itemb.y;
                        tb.width = itemb.width;
                        tb.height = itemb.height;
                        tb.frameNum = itemb.frameNum;
                        tb.frameSpan = itemb.frameSpan;
                        tb.line = itemb.line;
                        tb.bitmap = Load_Bitmap_FromFile(path, itemb.file);
                    }
                    t1.itemPic2.Add(t2);
                }
                itemPic1.Add(t1);
            }
        }
        public Texture Load_Bitmap_FromFile(string path, string file)
        {
            return Global.Load_Bitmap_FromFile(path, file);
        }
    }
    public class BalloonItemPic1
    {
        public int ID;
        public string name;
        public List<BalloonItemPic2> itemPic2;

        public BalloonItemPic1()
        {
            OnCreate();
            Init();
        }
        public void OnCreate()
        {
            ID = 0;
            name = "ERROR";
            itemPic2 = new List<BalloonItemPic2>();
        }
        public void Init()
        {
            itemPic2.Add(null);
        }
        public void Clear()
        {
            foreach (var t in itemPic2)
            {
                if (t != null)
                    t.Clear();
            }
            itemPic2.Clear();
        }
    }
    public class BalloonItemPic2
    {
        public int ID = 0;
        public string name = "ERROR";
        public BalloonItemPic_Base shopbitmap = null;
        public BalloonItemPic_Base[] itemPic_Base = new BalloonItemPic_Base[2] { new BalloonItemPic_Base(), new BalloonItemPic_Base() };
        public void Clear()
        {
            if (shopbitmap != null)
                shopbitmap.Clear();
            if (itemPic_Base[0] != null)
                itemPic_Base[0].Clear();
            if (itemPic_Base[1] != null)
                itemPic_Base[1].Clear();

        }
    }
    public class BalloonItemPic_Base
    {
        public int x = 0;
        public int y = 0;
        public int width = 0;
        public int height = 0;
        public int line = 1;
        public int frameNum = 1;
        public int frameSpan = 1;
        public Texture bitmap = null;
        public virtual void Clear()
        {
            if (bitmap != null)
                bitmap.Dispose();
            bitmap = null;
        }
    }

}
