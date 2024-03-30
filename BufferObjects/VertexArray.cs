using System;
using OpenTK.Graphics.OpenGL4;


namespace PhysicsEngine
{
    public sealed class VertexArray : IDisposable
    {
        private bool disposed;

        public readonly int VertexArrayObject;
        public readonly VertexBuffer VertexBuffer;
        public VertexArray(VertexBuffer vertexBuffer)
        {
            disposed = false;

            if (vertexBuffer == null)
            {
                throw new ArgumentNullException(nameof(vertexBuffer));
            }

            VertexBuffer = vertexBuffer;

            int vertexSizeInBytes = VertexBuffer.VertexInfo.SizeInBytes;
            VertexAttribute[] attributes = VertexBuffer.VertexInfo.VertexAttributes;

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer.VertexBufferObject);


            for (int i = 0; i < attributes.Length; i++)
            {
                VertexAttribute vertexAttribute = attributes[i];
                GL.VertexAttribPointer(vertexAttribute.Index, vertexAttribute.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, vertexAttribute.OffSet);
                GL.EnableVertexAttribArray(vertexAttribute.Index);

            }

            GL.BindVertexArray(0);

        }

        ~VertexArray()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (disposed) return;

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VertexArrayObject);


            disposed = true;


            GC.SuppressFinalize(this);
        }
    }
}
