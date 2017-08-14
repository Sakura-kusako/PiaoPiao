using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data.DXRender
{
    public class RenderEngine : IDisposable
    {
        private Direct3D _Direct3d;
        private Device _Device;
        private PresentParameters _Parameters;
        private bool _IsDeviceLost;

        private SharpDX.Direct3D9.Sprite _Sprite;

        private Effect _Effect;

        public Device Device { get { return _Device; } }
        public SharpDX.Direct3D9.Sprite DefaultSprite { get { return _Sprite; } }

        public event Action OnRender;

        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            [VertexField(DeclarationUsage.Position)]
            public Vector4 pos;
            [VertexField(DeclarationUsage.TextureCoordinate)]
            public Vector2 tex;
        }

        public RenderEngine(Control ctrl)
        {
            var present = new PresentParameters(ctrl.ClientSize.Width, ctrl.ClientSize.Height);
            present.PresentationInterval = PresentInterval.One;
            present.BackBufferFormat = Format.A8R8G8B8;
            _Parameters = present;

            _Direct3d = new Direct3D();
            _Device = new Device(_Direct3d, 0,
                DeviceType.Hardware,
                ctrl.Handle,
                CreateFlags.HardwareVertexProcessing,
                present);

            _Device.SetRenderState(RenderState.CullMode, false);
            _Device.SetRenderState(RenderState.ZFunc, Compare.Always);

            _Device.SetTransform(TransformState.View, Matrix.Identity);
            _Device.SetTransform(TransformState.Projection, _Device.GetViewProjectionMatrix());

            _Device.SetRenderState(RenderState.AlphaTestEnable, false);
            _Device.SetRenderState(RenderState.AlphaTestEnable, false);
            _Device.SetRenderState(RenderState.AlphaRef, 0.1f);
            _Device.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);

            _Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            _Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            _Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            _Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            _Effect = Effect.FromString(_Device, Shader.Value, ShaderFlags.None);
            _Effect.SetValue("mat_ViewProj", _Device.GetViewProjectionMatrix());

            _Sprite = new SharpDX.Direct3D9.Sprite(_Device);
        }

        public void Dispose()
        {
            if (_Device == null)
            {
                return;
            }

            Utilities.Dispose(ref _Sprite);

            SinglePixelBitmap.Dispose();
            DoublePixelBitmap.Dispose();

            foreach (var t in _SingleColorTextureList.Values)
            {
                t.Dispose();
            }
            _SingleColorTextureList.Clear();
            foreach (var t in _DoubleColorTextureList.Values)
            {
                t.Dispose();
            }
            _DoubleColorTextureList.Clear();

            Utilities.Dispose(ref _Effect);
            Utilities.Dispose(ref _Device);
            Utilities.Dispose(ref _Direct3d);
        }

        //TODO reset is not properly handled
        private void HandleDeviceLost()
        {
            var result = _Device.TestCooperativeLevel();
            if (result == ResultCode.DeviceNotReset)
            {
                try
                {
                    _Device.Reset(_Parameters);
                    _IsDeviceLost = false;
                }
                catch
                {
                }
            }
        }

        public void RenderAll()
        {
            if (_IsDeviceLost)
            {
                HandleDeviceLost();
                return;
            }

            _Device.BeginScene();
            _Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            //here you can set global transformation if necessary
            _Effect.SetValue("vec_Offset", new Vector4(0, 0, 0, 0));
            _Effect.SetValue("f_Scale", 1.0f);

            _Effect.Begin();
            _Effect.BeginPass(0);

            if (OnRender != null)
            {
                OnRender();
            }

            _Effect.EndPass();
            _Effect.End();

            _Sprite.Begin();
            //_Font.DrawText(_Sprite, "Hello World!", new RawRectangle(100, 100, 200, 120), FontDrawFlags.Left, new RawColorBGRA(0, 0, 0, 255));
            _Sprite.End();

            _Device.EndScene();

            //handle lost device
            try
            {
                _Device.Present();
            }
            catch (Exception e)
            {
                //var state = _Device.TestCooperativeLevel();
                var result = Result.GetResultFromException(e);
                if (result == ResultCode.DeviceLost)
                {
                    _IsDeviceLost = true;
                }
                else
                {
                    //error
                }
            }
        }

        #region Texture
        private MemoryStream _BitmapStream = new MemoryStream(1024 * 1024);
        public Texture CreateTextureFromBitmap(System.Drawing.Bitmap bitmap)
        {
            _BitmapStream.Seek(0, SeekOrigin.Begin);
            bitmap.Save(_BitmapStream, System.Drawing.Imaging.ImageFormat.Bmp);

            _BitmapStream.Seek(0, SeekOrigin.Begin);
            //set size in FromStream, or it will be resized
            return Texture.FromStream(_Device, _BitmapStream, bitmap.Width, bitmap.Height, 1,
                Usage.None, Format.A8R8G8B8, Pool.Managed, Filter.Point, Filter.Point, 0);
        }
        public Texture CreateTextureFromFile(string filename)
        {
            return Texture.FromFile(_Device, filename, Usage.None, Pool.Managed);
        }

        private Texture CreateColorTexture(uint color1, uint color2)
        {
            DoublePixelBitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(((int)color1) ^ 0xFF << 24));
            DoublePixelBitmap.SetPixel(1, 0, System.Drawing.Color.FromArgb(((int)color2) ^ 0xFF << 24));
            return CreateTextureFromBitmap(DoublePixelBitmap);
        }
        private Texture CreateColorTexture(uint color)
        {
            SinglePixelBitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(((int)color) | 0xFF << 24));
            return CreateTextureFromBitmap(SinglePixelBitmap);
        }

        private System.Drawing.Bitmap SinglePixelBitmap =
            new System.Drawing.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        private System.Drawing.Bitmap DoublePixelBitmap =
            new System.Drawing.Bitmap(2, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        private readonly Dictionary<uint, Texture> _SingleColorTextureList = new Dictionary<uint, Texture>();
        private readonly Dictionary<ulong, Texture> _DoubleColorTextureList = new Dictionary<ulong, Texture>();

        public Texture GetColorTexture(uint color)
        {
            Texture ret;
            if (_SingleColorTextureList.TryGetValue(color, out ret))
            {
                return ret;
            }

            ret = CreateColorTexture(color);
            _SingleColorTextureList.Add(color, ret);

            return ret;
        }

        public Texture GetColorTexture(uint color1, uint color2)
        {
            ulong combined = color1 << 32 | color2;
            Texture ret;
            if (_DoubleColorTextureList.TryGetValue(combined, out ret))
            {
                return ret;
            }

            ret = CreateColorTexture(color1, color2);
            _DoubleColorTextureList.Add(combined, ret);
            return ret;
        }

        #endregion
    }
}
