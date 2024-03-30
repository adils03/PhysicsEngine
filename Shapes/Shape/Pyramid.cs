using OpenTK.Mathematics;

namespace PhysicsEngine


{
    public class Pyramid : Shape
    {
        private float radius;
        private float height;
        private Color4 color;

        public Pyramid(Vector3 position, float radius, float height, Color4 color)
        {
            transform.position = position;
            this.radius = radius;
            this.height = height;
            this.color = color;
            AssignVerticesAndIndices();
            AssignBuffers();
        }

        protected override void AssignVerticesAndIndices()
        {
            int vertexCount = 0;
            Vertices = new VertexPositionColor[5];
            Vertices[vertexCount++] = new VertexPositionColor(new Vector3(transform.position.X, transform.position.Y + height, transform.position.Z), color);

            Vertices[vertexCount++] = new VertexPositionColor(new Vector3(transform.position.X - radius, transform.position.Y, transform.position.Z + radius), color);
            Vertices[vertexCount++] = new VertexPositionColor(new Vector3(transform.position.X + radius, transform.position.Y, transform.position.Z + radius), color);
            Vertices[vertexCount++] = new VertexPositionColor(new Vector3(transform.position.X + radius, transform.position.Y, transform.position.Z - radius), color);
            Vertices[vertexCount++] = new VertexPositionColor(new Vector3(transform.position.X - radius, transform.position.Y, transform.position.Z - radius), color);




            base.Indices = new int[20];
            int indexCount = 0;
            Indices[indexCount++] = 0;
            Indices[indexCount++] = 1;
            Indices[indexCount++] = 4;

            Indices[indexCount++] = 0;
            Indices[indexCount++] = 1;
            Indices[indexCount++] = 2;

            Indices[indexCount++] = 0;
            Indices[indexCount++] = 2;
            Indices[indexCount++] = 3;

            Indices[indexCount++] = 0;
            Indices[indexCount++] = 3;
            Indices[indexCount++] = 4;

            Indices[indexCount++] = 1;
            Indices[indexCount++] = 2;
            Indices[indexCount++] = 4;

            Indices[indexCount++] = 2;
            Indices[indexCount++] = 3;
            Indices[indexCount++] = 4;
        }

    }
}
