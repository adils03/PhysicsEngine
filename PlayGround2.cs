using Microsoft.Win32;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    public class PlayGround2 : Game
    {
        Vector3[] _pointLightPositions =
        {
                    new Vector3( -5.0f,  10.0f,  -5.0f),
                    new Vector3( -5.0f,  10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  -5.0f)
        };

        StringCubes StringCubes;

        public PlayGround2(NativeWindowSettings settings) : base(settings)
        {

            StringCubes = new StringCubes(new Vector3(0, 10.0f, 0));


            cameraManager.SetCameraSpeed(3);
            SetLamps();
        }
       
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);


            StringCubes.UpdateVelocity(KeyboardState, e);
            StringCubes.Update((float)e.Time);

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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
           

          

            base.OnUpdateFrame(e);
        }
        protected override void OnLoad()
        {
            base.OnLoad();





        }

    }
}
