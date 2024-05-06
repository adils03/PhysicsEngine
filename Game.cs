using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using PhysicsEngine.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace PhysicsEngine
{
    public class Game : GameWindow
    {
        CameraHelper cameraHelper;
        protected Camera cam;
        bool isLineMod = false;

        protected ShaderProgram objectShader;
        protected ShaderProgram lightingShader;

        public Game(NativeWindowSettings settings) : base(GameWindowSettings.Default, settings)
        {
            CenterWindow();
            cameraHelper = new CameraHelper(Size);
            cam = cameraHelper.GetCamera();
            CursorState = CursorState.Grabbed;

            objectShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/Shader.frag");
            lightingShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/Lighting.frag");
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f,0.2f,0.3f,1.0f);
            GL.Enable(EnableCap.DepthTest);
            IsVisible = true;

           
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;

            cameraHelper.CamControl(input, MouseState, e);

            if (input.IsKeyPressed(Keys.E))
            {
                isLineMod = !isLineMod;
                if (isLineMod)
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                }
                else
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
            }
            if (input.IsKeyDown(Keys.Escape)) { Close(); }

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            cameraHelper.GetCamera().AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
