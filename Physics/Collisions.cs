using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public static class Collisions
    {
        public static bool Collide(RigidBody bodyA, RigidBody bodyB, out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = 0;
            ShapeType shapeTypeA = bodyA.shapeType;
            ShapeType shapeTypeB = bodyB.shapeType;

            bool result;
            if (shapeTypeA is ShapeType.Cube)
            {
                if (shapeTypeB is ShapeType.Cube)
                {
                    result = Collisions.IntersectCubes((Cube)bodyA.shape, (Cube)bodyB.shape, 0.01f, out normal, out depth);
                    return result;
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    result = Collisions.IntersectCubeSphere(true, (Cube)bodyA.shape, (Sphere)bodyB.shape, 0, out normal, out depth);
                    return result;
                }
            }
            else if (shapeTypeA is ShapeType.Sphere)
            {
                if (shapeTypeB is ShapeType.Cube)
                {
                    result = Collisions.IntersectCubeSphere(false, (Cube)bodyB.shape, (Sphere)bodyA.shape, 0, out normal, out depth);
                    return result;
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    Sphere sphereA = (Sphere)bodyA.shape;
                    Sphere sphereB = (Sphere)bodyB.shape;
                    result = Collisions.IntersectSpheres(sphereA.Transform.Position, sphereA.radius, sphereB.Transform.Position, sphereB.radius, out normal, out depth);
                    return result;
                }
            }
            return false;
        }
        public static bool AABBCheck(AABB a, AABB b)
        {
            return (
                a.min.X <= b.max.X ||
                a.max.X >= b.min.X ||
                a.min.Y <= b.max.Y ||
                a.max.Y >= b.min.Y ||
                a.min.Z <= b.max.Z ||
                a.max.Z >= b.min.Z
                    );
        }
        public static void FindContactPoints(RigidBody bodyA, RigidBody bodyB,
           out Vector3 contact1, out Vector3 contact2, out Vector3 contact3, out Vector3 contact4, out int contactCount)
        { 
            contact1 = default(Vector3);
            contact2 = default(Vector3);
            contact3 = default(Vector3);
            contact4 = default(Vector3);
            contactCount = 0;
        }
            public static bool IntersectCubeSphere(bool reverse, Cube cube, Sphere sphere, float tolerance,
            out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = float.MaxValue;
            Vector3[] vertices = cube.GetVertices();
            List<Vector3> axises = [];
            Vector3[] normals = cube.GetNormals();

            Vector3 abA = vertices[1] - vertices[0];
            Vector3 adA = vertices[3] - vertices[0];
            Vector3 aeA = vertices[4] - vertices[0];

            Vector3 faceNormal1 = Vector3.Cross(abA, adA); axises.Add(faceNormal1);
            Vector3 faceNormal2 = Vector3.Cross(aeA, abA); axises.Add(faceNormal2);
            Vector3 faceNormal3 = Vector3.Cross(adA, aeA); axises.Add(faceNormal3);
            int cpIndex = FindClosestPointOnPolygon(sphere.Transform.Position, cube.GetVertices());
            Vector3 cp = cube.GetVertices()[cpIndex];
            Vector3 axisFromSphere = cp - sphere.Transform.Position;
            axises.Add(axisFromSphere);
            foreach (Vector3 axis in axises)
            {
                ProjectVertices(vertices, axis.Normalized(), out float minA, out float maxA);
                ProjectCircle(sphere, axis.Normalized(), tolerance, out float minB, out float maxB);
                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);
                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }
            Vector3 direction;
            if (reverse)
            {
                direction = sphere.Transform.Position - cube.Transform.Position;
            }
            else
            {
                direction = cube.Transform.Position - sphere.Transform.Position;
            }
            //normal = direction;
            if (Vector3.Dot(direction, normal) < 0)
            {
                normal = -normal;
            }
            //depth /= normal.Length;
            normal.Normalize();
            return true;
        }

        private static void ProjectCircle(Sphere sphere, Vector3 axis, float radiusTolerance, out float min, out float max)
        {
            Vector3 direction = Vector3.Normalize(axis);
            Vector3 directionAndRadius = direction * (sphere.radius - radiusTolerance);
            Vector3 p1 = sphere.Transform.Position + directionAndRadius;
            Vector3 p2 = sphere.Transform.Position - directionAndRadius;
            min = Vector3.Dot(p1, axis);
            max = Vector3.Dot(p2, axis);
            if (min > max)
            {
                //Swap
                (max, min) = (min, max);
            }
        }
        private static int FindClosestPointOnPolygon(Vector3 point, Vector3[] vertices)
        {
            int result = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                float distance = Vector3.Distance(v, point);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = i;
                }

            }
            return result;

        }




        public static bool IntersectCubes(Cube shapeA, Cube shapeB, float tolerance, out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = float.MaxValue;

            Vector3[] verticesA = shapeA.GetVertices();
            Vector3[] verticesB = shapeB.GetVertices();

            List<Vector3> axises = [];
            Vector3[] faceNormalsA = shapeA.GetNormals();
            Vector3[] faceNormalsB = shapeB.GetNormals();

            Vector3 faceNormalA1 = faceNormalsA[0];
            Vector3 faceNormalA2 = faceNormalsA[2];
            Vector3 faceNormalA3 = faceNormalsA[4];

            Vector3 faceNormalB1 = faceNormalsB[0];
            Vector3 faceNormalB2 = faceNormalsB[2];
            Vector3 faceNormalB3 = faceNormalsB[4];

            AddAxis(axises, faceNormalA1, faceNormalB1);
            AddAxis(axises, faceNormalA1, faceNormalB2);
            AddAxis(axises, faceNormalA1, faceNormalB3);
            AddAxis(axises, faceNormalA2, faceNormalB1);
            AddAxis(axises, faceNormalA2, faceNormalB2);
            AddAxis(axises, faceNormalA2, faceNormalB3);
            AddAxis(axises, faceNormalA3, faceNormalB1);
            AddAxis(axises, faceNormalA3, faceNormalB2);
            AddAxis(axises, faceNormalA3, faceNormalB3);

            foreach (Vector3 axis in axises)
            {
                ProjectVertices(verticesA, axis.Normalized(), out float minA, out float maxA);
                ProjectVertices(verticesB, axis.Normalized(), out float minB, out float maxB);
                if (minA + tolerance >= maxB || minB + tolerance >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);
                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            depth /= normal.Length;
            normal = normal.Normalized();

            Vector3 direction = shapeB.Transform.Position - shapeA.Transform.Position;
            if (Vector3.Dot(direction, normal) < 0)
            {
                normal = -normal;
            }


            return true;
        }

        private static void AddAxis(List<Vector3> axises, Vector3 faceNormalA1, Vector3 faceNormalB1)
        {
            Vector3 faceNormalCrossA1B1 = Vector3.Cross(faceNormalA1, faceNormalB1).Normalized();
            if (faceNormalCrossA1B1.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA1B1);
            }
        }

        private static void ProjectVertices(Vector3[] vertices, Vector3 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                float proj = Vector3.Dot(v, axis);

                if (proj < min) min = proj;
                if (proj > max) max = proj;
            }
        }
        public static bool IntersectSpheres(Vector3 centerA, float radiusA, Vector3 centerB, float radiusB,
           out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = 0;
            float distance = Vector3.Distance(centerA, centerB);
            float radii = radiusB + radiusA;

            if (distance >= radii)
            {
                return false;
            }
            normal = Vector3.Normalize(centerB - centerA);
            depth = radii - distance;

            return true;
        }
    }


}
