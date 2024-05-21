using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class PlayGround : Game
    {

        List<Shape> ShapeList;

        Cube cube;
        Sphere sphere;
        Sphere sphere2;
        Sphere sphere3;
        Sphere sphere4;
        Cube cubeRed;
        PhysicsWorld world;

        RigidBody ball1;
        RigidBody ball2;
        RigidBody cube1;
        RigidBody cube2;
        RigidBody obstacle5;

        public PlayGround(NativeWindowSettings settings) : base(settings)
        {
            ShapeList = new List<Shape>();
            sphere = new Sphere(Vector3.Zero, 0.7f, 10, Color4.White);
            sphere2 = new Sphere(Vector3.Zero, 0.3f, 10, Color4.White);
            sphere3 = new Sphere(Vector3.Zero, 0.3f, 10, Color4.White);
            sphere4 = new Sphere(Vector3.Zero, 0.3f, 10, Color4.White);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            world = new PhysicsWorld(PhysicsWorld.WORLD_GRAVITY, new Vector3(100, 100, 100));

            cameraManager.SetCameraSpeed(10);
            GL.PointSize(30);
            //RigidBody.CreateCubeBody(3, 10, 3, new Vector3(10, 10, 0), 1, true, 1, Color4.AliceBlue, out RigidBody obstacle1);
            //RigidBody.CreateCubeBody(3, 1, 3, new Vector3(0, 0, 0), 1, true, 1, Color4.AliceBlue, out RigidBody obstacle2);
            //RigidBody.CreateCubeBody(1, 1, 1, new Vector3(0, -10, 5), 1, true, 1, Color4.AliceBlue, out RigidBody obstacle3);
            //RigidBody.CreateCubeBody(1, 1, 1, new Vector3(10f, 0, 10f), 1, false, 0.5f, Color4.AliceBlue, out RigidBody obstacle4);
            RigidBody.CreateCubeBody(25, 25, 25, new Vector3(2, -10,2), 1, true, 0f, Color4.AliceBlue, out obstacle5);
            //RigidBody.CreateCubeBody(100, 1, 5, new Vector3(0, -7.3f, 0), 1, true, 0.5f, Color4.AliceBlue, out RigidBody obstacle6);
            //obstacle6.Rotate(new Vector3(90, 0, 0));
            RigidBody.CreateSphereBody(2, new Vector3(2, 15, 2), 1, false, .1f, Color4.Red, out ball1);
            RigidBody.CreateCubeBody(10, 1, 10, new Vector3(2, 15, 2), 1, false, .0f, Color4.Red, out cube1);
            RigidBody.CreateCubeBody(8, 5, 1, new Vector3(8f, 10, 10), 1, false, .7f, Color4.Red, out cube2);
            //RigidBody.CreateSphereBody(1, new Vector3(8, 10, 0), 1, false, .7f, Color4.Green, out ball2);
            //RigidBody.CreateSphereBody(1, new Vector3(8, 12, 0), 1, false, .7f, Color4.Yellow, out RigidBody ball3);
            //RigidBody.CreateSphereBody(1, new Vector3(1, 14, 0), 1, false, .7f, Color4.Blue, out RigidBody ball4);
            //RigidBody.CreateCubeBody(1, 1, 1, new Vector3(7, 16, 0), 1, false, .7f, Color4.AliceBlue, out RigidBody ball5);
            //RigidBody.CreateCubeBody(1, 1, 1, new Vector3(9, 16, 0), 1, false, .7f, Color4.AliceBlue, out RigidBody ball6);
            //obstacle1.Rotate(new Vector3(0, 0, 135));
            //obstacle2.Rotate(new Vector3(0, 0, 45));
            //world.AddBody(ball1);
            ////world.AddBody(ball2);
            world.AddBody(cube1);
            //world.AddBody(cube2);
            ////world.AddBody(ball3);
            ////world.AddBody(ball4);
            ////world.AddBody(ball5);
            ////world.AddBody(ball6);
            ////world.AddBody(obstacle1);
            ////world.AddBody(obstacle2);
            ////world.AddBody(obstacle3);
            ////world.AddBody(obstacle4);
            ////obstacle5.Rotate(new Vector3(10, 0, 0));
            world.AddBody(obstacle5);
            //world.AddBody(obstacle6);
            // Grid boyutları ve küp özellikleri
            int gridSizeX = 5; // X eksenindeki küp sayısı
            int gridSizeY = 3; // Y eksenindeki küp sayısı
            int gridSizeZ = 5; // Z eksenindeki küp sayısı
            float cubeSize = 1.0f; // Küp boyutu (en, boy, yükseklik)
            float spacing = 1.5f; // Küpler arası mesafe

            // Grid şeklinde küpler oluşturan döngü
            /*for (int x = 0; x < gridSizeX; x++)
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
                            1, // Kütle
                            false, // Dinamik mi (hareketli mi)
                            0.5f, // Sürtünme katsayısı
                            Color4.AliceBlue, // Renk
                            out RigidBody cube // Oluşturulan küpün referansı
                        );

                        world.AddBody(cube); // Küpü dünyaya ekle
                    }
                }
            }*/

        }
        int index = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            //Console.WriteLine(ball5.linearVelocity + " linear velocity");
            //ShapeController.ControllerA(_input, cube, e);
            //Console.WriteLine(cube1.linearVelocity);
            float dx = 0;
            float dy = 0;
            float dz = 0;
            float forceMagnitude = 50;
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
            if (input.IsKeyPressed(Keys.KeyPad8))
            {
                dy++;

            }
            if (input.IsKeyPressed(Keys.R))
            {
                //Vector3 rotateVector = Vector3.Cross (contact1 - cube1.position,contact1);

                //cube1.Rotate(rotateVector);

            }
            if (dx != 0 || dy != 0 || dz != 0)
            {
                Vector3 forceDirection = Vector3.Normalize(new Vector3(dx, dy, dz));
                Vector3 force = forceDirection * forceMagnitude;
                cube1.AddForce(force);
            }

            
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Collisions.FindContactPoints(cube1, obstacle5, out Vector3 contact1, out Vector3 contact2, out Vector3 contact3, out Vector3 contact4, out int contactCount);

            Console.WriteLine(contactCount);

            sphere.Teleport(contact1);
            sphere2.Teleport(contact2);
            sphere3.Teleport(contact3);
            sphere4.Teleport(contact4);
            base.OnRenderFrame(e);
            world.Update((float)e.Time, 1);

            sphere.RenderBasic();
            sphere2.RenderBasic();
            sphere3.RenderBasic();
            sphere4.RenderBasic();
            SwapBuffers();
        }


    }
}
