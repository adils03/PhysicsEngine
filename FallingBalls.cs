using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class FallingBalls : Game
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

        public FallingBalls(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            SetLamps();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            world = new PhysicsWorld(10f, new Vector3(1000, 1000, 1000), ResolveType.Basic);

            cameraManager.SetCameraSpeed(10);
            GL.PointSize(30);
            RigidBody.CreateCubeBody(25, 1, 8, new Vector3(20, 10, -20), 1, true, 1f, Color4.AliceBlue, out RigidBody obstacle1);
            obstacle1.Rotate(new Vector3(0, 45, 45));
            RigidBody.CreateCubeBody(25, 1, 8, new Vector3(20, 10, 20), 1, true, 1f, Color4.AliceBlue, out RigidBody obstacle2);
            obstacle2.Rotate(new Vector3(0, -45, 45));
            RigidBody.CreateCubeBody(25, 1, 8, new Vector3(-20, 10, 20), 1, true, 1f, Color4.AliceBlue, out RigidBody obstacle3);
            obstacle3.Rotate(new Vector3(0, 45, -45));
            RigidBody.CreateCubeBody(25, 1, 8, new Vector3(-20, 10, -20), 1, true, 1f, Color4.AliceBlue, out RigidBody obstacle4);
            obstacle4.Rotate(new Vector3(0, -45, -45));
            RigidBody.CreateCubeBody(50, 1, 50, new Vector3(0, -10, 0), 1, true, 1f, Color4.AliceBlue, out RigidBody obstacle5);
            world.AddBody(obstacle5);
            world.AddBody(obstacle1);
            world.AddBody(obstacle2);
            world.AddBody(obstacle3);
            world.AddBody(obstacle4);

        }
        int index = 0;
        Vector3[] positions= {
            new Vector3(20, 20, 20),
            new Vector3(20, 20, -20),
            new Vector3(-20, 20, 20),
            new Vector3(-20, 20, -20),
            };
        Random rnd = new Random();
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            if (input.IsKeyPressed(Keys.Up))
            {
                float r = (float)rnd.NextDouble();
                float g = (float)rnd.NextDouble();
                float b = (float)rnd.NextDouble();
                RigidBody.CreateSphereBody(1, positions[index], 1, false, 0.7f, new Color4(r,g,b,1), out RigidBody ball);
                world.AddBody(ball);
                index++;
                index = index % positions.Length;
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
