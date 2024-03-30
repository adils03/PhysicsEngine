using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace PhysicsEngine


{
    public class Line : Shape
    {
        private Vector3 startPosition;
        private Vector3 endPosition;
        private Color4 startPositionColor;
        private Color4 endPositionColor;
        public Line(Vector3 startPosition,Vector3 endPosition,Color4 startPositionColor ,Color4 endPositionColor)
        {
                
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.startPositionColor = startPositionColor == default ? Color4.Red : startPositionColor;
            this.endPositionColor = endPositionColor == default ? Color4.Red : endPositionColor;
            AssignVerticesAndIndices();
            AssignBuffers();
        }

        protected override void AssignVerticesAndIndices()
        {

            Vertices = new VertexPositionColor[2];
            Vertices[0] = new VertexPositionColor(startPosition, startPositionColor);
            Vertices[1] = new VertexPositionColor(endPosition, endPositionColor);

            base.Indices = new int[2];

            base.Indices[0] = 0;
            base.Indices[1] = 1;

        }
        public override void Render(PrimitiveType primitiveType = PrimitiveType.Lines)
        {
            base.Render(primitiveType);
        }
    }
    }
