using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public static class Collisions
    {
        public static bool GJK(Shape shapeA, Shape shapeB, out List<Vector3> simplex)
        {
            Vector3 supportPoint = Support(shapeA, shapeB, new Vector3(1, 0, 0));
            simplex = new(4)
            {
                supportPoint
            };
            Vector3 direction = -supportPoint;

            while (true)
            {
                supportPoint = Support(shapeA, shapeB, direction);
                if (Vector3.Dot(supportPoint, direction) <= 0)
                {
                    return false;
                }
                simplex.Add(supportPoint);//second point
                if (SimplexCase(ref simplex, ref direction))
                {
                    return true;
                }
            }

        }


        public static bool SimplexCase(ref List<Vector3> simplex, ref Vector3 direction)
        {
            switch (simplex.Count)
            {
                case 2:
                    return LineCase(ref simplex, ref direction);
                case 3:
                    return TriangleCase(ref simplex, ref direction);
                case 4:
                    return TetrahedronCase(ref simplex, ref direction);
                default:
                    return false;
            }
        }

        private static bool LineCase(ref List<Vector3> simplex, ref Vector3 direction)
        {
            //Vector3 ab = supporPoint2 - supporPoint;
            //Vector3 a0 = -supporPoint;
            Vector3 ab = simplex[1] - simplex[0];
            Vector3 a0 = -simplex[0];
            direction = Vector3.Cross(Vector3.Cross(ab, a0), ab);
            return false;
        }

        private static bool TriangleCase(ref List<Vector3> simplex, ref Vector3 direction)
        {
            //Vector3 ac = supportPoint3 - supporPoint;
            //Vector3 normalOfTriangle = Vector3.Cross(ab, ac).Normalized();
            Vector3 ab = simplex[1] - simplex[0];
            Vector3 ac = simplex[2] - simplex[0];
            Vector3 abc = Vector3.Cross(ab, ac);//normal of triangle
            direction = abc;
            return false;
        }

        private static bool TetrahedronCase(ref List<Vector3> simplex, ref Vector3 direction)
        {
            Vector3 ab = simplex[1] - simplex[0];
            Vector3 ac = simplex[2] - simplex[0];
            Vector3 abc = Vector3.Cross(ab, ac);//normal of triangle
            direction = abc;
            return true;
        }
        public static bool SameDirection(Vector3 direction, Vector3 ao)
        {
            return Vector3.Dot(direction, ao) > 0;
        }
        public static Vector3 Support(Shape shapeA, Shape shapeB, Vector3 direction, bool debug = false)
        {
            //Difference between largest dot products
            Vector3[] verticesA = shapeA.GetVertices();
            Vector3[] verticesB = shapeB.GetVertices();
            Vector3 maxDotProductA = FindFurthestPoint(verticesA, direction);
            Vector3 maxDotProductB = FindFurthestPoint(verticesB, -direction);
            if (debug)
            {
                Console.WriteLine(maxDotProductA + "MaxA");
                Console.WriteLine(maxDotProductB + "MaxB");
            }
            return maxDotProductA - maxDotProductB;
        }
        public static Vector3 FindFurthestPoint(Vector3[] vertices, Vector3 direction)
        {
            Vector3 result = Vector3.Zero;
            float value = float.MinValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                float dotProduct = Vector3.Dot(vertices[i], direction);
                if (dotProduct > value)
                {
                    value = dotProduct;
                    result = vertices[i];
                }
            }
            return result;
        }
        public static Vector3[] MinkowskiDifference(Shape shapeA, Shape shapeB)
        {
            Vector3[] verticesA = shapeA.GetVertices();
            Vector3[] verticesB = shapeB.GetVertices();

            Vector3[] returnVertices = new Vector3[verticesA.Length * verticesB.Length];
            int vertexCount = 0;

            for (int i = 0; i < verticesA.Length; i++)
            {
                for (int j = 0; j < verticesB.Length; j++)
                {
                    returnVertices[vertexCount++] = verticesA[i] - verticesB[j];
                }
            }

            return returnVertices;
        }

        public static bool IntersectPolygons(Vector3[] verticesA, Vector3[] verticesB)
        {
            for (int i = 0; i < verticesA.Length; i++)
            {
                Vector3 va = verticesA[i];
                Vector3 vb = verticesA[(i + 1) % verticesA.Length];

                Vector3 edge = vb - va;
                Vector3 axis = new Vector3(-edge.Y, edge.X, edge.Z);
                axis.Normalize();
                ProjectVertices(verticesA, axis, out float minA, out float maxA);
                ProjectVertices(verticesB, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }
            }
            for (int i = 0; i < verticesB.Length; i++)
            {
                Vector3 va = verticesB[i];
                Vector3 vb = verticesB[(i + 1) % verticesB.Length];

                Vector3 edge = vb - va;
                Vector3 axis = new Vector3(-edge.Y, edge.X, edge.Z);
                axis.Normalize();
                ProjectVertices(verticesA, axis, out float minA, out float maxA);
                ProjectVertices(verticesB, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }
            }

            return true;
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
            normal = Vector3.Normalize(centerA - centerB);
            depth = radii - distance;

            return true;
        }
    }


}


