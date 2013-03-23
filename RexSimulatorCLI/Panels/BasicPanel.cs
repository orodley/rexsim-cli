using System;

namespace RexSimulatorCLI.Panels
{
    public class BasicPanel : Panel
    {
        public BasicPanel()
        {
            InputRecieved += BasicPanel_InputRecieved;
        }

        void BasicPanel_InputRecieved(ConsoleKeyInfo info)
        {
            if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                switch (info.Key)
                {
                    //Fiddling around...
                    case ConsoleKey.D1: Console.Beep(100, 500); break;
                    case ConsoleKey.D2: Console.Beep(200, 500); break;
                    case ConsoleKey.D3: Console.Beep(300, 500); break;
                    case ConsoleKey.D4: Console.Beep(400, 500); break;
                    case ConsoleKey.D5: Console.Beep(500, 500); break;
                    case ConsoleKey.D6: Console.Beep(600, 500); break;
                    case ConsoleKey.D7: Console.Beep(700, 500); break;
                    case ConsoleKey.D8: Console.Beep(800, 500); break;
                    case ConsoleKey.D9: Console.Beep(900, 500); break;
                    case ConsoleKey.D0: Console.Beep(1000, 500); break;
                }
            }
            else
                Write(info.KeyChar);
        }
    }
}
