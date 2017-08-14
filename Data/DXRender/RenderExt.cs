using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DXRender
{
    static class RenderExt
    {
        public static Matrix GetViewProjectionMatrix(this Device device)
        {
            return Matrix.Multiply(device.GetDefault2DViewMatrix(), device.GetDefault2DProjectionMatrix());
        }

        public static Matrix GetDefault2DViewMatrix(this Device device)
        {
            return Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
        }

        public static Matrix GetDefault2DProjectionMatrix(this Device device)
        {
            float width = device.Viewport.Width;
            float height = device.Viewport.Height;
            return Matrix.OrthoOffCenterLH(0, width, 0, height, -100, 100) * Matrix.Scaling(1, -1, 1);
        }

        public static int GetWidth(this Texture texture)
        {
            var surface = texture.GetSurfaceLevel(0);
            var desc = surface.Description;
            return desc.Width;
        }

        public static int GetHeight(this Texture texture)
        {
            var surface = texture.GetSurfaceLevel(0);
            var desc = surface.Description;
            return desc.Height;
        }
    }
}
