using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DXRender
{
    class RenderFont : IDisposable
    {
        private RenderEngine _Engine;
        private SharpDX.Direct3D9.Font _Font;

        public RenderFont(RenderEngine re, string fontFace, int width, int height)
        {
            _Engine = re;
            _Font = new SharpDX.Direct3D9.Font(re.Device, height, width,FontWeight.Normal, 1, false,
                FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default,
                FontPitchAndFamily.Default, fontFace);
        }

        public void RenderText(string text, RawRectangle rect, RawColorBGRA color)
        {
            var s = _Engine.DefaultSprite;
            s.Begin();
            _Font.DrawText(s, text, rect, FontDrawFlags.Left, color);
            s.End();
        }

        public void Dispose()
        {
            _Font.Dispose();
        }
    }
}
