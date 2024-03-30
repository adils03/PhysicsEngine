using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace PhysicsEngine
{
    public class Game : GameWindow
    {
        public static Game? Instance { get; private set; }
        private ShaderProgram shaderProgram; //Shader Program
        private Camera Camera;
        const float cameraSpeed = 10f;
        const float sensitivity = 0.2f;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        private Matrix4 view;
        private Matrix4 projection;
        private Matrix4 model;
        public List<Shape> models = [];
        public List<RigidBody> rigidBodies = [];
        DateTime lastTime;
        int frameCount;
        double fpsTimer;
        double fpsTimerMax = 1;
        private bool isLineMod = false;
        private PrimitiveType renderType = PrimitiveType.Triangles;
        /// //////////////////////////////////////////////////////////////////////////////////
        //Deneme bölümü
        Random random = new Random();
        Cube cube;
        Cube cube2;
        Vector3[] minkowskiSum;
        /// //////////////////////////////////////////////////////////////////////////////////////////////////

        public Game(int width = 1280, int height = 768, string title = "Game")
            : base(
                  GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Title = title,
                      Size = new Vector2i(width, height),
                      WindowBorder = WindowBorder.Resizable,
                      StartVisible = false,
                      StartFocused = true,
                      API = ContextAPI.OpenGL,
                      Profile = ContextProfile.Core,
                      APIVersion = new Version(3, 3),
                      Vsync = VSyncMode.Off
                  })
        {
            this.CenterWindow(new Vector2i(width, height));
            Instance = this;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.PointSize(10f);
            lastTime = DateTime.Now;
            GL.Enable(EnableCap.DepthTest);//Bakılacak
            this.IsVisible = true;

            GL.ClearColor(new Color4(0.1f, 0.2f, 0.3f, 1f));
            #region Model Definitions
            RigidBody rigidBody = null;

            //*****************************************
            for (int i = 0; i < 15; i++)
            {
                float x = random.Next(0, 25);
                float z = random.Next(0, 25);
                float r = (float)random.NextDouble();
                float g = (float)random.NextDouble();
                float b = (float)random.NextDouble();
                Color4 color = new Color4(r, g, b, 255);
                Vector3 pos = new Vector3(x, 0, z);
                RigidBody.CreateCircleBody(1, pos, 1, false, 1, out rigidBody, out string errorMessage);
                rigidBodies.Add(rigidBody);
                Sphere sphere = new Sphere(pos, 1, 10, color);
            }
            //********************************************
            //_ = new PointCloud(Color4.Orange, new Vector3());//origin
            //cube = new Cube(new Vector3(5, 5, 5), Color4.Red);
            //cube2 = new Cube(new Vector3(5, 6.3f, 5), Color4.Red);
            //cube2.Rotate(new Vector3(45, 0, 0));
            //if (Collisions.GJK(cube, cube2, out  List<Vector3> simplexPoints))
            //{
            //    cube.ChangeColor(Color4.Yellow);
            //}
            //else
            //{
            //    cube.ChangeColor(Color4.Red);
            //}
            //Line line1 = new Line(simplexPoints[0], simplexPoints[1],Color4.Red,Color4.Red);
            //Line line2= new Line(simplexPoints[1], simplexPoints[2],Color4.Blue,Color4.Blue);
            //Line line3 = new Line(simplexPoints[2], simplexPoints[0],Color4.Yellow,Color4.Yellow);
            //Line line4 = new Line(simplexPoints[2], simplexPoints[3], Color4.Yellow, Color4.Yellow);
            //Line line5 = new Line(simplexPoints[1], simplexPoints[3], Color4.Yellow, Color4.Yellow);
            //Line line6 = new Line(simplexPoints[3], simplexPoints[0], Color4.Yellow, Color4.Yellow);
            //*******************************************
            #endregion

            #region Shader
            string vertexShaderCode =
                @"
                #version 330 core
                layout (location=0) in vec3 aPosition;
                layout (location=1) in vec4 aColor; // Renk bilgisini aldık

                out vec4 vColor; // Fragment shader'a göndereceğimiz renk bilgisi
                uniform mat4 Model;//Bakılacak
                uniform mat4 View;//Bakılacak
                uniform mat4 Projection;//Bakılacak
                void main(){
                    
                    gl_Position = vec4(aPosition, 1.0) * Model * View * Projection; // Sonucu saklamak için gl_Position'a atama yaptık
        
                    vColor = aColor; // Aldığımız renk bilgisini fragment shader'a aktardık
                }
                ";

            string pixelShaderCode =
                @"
                #version 330 core

                in vec4 vColor; // Vertex shader'dan gelen renk bilgisi

                out vec4 pixelColor; // Son çıktı rengi

                void main(){
                    pixelColor = vColor; // Gelen rengi direkt olarak çıktıya atadık
                }
                 ";

            this.shaderProgram = new ShaderProgram(vertexShaderCode, pixelShaderCode);
            #endregion

            view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);
            Camera = new Camera(Vector3.UnitZ * 10 + Vector3.UnitY * 10, Size.X / (float)Size.Y / 2);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            shaderProgram.Dispose();
        }


        private float cubeSpeed = 5f;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            #region FPS
            frameCount++;
            if (DateTime.Now - lastTime >= TimeSpan.FromSeconds(1))
            {
                frameCount = 0;
                lastTime += TimeSpan.FromSeconds(1);
            }
            fpsTimer -= e.Time;
            if (fpsTimer < 0)
            {
                fpsTimer = fpsTimerMax;
                this.Title = $"FPS: {frameCount}";
            }
            #endregion
            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            //if (input.IsKeyDown(Keys.Up))
            //{
            //    cube.Translate(new Vector3(0, 0, -1) * (float)e.Time * cubeSpeed);
            //}
            //if (input.IsKeyDown(Keys.Left))
            //{
            //    cube.Translate(new Vector3(-1, 0, 0) * (float)e.Time * cubeSpeed);
            //}
            //if (input.IsKeyDown(Keys.Right))
            //{
            //    cube.Translate(new Vector3(+1, 0, 0) * (float)e.Time * cubeSpeed);
            //}
            //if (input.IsKeyDown(Keys.Down))
            //{
            //    cube.Translate(new Vector3(0, 0, +1) * (float)e.Time * cubeSpeed);

            //}
            //if (input.IsKeyDown(Keys.KeyPad2))
            //{
            //    cube.Translate(new Vector3(0, -1, 0) * (float)e.Time * cubeSpeed);

            //}
            //if (input.IsKeyDown(Keys.KeyPad8))
            //{
            //    cube.Translate(new Vector3(0, +1, 0) * (float)e.Time * cubeSpeed);

            //}


            //********************
            if (input.IsKeyDown(Keys.Up))
            {
                rigidBodies[0].Move(new Vector3(0, 0, -1) * (float)e.Time * 10);
            }
            if (input.IsKeyDown(Keys.Left))
            {
                rigidBodies[0].Move(new Vector3(-1, 0, 0) * (float)e.Time * 10);
            }
            if (input.IsKeyDown(Keys.Right))
            {
                rigidBodies[0].Move(new Vector3(+1, 0, 0) * (float)e.Time * 10);
            }
            if (input.IsKeyDown(Keys.Down))
            {
                rigidBodies[0].Move(new Vector3(0, 0, +1) * (float)e.Time * 10);

            }
            if (input.IsKeyDown(Keys.KeyPad8))
            {
                rigidBodies[0].Move(new Vector3(0, +1, 0) * (float)e.Time * 10);

            }
            if (input.IsKeyDown(Keys.KeyPad2))
            {
                rigidBodies[0].Move(new Vector3(0, -1, 0) * (float)e.Time * 10);

            }
            //***************************

            //***************************
            for (int i = 0; i < rigidBodies.Count; i++)
            {
                RigidBody rigidBody = rigidBodies[i];
                Vector3 position = rigidBody.position;

                models[i].Teleport(position);

            }
            //**************************
            //************************
            for (int i = 0; i < models.Count - 1; i++)
            {
                RigidBody a = rigidBodies[i];
                for (int j = i + 1; j < models.Count; j++)
                {
                    RigidBody b = rigidBodies[j];
                    if (Collisions.IntersectSpheres(a.position, a.radius, b.position, b.radius, out Vector3 normal, out float depth))
                    {
                        a.Move(normal * depth / 2f);
                        b.Move(-normal * depth / 2f);
                    }

                }
            }
            //***************************

            #region WireMode
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
            #endregion

            #region Camera 
            if (input.IsKeyDown(Keys.W))
            {
                Camera.Position += Camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                Camera.Position -= Camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                Camera.Position -= Camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                Camera.Position += Camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                Camera.Position += Camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                Camera.Position -= Camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                Camera.Yaw += deltaX * sensitivity;
                Camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
            #endregion
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(this.shaderProgram.ShaderProgramObject);
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Render(renderType);
            }

            #region Projection assignments
            model = Matrix4.Identity;
            this.shaderProgram.SetUniform("Model", model);//Bakılacak
            this.shaderProgram.SetUniform("View", Camera.GetViewMatrix());
            this.shaderProgram.SetUniform("Projection", Camera.GetProjectionMatrix());
            #endregion

            this.Context.SwapBuffers();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            Camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            Camera.AspectRatio = Size.X / (float)Size.Y;
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button == MouseButton.Right)
            {
                ToggleRenderType();
            }
        }
        public void ToggleRenderType()
        {
            switch (renderType)
            {
                case PrimitiveType.Triangles:
                    renderType = PrimitiveType.Lines;
                    break;
                case PrimitiveType.Lines:
                    renderType = PrimitiveType.Points;
                    break;
                case PrimitiveType.Points:
                    renderType = PrimitiveType.Triangles;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
