using OpenTK.Mathematics;

namespace PhysicsEngine


{
    public class Sphere : Shape
    {
        public float radius;
        private int segments;
        private Color4 color;

        public Sphere(Vector3 position, float radius, int segments, Color4 color)
        {
            transform.position = position;
            this.radius = radius;
            this.segments = segments;
            this.color = color;
            SphereAssignVerticesAndIndices();
            AssignBuffers();
        }

        private void SphereAssignVerticesAndIndices()
        {
            int vertexCount = 0;
            int indexCount = 0;

            //Kürenin vertex ve indis sayısını hesapla
            int verticalSegments = segments;
            int horizontalSegments = segments * 2;
            int totalVertices = (verticalSegments + 1) * (horizontalSegments + 1);
            int totalIndices = verticalSegments * horizontalSegments * 6;

            //Vertex ve İndis dizileri
            Vertices = new VertexPositionColor[totalVertices];
            Indices = new int[totalIndices];

            //Küre yüzeyinin oluşturulması
            for (int i = 0; i <= verticalSegments; i++)
            {
                float v = (float)i / (float)verticalSegments;
                float latitude = (v - 0.5f) * MathF.PI;
                float sinLat = MathF.Sin(latitude);
                float cosLat = MathF.Cos(latitude);

                for (int j = 0; j <= horizontalSegments; j++)
                {
            
                   float u = (float)j / (float)horizontalSegments;
                    float longitude = u * MathF.PI * 2.0f;
                    float sinLong = MathF.Sin(longitude);
                    float cosLong = MathF.Cos(longitude);

                    float _x = cosLong * cosLat;
                    float _y = sinLat;
                    float _z = sinLong * cosLat;

                    //Kürenin her bir noktası için renk ataması 
                    Vertices[vertexCount] = new VertexPositionColor(new Vector3(_x * radius + transform.position.X, _y * radius + transform.position.Y, _z * radius + transform.position.Z), color);
                    vertexCount++;
                }
            }

            //Kürenin indislerinin oluşturulması
            for (int i = 0; i < verticalSegments; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int index0 = (i * (horizontalSegments + 1)) + j;
                    int index1 = index0 + horizontalSegments + 1;

                    //İndisler belirlenir ve indis dizisine atanır
                    Indices[indexCount++] = index0;
                    Indices[indexCount++] = index1;
                    Indices[indexCount++] = index0 + 1;

                    Indices[indexCount++] = index1;
                    Indices[indexCount++] = index1 + 1;
                    Indices[indexCount++] = index0 + 1;
                }
            }
        }
    }
}
