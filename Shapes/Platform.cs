using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;


namespace PhysicsEngine
{
    public class Platform : Cube
    {
        public Platform(Vector3 position, string texturePath1 = "Resources/zemin1.png", string path2 = "Resources/green.png") : 
            base(position, new Vector3(10,2,10), texturePath1,path2,ShapeShaderType.Textured)
        {
          
           
        }

      
    }
}
