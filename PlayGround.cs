using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class PlayGround : Game
    {   

        List<Shape> ShapeList;

        Cube cube;
        Cube cube2;
        Sphere sphere;
        Cube cubeRed;
        PhysicsWorld world;
        RigidBody ball5;
        public PlayGround(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            //cube = new Cube(ShapeShaderType.Textured);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            world = new PhysicsWorld(PhysicsWorld.WORLD_GRAVITY,new Vector3(100,100,100));
            camera.Position = new Vector3(0, 10, 10);
            cameraManager.SetCameraSpeed(5);
            RigidBody.CreateCubeBody(1, 10, 3, new Vector3(10, 10, 0), 1, true, 1, new Vector3(10, 10, 0), out RigidBody obstacle1);
            RigidBody.CreateCubeBody(1, 10, 3, new Vector3(0, 0, 0), 1, true, 1, new Vector3(10, 10, 0), out RigidBody obstacle2);
            RigidBody.CreateCubeBody(1, 1, 1, new Vector3(0, -10, 5), 1, true, 1, new Vector3(10, 10, 0), out RigidBody obstacle3);
            RigidBody.CreateCubeBody(1, 1, 1, new Vector3(10f, 0, 10f), 1, false, 0.5f, new Vector3(10, 10, 0), out RigidBody obstacle4);
            RigidBody.CreateCubeBody(20, 1, 20, new Vector3(0, -5f, 0), 1, true, 0.5f, new Vector3(10, 10, 0), out RigidBody obstacle5);
            RigidBody.CreateCubeBody(100, 1, 5, new Vector3(0, -7.3f, 0), 1, true, 0.5f, new Vector3(10, 10, 0), out RigidBody obstacle6);
            obstacle6.Rotate(new Vector3(90, 0, 0));
            RigidBody.CreateSphereBody(1, new Vector3(8, 12, 0), 1, false, .7f, new Vector3(10, 10, 0), out RigidBody ball1);
            RigidBody.CreateSphereBody(1, new Vector3(7, 12, 1), 1, false, .7f, new Vector3(10, 10, 0), out RigidBody ball2);
            RigidBody.CreateSphereBody(1, new Vector3(8, 12, 0), 1, false, .7f, new Vector3(10, 10, 0), out RigidBody ball3);
            RigidBody.CreateSphereBody(1, new Vector3(1, 14, 0), 1, false, .7f, new Vector3(10, 10, 0), out RigidBody ball4);
            RigidBody.CreateCubeBody(1, 1, 1, new Vector3(7, 16, 0), 1, false, .7f, new Vector3(10, 10, 0), out ball5);
            RigidBody.CreateCubeBody(1, 1, 1, new Vector3(9, 16, 0), 1, false, .7f, new Vector3(10, 10, 0), out RigidBody ball6);
            obstacle1.Rotate(new Vector3(0, 0, 135));
            obstacle2.Rotate(new Vector3(0, 0, 45));
            world.AddBody(ball1);
            world.AddBody(ball2);
            world.AddBody(ball3);
            world.AddBody(ball4);
            world.AddBody(ball5);
            world.AddBody(ball6);
            world.AddBody(obstacle1);
            world.AddBody(obstacle2);
            world.AddBody(obstacle3);
            world.AddBody(obstacle4);
            world.AddBody(obstacle5);
            world.AddBody(obstacle6);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var _input = KeyboardState;
            //Console.WriteLine(ball5.linearVelocity + " linear velocity");
            //ShapeController.ControllerA(_input, cube, e);
            //Console.WriteLine(cube1.linearVelocity);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            world.Update((float)e.Time, 1);
            SwapBuffers();
        }
      

    }
}
