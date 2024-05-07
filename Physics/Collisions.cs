using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public static class Collisions
    {
        public static bool IntersectCubes(Cube shapeA, Cube shapeB, float tolerance, out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = float.MaxValue;
            Vector3[] verticesA = shapeA.GetVertices();
            Vector3[] verticesB = shapeB.GetVertices();
            List<Vector3> axises = [];
            Vector3 abA = verticesA[1] - verticesA[0];
            Vector3 adA = verticesA[3] - verticesA[0];
            Vector3 aeA = verticesA[4] - verticesA[0];
            Vector3 faceNormalA1 = Vector3.Cross(abA, adA); axises.Add(faceNormalA1);
            Vector3 faceNormalA2 = Vector3.Cross(aeA, abA); axises.Add(faceNormalA2);
            Vector3 faceNormalA3 = Vector3.Cross(adA, aeA); axises.Add(faceNormalA3);
            Vector3 abB = verticesB[1] - verticesB[0];
            Vector3 adB = verticesB[3] - verticesB[0];
            Vector3 aeB = verticesB[4] - verticesB[0];
            Vector3 faceNormalB1 = Vector3.Cross(abB, adB); axises.Add(faceNormalB1);
            Vector3 faceNormalB2 = Vector3.Cross(aeB, abB); axises.Add(faceNormalB2);
            Vector3 faceNormalB3 = Vector3.Cross(adB, aeB); axises.Add(faceNormalB3);
            AddAxis(axises, faceNormalA1, faceNormalB1);

            Vector3 faceNormalCrossA1B2 = Vector3.Cross(faceNormalA1, faceNormalB2).Normalized();
            if (faceNormalCrossA1B2.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA1B2);
            }

            Vector3 faceNormalCrossA1B3 = Vector3.Cross(faceNormalA1, faceNormalB3).Normalized();
            if (faceNormalCrossA1B3.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA1B3);
            }

            Vector3 faceNormalCrossA2B1 = Vector3.Cross(faceNormalA2, faceNormalB1).Normalized();
            if (faceNormalCrossA2B1.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA2B1);
            }

            Vector3 faceNormalCrossA2B2 = Vector3.Cross(faceNormalA2, faceNormalB2).Normalized();
            if (faceNormalCrossA2B2.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA2B2);
            }

            Vector3 faceNormalCrossA2B3 = Vector3.Cross(faceNormalA2, faceNormalB3).Normalized();
            if (faceNormalCrossA2B3.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA2B3);
            }

            Vector3 faceNormalCrossA3B1 = Vector3.Cross(faceNormalA3, faceNormalB1).Normalized();
            if (faceNormalCrossA3B1.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA3B1);
            }

            Vector3 faceNormalCrossA3B2 = Vector3.Cross(faceNormalA3, faceNormalB2).Normalized();
            if (faceNormalCrossA3B2.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA3B2);
            }

            Vector3 faceNormalCrossA3B3 = Vector3.Cross(faceNormalA3, faceNormalB3).Normalized();
            if (faceNormalCrossA3B3.Length < 1.01f)
            {
                axises.Add(faceNormalCrossA3B3);
            }

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
        public static bool IntersectSpheres(SphereCollisionInfo sphereCollisionInfoA, SphereCollisionInfo sphereCollisionInfoB,
            out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = 0;

            Vector3 centerA = sphereCollisionInfoA.center;
            Vector3 centerB = sphereCollisionInfoB.center;

            float radiusA = sphereCollisionInfoA.radius;
            float radiusB = sphereCollisionInfoB.radius;

            float distance = Vector3.Distance(centerA, centerB);
            float radii = radiusA + radiusB;

            if (distance >= radii)
            {
                return false;
            }

            normal = Vector3.Normalize(centerB - centerA);
            depth = radii - distance;

            return true;
        }
    }

    public struct SphereCollisionInfo
    {
        public Vector3 center;
        public float radius;

        public SphereCollisionInfo(Sphere sphere)
        {
            center = sphere.Transform.Position;
            radius = sphere.radius;
        }
    }
}
