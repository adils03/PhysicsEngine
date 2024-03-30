using OpenTK.Mathematics;


namespace PhysicsEngine

{
    public class Cylinder : Shape
    {
        private float radius;
        private float height;
        private int segments;
        private Color4 color;

        public Cylinder(Vector3 position, float radius, float height, int segments, Color4 color)
        {
            transform.position = position;
            this.radius = radius;
            this.height = height;

            this.segments = segments;
            this.color = color;
            CylinderAssignVerticesAndIndices();
            AssignBuffers();
        }

        private void CylinderAssignVerticesAndIndices()
        {
            int vertexCount = 0;
            int indexCount = 0;

            int verticalSegments = segments;
            int totalVertices = (verticalSegments + 1) * 2 + 2; // Top and bottom vertices
            int totalIndices = verticalSegments * 6 + segments * 6; // Top and bottom indices

            Vertices = new VertexPositionColor[totalVertices];
            Indices = new int[totalIndices];

            // Create vertices for top and bottom
            Vector3 topCenter = new Vector3(transform.position.X, transform.position.Y + height / 2, transform.position.Z);
            Vector3 bottomCenter = new Vector3(transform.position.X, transform.position.Y - height / 2, transform.position.Z);

            Vertices[vertexCount++] = new VertexPositionColor(topCenter, color); // Top center vertex
            Vertices[vertexCount++] = new VertexPositionColor(bottomCenter, color); // Bottom center vertex

            // Create vertices for the cylinder body
            for (int i = 0; i <= verticalSegments; i++)
            {
                float theta = (float)i / (float)verticalSegments * MathF.PI * 2.0f;
                float _x = MathF.Cos(theta) * radius;
                float _z = MathF.Sin(theta) * radius;

                // Upper vertex
                Vertices[vertexCount++] = new VertexPositionColor(new Vector3(_x + transform.position.X, topCenter.Y, _z + transform.position.Z), color);
                // Lower vertex
                Vertices[vertexCount++] = new VertexPositionColor(new Vector3(_x + transform.position.X, bottomCenter.Y, _z + transform.position.Z), color);

                if (i < verticalSegments)
                {
                    // Indices for the sides
                    Indices[indexCount++] = vertexCount - 2;
                    Indices[indexCount++] = vertexCount;
                    Indices[indexCount++] = vertexCount - 1;

                    Indices[indexCount++] = vertexCount - 1;
                    Indices[indexCount++] = vertexCount;
                    Indices[indexCount++] = vertexCount + 1;
                }
            }

            // Indices for top and bottom faces
            for (int i = 0; i < verticalSegments; i++)
            {
                // Top face indices
                Indices[indexCount++] = 0; // Top center
                Indices[indexCount++] = i * 2 + 2; // Current upper vertex
                Indices[indexCount++] = (i + 1) * 2 + 2; // Next upper vertex

                // Bottom face indices
                Indices[indexCount++] = 1; // Bottom center
                Indices[indexCount++] = (i + 1) * 2 + 1; // Next lower vertex
                Indices[indexCount++] = i * 2 + 1; // Current lower vertex
            }
        }
    
    
}
}
