using System;
using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class Octree
    {
        private readonly Vector3 origin;
        private readonly Vector3 halfSize;
        private readonly List<RigidBody> objects;
        private readonly Octree[] children;
        private readonly float minSize;

        public Octree(Vector3 origin, float size, float minSize = 1.0f)
        {
            this.origin = origin;
            this.halfSize = new Vector3(size / 2);
            this.minSize = minSize;
            objects = new List<RigidBody>();
            children = new Octree[8];
        }

        public void Clear()
        {
            objects.Clear();
            for (int i = 0; i < 8; i++)
            {
                if (children[i] != null)
                {
                    children[i].Clear();
                    children[i] = null;
                }
            }
        }

        public void Insert(RigidBody body)
        {
            if (children[0] != null)
            {
                int index = GetOctant(body);
                if (index != -1)
                {
                    children[index].Insert(body);
                    return;
                }
            }

            objects.Add(body);

            if (objects.Count > 8 && halfSize.Length > minSize)
            {
                if (children[0] == null)
                {
                    Subdivide();
                }

                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetOctant(objects[i]);
                    if (index != -1)
                    {
                        children[index].Insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public void Remove(RigidBody body)
        {
            if (children[0] != null)
            {
                int index = GetOctant(body);
                if (index != -1)
                {
                    children[index].Remove(body);
                    return;
                }
            }

            objects.Remove(body);
        }

        public List<RigidBody> Retrieve(RigidBody body)
        {
            List<RigidBody> result = new List<RigidBody>(objects);

            if (children[0] != null)
            {
                int index = GetOctant(body);
                if (index != -1)
                {
                    result.AddRange(children[index].Retrieve(body));
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        result.AddRange(children[i].Retrieve(body));
                    }
                }
            }

            return result;
        }

        private void Subdivide()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector3 newOrigin = origin + halfSize * new Vector3(
                    (i & 1) == 0 ? -0.5f : 0.5f,
                    (i & 2) == 0 ? -0.5f : 0.5f,
                    (i & 4) == 0 ? -0.5f : 0.5f
                );

                children[i] = new Octree(newOrigin, halfSize.Length, minSize);
            }
        }

        private int GetOctant(RigidBody body)
        {
            int index = 0;
            Vector3 position = body.position;

            if (position.X >= origin.X) index |= 1;
            if (position.Y >= origin.Y) index |= 2;
            if (position.Z >= origin.Z) index |= 4;

            Vector3 bodyHalfSize = body.GetAABB().HalfSize;
            if (position.X - bodyHalfSize.X < origin.X - halfSize.X || position.X + bodyHalfSize.X > origin.X + halfSize.X ||
                position.Y - bodyHalfSize.Y < origin.Y - halfSize.Y || position.Y + bodyHalfSize.Y > origin.Y + halfSize.Y ||
                position.Z - bodyHalfSize.Z < origin.Z - halfSize.Z || position.Z + bodyHalfSize.Z > origin.Z + halfSize.Z)
            {
                return -1;
            }

            return index;
        }
    }
}
