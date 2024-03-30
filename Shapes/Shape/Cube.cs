using OpenTK.Mathematics;


namespace PhysicsEngine

{
    public class Cube : Shape
    {
        private Color4 color;
        public Cube(Vector3 position, Color4 color)
        {
            transform.position = position;
            this.color = color;
            
            AssignVerticesAndIndices();
            AssignBuffers();
        }
        protected override void AssignVerticesAndIndices()
        {
            int vertexCount = 0;
            Vertices = new VertexPositionColor[8];
            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(-transform.scale.X / 2, transform.scale.Y / 2, -transform.scale.Z / 2), color);
            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(transform.scale.X / 2, transform.scale.Y / 2, -transform.scale.Z / 2), color);
            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(transform.scale.X / 2, -transform.scale.Y / 2, -transform.scale.Z / 2), color);
            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(-transform.scale.X / 2, -transform.scale.Y / 2, -transform.scale.Z / 2), color);

            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(-transform.scale.X / 2, transform.scale.Y / 2, transform.scale.Z / 2), color);
            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(transform.scale.X / 2, transform.scale.Y / 2, transform.scale.Z / 2), color);
            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(transform.scale.X / 2, -transform.scale.Y / 2, transform.scale.Z / 2), color);
            Vertices[vertexCount++] = new VertexPositionColor(transform.position + new Vector3(-transform.scale.X / 2, -transform.scale.Y / 2, transform.scale.Z / 2),color);



            base.Indices = new int[36];
            int indexCount = 0;

            // Ön yüz
            base.Indices[indexCount++] = 0;
            base.Indices[indexCount++] = 1;
            base.Indices[indexCount++] = 2;
            base.Indices[indexCount++] = 0;
            base.Indices[indexCount++] = 2;
            base.Indices[indexCount++] = 3;

            // Arka yüz
            base.Indices[indexCount++] = 4;
            base.Indices[indexCount++] = 5;
            base.Indices[indexCount++] = 6;
            base.Indices[indexCount++] = 4;
            base.Indices[indexCount++] = 6;
            base.Indices[indexCount++] = 7;

            // Sol yüz
            base.Indices[indexCount++] = 0;
            base.Indices[indexCount++] = 4;
            base.Indices[indexCount++] = 7;
            base.Indices[indexCount++] = 0;
            base.Indices[indexCount++] = 7;
            base.Indices[indexCount++] = 3;

            // Sağ yüz
            base.Indices[indexCount++] = 1;
            base.Indices[indexCount++] = 5;
            base.Indices[indexCount++] = 6;
            base.Indices[indexCount++] = 1;
            base.Indices[indexCount++] = 6;
            base.Indices[indexCount++] = 2;

            // Üst yüz
            base.Indices[indexCount++] = 0;
            base.Indices[indexCount++] = 1;
            base.Indices[indexCount++] = 5;
            base.Indices[indexCount++] = 0;
            base.Indices[indexCount++] = 5;
            base.Indices[indexCount++] = 4;

            // Alt yüz
            base.Indices[indexCount++] = 2;
            base.Indices[indexCount++] = 3;
            base.Indices[indexCount++] = 7;
            base.Indices[indexCount++] = 2;
            base.Indices[indexCount++] = 7;
            base.Indices[indexCount++] = 6;
        }

    }
}
