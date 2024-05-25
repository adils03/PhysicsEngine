using OpenTK.Mathematics;


namespace PhysicsEngine
{
    public abstract class Joint
    {
        protected JointConnection jointConnection;
        protected RigidBody rigiA;
        protected int anchorAId;
        protected RigidBody rigiB;
        protected int anchorBId;

        public Joint(JointConnection jointConnection)
        {
            this.jointConnection = jointConnection;
            rigiA = this.jointConnection.rigidBodyA;
            anchorAId = this.jointConnection.anchorAId;

            rigiB = this.jointConnection.rigidBodyB;
            anchorBId = this.jointConnection.anchorBId;

            //if (GetType() == typeof(Joint))
            //{
            //    throw new InvalidOperationException("Cannot construct instances of abstract class 'Joint'");
            //}
        }

        public Vector3 GetAnchorAPos()
        {
            return rigiA.shape.GetAnchorPos(anchorAId);
        }

        public Vector3 GetAnchorBPos()
        {
            return rigiB.shape.GetAnchorPos(anchorBId);
        }

        public abstract void UpdateConnectionA();
        public abstract void UpdateConnectionB();

    }
}
