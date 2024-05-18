using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace PhysicsEngine
{
    class Program
    {

        static void Main()
        {
            NativeWindowSettings settings = new NativeWindowSettings()
            {
                Title = "PlayGround",
                Size = new Vector2i(1280,768),
                WindowBorder = WindowBorder.Fixed,
                StartVisible = false,
            };


            Console.WriteLine("sea");
            using PlayGround game = new(settings);
            game.Run();

        }


    }
}
