using System;
using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public readonly struct VertexAttribute
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int ComponentCount;
        public readonly int Offset;

        public VertexAttribute(string name, int index, int componentCount, int offset)
        {
            this.Name = name;
            this.Index = index;
            this.ComponentCount = componentCount;
            this.Offset = offset;
        }
    }
    public sealed class VertexInfo
    {
        public readonly Type Type;
        public readonly int SizeInBytes;
        public readonly VertexAttribute[] VertexAttributes;

        public VertexInfo(Type type, params VertexAttribute[] attributes)
        {
            this.Type = type;
            this.SizeInBytes = 0;

            this.VertexAttributes = attributes;

            for (int i = 0; i < this.VertexAttributes.Length; i++)
            {
                VertexAttribute attribute = this.VertexAttributes[i];
                this.SizeInBytes += attribute.ComponentCount * sizeof(float);
            }
        }
    }
    public readonly struct VertexPositionColor
    {
        public readonly Vector3 Position;
        public readonly Color4 Color;

        public static readonly VertexInfo VertexInfo = new(
            typeof(VertexPositionColor),
            new VertexAttribute("Position", 0, 3, 0),
            new VertexAttribute("Color", 1, 4, 3 * sizeof(float))
            );

        public VertexPositionColor(Vector3 position, Color4 color)
        {
            this.Position = position;
            this.Color = color;
        }
    }
    public struct VertexPositionNormal
    {
        public readonly Vector3 Position;
        public Vector3 Normal;

        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPositionNormal),
            new VertexAttribute("Position", 0, 3, 0),
            new VertexAttribute("Normal", 1, 3, 3 * sizeof(float))
            );

        public VertexPositionNormal(Vector3 position, Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
        }
    }
    public readonly struct VertexPositionTexture
    {
        public readonly Vector3 Position;
        public readonly Vector2 TexCoord;

        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPositionTexture),
            new VertexAttribute("Positon", 0, 3, 0),
            new VertexAttribute("TexCoord", 1, 2, 3 * sizeof(float))
            );

        public VertexPositionTexture(Vector3 position, Vector2 texCoord)
        {
            this.Position = position;
            this.TexCoord = texCoord;
        }
    }
    public readonly struct VertexPosition
    {
        public readonly Vector3 Position;
        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPosition),
            new VertexAttribute("Positon", 0, 3, 0));
        public VertexPosition(Vector3 position)
        {
            this.Position = position;
        }
    }
    public struct VertexPositionNormalTexture
    {
        public Vector3 Position;
        public Vector3 Normal;
        public readonly Vector2 TexCoord;

        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPositionNormalTexture),
            new VertexAttribute("Position", 0, 3, 0),
            new VertexAttribute("Normal", 1, 3, 3 * sizeof(float)),
            new VertexAttribute("TexCoord", 2, 2, 6 * sizeof(float))
        );

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            this.Position = position;
            this.Normal = normal;
            this.TexCoord = texCoord;
        }
    }

}
