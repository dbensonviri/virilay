using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using Viri.Lieutenants;
using System.Threading.Tasks;
using System.Windows.Forms;
public class TwitchPlays
{

 
        IntPtr _game;
        bool _amrunning { get { return _game == IntPtr.Zero; } }

        public void Soup()
        {
            _game = FindWindow("SDL_app", "Dungeon Crawl Stone Soup 0.15.1");

            Console.WriteLine("Done: " + _game.ToString());
        }

        [ViewerPermitted]
        public async void Do(string command)

        {
            SetForegroundWindow(_game);
            await Task.Delay(150);
            SendKeys.SendWait(command);
        }


        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    
} 