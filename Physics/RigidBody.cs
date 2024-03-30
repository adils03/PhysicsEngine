using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public enum ShapeType
    {
        Sphere = 0,
        Cube = 1
    }
    public class RigidBody
    {
        public Vector3 position;
        private Vector3 linearVelocity;
        private float rotation;
        private float angularVelocity;

        public readonly float density;
        public readonly float mass;
        public readonly float restitution;
        public readonly float area;

        public readonly bool isStatic;

        public readonly float radius;
        public readonly float width;
        public readonly float height;
        public readonly float depth;

        public readonly ShapeType modelType;
        public RigidBody(Vector3 position, float density, float mass, float restitution, float area
            , bool isStatic, float radius, float width, float height,float depth, ShapeType shapeType)
        {
            this.position = position;
            linearVelocity = Vector3.Zero;
            rotation = 0;
            angularVelocity = 0;

            this.density = density;
            this.mass = mass;
            this.restitution = restitution;
            this.area = area;

            this.isStatic = isStatic;
            this.radius = radius;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.modelType = shapeType;
        }
        public void Move(Vector3 amount)
        {
            this.position += amount;
        }
        public void MoveTo(Vector3 target)
        {
            this.position = target;
        }
        public static void CreateCircleBody(float radius, Vector3 position, float density, bool isStatic, float restitution,
                                        out RigidBody rigidBody, out string errorMessage)
        {
            rigidBody = null;
            errorMessage = string.Empty;

            float area = radius * radius * MathF.PI;
            restitution = MathHelper.Clamp(restitution, 0.0f, 1.0f);
            float mass = area * density;
            rigidBody = new RigidBody(position,density,mass,restitution,area,isStatic,radius,0,0,0,ShapeType.Sphere);
        }
        public static void CreateCubeBody(float width,float height,float depth, Vector3 position, float density, bool isStatic, float restitution,
                                        out RigidBody rigidBody, out string errorMessage)
        {
            rigidBody = null;
            errorMessage = string.Empty;

            float area = width * height * depth;    
            restitution = MathHelper.Clamp(restitution, 0.0f, 1.0f);
            float mass = area * density;
            rigidBody = new RigidBody(position, density, mass, restitution, area, isStatic, 0,width, height, depth, ShapeType.Cube);
        }
    }
}
