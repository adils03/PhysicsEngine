using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PhysicsEngine.Shapes;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class PlayGround : Game
    {
        TexturedCube cube;
        Vector3 lightPos = new Vector3(3,3,3);
        TexturedCube lamp;
        public PlayGround(NativeWindowSettings settings) : base(settings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            cube = new TexturedCube();
            lamp = new TexturedCube(lightPos,new Vector3(0.3f,0.3f,0.3f));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var _input = KeyboardState;
            if(_input.IsKeyDown(Keys.Q))
            {
                //cube.Translate(new Vector3(0, 0, 10) * (float)e.Time);
                cube.Rotate(new Vector3(90,0,0) * (float)e.Time);
            }
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            cube.RenderLighting(lightPos,cam,lightingShader);
            lamp.RenderObject(cam, objectShader);
            SwapBuffers();
        }
    }
}
