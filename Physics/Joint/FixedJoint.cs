using OpenTK.Mathematics;


namespace PhysicsEngine
{
    public class FixedJoint : Joint
    {
        private RigidBody rigiA;
        private RigidBody rigiB;
        private float initialLength;

        public FixedJoint(JointConnection connection) : base(connection) 
        {
            rigiA = jointConnection.rigidBodyA;
            rigiB = jointConnection.rigidBodyB;


            Vector3 anchorAPos = jointConnection.GetAnchorAPos();
            Vector3 anchorBPos = jointConnection.GetAnchorBPos();
            initialLength = (anchorAPos - anchorBPos).Length;
        }

        public override void UpdateConnectionA()
        {
            
        }

        public override void UpdateConnectionB() 
        {

        }
    }

}
