using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PhysicsEngine.Shapes;

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
        Platform platform;
        Sphere sphere;

        PhysicsWorld world;
        


        public PlayGround3(NativeWindowSettings settings) : base(settings)
        {
            platform = new Platform(new Vector3(0, 0, 0));
            world = new PhysicsWorld(PhysicsWorld.WORLD_GRAVITY, new Vector3(50, 50, 50));
           
            cameraManager.SetCameraSpeed(5);
            SetLamps();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            HandleJoints();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            
            //RigidBody.CreateCubeBody(1, 1, 1, new Vector3(0, 10, 4), 1, false, .8f, Color4.AliceBlue, out RigidBody cube);

            int anchorCircleId = sphere.CreateAnchor(new Vector3(50, 25, 0));
            RigidBody.CreateSphereBody(.5f, new Vector3(0.1f, 1, 0), 1, false, 1f, Color4.Red, out RigidBody obstacle2);
            world.AddBody(obstacle2);


            int anchorRectId = platform.CreateAnchor(new Vector3(50, 25,0));
            RigidBody.CreateCubeBody(platform, true, 1, out RigidBody obstacle1);
            world.AddBody(obstacle1);

            JointConnection jointConnection = new JointConnection(obstacle1, anchorRectId,obstacle2, anchorCircleId);
            joints.Add(new ForceJoint(jointConnection, 1));

            //world.AddBody(cube);



        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            platform.RenderBasic();
            
            world.Update((float)e.Time, 1);
            

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

