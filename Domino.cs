using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class Domino : Game
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
        RigidBody cube1;

        public Domino(NativeWindowSettings settings) : base(settings)
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
            RigidBody.CreateCubeBody(2, 2, 2, new Vector3(0, 100, 0), 1, false, .3f, Color4.Red, out cube1);
            world.AddBody(cube1);
            world.AddBody(obstacle5);
            // Grid boyutları ve küp özellikleri
            int gridSizeX = 1; // X eksenindeki küp sayısı
            int gridSizeY = 1; // Y eksenindeki küp sayısı
            int gridSizeZ = 15; // Z eksenindeki küp sayısı
            float cubeSize = 1f; // Küp boyutu (en, boy, yükseklik)
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
                            cubeSize*z/1.3f, // Küp genişliği
                            cubeSize*z*2, // Küp yüksekliği
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
            float dx = 0;
            float dy = 0;
            float dz = 0;
            float forceMagnitude = 150;
            if (input.IsKeyDown(Keys.Up))
            {
                dz--;

            }
            if (input.IsKeyDown(Keys.Left))
            {
                dx--;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                dx++;
            }
            if (input.IsKeyDown(Keys.Down))
            {
                dz++;

            }
            if (input.IsKeyDown(Keys.KeyPad2))
            {
                dy--;

            }
            if (input.IsKeyDown(Keys.KeyPad8))
            {
                dy++;

            }
            if (input.IsKeyDown(Keys.R))
            {
                Vector3 rotateVector = new Vector3(90, 0, 0) * (float)e.Time;

                cube1.Rotate(rotateVector);

            }
            if (input.IsKeyDown(Keys.Q))
            {
                Vector3 rotateVector = new Vector3(0, 0, 90) * (float)e.Time;

                cube1.Rotate(rotateVector);

            }
            if (dx != 0 || dy != 0 || dz != 0)
            {
                Vector3 forceDirection = Vector3.Normalize(new Vector3(dx, dy, dz));
                Vector3 force = forceDirection * forceMagnitude;
                cube1.AddForce(force);
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
