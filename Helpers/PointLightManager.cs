using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;




namespace PhysicsEngine
{

    //Singleton Design Pattern yaklaşımı kullanıldı
    public class PointLightManager
    {
        private static PointLightManager instance;

        private static readonly object lockObject = new object();// kilit 

        private PointLight[] pointLights = new PointLight[4];
        private PointLightManager()
        {
            Initialize();
        }
        public static PointLightManager GetInstance()
        {
            // Birden çok thread tarafından eş zamanlı erişimi engellemek için kilit mekanizması kullanılır
            lock (lockObject)
            {
                // Eğer örnek henüz oluşturulmadıysa oluşturulur
                if (instance == null)
                {
                    instance = new PointLightManager();

                }
                return instance;
            }
        }
        public PointLight[] GetPointLights()
        {
            return pointLights;
        }
        public void SetPointLightPosition(int index, Vector3 position)
        {
            if (index >= 0 && index < pointLights.Length)
            {
                pointLights[index].position = position;

            }
            else
            {              
                throw new Exception("Geçersiz endeks!");
            }
        }
        public void SetPointLightPosition(Vector3[] positions)
        {
            if (positions.Length == pointLights.Length)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    pointLights[i].position = positions[i];
                }

            }
            else
            {
                throw new Exception("Fazla veya Eksik nokta bilgisi pointLight sayısını kontrol et");
             
            }
        }
        private void Initialize()
        {

            Vector3[] _pointLightPositions =
            {
                    new Vector3( -5.0f,  10.0f,  -5.0f),
                    new Vector3( -5.0f,  10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  5.0f),
                    new Vector3( 5.0f,   10.0f,  -5.0f)
            };


            // Aynı özelliklere sahip bir ışık oluşturma
            PointLight sharedLight = new PointLight()
            {
                position = new Vector3(0.0f, 0.0f, 5.0f),// bu veriler güncelleniyor
                constant = 1f,
                linear = 0.09f,
                quadratic = 0.032f,
                ambient = new Vector3(0.02f), // Ortam (ambient) bileşeni
                diffuse = new Vector3(1f), // Yayılma (diffuse) bileşeni
                specular = new Vector3(2f) // Parlaklık (specular) bileşeni
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
