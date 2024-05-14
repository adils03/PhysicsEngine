using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace PhysicsEngine
{
    public class Game : GameWindow
    {
        protected CameraManager cameraManager;
        protected Camera camera;
        bool isLineMod = false;

        Sphere[] lampObjects = new Sphere[4];
        PointLight[] pointLights;

        protected ShaderProgram objectShader;
        protected ShaderProgram lightingShader;
        protected ShaderProgram lightingColorShader;

        public Game(NativeWindowSettings settings) : base(GameWindowSettings.Default, settings)
        {
            CenterWindow();
            cameraManager = CameraManager.GetInstance();
            camera = cameraManager.GetCamera();
            CursorState = CursorState.Grabbed;

            objectShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/Shader.frag");
            lightingShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/Lighting.frag");
            lightingColorShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/LightingColor.frag");


        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f,0.2f,0.3f,1.0f);
            GL.Enable(EnableCap.DepthTest);
            IsVisible = true;

            PointLightManager.GetInstance().SetPointLightPosition(0,new Vector3(0,5,0));// 0. indexteki pointlighte değer verdik not render işlemlerinden önce yapılmalı

            pointLights = PointLightManager.GetInstance().GetPointLights();

            for (int i = 0; i < pointLights.Length; i++)
            {
                lampObjects[i] = new Sphere(pointLights[i].position, 0.2f, 15, new Vector3(0,1,0));
            }
       
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < pointLights.Length; i++)
            {

                lampObjects[i].RenderObject(camera, objectShader);
            }


        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;

            cameraManager.CamControl(input, MouseState, e);

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
            cameraManager.GetCamera().AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
