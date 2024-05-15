

using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class PhysicsWorld
    {
        // Sabitler
        public static readonly float WORLD_GRAVITY = 9.81f;
        public static readonly float MOON_GRAVITY = 1.62f;
        public static readonly float MARS_GRAVITY = 3.71f;
        public static readonly float VENUS_GRAVITY = 8.87f;
        public static readonly float MIN_DENSITY = 0.0012f;
        public static readonly float MAX_DENSITY = 22.59f;
        public static readonly float MIN_MASS = 0.0001f;
        public static readonly float MAX_MASS = 100000f;
        public const int MIN_ITERATIONS = 1;
        public const int MAX_ITERATIONS = 128;

        public readonly List<Shape> shapes;
        public List<RigidBody> rigidBodies;
        private List<CollisionInfo> collisionInfos;
        private List<(int, int)> contactPairs;

        private Vector3 gravity;
        private Octree octree;

        public PhysicsWorld(float gravityAmount, Vector3 worldSize)
        {
            gravity = new Vector3(0, -gravityAmount, 0);
            shapes = new List<Shape>();
            rigidBodies = new List<RigidBody>();
            collisionInfos = new List<CollisionInfo>();
            contactPairs = new List<(int, int)>();
            octree = new Octree(worldSize, 1); // Octree'yi başlatırken dünya boyutunu ve minimum düğüm boyutunu belirtin.
        }

        public void AddBody(RigidBody rigidBody)
        {
            rigidBodies.Add(rigidBody);
            octree.Insert(rigidBody); // RigidBody nesnesini Octree'ye ekleyin.
        }

        public bool RemoveBody(RigidBody rigidBody)
        {
            if (rigidBody.shape != null)
            {
                shapes.Remove(rigidBody.shape);
            }

            bool removed = rigidBodies.Remove(rigidBody);

            if (removed)
            {
                octree.Remove(rigidBody); // RigidBody nesnesini Octree'den çıkarın.

                contactPairs = contactPairs
                    .Where(pair => rigidBodies[pair.Item1] != rigidBody && rigidBodies[pair.Item2] != rigidBody)
                    .ToList();

                collisionInfos = collisionInfos
                    .Where(info => info.bodyA != rigidBody && info.bodyB != rigidBody)
                    .ToList();
            }

            return removed;
        }

        public bool GetBody(int index, out RigidBody rigidBody)
        {
            rigidBody = null;
            if (index < 0 || index >= rigidBodies.Count)
            {
                return false;
            }
            rigidBody = rigidBodies[index];
            return true;
        }

        public void Update(float time, int iterations)
        {
            iterations = MathHelper.Clamp(iterations, MIN_ITERATIONS, MAX_ITERATIONS);

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                collisionInfos.Clear();
                contactPairs.Clear();

                StepBodies(time, iterations);
                BroadPhase();
                NarrowPhase();
            }
        }

        private void BroadPhase()
        {
            octree.Clear();
            foreach (var body in rigidBodies)
            {
                octree.Insert(body);
            }

            foreach (var body in rigidBodies)
            {
                List<RigidBody> potentialColliders = octree.Retrieve(body);
                foreach (var other in potentialColliders)
                {
                    if (body == other || (body.isStatic && other.isStatic))
                    {
                        continue;
                    }

                    if (!Collisions.AABBCheck(body.GetAABB(), other.GetAABB()))
                    {
                        continue;
                    }

                    contactPairs.Add((rigidBodies.IndexOf(body), rigidBodies.IndexOf(other)));
                }
            }
        }

        private void StepBodies(float time, int iterations)
        {
            foreach (var body in rigidBodies)
            {
                body.Update(time, gravity, iterations);
            }
        }

        private void NarrowPhase()
        {
            foreach (var pair in contactPairs)
            {
                RigidBody rigidBodyA = rigidBodies[pair.Item1];
                RigidBody rigidBodyB = rigidBodies[pair.Item2];

                if (Collisions.Collide(rigidBodyA, rigidBodyB, out Vector3 normal, out float depth))
                {
                    if (rigidBodyA.isStatic)
                    {
                        rigidBodyB.Move(normal * depth);
                    }
                    else if (rigidBodyB.isStatic)
                    {
                        rigidBodyA.Move(-normal * depth);
                    }
                    else
                    {
                        rigidBodyA.Move(-normal * depth / 2f);
                        rigidBodyB.Move(normal * depth / 2f);
                    }

                    Collisions.FindContactPoints(rigidBodyA, rigidBodyB, out Vector3 contact1, out Vector3 contact2, out Vector3 contact3, out Vector3 contact4, out int contactCount);
                    CollisionInfo collisionInfo = new CollisionInfo(rigidBodyA, rigidBodyB, normal, depth, contact1, contact2, contact3, contact4, contactCount);
                    ResolveCollision(in collisionInfo);
                }
            }
        }

        private void ResolveCollision(in CollisionInfo collisionInfo)
        {
            RigidBody bodyA = collisionInfo.bodyA;
            RigidBody bodyB = collisionInfo.bodyB;
            Vector3 normal = collisionInfo.normal;

            Vector3 relativeVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;

            if (Vector3.Dot(relativeVelocity, normal) > 0)
            {
                return;
            }

            float e = MathF.Min(bodyA.restitution, bodyB.restitution);

            float dampingFactor = 0.8f;
            e *= dampingFactor;

            float j = -(1 + e) * Vector3.Dot(relativeVelocity, normal);
            j /= bodyA.invMass + bodyB.invMass;

            Vector3 impulse = j * normal;

            if (!bodyA.isStatic)
            {
                bodyA.LinearVelocity -= impulse * bodyA.invMass;
            }
            if (!bodyB.isStatic)
            {
                bodyB.LinearVelocity += impulse * bodyB.invMass;
            }
        }

        private void ResolveCollisionRotation(in CollisionInfo collisionInfo)
        {
            RigidBody rigidBodyA = collisionInfo.bodyA;
            RigidBody rigidBodyB = collisionInfo.bodyB;
            Vector3 normal = collisionInfo.normal;
            Vector3 contact1 = collisionInfo.contact1;
            Vector3 contact2 = collisionInfo.contact2;
            Vector3 contact3 = collisionInfo.contact3;
            Vector3 contact4 = collisionInfo.contact4;
            int contactCount = collisionInfo.contactCount;

            float e = MathF.Min(rigidBodyA.restitution, rigidBodyB.restitution);

            Vector3[] contactList = new Vector3[] { contact1, contact2, contact3, contact4 };
            Vector3[] impulseList = new Vector3[4];
            Vector3[] rAPList = new Vector3[4];
            Vector3[] rBPList = new Vector3[4];

            for (int i = 0; i < contactCount; i++)
            {
                Vector3 rAP = contactList[i] - rigidBodyA.position;
                Vector3 rBP = contactList[i] - rigidBodyB.position;

                rAPList[i] = rAP;
                rBPList[i] = rBP;

                Vector3 angularVelocityA = rAP * rigidBodyA.angularVelocity;
                Vector3 angularVelocityB = rBP * rigidBodyB.angularVelocity;

                Vector3 relativeVelocity = (rigidBodyB.LinearVelocity + angularVelocityB) - (rigidBodyA.LinearVelocity + angularVelocityA);

                float vAB = Vector3.Dot(relativeVelocity, normal);

                if (vAB > 0)
                {
                    continue;
                }

                float j = -(1 + e) * vAB;
                float firstPart = Vector3.Dot(normal, normal) * (1 / rigidBodyA.mass + 1 / rigidBodyB.mass);
                Vector3 IrAPxNxrAP = Vector3.Cross(rigidBodyA.invInertia * Vector3.Cross(rAP, normal), rAP);
                Vector3 IrBPxNxrBP = Vector3.Cross(rigidBodyB.invInertia * Vector3.Cross(rBP, normal), rBP);
                float secondPart = Vector3.Dot(IrAPxNxrAP + IrBPxNxrBP, normal);

                j /= firstPart + secondPart;
                j /= contactCount;

                Vector3 impulse = j * normal;
                impulseList[i] = impulse;
            }

            for (int i = 0; i < contactCount; i++)
            {
                Vector3 impulse = impulseList[i];

                Vector3 rA = rAPList[i];
                Vector3 rB = rBPList[i];

                rigidBodyA.LinearVelocity += -impulse * rigidBodyA.invMass;
                rigidBodyA.angularVelocity += -Vector3.Cross(rA, impulse) * rigidBodyA.invInertia;
                rigidBodyB.LinearVelocity += impulse * rigidBodyB.invMass;
                rigidBodyB.angularVelocity += Vector3.Cross(rB, impulse) * rigidBodyB.invInertia;
            }
        }

    }
    public readonly struct CollisionInfo
    {
        public readonly RigidBody bodyA;
        public readonly RigidBody bodyB;
        public readonly Vector3 normal;
        public readonly float depth;
        public readonly Vector3 contact1;
        public readonly Vector3 contact2;
        public readonly Vector3 contact3;
        public readonly Vector3 contact4;
        public readonly int contactCount;

        public CollisionInfo(RigidBody bodyA, RigidBody bodyB, Vector3 normal, float depth,
            Vector3 contact1, Vector3 contact2, Vector3 contact3, Vector3 contact4, int contactCount)
        {
            this.bodyA = bodyA;
            this.bodyB = bodyB;
            this.normal = normal;
            this.depth = depth;
            this.contact1 = contact1;
            this.contact2 = contact2;
            this.contact3 = contact3;
            this.contact4 = contact4;
            this.contactCount = contactCount;
        }

    }
}
