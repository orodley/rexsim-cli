using System;
using System.Collections.Generic;

namespace RexSimulatorCLI.Panels
{
    public delegate void InputRecievedHandler(ConsoleKeyInfo info);

    public abstract class Panel
    {
        public string Name;

        public List<char[]> BufferedData;
        public int CursorLeft = 0, CursorTop = 0;

        public event InputRecievedHandler InputRecieved;

        protected Panel()
        {
            BufferedData = new List<char[]>();
            SetUpBuffer();
        }

        public void SetUpBuffer()
        {
            //Creates empty buffer of console window size
            for (var i = 0; i < Console.WindowHeight; i++)
            {
                BufferedData.Add(new char[Console.WindowWidth]);
            }
        }

        public void SetCursorPosition(int left, int top)
        {
            CursorLeft = left;
            CursorTop = top;
        }

        public void Clear()
        {
            SetUpBuffer();
            if (Program.PanelManager.ActivePanel == this)
            {
                Console.Clear();
            }
        }

        public void SendInput(ConsoleKeyInfo info)
        {
            //Do a check to see if handlers have been attached to event
            if (InputRecieved != null)
            {
                //Fire event
                InputRecieved(info);
            }
        }

        public void Write(object o)
        {
            foreach (var c in o.ToString())
            {
                //Check if this is a new line
                if (c == '\n')
                {
                    //Are we the active panel?
                    if (Program.PanelManager.ActivePanel == this)
                    {
                        Console.SetCursorPosition(CursorLeft, CursorTop + 1);
                        Console.Write('\n');
                    }
                    //Increment cursor position
                    CursorTop++;
                    //Check to see if we are at the end of the buffer
                    if (CursorTop == Console.WindowHeight - 1)
                    {
                        CursorLeft = 0;
                        CursorTop--;
                        //move buffer up 1
                        BufferedData.RemoveAt(0);
                        BufferedData.Add(new char[Console.WindowWidth]);
                    }
                    CursorLeft = 0;
                    continue;
                }

                if (Program.PanelManager.ActivePanel == this && BufferedData[CursorTop][CursorLeft] != c)
                {
                    Console.SetCursorPosition(CursorLeft, CursorTop);
                    Console.Write(c);
                }
                BufferedData[CursorTop][CursorLeft] = c;
                CursorLeft++;
                if (CursorLeft == Console.WindowWidth - 1)
                {
                    CursorLeft = 0;
                    CursorTop++;
                }
            }
        }

        public void SendBufferToStdout()
        {
            Console.Clear();
            //Loop through buffer
            for (int top = 0; top < BufferedData.Count; top++)
            {
                for (int left = 0; left < BufferedData[top].Length; left++)
                {
                    //discard any empty values
                    if (BufferedData[top][left] == '\0' || BufferedData[top][left] == ' ')
                        continue;
                    Console.SetCursorPosition(left, top);
                    Console.Write(BufferedData[top][left]);
                }
            }
            Console.SetCursorPosition(CursorLeft, CursorTop);
        }
    }
}
