using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace PhysicsEngine
{
    public class Game : GameWindow
    {
        CameraHelper cameraHelper;
        protected Camera camera;
        bool isLineMod = false;


        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3( -5.0f,  10.0f,  -5.0f),
            new Vector3( -5.0f,  10.0f,  5.0f),
            new Vector3( 5.0f,   10.0f,  5.0f),
            new Vector3( 5.0f,   10.0f,  -5.0f)
        };

        protected PointLight[] pointLights = new PointLight[4];
        Sphere[] lampObjects = new Sphere[4];



        protected ShaderProgram objectShader;
        protected ShaderProgram lightingShader;
        protected ShaderProgram lightingColorShader;

        public Game(NativeWindowSettings settings) : base(GameWindowSettings.Default, settings)
        {
            CenterWindow();
            cameraHelper = new CameraHelper(Size);
            camera = cameraHelper.GetCamera();
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

            for (int i = 0; i < lampObjects.Length; i++)
            {
                lampObjects[i] = new Sphere(_pointLightPositions[i], 0.2f, 15, new Vector3(0,1,0));
            }
            // Aynı özelliklere sahip bir ışık oluşturma
            PointLight sharedLight = new PointLight()
            {
                position = new Vector3(0.0f, 0.0f, 5.0f),// bu veriler güncelleniyor
                constant = 0.1f,
                linear = 0.00009f,
                quadratic = 0.00032f,
                ambient = new Vector3(1.0f, 1.0f, 1.0f), // Ortam (ambient) bileşeni
                diffuse = new Vector3(1.0f, 1.0f, 1.0f), // Yayılma (diffuse) bileşeni
                specular = new Vector3(2.0f, 2.0f, 2.0f) // Parlaklık (specular) bileşeni
            };

            // 4 adet aynı özelliklere sahip ışık oluşturma
            for (int i = 0; i < pointLights.Length; i++)
            {
                pointLights[i] = sharedLight;
                pointLights[i].position = _pointLightPositions[i];
            }


        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //for (int i = 0; i < pointLights.Length; i++)
            //{

            //    lampObjects[i].RenderObject(camera, objectShader);
            //}


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
