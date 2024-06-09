using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class SpringJoint : Joint
    {
        private float springConstant;
        private float restLength;

        public SpringJoint(JointConnection connection, float springConstant, float restLength) : base(connection)
        {
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        public Vector3 CustomScale(Vector3 vecA, float scale)
        {
            return new Vector3(vecA.X * scale, vecA.Y * scale, vecA.Z * scale);
        }

        public override void UpdateConnectionA()
        {
            if (rigiA.isStatic) return;

            Vector3 anchorAPos = GetAnchorAPos();
            Vector3 anchorBPos = GetAnchorBPos();

            Vector3 direction = anchorBPos - anchorAPos;
            float distance = direction.Length;
            float restDistance = distance - restLength;

            float forceHalving = rigiB.isStatic ? 1 : 0.5f;
            float forceMagnitude = restDistance * restLength * springConstant * forceHalving;

            direction.Normalize();
            Vector3 force = CustomScale(direction, forceMagnitude);
            rigiA.AddForceAtPoint(anchorAPos, force);

           
        }

        public override void UpdateConnectionB()
        {
            if (rigiB.isStatic) return;

            Vector3 anchorAPos = GetAnchorAPos();
            Vector3 anchorBPos = GetAnchorBPos();

            Vector3 direction = anchorAPos - anchorBPos;
            float distance = direction.Length;
            float restDistance = distance - restLength;

            float forceHalving = rigiA.isStatic ? 1f : 0.5f;
            float forceMagnitude = restDistance * restLength * springConstant * forceHalving;

            direction.Normalize();
            Vector3 force = CustomScale(direction, forceMagnitude);
            rigiB.AddForceAtPoint(anchorBPos, force);

        }
    }
}
