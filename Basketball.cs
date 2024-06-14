using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

namespace PhysicsEngine
{
    public class Basketball : Game
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

        public Basketball(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            SetLamps();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            world = new PhysicsWorld(10, new Vector3(1000, 1000, 1000), ResolveType.Basic);

            cameraManager.SetCameraSpeed(10);
            GL.PointSize(30);

            int numCubes1 = 10; 
            float radius1 = 3; 

            for (int i = 0; i < numCubes1; i++)
            {
                float angle = i * 2 * MathF.PI / numCubes1; 
                float x = radius1 * MathF.Cos(angle); 
                float z = radius1 * MathF.Sin(angle); 

                Vector3 position = new Vector3(x, 0, z);

                RigidBody.CreateCubeBody(2, 1, 2, position, 1, true, 0f, Color4.AliceBlue, out RigidBody cube);
                world.AddBody(cube);
            }

        }
        Random rnd = new Random();
        float power;
        bool isPower = false;
        RigidBody ball;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            if (input.IsKeyPressed(Keys.Up))
            {

                isPower = true;
            }
            if (isPower) {
                power += (float)e.Time*3;
            }
            if (input.IsKeyReleased(Keys.Up))
            {
                RigidBody.CreateSphereBody(1, new Vector3(0, 10, -20), 1, false, 1, Color4.DarkGray, out ball);
                world.AddBody(ball);
                ball.linearVelocity = new Vector3(0, 0, power);
                isPower = false;
                power = 0;
            }

            Console.WriteLine(power);

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
