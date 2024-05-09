using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;




namespace PhysicsEngine
{
    public class PointLightFactory
    {
        private static readonly Vector3[] _pointLightPositions =
        {
            new Vector3( -5.0f,  10.0f,  -5.0f),
            new Vector3( -5.0f,  10.0f,  5.0f),
            new Vector3( 5.0f,   10.0f,  5.0f),
            new Vector3( 5.0f,   10.0f,  -5.0f)
        };

        private static bool initialized = false;

        private static PointLight[] pointLights = new PointLight[4];

        public static PointLight[] GetPointLights() 
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }
            return pointLights;
        }
        private static void Initialize() 
        {
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
    }
}
