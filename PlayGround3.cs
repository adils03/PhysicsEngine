using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PhysicsEngine.Shapes;
using System;

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
        Sphere sphere2;

        PhysicsWorld world;
        RigidBody obstacle2;
        RigidBody obstacle1;
        


        public PlayGround3(NativeWindowSettings settings) : base(settings)
        {
          
            sphere = new Sphere(new Vector3(0, 0, 0), 0.3f, 5,Color4.AliceBlue);
            sphere2 = new Sphere(new Vector3(0, 0, 0), 0.3f, 5, Color4.AliceBlue);
            world = new PhysicsWorld(9.81f, new Vector3(50, 50, 50));
           
            cameraManager.SetCameraSpeed(5);
            SetLamps();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            HandleJoints();
            obstacle1.shape.SetAnchorPos(obstacle1.position,0);
            obstacle2.shape.SetAnchorPos(obstacle2.position, 0);

            sphere.Teleport(obstacle1.shape.GetAnchorPos(0));
            sphere2.Teleport(obstacle2.shape.GetAnchorPos(0));



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
            if (input.IsKeyPressed(Keys.KeyPad8))
            {
                dy++;

            }
            if (dx != 0 || dy != 0 || dz != 0)
            {
                Vector3 forceDirection = Vector3.Normalize(new Vector3(dx, dy, dz));
                Vector3 force = forceDirection * forceMagnitude;
                obstacle2.AddForce(force);
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            RigidBody.CreateCubeBody(15, 2, 15, new Vector3(0,0, 0), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle3);
            world.AddBody(obstacle3);


            RigidBody.CreateCubeBody(15, 10, 2, new Vector3(0, 2, 6.5f), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle4);
            world.AddBody(obstacle4);

            RigidBody.CreateCubeBody(15, 10, 2, new Vector3(0, 2, -6.5f), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle5);
            world.AddBody(obstacle5);

            RigidBody.CreateCubeBody(2, 10, 15, new Vector3(-6.5f, 2, 0), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle6);
            world.AddBody(obstacle6);

            RigidBody.CreateCubeBody(2, 10, 15, new Vector3(6.5f, 2, 0), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle7);
            world.AddBody(obstacle7);


            RigidBody.CreateSphereBody(.5f, new Vector3(0.1f, 2, 0), 1, false, 1f, Color4.Red, out obstacle2);
            int anchorCircleId = obstacle2.shape.CreateAnchor(new Vector3(0, 0, 0));
            world.AddBody(obstacle2);


            RigidBody.CreateSphereBody(.5f, new Vector3(0.5f, 2, 0), 1, false, 1f, Color4.Yellow, out obstacle1);
            int anchorRectId = obstacle1.shape.CreateAnchor(new Vector3(0, 0,0));
            world.AddBody(obstacle1);

            JointConnection jointConnection = new JointConnection(obstacle1, anchorRectId,obstacle2, anchorCircleId);
            joints.Add(new ForceJoint(jointConnection, 5));



            
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
           
           
          
            
            world.Update((float)e.Time, 1);
            sphere.RenderBasic();
            sphere2.RenderBasic();

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

