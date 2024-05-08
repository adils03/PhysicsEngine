﻿using OpenTK.Mathematics;
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

        Cube cubeRed;
        public PlayGround(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            cube = new Cube(); ShapeList.Add(cube);
            cube2 = new Cube(); ShapeList.Add(cube2);
            Sphere sphere = new Sphere(Vector3.Zero, 1, 15, Color4.Red); ShapeList.Add(sphere);

            cubeRed = new Cube(new Vector3(10,10,10),new Vector3(3));


        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var _input = KeyboardState;

            ShapeController.ControllerA(_input,cube,e);
            ShapeController.ControllerB(_input,cube2,e);
        
            //çarpışma
            if (Collisions.IntersectCubes(cube, cube2, 0.02f, out Vector3 normal, out float depth))
            {
                cube.Translate(-normal * depth / 2);
                cube2.Translate(normal * depth / 2);
            }
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            for (int i = 0; i < ShapeList.Count; i++) 
            {
                ShapeList[i].RenderLighting(pointLights, cam, lightingShader);
            }

            cubeRed.RenderColorLighting(pointLights, cam, lightingColorShader,new Vector3(1,0.5f,0.5f));
           
            SwapBuffers();
        }
      

    }
}
