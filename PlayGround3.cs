using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class PlayGround3 : Game
    {
        Vector3[] _pointLightPositions =
        {
        new Vector3( -5.0f,  10.0f,  -5.0f),
                    new Vector3( -5.0f,  10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  -5.0f)
        };

        private List<Joint> joints = new List<Joint>();

        Sphere sphere;
        Cube cube;

        PhysicsWorld world;
        RigidBody obstacle1;
        RigidBody obstacle2;
        RigidBody obstacle3;
        RigidBody obstacle4;

        public PlayGround3(NativeWindowSettings settings) : base(settings)
        {
            sphere = new Sphere(new Vector3(0, 0, 0), 0.5f, 5, Color4.AliceBlue);
            cube = new Cube(new Vector3(0, 0, 0));
            world = new PhysicsWorld(9.81f, new Vector3(50, 50, 50));

            cameraManager.SetCameraSpeed(5);
            SetLamps();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            HandleJoints();
            obstacle1.shape.SetAnchorPos(obstacle1.position, 0);
            obstacle4.shape.SetAnchorPos(obstacle4.position, 0);
            //obstacle3.shape.SetAnchorPos(obstacle3.position, 0);

            //sphere.Teleport(obstacle1.shape.GetAnchorPos(0));
            //cube.Teleport(obstacle2.shape.GetAnchorPos(0));

            var input = KeyboardState;
            float dx = 0;
            float dy = 0;
            float dz = 0;
            float forceMagnitude = 50;
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
            if (input.IsKeyPressed(Keys.KeyPad8))
            {
                dy++;

            }
            if (dx != 0 || dy != 0 || dz != 0)
            {
                Vector3 forceDirection = Vector3.Normalize(new Vector3(dx, dy, dz));
                Vector3 force = forceDirection * forceMagnitude;
                obstacle1.AddForce(force);
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            cube.Rotate(new Vector3(45, 45, 45));

            //RigidBody.CreateCubeBody(15, 2, 15, new Vector3(0, 0, 0), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle8);
            //world.AddBody(obstacle8);

            //RigidBody.CreateCubeBody(15, 10, 2, new Vector3(0, 2, 6.5f), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle4);
            //world.AddBody(obstacle4);

            //RigidBody.CreateCubeBody(15, 10, 2, new Vector3(0, 2, -6.5f), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle5);
            //world.AddBody(obstacle5);

            //RigidBody.CreateCubeBody(2, 10, 15, new Vector3(-6.5f, 2, 0), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle6);
            //world.AddBody(obstacle6);

            //RigidBody.CreateCubeBody(2, 10, 15, new Vector3(6.5f, 2, 0), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle7);
            //world.AddBody(obstacle7);


            ///////////////////////////// burası /////

            RigidBody.CreateSphereBody(0.001f, new Vector3(0, 8f, 0), 1, true, 1, Color4.Yellow, out obstacle3);
            //int anchorCircleId = obstacle1.shape.CreateAnchor(new Vector3(0, 0, 0));
            world.AddBody(obstacle3);

            RigidBody.CreateSphereBody(0.5f,new Vector3(0,6,0),1,false,1,Color4.Yellow,out obstacle1);
            int anchorCircleId = obstacle1.shape.CreateAnchor(new Vector3(0, 0, 0));
            world.AddBody(obstacle1);

            RigidBody.CreateCylinderBody(5,5, new Vector3(0, 8, 0), 2, true, 2, Color4.DarkCyan, out obstacle4);
            int anchorCircleId2 = obstacle4.shape.CreateAnchor(new Vector3(0, 0, 0));
            world.AddBody(obstacle4);

            //RigidBody.CreateCubeBody(1.5f, 1.5f, 1.5f, new Vector3(1, 20, 0), 1, true, 0.5f, Color4.Yellow, out obstacle2);
            //int anchorCircleId2 = obstacle2.shape.CreateAnchor(new Vector3(0, 0, 0));
            //world.AddBody(obstacle2);

            //RigidBody.CreateCubeBody(1, 1, 1, new Vector3(1, 18, 0), 1, false, 0.5f, Color4.Red, out obstacle3);
            //int anchorCircleId3 = obstacle3.shape.CreateAnchor(new Vector3(0, 0, 0));
            //world.AddBody(obstacle3);

            JointConnection jointConnection = new JointConnection(obstacle1, anchorCircleId, obstacle4, anchorCircleId2);
            ////JointConnection jointConnection2 = new JointConnection(obstacle2, anchorCircleId3, obstacle3, anchorCircleId3);

            joints.Add(new SpringJoint(jointConnection, 2f, 5));
            ////joints.Add(new SpringJoint(jointConnection2, 2f, 5));
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            world.Update((float)e.Time, 1);
            //sphere.RenderBasic();
            //cube.RenderBasic();

            RenderLamps();
            SwapBuffers();
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
        public void HandleJoints()
        {
            foreach (var joint in joints)
            {
                // Console.WriteLine(joint.jointConnection.anchorA);
                joint.UpdateConnectionA();
                joint.UpdateConnectionB();
            }
        }
    }
}

