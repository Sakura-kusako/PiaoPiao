using Data.DXRender;
using Data.Globals;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RectangleF = System.Drawing.RectangleF;
using Rectangle = System.Drawing.Rectangle;

namespace Data.PPDevices
{
    /*
    struct Vertex
    {
        public Vector4 position;
        public Vector4 texture;
        public ColorBGRA color;
    }
    */
    public class PPDevice
    {
        public VertexDeclaration Decl_Tex;
        private VertexBuffer vertices;
        public Device device;
        public Texture green;
        public Texture blue1;
        public Texture blue2;
        public Texture red;
        public Texture pink;

        public PPDevice(IntPtr handle)
        {
            device = new Device(new Direct3D(), 0, DeviceType.Hardware, handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(800, 600));
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            device.SetRenderState(RenderState.AlphaTestEnable, false);

            //device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            //device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            //device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
            device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            device.SetTextureStageState(0, TextureStage.ColorOperation, TextureArgument.Specular);
            device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
            device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);

            device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureArgument.TFactor);
            device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Diffuse);
            device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Texture);

            vertices = new VertexBuffer(device, 6 * 36, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed , 0),
                new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(0, 32, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };
            Decl_Tex = new VertexDeclaration(device, vertexElems);
            /*
            green = LoadBitmapFromFile(_Static.Poi.path + @"test\green.png");
            blue1 = LoadBitmapFromFile(_Static.Poi.path + @"test\blue1.png");
            blue2 = LoadBitmapFromFile(_Static.Poi.path + @"test\blue2.png");
            red = LoadBitmapFromFile(_Static.Poi.path + @"test\red.png");
            pink = LoadBitmapFromFile(_Static.Poi.path + @"test\pink.png");
            */
        }
        public void BeginDraw()
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();
        }
        public void EndDraw()
        {
            device.EndScene();
            device.Present();
        }
        public void BitBlt(Resources.BalloonItemPic_Base pic, RectangleF pos, RectangleF tex, float alpha = 1.0f)
        {
            if (pic == null) return;
            if (pic.bitmap == null) return;
            float w = (int)pic.bitmap.GetWidth() + 1;
            float h = (int)pic.bitmap.GetHeight() + 1;

            _Sprite.Texture = pic.bitmap;
            _Sprite.Left = (int)(pos.X);
            _Sprite.Top = (int)(pos.Y + 0.1f);
            _Sprite.Alpha = alpha;
            //_Sprite.SizeY = pos.Width;
            //_Sprite.SizeX = pos.Height;
            //_Sprite.Rotation = 0;
            _Sprite.TextureLeft = (tex.Left + 1) / w;
            _Sprite.TextureTop = (tex.Top + 1) / h;
            _Sprite.TextureRight = (tex.Right + 1) / w;
            _Sprite.TextureBottom = (tex.Bottom + 1) / h;

            //_Sprite.ScaleX = pos.Width / (pic.GetWidth() * 1);
            //_Sprite.ScaleY = pos.Height / (pic.GetHeight() * 1);

            _Sprite.ScaleX = pos.Width / (w - 1);
            _Sprite.ScaleY = pos.Height / (h - 1);

            _Sprite.Render();
        }
        public void BitBlt(Resources.BalloonItemPic_Base pic, float x, float y, RectangleF tex, float alpha = 1.0f)
        {
            /*
            float w = pic.bitmap.GetWidth()+1;
            float h = pic.bitmap.GetHeight()+1;

            BitBlt( pic.bitmap,
                    new RectangleF((int)x,(int)y, (int)(tex.Width), (int)(tex.Height)),
                    new RectangleF((int)(tex.X+1) / w, (int)(tex.Y+1) / h, (int)(tex.Width) / w, (int)(tex.Height) / h),
                    alpha);
            */
            if (pic == null) return;
            if (pic.bitmap == null) return;
            float w = (int)pic.bitmap.GetWidth() + 1;
            float h = (int)pic.bitmap.GetHeight() + 1;

            _Sprite.Texture = pic.bitmap;
            _Sprite.Left = (x);
            _Sprite.Top = (y + 0.01f);
            _Sprite.Alpha = alpha;
            //_Sprite.SizeY = pos.Width;
            //_Sprite.SizeX = pos.Height;
            //_Sprite.Rotation = 0;
            _Sprite.TextureLeft = (tex.Left + 1) / w;
            _Sprite.TextureTop = (tex.Top + 1) / h;
            _Sprite.TextureRight = (tex.Right + 1) / w;
            _Sprite.TextureBottom = (tex.Bottom + 1) / h;

            //_Sprite.ScaleX = pos.Width / (pic.GetWidth() * 1);
            //_Sprite.ScaleY = pos.Height / (pic.GetHeight() * 1);

            _Sprite.ScaleX = tex.Width / (w - 1);
            _Sprite.ScaleY = tex.Height / (h - 1);

            _Sprite.Render();
        }
        public void BitBlt(string str, Rectangle rect, Color color, int width, int height, string ZiTi = "宋体")
        {
            //_Font.RenderText("BakaBakaBakaBakaBaka", new SharpDX.Mathematics.Interop.RawRectangle(0, 0, 105, 100), SharpDX.Color.LightSkyBlue);
            if (_Font != null)
            {
                _Font.Dispose();
            }
            _Font = new RenderFont(_Engine, ZiTi, width, height);
            _Font.RenderText(str, new SharpDX.Mathematics.Interop.RawRectangle(rect.X, rect.Y, rect.Width, rect.Height),
                            color);
        }
        private void BitBlt(Texture pic, RectangleF pos, RectangleF tex, float alpha = 1.0f)
        {
            /*
                var Vcolor = new Color(new Vector4(1.0f, 1.0f, 1.0f, alpha));
                Vertex[] vertex = new Vertex[4]
                {
                    //左上，右上，左下，右下;
                    new Vertex() { color = Vcolor,position = new Vector4(pos.Left, pos.Top, 0.0f, 1.0f),texture = new Vector4(tex.Left,tex.Top,0.0f,1.0f)},
                    new Vertex() { color = Vcolor,position = new Vector4(pos.Right, pos.Top, 0.0f, 1.0f),texture = new Vector4(tex.Right,tex.Top,0.0f,1.0f)},
                    new Vertex() { color = Vcolor,position = new Vector4(pos.Left, pos.Bottom, 0.0f, 1.0f),texture = new Vector4(tex.Left,tex.Bottom,0.0f,1.0f)},
                    new Vertex() { color = Vcolor,position = new Vector4(pos.Right, pos.Bottom, 0.0f, 1.0f),texture = new Vector4(tex.Right,tex.Bottom,0.0f,1.0f)},
                };
                vertices.Lock(0, 0, LockFlags.Discard).WriteRange(new[] {
                    vertex[0],
                    vertex[1],
                    vertex[2],
                    vertex[3],
                    vertex[2],
                    vertex[1],
                });
                vertices.Unlock();

                device.SetStreamSource(0, vertices, 0, 36);
                device.SetTexture(0, pic);
                device.VertexDeclaration = Decl_Tex;
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            */

            float w = pic.GetWidth();
            float h = pic.GetHeight();

            _Sprite.Texture = pic;
            _Sprite.Left = pos.X;
            _Sprite.Top = pos.Y;
            _Sprite.Alpha = alpha;
            //_Sprite.SizeY = pos.Width;
            //_Sprite.SizeX = pos.Height;
            //_Sprite.Rotation = 0;
            _Sprite.TextureLeft = tex.Left;
            _Sprite.TextureTop = tex.Top;
            _Sprite.TextureRight = tex.Right;
            _Sprite.TextureBottom = tex.Bottom;

            //_Sprite.ScaleX = pos.Width / (pic.GetWidth() * 1);
            //_Sprite.ScaleY = pos.Height / (pic.GetHeight() * 1);

            _Sprite.ScaleX = pos.Width / w;
            _Sprite.ScaleY = pos.Height / h;

            _Sprite.Render();

            //_Font.RenderText("BakaBakaBakaBakaBaka", new SharpDX.Mathematics.Interop.RawRectangle(0, 0, 105, 100), SharpDX.Color.LightSkyBlue);

        }
        public void BitBlt_Rect(Texture poi, RectangleF pos, float width = 1.0f, float alpha = 1.0f)
        {
            /*
            */
            BitBlt(poi, new RectangleF(pos.Left, pos.Top, pos.Width, width), new RectangleF(0, 0, 1, 1), alpha);
            BitBlt(poi, new RectangleF(pos.Left, pos.Top, width, pos.Height), new RectangleF(0, 0, 1, 1), alpha);
            BitBlt(poi, new RectangleF(pos.Right - width, pos.Top, width, pos.Height), new RectangleF(0, 0, 1, 1), alpha);
            BitBlt(poi, new RectangleF(pos.Left, pos.Bottom - width, pos.Width, width), new RectangleF(0, 0, 1, 1), alpha);
        }
        public void BitBlt_Rect_Green(RectangleF pos, float width = 1.0f, float alpha = 1.0f)
        {
            BitBlt_Rect(green, pos, width, alpha);
        }
        public void BitBlt_Rect_Red(RectangleF pos, float width = 1.0f, float alpha = 1.0f)
        {
            BitBlt_Rect(red, pos, width, alpha);
        }
        public void BitBlt_Rect_Blue1(RectangleF pos, float width = 1.0f, float alpha = 1.0f)
        {
            BitBlt_Rect(blue1, pos, width, alpha);
        }
        public void BitBlt_Rect_Blue2(RectangleF pos, float width = 1.0f, float alpha = 1.0f)
        {
            BitBlt_Rect(blue2, pos, width, alpha);
        }
        public void BitBlt_Rect_Pink(RectangleF pos, float width = 1.0f, float alpha = 1.0f)
        {
            BitBlt_Rect(pink, pos, width, alpha);
        }

        private RenderEngine _Engine;
        private RenderFont _Font;
        private RenderSprite _Sprite;
        private MemoryStream _BitmapStream = new MemoryStream(1024 * 1024);

        public Texture CreateTextureFromBitmap(System.Drawing.Bitmap bitmap)
        {
            _BitmapStream.Seek(0, SeekOrigin.Begin);
            bitmap.Save(_BitmapStream, System.Drawing.Imaging.ImageFormat.Bmp);

            _BitmapStream.Seek(0, SeekOrigin.Begin);
            //set size in FromStream, or it will be resized
            return Texture.FromStream(device, _BitmapStream, bitmap.Width, bitmap.Height, 1,
                Usage.None, Format.A8R8G8B8, Pool.Managed, Filter.Point, Filter.Point, 0);
        }
        public Texture LoadBitmapFromFile(string file)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(file);
            int w = GlobalB.Get2Min(bitmap.Size.Width);
            int h = GlobalB.Get2Min(bitmap.Size.Height);
            var bitmap2 = new System.Drawing.Bitmap(w, h);
            var g = System.Drawing.Graphics.FromImage(bitmap2);
            g.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height));
            g.Save();
            var tex = CreateTextureFromBitmap(bitmap2);
            bitmap.Dispose();
            bitmap2.Dispose();
            g.Dispose();
            return tex;
            //return Texture.FromFile(device, file);
            //return _Engine.CreateTextureFromFile(file);
            //return _Engine.CreateTextureFromBitmap(new System.Drawing.Bitmap(file));
        }
        public PPDevice(System.Windows.Forms.Control handle,Action draw)
        {
            string path = GlobalB.GetRootPath() + @"\ResPoi\";
            _Engine = new RenderEngine(handle);
            _Sprite = new RenderSprite(_Engine);
            _Engine.OnRender += draw;
            device = _Engine.Device;
            green = LoadBitmapFromFile(path + @"green.png");
            blue1 = LoadBitmapFromFile(path + @"blue1.png");
            blue2 = LoadBitmapFromFile(path + @"blue2.png");
            red = LoadBitmapFromFile(path + @"red.png");
            pink = LoadBitmapFromFile(path + @"pink.png");
        }
        public void RenderAll()
        {
            _Engine.RenderAll();
        }
        public void Clear()
        {
            //_Engine.Dispose();
        }
    }
}
