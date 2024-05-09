using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    public sealed class ShaderFactory
    {
        private static ShaderProgram lampShader;
        private static ShaderProgram lightingShader;
        private static ShaderProgram lightingColorShader;
        private static bool initialized = false;
        private static object lockObject = new object();
        private ShaderFactory()
        {
            // Private constructor to prevent instantiation
           
        }
        public static ShaderProgram GetLampShader()
        {
            Initialize();
            return lampShader;
        }
        public static ShaderProgram GetLightingShader()
        {
            Initialize();
            return lightingShader;
        }
        public static ShaderProgram GetLightingColorShader()
        {
            Initialize();
            return lightingColorShader;
        }
        private static void Initialize()
        {
            lock (lockObject)
            {
                if (!initialized)
                {
                    lampShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/Shader.frag");
                    lightingShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/Lighting.frag");
                    lightingColorShader = new ShaderProgram("Shaders/Shader.vert", "Shaders/LightingColor.frag");
                    initialized = true;
                }
            }
        }
    }

}
