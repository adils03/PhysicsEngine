using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;


namespace PhysicsEngine
{
    public class CameraManager
    {
        private readonly Camera _camera;

        private static CameraManager instance;

        private static readonly object lockObject = new object();

        private bool _firstMove = true;
        private Vector2 _lastPos;

        private float cameraSpeed = 1f;


        public static CameraManager GetInstance()
        {
            lock (lockObject)
            {
                // Eğer örnek henüz oluşturulmadıysa oluşturulur
                if (instance == null)
                {
                    instance = new CameraManager();
                }
                return instance;
            }
        }
        private CameraManager()
        {
             Vector2i Size = new Vector2i(1280, 768);
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
        }      
        public void CamControl(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            const float sensitivity = 0.17f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state


            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }
        public Camera GetCamera()
        {
            return _camera;
        }

        public void SetCameraSpeed(float speed)
        {
            cameraSpeed = speed;
        }
    }
}
