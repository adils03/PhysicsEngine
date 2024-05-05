using PhysicsEngine.Common.BufferObjects;
using OpenTK.Mathematics;
using PhysicsEngine.Common;

namespace PhysicsEngine.Shapes
{
    internal class TexturedCube : Shape
    {
        Vector3[] Corners;
        int[][] FaceIndices;
        Vector3[] Normals;
        public TexturedCube(Vector3 position, Vector3 scale)
        {
            Transform.Position = position;
            Transform.Scale = scale;

            AssignVerticesIndices();
            LoadTexture();
            CreateBuffers();
        }
        public TexturedCube()
        {
            AssignVerticesIndices();
            LoadTexture();
            CreateBuffers();
        }
        protected override void AssignVerticesIndices()
        {
            AssignCorners();

            FaceIndices =
            [
              [0, 1, 2, 2, 3, 0], // Ön yüz
              [5, 4, 7, 7, 6, 5], // Arka yüz
              [1, 5, 6, 6, 2, 1], // Sağ yüz
              [4, 0, 3, 3, 7, 4], // Sol yüz
              [3, 2, 6, 6, 7, 3], // Üst yüz
              [4, 5, 1, 1, 0, 4]  // Alt yüz
            ];

            CreateFaceNormals(Corners);

            Vector2[] texCoords =
            {
               new(0, 0), //  sol alt 
               new(1, 0), //  sağ alt
               new(1, 1), //  sağ üst                
               new(1, 1), //  sağ üst 
               new(0, 1), //  sol üst
               new(0, 0), //  sol alt
            };



            vertices = new VertexPositionNormalTexture[FaceIndices.Length * 6];
            indices = new int[FaceIndices.Length * 6];

            int vertexIndex = 0;
            int indexIndex = 0;

            for (int i = 0; i < FaceIndices.Length; i++)
            {
                int[] face = FaceIndices[i];

                for (int j = 0; j < face.Length; j++)
                {
                    vertices[vertexIndex] = new VertexPositionNormalTexture(Corners[face[j]], Vector3.Zero, texCoords[j]);
                    indices[indexIndex++] = vertexIndex++;
                }
            }
            AssignNormals();

        }


        protected override void AssignNormals()
        {
            int vertexIndex = 0;
            int indexIndex = 0;
            for (int i = 0; i < FaceIndices.Length; i++)
            {
                int[] face = FaceIndices[i];

                for (int j = 0; j < face.Length; j++)
                {
                    vertices[vertexIndex].Normal = Normals[i];
                    indices[indexIndex++] = vertexIndex++;
                }
            }

        }
        public void AssignCorners()
        {
            float X = Transform.Scale.X / 2;
            float Y = Transform.Scale.Y / 2;
            float Z = Transform.Scale.Z / 2;

            Vector3 position = Transform.Position;

            Corners =
            [
                position + new Vector3(-X, -Y, -Z), // 0
                position + new Vector3( X, -Y, -Z), // 1
                position + new Vector3( X,  Y, -Z), // 2
                position + new Vector3(-X,  Y, -Z), // 3
                position + new Vector3(-X, -Y,  Z), // 4
                position + new Vector3( X, -Y,  Z), // 5
                position + new Vector3( X,  Y,  Z), // 6
                position + new Vector3(-X,  Y,  Z)  // 7
            ];
            
        }
        protected override void CreateFaceNormals(Vector3[] corners)
        {
            Vector3[] normals = new Vector3[6];
            Vector3 ba = corners[0] - corners[1];
            Vector3 bc = corners[2] - corners[1];
            Vector3 frontFace = Vector3.Cross(bc, ba);
            Vector3 backFace = -frontFace;
            Vector3 bf = corners[5] - corners[1];
            Vector3 rightFace = Vector3.Cross(bf, bc);
            Vector3 leftFace = -rightFace;
            Vector3 cg = corners[6] - corners[2];
            Vector3 cd = corners[3] - corners[2];
            Vector3 upFace = Vector3.Cross(cg, cd);
            Vector3 downFace = -upFace;
            normals[0] = backFace;
            normals[1] = frontFace;
            normals[2] = leftFace;
            normals[3] = rightFace;
            normals[4] = downFace;
            normals[5] = upFace;

            this.Normals = normals;
                   
        }

        protected override void LoadTexture(string diffuseMap = "Resources/container2.png", string specularMap = "Resources/container2_specular.png")
        {
            base.diffuseMap = Texture.LoadFromFile(diffuseMap);
            base.specularMap = Texture.LoadFromFile(specularMap);
        }
    }
}
