using SharpDX.DirectInput;
using System;
using System.IO.Ports;

namespace SerialRobotController
{
    internal class Program
    {
        private static SerialPort _serialPort;

        private static void Main(string[] args)
        {
            var port = SelectComPort(SerialPort.GetPortNames());
            if (port == null)
            {
                Console.WriteLine("No COM port found");
                return;
            }
            var directInput = new DirectInput();

            Guid joystickGuid = GetJoystickGuid(directInput);
            
            var keyboard = new Keyboard(directInput);
            //keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();
            var joystick = new Joystick(directInput, joystickGuid);
            //joystick.Properties.BufferSize = 128;
            joystick.Acquire();

            _serialPort = new SerialPort();

            _serialPort.PortName = port;
            _serialPort.BaudRate = 115200;
            _serialPort.RtsEnable = true;

            _serialPort.Open();
            DateTime lastKeyStroke = DateTime.MaxValue;

            while (true)
            {
                System.Threading.Thread.Sleep(50);

                joystick.Poll();
                var state = joystick.GetCurrentState();

                keyboard.Poll();
                var keyboardstate = keyboard.GetCurrentState();

                HidMessage message = new HidMessage();
                message.SetJoystickState(state);
                message.SetKeyboardState(keyboardstate);
                message.Send(_serialPort);

                while (_serialPort.BytesToRead > 0)
                {
                    int c = _serialPort.ReadChar();
                    //if (c < 2)
                    //    Console.Write(c);
                    //else if (c == 2)
                    //    Console.Write(' ');
                    //else if (c == 32)
                    //    Console.WriteLine();
                    Console.Write((char)c);
                }
            }
        }

        private static string SelectComPort(string[] comPorts)
        {
            if (comPorts.Length == 0)
                return null;
            if (comPorts.Length == 1)
                return comPorts[0];

            for (int i = 0; i < comPorts.Length; i++)
            {
                Console.WriteLine($"{i}: {comPorts[i]}");
            }
            char c = '\0';
            while(true)
            {
                var key = Console.ReadKey(true);

                Console.WriteLine(key.KeyChar);
                if (!char.IsDigit(key.KeyChar))
                    continue;

                int no = int.Parse(key.KeyChar.ToString());
                if (no >= comPorts.Length)
                    continue;
                return comPorts[no];
            }

            return comPorts[int.Parse(c.ToString())];
        }

        private static Guid GetJoystickGuid(DirectInput directInput)
        {
            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.FirstPerson, DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;
            return joystickGuid;
        }
    }
}
