
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
