using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class Cylinder : Shape
    {
        private float radius;
        private float height;
        private int segments;

        public Cylinder(Vector3 position, float radius, float height, int segments, Color4 color)
        {
            Transform.Position = position;
            this.radius = radius;
            this.height = height;
            this.segments = segments;
            base.Color = color;
            Corners = null;

            CylinderAssignVerticesAndIndices();
            Normal();
            LoadTexture();
            CreateBuffers();
        }

        public Cylinder(ShapeShaderType type, Vector3 position, Color4 color, float radius = 2, float height = 0.3f, int segments = 50)
        {
            base.ShaderType = type;
            Transform.Position = position;
            this.radius = radius;
            this.height = height;
            this.segments = segments;
            base.Color = color;
            Corners = null;

            CylinderAssignVerticesAndIndices();
            Normal();
            LoadTexture();
            CreateBuffers();
        }

        private void CylinderAssignVerticesAndIndices()
        {
            int vertexCount = 0;
            int indexCount = 0;

            int verticalSegments = segments;
            int totalVertices = (verticalSegments + 1) * 2 + 2; // Üst ve alt kapaklar için ekstra 2 vertex
            int totalIndices = verticalSegments * 12; // Her segment için 12 index (üst ve alt kapaklar dahil)

            Vertices = new VertexPositionNormalTexture[totalVertices];
            Indices = new int[totalIndices];

            // Alt ve üst yüzey merkez vertexleri
            Vertices[vertexCount++] = new VertexPositionNormalTexture(new Vector3(Transform.Position.X, Transform.Position.Y, Transform.Position.Z), Vector3.UnitY, Vector2.Zero); // Alt yüzey merkez vertexi
            Vertices[vertexCount++] = new VertexPositionNormalTexture(new Vector3(Transform.Position.X, Transform.Position.Y + height, Transform.Position.Z), Vector3.UnitY, Vector2.One); // Üst yüzey merkez vertexi

            // Yüzeyin oluşturulması
            for (int i = 0; i <= verticalSegments; i++)
            {
                float theta = (float)i / (float)verticalSegments * MathF.PI * 2.0f;
                float posX = MathF.Cos(theta) * radius;
                float posZ = MathF.Sin(theta) * radius;

                Vertices[vertexCount] = new VertexPositionNormalTexture(new Vector3(posX + Transform.Position.X, Transform.Position.Y, posZ + Transform.Position.Z), Vector3.UnitY, new Vector2((float)i / verticalSegments, 0)); // Alt yüzey vertexleri
                Vertices[vertexCount + 1] = new VertexPositionNormalTexture(new Vector3(posX + Transform.Position.X, Transform.Position.Y + height, posZ + Transform.Position.Z), Vector3.UnitY, new Vector2((float)i / verticalSegments, 1)); // Üst yüzey vertexleri

                if (i < verticalSegments)
                {
                    // Alt ve üst kapaklar
                    Indices[indexCount++] = 0; // Alt yüzey merkez vertexi
                    Indices[indexCount++] = vertexCount;
                    Indices[indexCount++] = vertexCount + 2;

                    Indices[indexCount++] = 1; // Üst yüzey merkez vertexi
                    Indices[indexCount++] = vertexCount + 1;
                    Indices[indexCount++] = vertexCount + 3;

                    // Yan yüzeyler
                    Indices[indexCount++] = vertexCount;
                    Indices[indexCount++] = vertexCount + 1;
                    Indices[indexCount++] = vertexCount + 3;

                    Indices[indexCount++] = vertexCount;
                    Indices[indexCount++] = vertexCount + 3;
                    Indices[indexCount++] = vertexCount + 2;
                }

                vertexCount += 2;
            }
        }

        public void Normal()
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector3 normal = Vertices[i].Position - Transform.Position;
                normal.Normalize();
                Vertices[i].Normal = normal;
            }
        }

        protected override void LoadTexture(string diffuseMapPath = "Resources/green.png", string specularMapPath = "Resources/container2_specular.png")
        {
            base.diffuseMap = Texture.LoadFromFile(diffuseMapPath);
            base.specularMap = Texture.LoadFromFile(specularMapPath);
        }
    }
}