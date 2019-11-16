using System;
using System.Linq;
using System.IO.Ports;
using SharpDX.DirectInput;

namespace SerialRobotController
{
    public class HidMessage
    {
        //
        // 10000000 10000000 10000000 10000000 10000000 10000000 10000000 10000000
        // byte0    byte1    byte2    byte3    byte4    byte5    byte6    byte7
        // Modifier Arrows   ???      ???      Analog0X Analog0Y Analog1X Analog1Y
        //

        /// <summary>
        /// 128 = correct message
        /// 64 = ?
        /// 32 = ?
        /// 16 = ?
        /// 8 = ?
        /// 4 = alt
        /// 2 = shift
        /// 1 = ctrl
        /// </summary>
        private byte byte0 = 128;

        /// <summary>
        /// 128 = ?
        /// 64 = ?
        /// 32 = ?
        /// 16 = ?
        /// 8 = up
        /// 4 = down
        /// 2 = left
        /// 1 = right
        /// </summary>
        private byte byte1 = 128;
        private byte byte2 = 128;
        private byte byte3 = 128;
        private byte byte4 = 128;
        private byte byte5 = 128;
        private byte byte6 = 128;
        private byte byte7 = 128;

        public HidMessage()
        {

        }
        public HidMessage(ConsoleKeyInfo key)
        {
            SetConsoleKey(key);
        }

        public void SetConsoleKey(ConsoleKeyInfo key)
        {
            byte0 = 128;
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                byte0 |= 1;
            }
            if (key.Modifiers == ConsoleModifiers.Shift)
            {
                byte0 |= 2;
            }
            if (key.Modifiers == ConsoleModifiers.Alt)
            {
                byte0 |= 4;
            }

            /// arrows
            byte1 = 128;
            if (key.Key == ConsoleKey.UpArrow)
            {
                byte1 |= 8;
            }
            if (key.Key == ConsoleKey.DownArrow)
            {
                byte1 |= 4;
            }
            if (key.Key == ConsoleKey.LeftArrow)
            {
                byte1 |= 2;
            }
            if (key.Key == ConsoleKey.RightArrow)
            {
                byte1 |= 1;
            }
        }

        internal void SetKeyboardState(KeyboardState state)
        {
            if (state.PressedKeys.Exists(k => k == Key.Right))
                byte1 |= 1;
            if (state.PressedKeys.Exists(k => k == Key.Left))
                byte1 |= 2;
            if (state.PressedKeys.Exists(k => k == Key.Down))
                byte1 |= 4;
            if (state.PressedKeys.Exists(k => k == Key.Up))
                byte1 |= 8;

            if (state.PressedKeys.Exists(k => k == Key.RightControl))
                byte0 |= 1;
            if (state.PressedKeys.Exists(k => k == Key.LeftControl))
                byte0 |= 1;

            if (state.PressedKeys.Exists(k => k == Key.LeftShift))
                byte0 |= 2;
            if (state.PressedKeys.Exists(k => k == Key.RightShift))
                byte0 |= 2;

            if (state.PressedKeys.Exists(k => k == Key.LeftAlt))
                byte0 |= 4;
            if (state.PressedKeys.Exists(k => k == Key.RightAlt))
                byte0 |= 4;
        }

        internal void SetJoystickState(JoystickState state)
        {
            if(state.X > 45000)
                byte1 |= 1;
            if (state.X < 25000)
                byte1 |= 2;
            if (state.Y > 45000)
                byte1 |= 4;
            if (state.Y < 25000)
                byte1 |= 8;
            if (state.Buttons[3])
                byte0 |= 1;
            if (state.Buttons[2])
                byte0 |= 2;

            byte4 += (byte)(state.X / 512);
            byte5 += (byte)(state.Y / 512);

            byte6 += (byte)(state.RotationX / 512);
            byte7 += (byte)(state.RotationY / 512);

            //Console.Write("XY:");
            //Console.Write(Convert.ToString((byte)(state.X / 512), 2).PadLeft(8, '0'));
            //Console.Write(" ");
            //Console.Write(Convert.ToString((byte)(state.Y / 512), 2).PadLeft(8, '0'));

            //Console.Write("   Rotation:");
            //Console.Write(Convert.ToString((byte)(state.RotationX / 512), 2).PadLeft(8, '0'));
            //Console.Write(" ");
            //Console.Write(Convert.ToString((byte)(state.RotationY / 512), 2).PadLeft(8, '0'));
            //Console.WriteLine();
        }

        public void Send(SerialPort serialPort)
        {
            serialPort.Write(new byte[] { byte0, byte1, byte2, byte3, byte4, byte5, byte6, byte7, 0 }, 0, 9);
        }
    }
}