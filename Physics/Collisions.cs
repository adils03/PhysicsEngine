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
            ShapeType shapeTypeA = bodyA.shapeType;
            ShapeType shapeTypeB = bodyB.shapeType;
            contact1 = Vector3.Zero;
            contact2 = Vector3.Zero;
            contact3 = Vector3.Zero;
            contact4 = Vector3.Zero;
            contactCount = 0;

            if (shapeTypeA is ShapeType.Cube)
            {
                if (shapeTypeB is ShapeType.Cube)
                {
                    //FindContactPointsCubes(bodyA.shape.GetVertices(), bodyB.shape.GetVertices(), out contact1, out contact2, out contact3, out contact4, out contactCount);
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    FindContactPointSphereCube(bodyB.position, bodyA.shape.GetVertices(), out contact1,out contact2,out contact3);
                    contactCount = 1;
                }
            }
            else if (shapeTypeA is ShapeType.Sphere)
            {
                if (shapeTypeB is ShapeType.Cube)
                {
                    FindContactPointSphereCube(bodyA.position, bodyB.shape.GetVertices(), out contact1, out contact2, out contact3);
                    contactCount = 1;
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    Collisions.FindContactPointSpheres(bodyA.position, bodyA.radius, bodyB.position, out contact1);
                    contactCount = 1;
                }
            }



        }
        private static void FindContactPointSphereCube(Vector3 sphereCenter, Vector3[] vertices, out Vector3 contact1,out Vector3 cp1,out Vector3 cp2)
        {
            contact1 = Vector3.Zero;
             cp1 = Vector3.Zero;
             cp2 = Vector3.Zero;
            Edge edge1 = new Edge();
            Edge edge2 = new Edge();
            float minDistSq1 = float.MaxValue;
            float minDistSq2 = float.MaxValue;

            Edge[] edges = GetEdges(vertices);

            for (int i = 0; i < edges.Length; i++)
            {

                Collisions.PointSegmentDistance(sphereCenter, edges[i], out float distanceSquared, out Vector3 contact);

                if (distanceSquared < minDistSq1)
                {
                    minDistSq2 = minDistSq1; // Eski en küçük mesafeyi ikinci en küçük mesafe yap
                    cp2 = cp1; // Eski en küçük temas noktasını ikinci temas noktası yap
                    edge2 = edge1;
                    minDistSq1 = distanceSquared; // Yeni en küçük mesafeyi ayarla
                    cp1 = contact; // Yeni temas noktasını ayarla
                    edge1 = edges[i];
                }
                else if (distanceSquared < minDistSq2)
                {
                    minDistSq2 = distanceSquared; // Yeni ikinci en küçük mesafeyi ayarla
                    cp2 = contact; // Yeni ikinci temas noktasını ayarla
                    edge2 = edges[i];
                }
            }

            Vector3 sameCorner;

            if (edge1.a == edge2.a || edge1.a == edge2.b)
            {
                sameCorner = edge1.a;
            }
            else
            {
                sameCorner = edge1.b;
            }
            contact1 = (cp1 + cp2) / 2;

            Vector3 delta = contact1 - sameCorner;
            contact1 += delta;
                
        }
        private static Edge[] GetEdges(Vector3[] vertices)
        {
            Edge[] edges = new Edge[12];
            edges[0] = new Edge(vertices[1], vertices[0]);
            edges[1] = new Edge(vertices[2], vertices[1]);
            edges[2] = new Edge(vertices[3], vertices[2]);
            edges[3] = new Edge(vertices[0], vertices[3]);

            edges[4] = new Edge(vertices[5], vertices[4]);
            edges[5] = new Edge(vertices[6], vertices[5]);
            edges[6] = new Edge(vertices[7], vertices[6]);
            edges[7] = new Edge(vertices[4], vertices[7]);

            edges[8] = new Edge(vertices[0], vertices[4]);
            edges[9] = new Edge(vertices[3], vertices[7]);
            edges[10] = new Edge(vertices[1], vertices[5]);
            edges[11] = new Edge(vertices[2], vertices[6]);

            return edges;
        }
        private static void FindContactPointSpheres(Vector3 centerA, float radiusA, Vector3 centerB, out Vector3 contactPoint)
        {
            Vector3 ab = (centerB - centerA).Normalized();
            contactPoint = centerA + radiusA * ab;
        }

        private static void PointSegmentDistance(Vector3 point, Edge edge, out float distanceSquared, out Vector3 closestPoint)
        {
            Vector3 a = edge.a;
            Vector3 b = edge.b;

            Vector3 ab = b - a;
            Vector3 ap = point - a;

            float proj = Vector3.Dot(ap, ab);
            float abLenSq = ab.LengthSquared;
            float d = proj / abLenSq;

            if (d <= 0)
            {
                closestPoint = a;
            }
            else if (d >= 1f)
            {
                closestPoint = b;
            }
            else
            {
                closestPoint = a + ab * d;
            }

            distanceSquared = Vector3.DistanceSquared(point, closestPoint);

        }

        public static Vector3 FindClosestFacePoint(Vector3 point, Vector3[] vertices)
        {
            Vector3 closest = Vector3.Zero;
            List<Vector3> normals = GetFaceNormals(vertices);
            float maxDist = float.MinValue;
            for (int i = 0; i < normals.Count; i++)
            {
                float value = Vector3.Dot(point, normals[i]);
                if(value > maxDist)
                {
                    closest = normals[i];
                }
            }

            return closest;

        }

        public static List<Vector3> GetFaceNormals(Vector3[] vertices)
        {
            List<Vector3> normals = [];
            Vector3 abA = vertices[1] - vertices[0];
            Vector3 adA = vertices[3] - vertices[0];
            Vector3 aeA = vertices[4] - vertices[0];

            Vector3 faceNormal1 = Vector3.Cross(abA, adA); 
            Vector3 faceNormal2 = Vector3.Cross(aeA, abA); 
            Vector3 faceNormal3 = Vector3.Cross(adA, aeA); 

            normals.Add(faceNormal1);
            normals.Add(-faceNormal1);
            normals.Add(faceNormal2);
            normals.Add(-faceNormal2);
            normals.Add(faceNormal3);
            normals.Add(-faceNormal3);
            return normals;
        }

        public static bool IntersectCubeSphere(bool reverse, Cube cube, Sphere sphere, float tolerance,
            out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = float.MaxValue;
            Vector3[] vertices = cube.GetVertices();
            List<Vector3> axises = [];
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
        private static bool NearlyEqual(Vector3 a, Vector3 b)
        {
            return NearlyEqual(a.X, b.X) && NearlyEqual(a.Y, b.Y) && NearlyEqual(a.Z, b.Z);
        }
        private static bool NearlyEqual(float a, float b)
        {
            return MathF.Abs(a - b) < 0.0005f;
        }
    }

    public readonly struct Edge
    {
        public readonly Vector3 a;
        public readonly Vector3 b;

        public Edge(Vector3 a, Vector3 b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
