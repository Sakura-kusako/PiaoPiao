using Data.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Resources
{
    public class ResManager
    {
        public string path;
        public BalloonItemPic itemPic;
        public BalloonPic pic;

        public ResManager()
        {
            OnCreate();
        }
        public void OnCreate()
        {
            itemPic = new BalloonItemPic();
            pic = new BalloonPic();
            path = GlobalB.GetRootPath()+"\\";
        }
        public void Init()
        {
            path = GlobalB.GetRootPath() + "\\";
        }
        public void Clear()
        {
            if (itemPic != null)
                itemPic.Clear();
            itemPic = null;
            pic = null;
        }
        public void LoadItemPic()
        {
            itemPic.Init(Global.GetXmlManager().itemPics, path);
        }
        public void LoadPic()
        {
            pic.Init(Global.GetXmlManager().pics, path);
        }

        public BalloonItemPic_Base GetItemPic_Big(int type, int id)
        {
            var item2 = itemPic.itemPic1[type].itemPic2[id];
            if (item2 == null) return null;
            var pic_poi = item2.itemPic_Base[0];
            if (pic_poi == null) return null;
            if (pic_poi.bitmap == null) return null;
            return pic_poi;
        }
    }

}
