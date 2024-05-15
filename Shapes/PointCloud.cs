using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    public class PointCloud : Shape
    {
        private Color4 color;
        private Vector3[] _vertices;
        public PointCloud(Color4 color, params Vector3[] vertices)
        {
            this.color = color;
            _vertices = vertices;
            AssignVerticesIndices();
            LoadTexture();
            CreateBuffers();
        }
        protected override void AssignVerticesIndices()
        {
            int vertexCount = 0;
            Vertices = new VertexPositionNormalTexture[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
            {
                Vertices[vertexCount++] = new VertexPositionNormalTexture(_vertices[i],Vector3.Zero,Vector2.Zero);

            }
            base.Indices = new int[_vertices.Length];
            int indiceCount = 0;
            for (int i = 0; i < Indices.Length; i++)
            {
                Indices[indiceCount++] = i;
            }
        }
        protected override void LoadTexture(string diffuseMapPath = "Resources/green.png", string specularMapPath = "Resources/container2_specular.png")
        {
            base.diffuseMap = Texture.LoadFromFile(diffuseMapPath);
            base.specularMap = Texture.LoadFromFile(specularMapPath);
        }
    }
}
