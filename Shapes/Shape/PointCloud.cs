using OpenTK.Mathematics;

namespace PhysicsEngine


{
    public class PointCloud : Shape
    {
        private Color4 color;
        private Vector3[] _vertices;
        public PointCloud(Color4 color,params Vector3[] vertices)
        {
            this.color = color;
            _vertices = vertices;
            AssignVerticesAndIndices();
            AssignBuffers();
        }
        protected override void AssignVerticesAndIndices()
        {
            int vertexCount = 0;
            Vertices = new VertexPositionColor[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
            {
                Vertices[vertexCount++] = new VertexPositionColor(_vertices[i], color);

            }
            base.Indices = new int[_vertices.Length];
            int indiceCount = 0;
            for (int i = 0; i < Indices.Length; i++)
            {
                Indices[indiceCount++] = i;
            }
        }
    }
}
