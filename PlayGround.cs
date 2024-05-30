using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class PlayGround : Game
    {
        Vector3[] _pointLightPositions =
        {
                    new Vector3( -5.0f,  10.0f,  -5.0f),
                    new Vector3( -5.0f,  10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  -5.0f)
        };

        PhysicsWorld world;



        public PlayGround(NativeWindowSettings settings) : base(settings)
        {

            world = new PhysicsWorld(PhysicsWorld.WORLD_GRAVITY, new Vector3(100, 100, 100));
            SetLamps();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            cameraManager.SetCameraSpeed(10);


            GL.PointSize(10);

            Platform platform = new Platform(new Vector3(0, 0, 0));
            RigidBody.CreateCubeBody(platform, true, 0.7f, out RigidBody platformRigidBody);
            world.AddBody(platformRigidBody);

            //Cube a = new Cube(new Vector3(0,10,0));
            //a.ShaderType = ShapeShaderType.Textured;
            //a.Color = Color4.Black;


            //RigidBody.CreateCubeBody(a, false , 0.7f, out RigidBody cubeRBody);
            //world.AddBody(cubeRBody);


            // Grid boyutları ve küp özellikleri
            int gridSizeX = 3; // X eksenindeki küp sayısı
            int gridSizeY = 3; // Y eksenindeki küp sayısı
            int gridSizeZ = 3; // Z eksenindeki küp sayısı
            float cubeSize = 2f; // Küp boyutu (en, boy, yükseklik)
            float spacing = 2.5f; // Küpler arası mesafe

            // Grid şeklinde küpler oluşturan döngü
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    for (int z = 0; z < gridSizeZ; z++)
                    {
                        Vector3 position = new Vector3(
                            x * spacing * 2,
                            y * spacing * 2,
                            z * spacing * 2
                        );

                        RigidBody.CreateCubeBody(
                            cubeSize, // Küp genişliği
                            cubeSize, // Küp yüksekliği
                            cubeSize, // Küp derinliği
                            position, // Küp pozisyonu
                            0.7f, // Kütle
                            false, // Dinamik mi (hareketli mi)
                            0.7f, // Sürtünme katsayısı
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
           
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            RenderLamps();
            world.Update((float)e.Time, 1);



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

    }
}
