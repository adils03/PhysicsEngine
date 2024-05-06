using OpenTK.Mathematics;
using PhysicsEngine.Common;
using PhysicsEngine.Common.BufferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine.Shapes
{
    public class Sphere : Shape
    {
        public float radius;
        private int segments;
        private Color4 color;

        public Sphere(Vector3 position, float radius, int segments, Color4 color)
        {
            Transform.Position = position;
            this.radius = radius;
            this.segments = segments;
            this.color = color;
            SphereAssignVerticesAndIndices();
            Normal();
            LoadTexture();
            CreateBuffers();
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
            vertices = new VertexPositionNormalTexture[totalVertices];
            indices = new int[totalIndices];

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
                    vertices[vertexCount] = new VertexPositionNormalTexture(new Vector3(_x * radius + Transform.Position.X, _y * radius + Transform.Position.Y, _z * radius + Transform.Position.Z),Vector3.Zero,Vector2.Zero);
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
                    indices[indexCount++] = index0;
                    indices[indexCount++] = index1;
                    indices[indexCount++] = index0 + 1;

                    indices[indexCount++] = index1;
                    indices[indexCount++] = index1 + 1;
                    indices[indexCount++] = index0 + 1;
                }

            }

        }
        public void Normal()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 normal = vertices[i].Position - Transform.Position;
                vertices[i].Normal = normal;
            }

        }
        protected override void LoadTexture(string diffuseMapPath = "Resources/container2.png", string specularMapPath = "Resources/container2_specular.png")
        {
            base.diffuseMap = Texture.LoadFromFile(diffuseMapPath);
            base.specularMap = Texture.LoadFromFile(specularMapPath);
        }
    }
}
