using OpenTK.Mathematics;

namespace PhysicsEngine

{
    public class Cone : Shape
    {
        private float radius;
        private float height;
        private int segments;
        private Color4 color;

        public Cone(Vector3 position, float radius, float height, int segments, Color4 color)
        {
            transform.position = position;
            this.radius = radius;
            this.height = height;

            this.segments = segments;
            this.color = color;
            ConeAssignVerticesAndIndices();
            AssignBuffers();
        }

        private void ConeAssignVerticesAndIndices()
        {
            int vertexCount = 0;
            int indexCount = 0;

            int verticalSegments = segments;
            int totalVertices = (verticalSegments + 1) * 2 + 1; // Top and bottom vertices, plus apex
            int totalIndices = verticalSegments * 6 + verticalSegments * 3; // Side indices and bottom indices

            Vertices = new VertexPositionColor[totalVertices];
            Indices = new int[totalIndices];

            // Create vertices for the apex
            Vector3 apex = new Vector3(transform.position.X, transform.position.Y+ height/2, transform.position.Z);
            Vertices[vertexCount++] = new VertexPositionColor(apex, color);

            // Create vertices for the bottom circle
            for (int i = 0; i <= verticalSegments; i++)
            {
                float theta = (float)i / (float)verticalSegments * MathF.PI * 2.0f;
                float _x = transform.position.X + MathF.Cos(theta) * radius;
                float _z = transform.position.Z + MathF.Sin(theta) * radius;

                // Bottom vertex
                Vertices[vertexCount++] = new VertexPositionColor(new Vector3(_x, transform.position.Y-height/2, _z), color);

                if (i < verticalSegments)
                {
                    // Indices for the sides
                    Indices[indexCount++] = 0; // Apex
                    Indices[indexCount++] = vertexCount - 1; // Current bottom vertex
                    Indices[indexCount++] = vertexCount; // Next bottom vertex
                }
            }

            // Indices for the bottom circle
            for (int i = 1; i <= verticalSegments; i++)
            {
                Indices[indexCount++] = 1; // Center bottom vertex
                Indices[indexCount++] = i + 1; // Current bottom vertex
                Indices[indexCount++] = i == verticalSegments ? 2 : i + 2; // Next bottom vertex or wrap around to the second vertex
            }
        }

    }
}
