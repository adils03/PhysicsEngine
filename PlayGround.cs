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
        public PlayGround(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            cube = new Cube(ShapeShaderType.Textured);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            //world = new PhysicsWorld(PhysicsWorld.WORLD_GRAVITY);
            //cube = new Cube(); ShapeList.Add(cube);
            ////cube2 = new Cube(); ShapeList.Add(cube2);
            //sphere = new Sphere(Vector3.One, 1, 15, new Vector3(1)); ShapeList.Add(sphere);

            //cubeRed = new Cube(new Vector3(10,10,10),new Vector3(3));


        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //world.Update((float)e.Time,1);
            var _input = KeyboardState;

            ShapeController.ControllerA(_input, cube, e);
            ////ShapeController.ControllerB(_input,cube2,e);

            ////çarpışma
            //if (Collisions.IntersectCubeSphere(true,cube, sphere, 0, out Vector3 normal, out float depth))
            //{
            //    cube.Translate(-normal * depth / 2);
            //    sphere.Translate(normal * depth / 2);
            //}
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //for (int i = 0; i < ShapeList.Count; i++) 
            //{
            //    ShapeList[i].RenderLighting(pointLights, camera, lightingShader);
            //}

            //cubeRed.RenderColorLighting(pointLights, camera, lightingColorShader,new Vector3(1,0.5f,0.5f));

            cube.RenderBasic();
           
            SwapBuffers();
        }
      

    }
}
