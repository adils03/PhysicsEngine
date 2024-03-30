using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class Tetrahedron : Shape
    {
        private Color4 color;
        public Tetrahedron(Vector3 position, float size, Color4 color)
        {
            transform.position = position;
            this.color = color;

            AssignVerticesAndIndices(size);
            AssignBuffers();
        }

        protected void AssignVerticesAndIndices(float size)
        {
            float halfSize = size / 2.0f;

            Vertices = new VertexPositionColor[4];
            Vertices[0] = new VertexPositionColor(transform.position + new Vector3(0.0f, 0.0f, -halfSize), color);
            Vertices[1] = new VertexPositionColor(transform.position + new Vector3(0.0f, 0.0f, halfSize), color);
            Vertices[2] = new VertexPositionColor(transform.position + new Vector3(halfSize, 0.0f, 0.0f), color);
            Vertices[3] = new VertexPositionColor(transform.position + new Vector3(0.0f, size, 0.0f), color);

            base.Indices = new int[12];

            base.Indices[0] = 0;
            base.Indices[1] = 1;
            base.Indices[2] = 2;

            base.Indices[3] = 0;
            base.Indices[4] = 1;
            base.Indices[5] = 3;

            base.Indices[6] = 0;
            base.Indices[7] = 2;
            base.Indices[8] = 3;

            base.Indices[9] = 1;
            base.Indices[10] = 2;
            base.Indices[11] = 3;
        }


    }
}
