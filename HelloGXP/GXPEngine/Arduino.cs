using GXPEngine;
using System;
using System.IO.Ports;
using System.Threading;

namespace System
{
    public class Arduino
    {

        public void ReadInput(SerialPort port)
        {
        string a = port.ReadExisting();

        if (a != "") Console.WriteLine("Read from port: " + a);

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                port.Write(key.KeyChar.ToString());
            }
        //System.Threading.Thread.Sleep(30);
        }
    }
}