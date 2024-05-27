using OpenTK.Mathematics;
using System;

namespace PhysicsEngine
{

    public class RigidBodyParameters
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public float Density { get; set; } = 1.0f;
        public float Mass { get; set; } = 1.0f;
        public float Restitution { get; set; } = 0.5f;
        public float Area { get; set; } = 1.0f;
        public bool IsStatic { get; set; } = false;
        public float Radius { get; set; } = 1.0f;
        public float Width { get; set; } = 1.0f;
        public float Height { get; set; } = 1.0f;
        public float Depth { get; set; } = 1.0f;
        public ShapeType ShapeType { get; set; } = ShapeType.Cube;
        public Color4 Color { get; set; } = Color4.White;
    }

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
        public Vector3 angularVelocity;
        private Vector3 force;

        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                shape.Teleport(position); // Shape'in pozisyonunu güncelle
            }
        }

        public readonly float density;
        public readonly float mass;
        public readonly float invMass;
        public readonly float restitution;
        public readonly float area;
        public readonly float inertia;
        public readonly float invInertia;
        public readonly bool isStatic;
        public readonly float radius;
        public readonly float width;
        public readonly float height;
        public readonly float depth;
        public readonly ShapeType shapeType;
        public readonly Shape shape;

        public RigidBody(RigidBodyParameters parameters)
        {
            this.position = parameters.Position;
            this.linearVelocity = Vector3.Zero;
            this.angle = Vector3.Zero;
            this.angularVelocity = Vector3.Zero;
            this.force = Vector3.Zero;
            this.density = parameters.Density;
            this.mass = parameters.Mass;
            this.restitution = parameters.Restitution;
            this.area = parameters.Area;
            this.isStatic = parameters.IsStatic;
            this.radius = parameters.Radius;
            this.width = parameters.Width;
            this.height = parameters.Height;
            this.depth = parameters.Depth;
            this.shapeType = parameters.ShapeType;

            this.inertia = CalculateRotationalInertia();
            if (!isStatic)
            {
                invMass = 1.0f / mass;
                invInertia = 1.0f / inertia;
            }
            else
            {
                invMass = 0f;
                invInertia = 0f;
            }

            shape = shapeType switch
            {
                ShapeType.Sphere => new Sphere(ShapeShaderType.ColorLight, position, parameters.Color, radius),
                ShapeType.Cube => new Cube(position, ShapeShaderType.Textured, new Vector3(width, height, depth)),
                _ => throw new ArgumentException("Unsupported shape type")
            };
        }

        private float CalculateRotationalInertia()
        {
            return shapeType switch
            {
                ShapeType.Sphere => (2f / 5) * mass * radius * radius,
                ShapeType.Cube => (1f / 12) * mass * (height * height + width * width),
                _ => 0
            };
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

        public void Update(float time, Vector3 gravity, int iterations)
        {
            shape.RenderBasic();

            if (isStatic) return;

            Vector3 acceleration = force / mass;

            // Adding gravity and acceleration to linear velocity
            linearVelocity += gravity * time;
            linearVelocity += acceleration * time;

            // Implementing a drag force proportional to the velocity
            float dragCoefficient = 0.5f; // This can be adjusted based on your requirements
            Vector3 dragForce = -linearVelocity * dragCoefficient;
            linearVelocity += dragForce * time;

            // Updating position
            Position += linearVelocity * time;

            // Updating angle
            angle += angularVelocity * time;

            // Rotating the shape
            shape.Rotate(angle);

            // Resetting angle and force for the next iteration
            angle = Vector3.Zero;
            force = Vector3.Zero;

            Console.WriteLine("linear " + linearVelocity);
        }


        public void AddForce(Vector3 amount) => force += amount;
        public void Move(Vector3 moveVector)
        {
            shape.Translate(moveVector);
            position += moveVector;
        }

        public void Rotate(Vector3 rotateVector)
        {
            
            shape.Rotate(rotateVector);
        }

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
            var parameters = new RigidBodyParameters
            {
                Position = position,
                Density = density,
                Mass = mass,
                Restitution = restitution,
                Area = area,
                IsStatic = isStatic,
                Radius = radius,
                ShapeType = ShapeType.Sphere,
                Color = color
            };
            rigidBody = new RigidBody(parameters);
        }

        public static void CreateCubeBody(float width, float height, float depth, Vector3 position, float density, bool isStatic, float restitution, Color4 color, out RigidBody rigidBody)
        {
            float area = width * height * depth;
            restitution = MathHelper.Clamp(restitution, 0.0f, 1.0f);
            float mass = area * density;
            var parameters = new RigidBodyParameters
            {
                Position = position,
                Density = density,
                Mass = mass,
                Restitution = restitution,
                Area = area,
                IsStatic = isStatic,
                Width = width,
                Height = height,
                Depth = depth,
                ShapeType = ShapeType.Cube,
                Color = color
            };
            rigidBody = new RigidBody(parameters);
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
            var parameters = new RigidBodyParameters
            {
                Position = position,
                Density = density,
                Mass = mass,
                Restitution = restitution,
                Area = area,
                IsStatic = isStatic,
                Width = width,
                Height = height,
                Depth = depth,
                ShapeType = ShapeType.Cube,
                Color = Color4.AliceBlue
            };
            rigidBody = new RigidBody(parameters);
        }


        public static RigidBody CreateSphereBody(RigidBodyParameters parameters)
        {
            if (parameters.ShapeType != ShapeType.Sphere)
            {
                throw new ArgumentException("ShapeType must be Sphere for this method.");
            }

            parameters.Area = parameters.Radius * parameters.Radius * MathF.PI;
            parameters.Restitution = MathHelper.Clamp(parameters.Restitution, 0.0f, 1.0f);
            parameters.Mass = parameters.Area * parameters.Density;
            return new RigidBody(parameters);
        }

        public static RigidBody CreateCubeBody(RigidBodyParameters parameters)
        {
            if (parameters.ShapeType != ShapeType.Cube)
            {
                throw new ArgumentException("ShapeType must be Cube for this method.");
            }

            parameters.Area = parameters.Width * parameters.Height * parameters.Depth;
            parameters.Restitution = MathHelper.Clamp(parameters.Restitution, 0.0f, 1.0f);
            parameters.Mass = parameters.Area * parameters.Density;
            return new RigidBody(parameters);
        }

        public static RigidBody CreateCubeBody(Cube cube, bool isStatic, float restitution)
        {
            float width = cube.Transform.Scale.X;
            float height = cube.Transform.Scale.Y;
            float depth = cube.Transform.Scale.Z;
            float density = 1;
            Vector3 position = cube.Transform.Position;
            float area = width * height * depth;
            restitution = MathHelper.Clamp(restitution, 0.0f, 1.0f);
            float mass = area * density;
            var parameters = new RigidBodyParameters
            {
                Position = position,
                Density = density,
                Mass = mass,
                Restitution = restitution,
                Area = area,
                IsStatic = isStatic,
                Width = width,
                Height = height,
                Depth = depth,
                ShapeType = ShapeType.Cube,
                Color = Color4.AliceBlue
            };
            return new RigidBody(parameters);
        }

    }
}
