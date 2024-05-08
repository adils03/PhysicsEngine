using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public class ShapeController
    {
        public static void ControllerA(KeyboardState _input, Shape shape, FrameEventArgs e)
        {
            float dx = 0;
            float dy = 0;
            float dz = 0;

            if (_input.IsKeyDown(Keys.Up))
            {
                dz--;

            }
            if (_input.IsKeyDown(Keys.Left))
            {
                dx--;
            }
            if (_input.IsKeyDown(Keys.Right))
            {
                dx++;
            }
            if (_input.IsKeyDown(Keys.Down))
            {
                dz++;

            }
            if (_input.IsKeyDown(Keys.KeyPad2))
            {
                dy--;

            }
            if (_input.IsKeyDown(Keys.KeyPad8))
            {
                dy++;

            }
            if (_input.IsKeyPressed(Keys.R))
            {
                shape.Rotate(new Vector3(45, 0, 0));
            }
            shape.Translate(new Vector3(dx, dy, dz) * (float)e.Time);
        }
        public static void ControllerB(KeyboardState _input, Shape shape, FrameEventArgs e)
        {
            float _dx = 0;
            float _dy = 0;
            float _dz = 0;
            if (_input.IsKeyDown(Keys.I))
            {
                _dz--;

            }
            if (_input.IsKeyDown(Keys.J))
            {
                _dx--;
            }
            if (_input.IsKeyDown(Keys.L))
            {
                _dx++;
            }
            if (_input.IsKeyDown(Keys.K))
            {
                _dz++;

            }
            if (_input.IsKeyDown(Keys.U))
            {
                _dy--;

            }
            if (_input.IsKeyDown(Keys.O))
            {
                _dy++;

            }
            shape.Translate(new Vector3(_dx, _dy, _dz) * (float)e.Time);
        }
    }
}
