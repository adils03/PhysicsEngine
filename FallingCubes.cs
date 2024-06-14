using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class FallingCubes : Game
    {

        Vector3[] _pointLightPositions =
        {
             new Vector3( -5.0f,  15.0f,  -15.0f),
             new Vector3( -5.0f,  15.0f,  15.0f),
             new Vector3( 5.0f,   15.0f,  15.0f),
             new Vector3( 5.0f,   15.0f,  -15.0f)
        };
        List<Shape> ShapeList;

        PhysicsWorld world;

        public FallingCubes(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            SetLamps();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            world = new PhysicsWorld(10f, new Vector3(1000, 1000, 1000), ResolveType.RotationAndFriction);

            cameraManager.SetCameraSpeed(10);
            GL.PointSize(30);
            RigidBody.CreateCubeBody(100, 1, 100, new Vector3(0, -10, 40), 1, true, 0f, Color4.AliceBlue, out RigidBody obstacle5);
            world.AddBody(obstacle5);
            // Grid boyutları ve küp özellikleri
            int gridSizeX = 1; // X eksenindeki küp sayısı
            int gridSizeY = 10; // Y eksenindeki küp sayısı
            int gridSizeZ = 15; // Z eksenindeki küp sayısı
            float cubeSize = 2.0f; // Küp boyutu (en, boy, yükseklik)
            float spacing = 5f; // Küpler arası mesafe

            // Grid şeklinde küpler oluşturan döngü
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    for (int z = 0; z < gridSizeZ; z++)
                    {
                        Vector3 position = new Vector3(
                            x * spacing,
                            y * spacing,
                            z * spacing
                        );

                        RigidBody.CreateCubeBody(
                            cubeSize, // Küp genişliği
                            cubeSize, // Küp yüksekliği
                            cubeSize, // Küp derinliği
                            position, // Küp pozisyonu
                            1f, // Kütle
                            false, // Dinamik mi (hareketli mi)
                            1f, // Sürtünme katsayısı
                            Color4.AliceBlue, // Renk
                            out RigidBody cube // Oluşturulan küpün referansı
                        );

                        world.AddBody(cube); // Küpü dünyaya ekle
                    }
                }
            }

        }
        int index = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            if( input.IsKeyPressed(Keys.Up))
            {
                RigidBody.CreateSphereBody(1,new Vector3(0,10,-20),1,false,1,Color4.DarkGray,out RigidBody ball);
                world.AddBody(ball);
                ball.linearVelocity = new Vector3(0,0,30);
            }

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
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            world.Update((float)e.Time, 2);
            RenderLamps();
            SwapBuffers();

        }


    }
}
