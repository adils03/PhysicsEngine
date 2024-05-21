using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace PhysicsEngine.Shapes
{
    public class Line {
        public static void DrawLine(Vector3 startPosition, Vector3 endPosition, Color4 color)
        {
            GL.Color4(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(startPosition);
            GL.Vertex3(endPosition);
            GL.End();
        }
    }
    
}
