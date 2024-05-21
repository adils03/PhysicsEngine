using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace PhysicsEngine
{
    public class JointConnection
    {
        public RigidBody rigidBodyA;
        public int anchorAId;
        public RigidBody rigidBodyB;
        public int anchorBId;

        public JointConnection(RigidBody rigidBodyA, int anchorAId, RigidBody rigidBodyB, int anchorBId)
        {
            this.rigidBodyA = rigidBodyA;
            this.anchorAId = anchorAId;
            this.rigidBodyB = rigidBodyB;
            this.anchorBId = anchorBId;
        }
    }
}
