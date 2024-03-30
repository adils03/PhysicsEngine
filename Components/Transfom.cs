using OpenTK.Mathematics;


namespace PhysicsEngine
{
    public class Transfom
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public Transfom()
        {
            position = new Vector3(0, 0, 0);
            rotation = Quaternion.Identity;
            scale = new Vector3(1,1,1);
        }
    }
}
