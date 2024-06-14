

using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public enum ResolveType
    {
        Basic,
        Rotation,
        RotationAndFriction
    }
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

        private readonly ResolveType ResolveType;

        public PhysicsWorld(float gravityAmount, Vector3 worldSize, ResolveType resolveType = ResolveType.Basic)
        {
            gravity = new Vector3(0, -gravityAmount, 0);
            shapes = new List<Shape>();
            rigidBodies = new List<RigidBody>();
            collisionInfos = new List<CollisionInfo>();
            contactPairs = new List<(int, int)>();
            octree = new Octree(worldSize, 1);
            this.ResolveType = resolveType;
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
                    switch (ResolveType)
                    {
                        case ResolveType.Basic:
                            ResolveCollision(in collisionInfo);
                            break;
                        case ResolveType.Rotation:
                            ResolveCollisionRotation(in collisionInfo);
                            break;
                        case ResolveType.RotationAndFriction:
                            ResolveCollisionRotationFriction(in collisionInfo);
                            break;
                    }

                }
            }
        }

        private void ResolveCollision(in CollisionInfo collisionInfo)
        {
            RigidBody bodyA = collisionInfo.bodyA;
            RigidBody bodyB = collisionInfo.bodyB;
            Vector3 normal = collisionInfo.normal;

            Vector3 relativeVelocity = bodyB.linearVelocity - bodyA.linearVelocity;

            if (Vector3.Dot(relativeVelocity, normal) > 0)
            {
                return;
            }

            float e = MathF.Min(bodyA.restitution, bodyB.restitution);
            float j = -(1 + e) * Vector3.Dot(relativeVelocity, normal);
            j /= bodyA.invMass + bodyB.invMass;

            Vector3 impulse = j * normal;

            if (!bodyA.isStatic)
            {
                bodyA.linearVelocity -= impulse * bodyA.invMass;
            }
            if (!bodyB.isStatic)
            {
                bodyB.linearVelocity += impulse * bodyB.invMass;
            }
        }

        private void ResolveCollisionRotation(in CollisionInfo collisionInfo)
        {
            RigidBody bodyA = collisionInfo.bodyA;
            RigidBody bodyB = collisionInfo.bodyB;
            Vector3 normal = collisionInfo.normal;

            if (collisionInfo.contactCount == 0)
            {
                return;
            }

            // Average the contact points
            Vector3 contactPoint = (collisionInfo.contact1 + collisionInfo.contact2 + collisionInfo.contact3 + collisionInfo.contact4) / collisionInfo.contactCount;
            // Calculate the relative positions of the contact point
            Vector3 ra = contactPoint - bodyA.position;
            Vector3 rb = contactPoint - bodyB.position;

            // Calculate the relative velocity at the contact point
            Vector3 relativeVelocity = (bodyB.linearVelocity + bodyB.angularVelocity ) -
                                       (bodyA.linearVelocity + bodyA.angularVelocity );

            // Early exit if the velocities are separating
            if (Vector3.Dot(relativeVelocity, normal) > 0)
            {
                return;
            }

            // Calculate the angular impulse
            float e = MathF.Min(bodyA.restitution, bodyB.restitution);

            // Calculate the cross products
            Vector3 raCrossN = Vector3.Cross(ra, normal);
            Vector3 rbCrossN = Vector3.Cross(rb, normal);

            Matrix3 invInertiaA = bodyA.invInertiaTensor;
            Matrix3 invInertiaB = bodyB.invInertiaTensor;


            float denominator = Vector3.Dot(normal, normal) * (bodyA.invMass + bodyB.invMass) +
                               Vector3.Dot(Vector3.Cross((invInertiaA * raCrossN), ra) + Vector3.Cross((invInertiaB * rbCrossN), rb), normal);

            // Calculate the impulse scalar
            float j = -(1 + e) * Vector3.Dot(relativeVelocity, normal) / denominator;

            // Compute the linear and angular impulses
            Vector3 impulse = j * normal;
            Console.WriteLine(impulse);
            // Apply the impulses
            if (!bodyA.isStatic)
            {
                bodyA.linearVelocity -= impulse * bodyA.invMass;
                bodyA.angularVelocity -= invInertiaA *Vector3.Cross(ra , impulse) ;
            }

            if (!bodyB.isStatic)
            {
                bodyB.linearVelocity += impulse * bodyB.invMass;
                bodyB.angularVelocity += invInertiaB *Vector3.Cross(rb, impulse) ;
            }
        }

        private void ResolveCollisionRotationFriction(in CollisionInfo collisionInfo)
        {
            RigidBody bodyA = collisionInfo.bodyA;
            RigidBody bodyB = collisionInfo.bodyB;
            Vector3 normal = collisionInfo.normal;

            if (collisionInfo.contactCount == 0)
            {
                return;
            }

            // Average the contact points
            Vector3 contactPoint = (collisionInfo.contact1 + collisionInfo.contact2 + collisionInfo.contact3 + collisionInfo.contact4) / collisionInfo.contactCount;
            // Calculate the relative positions of the contact point
            Vector3 ra = contactPoint - bodyA.position;
            Vector3 rb = contactPoint - bodyB.position;

            // Calculate the relative velocity at the contact point
            Vector3 relativeVelocity = bodyB.linearVelocity + bodyB.angularVelocity -
                                       bodyA.linearVelocity + bodyA.angularVelocity;

            // Early exit if the velocities are separating
            if (Vector3.Dot(relativeVelocity, normal) > 0)
            {
                return;
            }

            // Calculate the angular impulse
            float e = MathF.Min(bodyA.restitution, bodyB.restitution);

            // Calculate the cross products
            Vector3 raCrossN = Vector3.Cross(ra, normal);
            Vector3 rbCrossN = Vector3.Cross(rb, normal);

            // Calculate the inverse inertia scalars
            Matrix3 invInertiaA = bodyA.invInertiaTensor;
            Matrix3 invInertiaB = bodyB.invInertiaTensor;

            float denominator = Vector3.Dot(normal, normal) * (bodyA.invMass + bodyB.invMass) +
                                Vector3.Dot(Vector3.Cross((invInertiaA * raCrossN), ra) + Vector3.Cross((invInertiaB * rbCrossN), rb), normal);

            // Calculate the impulse scalar
            float j = -(1 + e) * Vector3.Dot(relativeVelocity, normal) / denominator;

            // Compute the linear and angular impulses
            Vector3 impulse = j * normal;

            // Apply the impulses
            if (!bodyA.isStatic)
            {
                bodyA.linearVelocity -= impulse * bodyA.invMass;
                bodyA.angularVelocity -= invInertiaA * Vector3.Cross(ra, impulse);
            }

            if (!bodyB.isStatic)
            {
                bodyB.linearVelocity += impulse * bodyB.invMass;
                bodyB.angularVelocity += invInertiaB * Vector3.Cross(rb, impulse);
            }

            // Calculate friction impulse
            Vector3 tangent = relativeVelocity - (normal * Vector3.Dot(relativeVelocity, normal));
            if (tangent.LengthSquared > 0.0001f)
            {
                tangent = Vector3.Normalize(tangent);
            }

            // Calculate friction impulse scalar
            float jt = -Vector3.Dot(relativeVelocity, tangent) / denominator;

            // Statik ve dinamik sürtünme katsayılarını belirle
            float staticFriction = MathF.Sqrt(bodyA.staticFriction * bodyB.staticFriction);
            float dynamicFriction = MathF.Sqrt(bodyA.dynamicFriction * bodyB.dynamicFriction);

            Vector3 frictionImpulse;
            if (MathF.Abs(jt) < j * staticFriction)
            {
                // Statik sürtünme durumu
                frictionImpulse = jt * tangent;
            }
            else
            {
                // Dinamik sürtünme durumu
                frictionImpulse = -j * dynamicFriction * tangent;
            }

            // Apply friction impulses
            if (!bodyA.isStatic)
            {
                bodyA.linearVelocity -= frictionImpulse * bodyA.invMass;
                bodyA.angularVelocity -= invInertiaA * Vector3.Cross(ra, frictionImpulse);
            }

            if (!bodyB.isStatic)
            {
                bodyB.linearVelocity += frictionImpulse * bodyB.invMass;
                bodyB.angularVelocity += invInertiaB * Vector3.Cross(rb, frictionImpulse);
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

