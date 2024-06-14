using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

namespace PhysicsEngine
{
    public class Space : Game
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

        public Space(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            SetLamps();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            world = new PhysicsWorld(0, new Vector3(1000, 1000, 1000), ResolveType.RotationAndFriction);

            cameraManager.SetCameraSpeed(10);
            GL.PointSize(30);
            RigidBody.CreateSphereBody(10, Vector3.Zero, 1, true, 1,Color4.Orange, out RigidBody planet);
            world.AddBody(planet);
            RigidBody.CreateCubeBody(3, 3, 3, new Vector3(-50, 0, 0), 1, false, 0f, Color4.AliceBlue, out cube1);
            world.AddBody(cube1);

            int numCubes1 = 50; 
            float radius1 = 15f; 

            int numCubes2 = 40; 
            float radius2 = 13f; 

            for (int i = 0; i < numCubes1; i++)
            {
                float angle = i * 2 * MathF.PI / numCubes1; 
                float x = radius1 * MathF.Cos(angle); 
                float z = radius1 * MathF.Sin(angle); 

                Vector3 position = new Vector3(x, 0, z);

                RigidBody.CreateCubeBody(1, 1, 1, position, 1, false, 0f, Color4.AliceBlue, out RigidBody cube);
                world.AddBody(cube);
            }

            for (int i = 0; i < numCubes2; i++)
            {
                float angle = i * 2 * MathF.PI / numCubes2;
                float x = radius2 * MathF.Cos(angle); 
                float z = radius2 * MathF.Sin(angle);

                Vector3 position = new Vector3(x, 0, z);

                RigidBody.CreateCubeBody(1, 1, 1, position, 1, false, 0f, Color4.AliceBlue, out RigidBody cube);
                world.AddBody(cube);
            }

        }
        Random rnd = new Random();
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            float dx = 0;
            float dy = 0;
            float dz = 0;
            float forceMagnitude = 250;
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
                cube1.AddForce(force);
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
