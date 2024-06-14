using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class Labyrent : Game
    {

        Vector3[] _pointLightPositions =
        {
             new Vector3( -5.0f,  15.0f,  -15.0f),
             new Vector3( -5.0f,  15.0f,  15.0f),
             new Vector3( 5.0f,   15.0f,  15.0f),
             new Vector3( 5.0f,   15.0f,  -15.0f)
        };
        List<Shape> ShapeList;

        PhysicsWorld world;
        RigidBody cube1;

        public Labyrent(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            SetLamps();
        }
        int[,] maze = new int[10, 10] {
    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
    {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
    {1, 0, 1, 0, 1, 0, 1, 1, 0, 1},
    {1, 0, 1, 0, 0, 0, 0, 1, 0, 1},
    {1, 0, 1, 1, 1, 1, 0, 1, 0, 1},
    {1, 0, 0, 0, 0, 1, 0, 0, 0, 1},
    {1, 0, 1, 1, 0, 1, 1, 1, 0, 1},
    {1, 0, 1, 0, 0, 0, 0, 1, 0, 1},
    {1, 0, 0, 0, 1, 1, 0, 0, 0, 1},
    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
};

        protected override void OnLoad()
        {
            base.OnLoad();
            world = new PhysicsWorld(10f, new Vector3(1000, 1000, 1000), ResolveType.Basic);
            RigidBody.CreateCubeBody(2, 2, 2, new Vector3(0, 100, 0), 1, false, .3f, Color4.Red, out cube1);
            cameraManager.SetCameraSpeed(10);
            GL.PointSize(30);
            for (int z = 0; z < 10; z++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (maze[z, x] == 1)
                    {
                        Vector3 position = new Vector3(x, 0, z); // Küplerin Y ekseninde (0) olacak şekilde pozisyon ayarladık
                        RigidBody.CreateCubeBody(1, 1, 1, position, 1, true, 0f, Color4.AliceBlue, out RigidBody obstacle);
                        world.AddBody(obstacle);
                    }
                }
            }
            RigidBody.CreateCubeBody(10, 1, 10, new Vector3(5, -1, 5), 1, true, 0f, Color4.AliceBlue, out RigidBody floor);
            RigidBody.CreateCubeBody(0.7f, 0.7f, 0.7f, new Vector3(0, 10, 0), 1, false, 0f, Color4.AliceBlue, out cube1);
            world.AddBody(floor);
            world.AddBody(cube1);

        }
        int index = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            float dx = 0;
            float dy = 0;
            float dz = 0;
            float forceMagnitude = 5;
            if (input.IsKeyDown(Keys.Up))
            {
                dz--;

            }
            if (input.IsKeyDown(Keys.Left))
            {
                dx--;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                dx++;
            }
            if (input.IsKeyDown(Keys.Down))
            {
                dz++;

            }
            if (input.IsKeyDown(Keys.KeyPad2))
            {
                dy--;

            }
            if (input.IsKeyDown(Keys.KeyPad8))
            {
                dy++;

            }
            if (dx != 0 || dy != 0 || dz != 0)
            {
                Vector3 forceDirection = Vector3.Normalize(new Vector3(dx, dy, dz));
                Vector3 force = forceDirection * forceMagnitude;
                cube1.Move(force * (float)e.Time);
            }

        }

        public void SetLamps()
        {
            PointLightManager.GetInstance().SetPointLightPosition(this._pointLightPositions);
            pointLights = PointLightManager.GetInstance().GetPointLights();

            for (int i = 0; i < pointLights.Length; i++)
            {
                lampObjects[i] = new Sphere(pointLights[i].position, 0.2f, 15, Color4.White);
            }
        }
        public void RenderLamps()
        {
            for (int i = 0; i < pointLights.Length; i++)
            {

                lampObjects[i].RenderObject(camera, objectShader);
            }
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            world.Update((float)e.Time, 2);
            RenderLamps();
            SwapBuffers();

        }


    }
}
