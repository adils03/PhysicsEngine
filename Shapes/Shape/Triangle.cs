using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class Triangle : Shape
    {
        private Color4 color;
        public Triangle(Vector3 position, float size, Color4 color)
        {
            transform.position = position;
            this.color = color;

            AssignVerticesAndIndices(size);
            AssignBuffers();
        }

        protected void AssignVerticesAndIndices(float size)
        {
            float halfSize = size / 2.0f;

            Vertices = new VertexPositionColor[3];
            Vertices[0] = new VertexPositionColor(transform.position + new Vector3(0.0f, 0.0f, -halfSize), color);
            Vertices[1] = new VertexPositionColor(transform.position + new Vector3(0.0f, 0.0f, halfSize), color);
            Vertices[2] = new VertexPositionColor(transform.position + new Vector3(halfSize, 0.0f, 0.0f), color);

            base.Indices = new int[3];

            base.Indices[0] = 0;
            base.Indices[1] = 1;
            base.Indices[2] = 2;

            
        }
    }
}
