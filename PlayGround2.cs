using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    public class PlayGround2 : Game
    {


        Sphere Sphere;


        public PlayGround2(NativeWindowSettings settings) : base(settings)
        {
         
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Vector3 position = new Vector3(0,0,0);
            float radius = 1f;
            int segment = 15;
            Vector3 color = new Vector3(0.5f,0.2f,0.3f);

            Sphere = new Sphere(position,radius,segment,Color4.Bisque);

        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            var lights = PointLightManager.GetInstance().GetPointLights();
            var shader = ShaderManager.GetLightingColorShader();
            Vector3 color = new Vector3(0.5f, 0.2f, 0.3f);
            var cam = cameraManager.GetCamera();

            this.Sphere.RenderColorLighting(lights,cam,shader);

            SwapBuffers();
        }
    }
}
