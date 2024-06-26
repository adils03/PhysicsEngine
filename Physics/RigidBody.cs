﻿using OpenTK.Mathematics;


namespace PhysicsEngine
{
    public readonly struct AABB
    {
        public readonly Vector3 min;
        public readonly Vector3 max;

        public AABB(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public AABB(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            min = new Vector3(minX, minY, minZ);
            max = new Vector3(maxX, maxY, maxZ);
        }

        public Vector3 HalfSize => (max - min) * 0.5f;
    }

    public enum ShapeType
    {
        Sphere = 0,
        Cube = 1
    }

    public class RigidBody
    {
        public Vector3 position;
        public Vector3 linearVelocity;
        private Vector3 angle;
        private Vector3 torqueAccumulator;
        private Vector3 forceAccumulator;
        public Vector3 angularVelocity;
        private Vector3 force;

        public readonly float damping;

        public readonly float density;
        public readonly float mass;
        public readonly float invMass;
        public readonly float restitution;
        public readonly float area;
        public readonly float staticFriction;
        public readonly float dynamicFriction;
        public readonly bool isStatic;
        public readonly float radius;
        public readonly float width;
        public readonly float height;
        public readonly float depth;
        public readonly bool isKinematic;
       
        public readonly ShapeType shapeType;
        public readonly Shape shape;

        public readonly Matrix3 inertiaTensor;
        public readonly Matrix3 invInertiaTensor;
        
        public RigidBody(Vector3 position, float density, float mass, float restitution, float area, bool isStatic, float radius, float width, float height, float depth, ShapeType shapeType, Color4 color)
        {
            this.position = position;
            this.linearVelocity = Vector3.Zero;
            this.angle = Vector3.Zero;
            this.angularVelocity = Vector3.Zero;
            this.force = Vector3.Zero;
            this.density = density;
            this.mass = mass;
            this.restitution = restitution;
            this.area = area;
            this.isStatic = isStatic;
            this.radius = radius;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.shapeType = shapeType;
            this.torqueAccumulator = Vector3.Zero;
            this.forceAccumulator = Vector3.Zero;

            this.staticFriction = 0.8f;
            this.dynamicFriction = 0.4f;
            this.damping = 0.999f;
            if (!isStatic)
            {
                invMass = 1.0f / mass;
                invInertiaTensor = GetInverseInertiaTensor();
                inertiaTensor = GetInertiaTensor();
            }
            else
            {
                invMass = 0f;
                isKinematic = true;
                inertiaTensor = Matrix3.Zero;
                invInertiaTensor = Matrix3.Zero;
            }
            

            shape = shapeType switch
            {
                ShapeType.Sphere => new Sphere(ShapeShaderType.ColorLight, position, color, radius),
                ShapeType.Cube => new Cube(position, ShapeShaderType.Textured, new Vector3(width, height, depth)),
                _ => throw new ArgumentException("Unsupported shape type")
            };
        }
        public void Update(float time, Vector3 gravity, int iterations)
        {
            shape.RenderBasic();

            if (isStatic) return;

            time /= iterations;
            Vector3 acceleration = force/mass;
            linearVelocity += acceleration * time;

            linearVelocity += gravity * time;

            Move(linearVelocity * time);
            angularVelocity *= 30;
            angle += angularVelocity * time;
            Rotate(angle);
            //linearVelocity *= damping;
            angle *= damping;

            position = shape.Transform.Position;
            angularVelocity = Vector3.Zero;
            force = Vector3.Zero;
        }
        /*public void Update(float time, Vector3 gravity, int iterations)
        {
            shape.RenderBasic();

            if (isStatic) return;

            // Zaman dilimini iterasyon sayısına böl
            time /= iterations;

            // Kuvvet birikimcisini kullanarak lineer hızlanmayı hesapla
            Vector3 totalForce = force + forceAccumulator; // Eklenen kuvvet birikimcisini kullan
            Vector3 acceleration = totalForce / mass;

            // Lineer hızı güncelle
            linearVelocity += gravity * time;
            linearVelocity += acceleration * time;
            //linearVelocity *= 0.9999f;

            // Konumu güncelle
            Move(linearVelocity * time);

            // Açısal hızlanmayı hesapla
            angularVelocity *= 0.8f;

            // Açıyı güncelle
            angle += angularVelocity * time;
            shape.Rotate(angle);
           
            // RigidBody'nin pozisyonunu shape'in pozisyonuyla güncelle
            position = shape.Transform.Position;

            // Kuvvet ve tork birikimcilerini sıfırla
            forceAccumulator = Vector3.Zero;
            torqueAccumulator = Vector3.Zero;

            // Kuvvet birikimcisini sıfırla
            force = Vector3.Zero;
        }*/

        public float CalculateRotationalInertiaX()
        {
            return shapeType switch
            {
                ShapeType.Sphere => (2f / 5) * mass * radius * radius,
                ShapeType.Cube => (1f / 12) * mass * (height * height + depth * depth),
                _ => 0
            };
        }
        public float CalculateRotationalInertiaZ()
        {
            return shapeType switch
            {
                ShapeType.Sphere => (2f / 5) * mass * radius * radius,
                ShapeType.Cube => (1f / 12) * mass * (width * width + depth * depth),
                _ => 0
            };
        }
        public float CalculateRotationalInertiaY()
        {
            return shapeType switch
            {
                ShapeType.Sphere => (2f / 5) * mass * radius * radius,
                ShapeType.Cube => (1f / 12) * mass * (width * width + height * height),
                _ => 0
            };
        }


        public Matrix3 GetInertiaTensor()
        {
            Matrix3 matrix3D = new Matrix3(
                new Vector3(CalculateRotationalInertiaX(),0,0), 
                new Vector3(0,CalculateRotationalInertiaY(),0), 
                new Vector3(0,0,CalculateRotationalInertiaZ()));
            return matrix3D;
        }

        public Matrix3 GetInverseInertiaTensor()
        {
            Matrix3 matrix3D = new Matrix3(
                new Vector3(1/CalculateRotationalInertiaX(), 0, 0),
                new Vector3(0, 1/CalculateRotationalInertiaY(), 0),
                new Vector3(0, 0,1/ CalculateRotationalInertiaZ()));
            return matrix3D;
        }

        public AABB GetAABB()
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            if (shapeType == ShapeType.Cube)
            {
                Vector3[] vertices = shape.GetVertices();
                foreach (var v in vertices)
                {
                    if (v.X < minX) minX = v.X;
                    if (v.Y < minY) minY = v.Y;
                    if (v.Z < minZ) minZ = v.Z;
                    if (v.X > maxX) maxX = v.X;
                    if (v.Y > maxY) maxY = v.Y;
                    if (v.Z > maxZ) maxZ = v.Z;
                }
            }
            else if (shapeType == ShapeType.Sphere)
            {
                minX = position.X - radius;
                minY = position.Y - radius;
                minZ = position.Z - radius;
                maxX = position.X + radius;
                maxY = position.Y + radius;
                maxZ = position.Z + radius;
            }

            return new AABB(minX, minY, minZ, maxX, maxY, maxZ);
        }
        public void AddForce(Vector3 amount) => force += amount;
        public void AddAngularVelocity(Vector3 amount) => angularVelocity += amount;
        public void AddForceAtPoint(Vector3 atPoint, Vector3 force)
        {
            Vector3 direction = atPoint - position;
            forceAccumulator += force;
            torqueAccumulator += Vector3.Cross(direction, force);
            //Console.WriteLine(torqueAccumulator);
        }
        public void Move(Vector3 moveVector) => shape.Translate(moveVector);
        public void Rotate(Vector3 rotateVector) => shape.Rotate(rotateVector);
        public void MoveTo(Vector3 target)
        {
            position = target;
            shape.Teleport(target);
        }
        public static void CreateSphereBody(float radius, Vector3 position, float density, bool isStatic, float restitution, Color4 color, out RigidBody rigidBody)
        {
            float area = radius * radius * MathF.PI;
            restitution = MathHelper.Clamp(restitution, 0.0f, 1.0f);
            float mass = area * density;
            rigidBody = new RigidBody(position, density, mass, restitution, area, isStatic, radius, 0, 0, 0, ShapeType.Sphere, color);
        }


        public static void CreateCubeBody(float width, float height, float depth, Vector3 position, float density, bool isStatic, float restitution, Color4 color, out RigidBody rigidBody)
        {
            float area = width * height * depth;
            restitution = MathHelper.Clamp(restitution, 0.0f, 1.0f);
            float mass = area * density;
            rigidBody = new RigidBody(position, density, mass, restitution, area, isStatic, 0, width, height, depth, ShapeType.Cube, color);
        }

        public static void CreateCubeBody(Cube cube, bool isStatic, float restitution, out RigidBody rigidBody)
        {
            float width = cube.Transform.Scale.X;
            float height = cube.Transform.Scale.Y;
            float depth = cube.Transform.Scale.Z;
            float density = 1;
            Vector3 position = cube.Transform.Position;
            float area = width * height * depth;
            restitution = MathHelper.Clamp(restitution, 0.0f, 1.0f);
            float mass = area * density;
            rigidBody = new RigidBody(position, density, mass, restitution, area, isStatic, 0, width, height, depth, ShapeType.Cube, Color4.AliceBlue);
        }

    }
}
