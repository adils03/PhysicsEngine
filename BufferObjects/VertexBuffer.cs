using System;
using OpenTK.Graphics.OpenGL4;


namespace PhysicsEngine
{
    public sealed class VertexBuffer : IDisposable
    {
        public static readonly int MaxVertexCount = 100000;
        public static readonly int MinVertexCount = 1;

        private bool disposed;

        public readonly int VertexBufferObject;
        public readonly VertexInfo VertexInfo;
        public readonly int VertexCount;
        public readonly bool IsStatic;
        public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = true)
        {
            disposed = false;

            if (vertexCount < MinVertexCount || vertexCount > MaxVertexCount)
            {
                throw new ArgumentException(nameof(vertexCount));
            }
            VertexInfo = vertexInfo;
            VertexCount = vertexCount;
            IsStatic = isStatic;

            BufferUsageHint bufferUsageHint = BufferUsageHint.StaticDraw;
            if (!IsStatic)
            {
                bufferUsageHint = BufferUsageHint.StreamDraw;
            }
            //Vertices için buffer oluşturduk  sonra bufferımızı serbest bıraktık
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, nint.Zero, bufferUsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }

        ~VertexBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);


            disposed = true;


            GC.SuppressFinalize(this);
        }

        public void SetData<T>(T[] data, int count) where T : struct
        {
            if (typeof(T) != VertexInfo.Type)
            {
                throw new ArgumentException("Generic type 'T' doesn't match the vertex type of the vertex buffer.");
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if (count <= 0 ||
                count > VertexCount ||
                count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            //Vertex buuferımıza datamızı kaydettik
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferSubData(BufferTarget.ArrayBuffer, nint.Zero, count * VertexInfo.SizeInBytes, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }



    }
}
