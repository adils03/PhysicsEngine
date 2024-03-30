using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class Shape
    {
        protected VertexPositionColor[] Vertices;
        protected int[] Indices;
        private VertexBuffer VertexBuffer;
        private IndexBuffer IndexBuffer;
        private VertexArray? VertexArray;
        public Transfom transform = new();

        /// <summary>
        /// Create and assign buffers to shape.
        /// </summary>
        protected void AssignBuffers()
        {
            VertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, Vertices.Length, true);
            VertexBuffer.SetData(Vertices, Vertices.Length);

            IndexBuffer = new IndexBuffer(Indices.Length, true);
            IndexBuffer.SetData(Indices, Indices.Length);

            VertexArray = new VertexArray(VertexBuffer);

            Game.Instance.models.Add(this);

        }
        ~Shape()
        {
            Destroy();
        }

        /// <summary>
        /// Renders the shape.
        /// </summary>
        public virtual void Render(PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            VertexBuffer.SetData(Vertices, Vertices.Length);
            GL.BindVertexArray(VertexArray.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer.IndexBufferObject);
            GL.DrawElements(primitiveType, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// Assign vertices and indices of shape.
        /// </summary>
        protected virtual void AssignVerticesAndIndices() { }
        /// <summary>
        /// Destroy the shape.
        /// </summary>
        public void Destroy()
        {
            VertexArray?.Dispose();

            IndexBuffer?.Dispose();

            VertexBuffer?.Dispose();

            Game.Instance.models.Remove(this);

            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Rotate shape with given vector.
        /// </summary>
        /// <param name="rotateVector">Rotae vector.</param>
        public void Rotate(Vector3 rotateVector)
        {
            // Derece cinsinden açıları radyan cinsine dönüştürme
            float radiansX = MathHelper.DegreesToRadians(rotateVector.X);
            float radiansY = MathHelper.DegreesToRadians(rotateVector.Y);
            float radiansZ = MathHelper.DegreesToRadians(rotateVector.Z);

            // Radyan cinsinden açıları kullanarak Quaternion oluşturma
            Quaternion quaternion = Quaternion.FromEulerAngles(radiansX, radiansY, radiansZ);

            transform.rotation = quaternion;

            for (int i = 0; i < Vertices.Length; i++)
            {
                // Vertex pozisyonunu radyan cinsinden döndürme
                Vector3 newPosition = Vector3.Transform(Vertices[i].Position - transform.position, quaternion) + transform.position;
                Vertices[i].Position = newPosition;
            }
        }

        /// <summary>
        /// Translate shape with given vector.
        /// </summary>
        /// <param name="translateVector">Translate vector.</param>
        public void Translate(Vector3 translateVector)
        {
            transform.position += translateVector;
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position += translateVector;
            }
        }
        /// <summary>
        /// Teleport shape to given vector.
        /// </summary>
        /// <param name="point">Teleport vector.</param>
        public void Teleport(Vector3 point)
        {
            Vector3 offset = point - transform.position;

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position += offset;
            }

            transform.position = point;
        }

        /// <summary>
        /// Scale the shape with given vector.
        /// </summary>
        /// <param name="scaleVector">Scale vector.</param>
        public void Scale(Vector3 scaleVector)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position *= scaleVector;
                Vertices[i].Position -= transform.position;
            }
            transform.scale = scaleVector;
        }

        /// <summary>
        /// Change the color of shape.
        /// </summary>
        /// <param name="color"></param>
        public void ChangeColor(Color4 color)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Color = color;
            }
        }
        /// <summary>
        /// Return the vertices of shape.
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetVertices()
        {
            Vector3[] _vertices = new Vector3[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
            {
                _vertices[i] = Vertices[i].Position;
            }
            return _vertices;
        }
        /// <summary>
        /// Set the vertices of shape.
        /// </summary>
        /// <param name="_vertices">Vertices to set.</param>
        public void SetVertices(params Vector3[] _vertices)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = _vertices[i];
            }

        }

        public void ChangeVerticeColor(Vector3 vertex, Color4 color,bool debug = false)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                if (Vertices[i].Position == vertex)
                {
                    if (debug)
                    {
                        Console.WriteLine("bulundu");

                    }
                    Vertices[i].Color = color;
                }

            }
        }
    }
}
