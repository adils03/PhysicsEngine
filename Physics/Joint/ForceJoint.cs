using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace PhysicsEngine
{
    public class ForceJoint : Joint
    {
        private float strength;

        public ForceJoint(JointConnection connection, float strength) : base(connection)
        {
            this.strength = strength;
        }
        public Vector3 CustomScale(Vector3 vecA, float scale)
        {
            return new Vector3(vecA.X * scale, vecA.Y * scale, vecA.Z * scale);
        }
        public override void UpdateConnectionA()
        {
            Vector3 anchorAPos = GetAnchorAPos();
            Vector3 anchorBPos = GetAnchorBPos();

            Vector3 direction = anchorBPos - anchorAPos;
            direction.Normalize();
            rigiA.AddForceAtPoint(anchorBPos, CustomScale(direction, strength * 0.5f));
        }

        public override void UpdateConnectionB()
        {
            Vector3 anchorAPos = GetAnchorAPos();
            Vector3 anchorBPos = GetAnchorBPos();

            Vector3 direction = anchorAPos - anchorBPos;
            direction.Normalize();
            rigiB.AddForceAtPoint(anchorAPos, CustomScale(direction, strength * 0.5f));
        }
    }
}
