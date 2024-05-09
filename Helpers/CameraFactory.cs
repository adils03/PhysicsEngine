using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace PhysicsEngine
{
    public sealed class CameraFactory
    {
        private static Camera _camera;
        public static Camera GetCam()
        {
            if(_camera == null)
            {
                 Vector2i Size = new Vector2i(1280, 768);
                _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            }
            return _camera;
        }
        private CameraFactory()
        {
            
        }
    }
}
