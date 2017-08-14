using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.DXRender
{
    public class VertexFieldAttribute : Attribute
    {
        private DeclarationUsage usage;

        public VertexFieldAttribute(DeclarationUsage usage)
        {
            this.usage = usage;
        }

        public DeclarationUsage Usage
        {
            get
            {
                return usage;
            }
        }
    }

    public static class VertexReflection
    {
        private static DeclarationType MapType(Type type)
        {
            if (type.Equals(typeof(Vector2)))
            {
                return DeclarationType.Float2;
            }
            else if (type.Equals(typeof(Vector3)))
            {
                return DeclarationType.Float3;
            }
            else if (type.Equals(typeof(Vector4)))
            {
                return DeclarationType.Float4;
            }
            return DeclarationType.Unused;
        }

        public static VertexDeclaration CreateVertexDeclaration<T>(Device device) where T : struct
        {
            List<VertexElement> ret = new List<VertexElement>();
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                var attr = Utilities.GetCustomAttribute<VertexFieldAttribute>(field);
                if (attr != null)
                {
                    short offset = (short)Marshal.OffsetOf(type, field.Name).ToInt32();
                    DeclarationType dt = MapType(field.FieldType);
                    ret.Add(new VertexElement(0, offset, dt, DeclarationMethod.Default, attr.Usage, 0));
                }
            }
            ret.Sort((VertexElement a, VertexElement b) => a.Offset.CompareTo(b.Offset));
            ret.Add(VertexElement.VertexDeclarationEnd);
            return new VertexDeclaration(device, ret.ToArray());
        }
    }
}
