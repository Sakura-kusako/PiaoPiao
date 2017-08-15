using Data.Globals;
using Data.XML;
using SharpDX.Direct3D9;
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
            path = GlobalB.GetRootPath()+"\\";
            itemPic = new BalloonItemPic();
            pic = new BalloonPic();
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
            if (pic != null)
                pic.Clear();
            pic = null;
        }
        public void LoadItemPic()
        {
            if (itemPic == null)
                itemPic = new BalloonItemPic();
            itemPic.Init(Global.GetXmlManager().itemPics, path);
        }
        public void LoadPic()
        {
            if (pic == null)
                pic = new BalloonPic();
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
        public Texture GetMapBitmap(int typeID, int bitmapID)
        {
            var pic = this.pic.pic1;
            if (typeID > 0 && typeID < pic.Count)
            {
                var pic2 = pic[typeID].pic2;
                if (0 < bitmapID && bitmapID < pic2.Count)
                {
                    var pic3 = pic2[bitmapID];
                    if (pic3.isLoad)
                    {
                        return pic3.bitmap;
                    }
                    else
                    {
                        string file = Global.GetXmlManager().pics[typeID-1].itemPics[bitmapID-1].file;
                        pic3.isLoad = true;
                        pic3.bitmap = this.pic.Load_Bitmap_FromFile(this.pic.path, file);
                        return pic3.bitmap;
                    }
                }
            }
            //msg = "图片不存在：typeID = " + typeID + " bitmapID = " + bitmapID;
            return null;
        }
        public BalloonPic2 GetMapPic2(int typeID, int bitmapID)
        {
            var pic = this.pic.pic1;
            if (typeID > 0 && typeID < pic.Count)
            {
                var pic2 = pic[typeID].pic2;
                if (0 < bitmapID && bitmapID < pic2.Count)
                {
                    var pic3 = pic2[bitmapID];
                    if (pic3.isLoad == false)
                    {
                        string file = Global.GetXmlManager().pics[typeID - 1].itemPics[bitmapID - 1].file;
                        pic3.isLoad = true;
                        pic3.bitmap = this.pic.Load_Bitmap_FromFile(this.pic.path, file);
                    }
                    return pic3;
                }
            }
            //msg = "图片不存在：typeID = " + typeID + " bitmapID = " + bitmapID;
            return null;
        }
        public BalloonPic2 GetMapPic2(Pic p)
        {
            var pic = this.pic.pic1;
            if (p.typeID > 0 && p.typeID < pic.Count)
            {
                var pic2 = pic[p.typeID].pic2;
                if (0 < p.ID && p.ID < pic2.Count)
                {
                    var pic3 = pic2[p.ID];
                    pic3.x = p.x;
                    pic3.y = p.y;
                    if (pic3.isLoad == false)
                    {
                        string file = Global.GetXmlManager().pics[p.typeID - 1].itemPics[p.ID - 1].file;
                        pic3.isLoad = true;
                        pic3.bitmap = this.pic.Load_Bitmap_FromFile(this.pic.path, file);
                    }
                    return pic3;
                }
            }
            //msg = "图片不存在：typeID = " + p.typeID + " bitmapID = " + p.ID;
            return null;
        }

    }

}
