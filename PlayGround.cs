using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhysicsEngine
{
    public class PlayGround : Game
    {

        PointLight[] pointLights = new PointLight[4];

        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3( 5.0f,  0.0f,  0.0f),
            new Vector3( 2.3f, -3.3f,  -4.0f),
            new Vector3(-4.0f,  2.0f, -6.0f),
            new Vector3( 0.0f, 12.0f,  -3.0f)
        };

        Sphere[] lampObjects = new Sphere[4];

        Cube cube;
        Cube lamp;

        Sphere sphere;
        public PlayGround(NativeWindowSettings settings) : base(settings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            cube = new Cube();
            sphere = new Sphere(Vector3.Zero, 1, 15, Color4.Red);

            for (int i = 0; i < lampObjects.Length; i++)
            {
                lampObjects[i] = new Sphere(_pointLightPositions[i], 0.2f , 15, Color4.Red);
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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var _input = KeyboardState;
            if(_input.IsKeyDown(Keys.Q))
            {
                cube.Teleport(new Vector3(3, 3,3));
                //cube.Rotate(new Vector3(190,190,190) * (float)e.Time);
            }
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            cube.RenderLighting(pointLights, cam,lightingShader);

            for (int i = 0; i < pointLights.Length; i++)
            {
                lampObjects[i].RenderObject(cam, objectShader);
            }

            sphere.RenderLighting(pointLights, cam, lightingShader);
            SwapBuffers();
        }
    }
}
