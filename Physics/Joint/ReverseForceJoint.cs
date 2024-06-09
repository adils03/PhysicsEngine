using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class ReverseForceJoint : Joint
    {
        private float strength;
        private float maxAffectDistance;

        public ReverseForceJoint(JointConnection connection, float strength, float maxAffectDistance) : base(connection)
        {
            this.strength = strength;
            this.maxAffectDistance = maxAffectDistance;
        }

        public override void UpdateConnectionA()
        {
            if (rigiA.isKinematic) return;

            Vector3 anchorAPos = GetAnchorAPos();
            Vector3 anchorBPos = GetAnchorBPos();

            Vector3 direction = anchorBPos - anchorAPos;
            float distance = direction.Length;
            direction.Normalize();
            float forceMagnitude = MathF.Max(0, maxAffectDistance - distance);

            float forceHalving = rigiB.isKinematic ? 1 : 0.5f;
            rigiA.AddForceAtPoint(anchorBPos, direction * forceMagnitude * strength * forceHalving * -1);     
        }

        public override void UpdateConnectionB()
        {
            if (rigiB.isKinematic) return;

            Vector3 anchorAPos = GetAnchorAPos();
            Vector3 anchorBPos = GetAnchorBPos();

            Vector3 direction = anchorAPos - anchorBPos;
            float distance = direction.Length;
            direction.Normalize();
            float forceMagnitude = MathF.Max(0, maxAffectDistance - distance);

            float forceHalving = rigiA.isKinematic ? 1 : 0.5f;
            rigiB.AddForceAtPoint(anchorAPos, direction * forceMagnitude * strength * forceHalving * -1);         
        }
    }
}
