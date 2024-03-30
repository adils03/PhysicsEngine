using System;
using OpenTK.Graphics.OpenGL4;

namespace PhysicsEngine
{
    public sealed class IndexBuffer : IDisposable
    {
        public static readonly int MinIndexCount = 1;
        public static readonly int MaxIndexCount = 250000;

        private bool disposed;

        public readonly int IndexBufferObject;
        public readonly int IndexCount;
        public readonly bool IsStatic;
        public IndexBuffer(int indexCount, bool isStatic = true)
        {
            disposed = false;
            if (indexCount < MinIndexCount ||
                indexCount > MaxIndexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount));
            }
            IndexCount = indexCount;
            IsStatic = isStatic;
            BufferUsageHint bufferUsageHint = BufferUsageHint.StaticDraw;
            if (!IsStatic)
            {
                bufferUsageHint = BufferUsageHint.StreamDraw;
            }
            IndexBufferObject = GL.GenBuffer();
            //Index için buffer oluşturduk sonra bufferımızı serbest bıraktık
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexCount * sizeof(int), nint.Zero, bufferUsageHint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        ~IndexBuffer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (disposed) return;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(IndexBufferObject);

            disposed = true;
            GC.SuppressFinalize(this);
        }

        public void SetData(int[] data, int count)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if (count <= 0 ||
                count > IndexCount ||
                count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            //Bufferımıza datamızı aktardık
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferObject);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, nint.Zero, count * sizeof(int), data);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

    }
}
