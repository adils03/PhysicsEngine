using System;
using OpenTK.Mathematics;

namespace PhysicsEngine
{
    /// <summary>
    /// Represents a vertex attribute used in shader code.
    /// </summary>
    public readonly struct VertexAttribute
    {
        /// <summary>
        /// Gets the name of the attribute related to shader code.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets the index of the attribute related to shader code.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Gets the number of components in the attribute. 
        /// For example, Color4 has 4 components, Vector2 has 2 components.
        /// </summary>
        public readonly int ComponentCount;

        /// <summary>
        /// Gets the offset in bytes to access this attribute.
        /// </summary>
        public readonly int OffSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexAttribute"/> struct.
        /// </summary>
        /// <param name="name">The name of the attribute related to shader code.</param>
        /// <param name="index">The index of the attribute related to shader code.</param>
        /// <param name="componentCount">The number of components in the attribute.</param>
        /// <param name="offSet">The offset in bytes to access this attribute.</param>
        public VertexAttribute(string name, int index, int componentCount, int offSet)
        {
            Name = name;
            Index = index;
            ComponentCount = componentCount;
            OffSet = offSet;
        }
    }


    /// <summary>
    /// Represents information about a vertex structure.
    /// </summary>
    public sealed class VertexInfo
    {
        /// <summary>
        /// Gets the type of vertex structure.
        /// </summary>
        public Type Type;

        /// <summary>
        /// Gets the size in bytes of attributes in the vertex structure.
        /// </summary>
        public int SizeInBytes;

        /// <summary>
        /// Gets the array of vertex attributes associated with the vertex structure.
        /// </summary>
        public VertexAttribute[] VertexAttributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexInfo"/> class.
        /// </summary>
        /// <param name="type">The type of vertex structure.</param>
        /// <param name="attributes">The vertex attributes associated with the vertex structure.</param>
        public VertexInfo(Type type, params VertexAttribute[] attributes)
        {
            Type = type;
            SizeInBytes = 0;
            VertexAttributes = attributes;

            for (int i = 0; i < VertexAttributes.Length; i++)
            {
                VertexAttribute vertexAttribute = VertexAttributes[i];
                SizeInBytes += vertexAttribute.ComponentCount * sizeof(float);
            }
        }
    }


    /// <summary>
    /// Represents a vertex with position and color attributes.
    /// </summary>
    public struct VertexPositionColor
    {
        /// <summary>
        /// Gets the position of the vertex.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Gets the color of the vertex.
        /// </summary>
        public  Color4 Color;

        /// <summary>
        /// Gets the vertex information for <see cref="VertexPositionColor"/>.
        /// </summary>
        public static readonly VertexInfo VertexInfo = new VertexInfo
        (
            typeof(VertexPositionColor),
            new VertexAttribute("Position", 0, 3, 0), //3= Vector3 component amount //0 = It's the first attribute.That's why no offset
            new VertexAttribute("Color", 1, 4, 3 * sizeof(float))//4= Color4 component amount //3 = First attribute has 3 components
        );

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPositionColor"/> struct.
        /// </summary>
        /// <param name="position">The position of the vertex.</param>
        /// <param name="color">The color of the vertex.</param>
        public VertexPositionColor(Vector3 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }

}
