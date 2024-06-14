using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class Cube : Shape
    {
      
        public Cube(Vector3 position)
        {
            base.Transform.Position = position;

            AssignVerticesIndices();
            LoadTexture();
            CreateBuffers();
        }
        public Cube()
        {
            AssignVerticesIndices();
            LoadTexture();
            CreateBuffers();
        }
        public Cube(Vector3 position, ShapeShaderType type, Color4 color, Vector3 scale)
        {
            base.ShaderType = type;
            base.Transform.Position = position;
            base.Color = color;
            base.Transform.Scale = scale;
            AssignVerticesIndices();
            LoadTexture();
            CreateBuffers();
        }
        public Cube(Vector3 position, Vector3 scale, string texturePath1,string texturePath2,ShapeShaderType type)
        {
            base.ShaderType = type;
            base.Transform.Position = position;
            base.Transform.Scale = scale;
            AssignVerticesIndices();
            LoadTexture(texturePath1,texturePath1);
            CreateBuffers();
        }

        public Cube(Vector3 position,ShapeShaderType type, Vector3 scale)
        {
            base.ShaderType = type;
            base.Transform.Position = position;
            base.Transform.Scale = scale;
            AssignVerticesIndices();
            LoadTexture();
            CreateBuffers();
        }

        public Cube(string texturePath1)
        {
            AssignVerticesIndices();
            LoadTexture(texturePath1);
            CreateBuffers();
        }

        protected override void AssignVerticesIndices()
        {
            AssignVertices();

            int[][] faceIndices =
            {
                 new int[] { 0, 1, 2, 2, 3, 0 }, // Ön yüz
                 new int[] { 5, 4, 7, 7, 6, 5 }, // Arka yüz
                 new int[] { 1, 5, 6, 6, 2, 1 }, // Sağ yüz
                 new int[] { 4, 0, 3, 3, 7, 4 }, // Sol yüz
                 new int[] { 3, 2, 6, 6, 7, 3 }, // Üst yüz
                 new int[] { 4, 5, 1, 1, 0, 4 }  // Alt yüz
            };


            Vector3[] normals =
            {
                 new Vector3(0, 0, -1), // Ön yüz
                 new Vector3(0, 0, 1),  // Arka yüz
                 new Vector3(1, 0, 0),  // Sağ yüz
                 new Vector3(-1, 0, 0), // Sol yüz
                 new Vector3(0, 1, 0),  // Üst yüz
                 new Vector3(0, -1, 0)  // Alt yüz
            };

            Vector2[] texCoords =
            {
                  new Vector2(0, 0), //  köşe çizim sırasına göre textcord veriyoruz 0,0 sol alt 
                  new Vector2(1, 0), //  sağ alt
                  new Vector2(1, 1), //  sağ üst                
                  new Vector2(1, 1), //  sağ üst 
                  new Vector2(0, 1), //  sol üst
                  new Vector2(0, 0), //  sol alt
                  // normalde 4 adet texturecordinat olur fakat bir kare için 6 index tanımladığımız için for döngüsündekolay kontrol sağlamak
                  // adına bu yönteme başvurduk
            };

            //Texture defaultTexture = Texture.LoadFromFile("Resources/container.png");

            Vertices = new VertexPositionNormalTexture[faceIndices.Length * 6];
            Indices = new int[faceIndices.Length * 6];

            int vertexIndex = 0;
            int indexIndex = 0;

            for (int i = 0; i < faceIndices.Length; i++)
            {
                int[] face = faceIndices[i];

                for (int j = 0; j < face.Length; j++)
                {
                    Vertices[vertexIndex] = new VertexPositionNormalTexture(Corners[face[j]], normals[i], texCoords[j]);
                    Indices[indexIndex++] = vertexIndex++;
                }
            }

            //texture = defaultTexture;
        }
        private void AssignVertices()
        {
            float X = base.Transform.Scale.X / 2;
            float Y = base.Transform.Scale.Y / 2;
            float Z = base.Transform.Scale.Z / 2;

            Vector3 position = base.Transform.Position;

            Corners = new Vector3[]
            {
                   position + new Vector3(-X, -Y, -Z), // 0
                   position + new Vector3( X, -Y, -Z), // 1
                   position + new Vector3( X,  Y, -Z), // 2
                   position + new Vector3(-X,  Y, -Z), // 3
                   position + new Vector3(-X, -Y,  Z), // 4
                   position + new Vector3( X, -Y,  Z), // 5
                   position + new Vector3( X,  Y,  Z), // 6
                   position + new Vector3(-X,  Y,  Z)  // 7
            };
            
        }

        public override Vector3[] GetVertices()
        {
            return Corners;
        }
        public override Vector3[] GetNormals()
        {
            Vector3[] normals = new Vector3[6];
            int normalIndex = 0;
            for (int i = 0; i < Vertices.Length; i+=6)
            {
                normals[normalIndex++] = Vertices[i].Normal;
            }
            return normals;
        }

        protected override void LoadTexture(string diffuseMapPath = "Resources/container2.png", string specularMapPath = "Resources/container2_specular.png")
        {
            base.diffuseMap = Texture.LoadFromFile(diffuseMapPath);
            base.specularMap = Texture.LoadFromFile(diffuseMapPath);
        }

    }
}
