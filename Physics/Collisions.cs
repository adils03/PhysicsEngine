using OpenTK.Mathematics;
using System.Diagnostics.Contracts;

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
                    FindContactPointsCubes(bodyA.shape.GetVertices(), bodyB.shape.GetVertices(), out contact1, out contact2, out contact3, out contact4, out contactCount);
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    FindContactPointSphereCube(bodyB.position, bodyA.shape.GetVertices(), out contact1);
                    contactCount = 1;
                }
            }
            else if (shapeTypeA is ShapeType.Sphere)
            {
                if (shapeTypeB is ShapeType.Cube)
                {
                    FindContactPointSphereCube(bodyA.position, bodyB.shape.GetVertices(), out contact1);
                    contactCount = 1;
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    Collisions.FindContactPointSpheres(bodyA.position, bodyA.radius, bodyB.position, out contact1);
                    contactCount = 1;
                }
            }



        }

        private static void FindContactPointsCubes(Vector3[] verticesA, Vector3[] verticesB,
            out Vector3 contact1, out Vector3 contact2, out Vector3 contact3, out Vector3 contact4, out int contactCount)
        {
            contact1 = Vector3.Zero;
            contact2 = Vector3.Zero;
            contact3 = Vector3.Zero;
            contact4 = Vector3.Zero;
            contactCount = 0;

            Face[] facesA = GetCubeFaces(verticesA);
            Face[] facesB = GetCubeFaces(verticesB);

            float minDistance = float.MaxValue;

            for (int i = 0; i < verticesB.Length; i++)
            {
                Face closestFaceA = FindClosestFace(facesA, verticesB[i]);
                Vector3 closestPoint = FindClosestPointOnFace(closestFaceA, verticesB[i], out float distance);

                if (NearlyEqual(distance, minDistance))
                {
                    if (!NearlyEqual(closestPoint, contact1) &&
                        !NearlyEqual(closestPoint, contact2) &&
                        !NearlyEqual(closestPoint, contact3) &&
                        !NearlyEqual(closestPoint, contact4))
                    {
                        if (contactCount == 1)
                            contact2 = closestPoint;
                        else if (contactCount == 2)
                            contact3 = closestPoint;
                        else if (contactCount == 3)
                            contact4 = closestPoint;

                        if (contactCount < 4)
                            contactCount++;
                    }
                }
                else if (distance < minDistance)
                {
                    minDistance = distance;
                    contact1 = closestPoint;
                    contact2 = default;
                    contact3 = default;
                    contact4 = default;
                    contactCount = 1;
                }

            }
            for (int i = 0; i < verticesA.Length; i++)
            {
                Face closestFaceB = FindClosestFace(facesB, verticesA[i]);
                Vector3 closestPoint = FindClosestPointOnFace(closestFaceB, verticesA[i], out float distance);

                if (NearlyEqual(distance, minDistance))
                {
                    if (!NearlyEqual(closestPoint, contact1) &&
                        !NearlyEqual(closestPoint, contact2) &&
                        !NearlyEqual(closestPoint, contact3) &&
                        !NearlyEqual(closestPoint, contact4))
                    {
                        if (contactCount == 1)
                            contact2 = closestPoint;
                        else if (contactCount == 2)
                            contact3 = closestPoint;
                        else if (contactCount == 3)
                            contact4 = closestPoint;

                        if (contactCount < 4)
                            contactCount++;
                    }
                }
                else if (distance < minDistance)
                {
                    minDistance = distance;
                    contact1 = closestPoint;
                    contact2 = default;
                    contact3 = default;
                    contact4 = default;
                    contactCount = 1;
                }

            }


        }
        private static void FindContactPointSphereCube(Vector3 sphereCenter, Vector3[] vertices, out Vector3 contact1)
        {
            Face[] faces = GetCubeFaces(vertices);
            Face closestFace = FindClosestFace(faces, sphereCenter);

            contact1 = FindClosestPointOnFace(closestFace, sphereCenter,out float distance);
        }

        private static Vector3 FindClosestPointOnFace(Face face, Vector3 point,out float distance)
        {
            Edge edge1 = face.edge1;
            Edge edge2 = face.edge2;

            Collisions.PointSegmentDistance(point, edge1, out float distanceSquared, out Vector3 contact1);
            Collisions.PointSegmentDistance(point, edge2, out float distanceSquared2, out Vector3 contact2);

            Vector3 sameCorner = face.b;

            Vector3 contact = (contact1 + contact2) / 2;

            Vector3 delta = contact - sameCorner;
            contact += delta;
            distance = Vector3.DistanceSquared(point,contact);
            return contact;
        }


        private static Face FindClosestFace(Face[] faces, Vector3 point)
        {
            Face closestFace = new Face();
            float closestFaceValue = float.MinValue;

            for (int i = 0; i < faces.Length; i++)
            {
                float value = Vector3.Dot(faces[i].FaceNormal, point - faces[i].faceCenter);
                if (value > closestFaceValue)
                {
                    closestFaceValue = value;
                    closestFace = faces[i];
                }
            }
            return closestFace;
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
                if (value > maxDist)
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
            axises.Add(faceNormalA1);
            axises.Add(faceNormalA2);
            axises.Add(faceNormalA3);
            axises.Add(faceNormalB1);
            axises.Add(faceNormalB2);
            axises.Add(faceNormalB3);
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

        private static void AddAxis(List<Vector3> axises, Vector3 faceNormalA, Vector3 faceNormalB)
        {
            Vector3 faceNormalCross = Vector3.Cross(faceNormalA, faceNormalB).Normalized();
            if (faceNormalCross.Length < 1.01f)
            {
                axises.Add(faceNormalCross);
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

        public static Face[] GetCubeFaces(Vector3[] vertices)
        {
            Face[] faces = new Face[6];
            faces[0] = new Face(vertices[0], vertices[1], vertices[2], vertices[3]);
            faces[1] = new Face(vertices[3], vertices[2], vertices[6], vertices[7]);
            faces[2] = new Face(vertices[7], vertices[6], vertices[5], vertices[4]);
            faces[3] = new Face(vertices[4], vertices[5], vertices[1], vertices[0]);
            faces[4] = new Face(vertices[1], vertices[5], vertices[6], vertices[2]);
            faces[5] = new Face(vertices[4], vertices[0], vertices[3], vertices[7]);

            return faces;
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

    public readonly struct Face
    {
        public readonly Vector3 a;
        public readonly Vector3 b;
        public readonly Vector3 c;
        public readonly Vector3 d;

        public readonly Vector3 faceCenter;

        public readonly Vector3 FaceNormal;

        public readonly Edge edge1;
        public readonly Edge edge2;
        public readonly Edge edge3;
        public readonly Edge edge4;

        public Face(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            faceCenter = (a + b + c + d) / 4;
            FaceNormal = Vector3.Cross(c - a, b - a).Normalized();
            edge1 = new Edge(a, b);
            edge2 = new Edge(b, c);
            edge3 = new Edge(c, d);
            edge4 = new Edge(a, d);
        }
    }
}
