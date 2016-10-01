using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;

namespace UruchamianiePokazuSlajdow
{
    class Program
    {
        static void Main(string[] args)
        {
            // cmd -> cd -> "ChangeInputLanguage.exe start_offest close_offset directory_opening_offset between_keys_offset nazwa_folderu_na_pulpicie"

            Thread.Sleep(int.Parse(args[0]));

            string slidesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), args[4]);

            System.Diagnostics.Process.Start(slidesDir);

            // otwieranie folderu
            Thread.Sleep(int.Parse(args[2]));

            InputSimulator.SimulateKeyDown(VirtualKeyCode.MENU);
            Thread.Sleep(int.Parse(args[3]));
            InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_J);
            Thread.Sleep(int.Parse(args[3]));
            InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_P);
            Thread.Sleep(int.Parse(args[3]));
            InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_S);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.MENU);

            Thread.Sleep(int.Parse(args[1]));
        }
    }
}
