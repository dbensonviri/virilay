using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using Viri.Lieutenants;
using System.Threading.Tasks;
using System.Windows.Forms;
public class TwitchPlays
{


    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
    const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
    const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    const uint MOUSEEVENTF_LEFTUP = 0x0004;
    const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
    const uint MOUSEEVENTF_MOVE = 0x0001;
    const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    const uint MOUSEEVENTF_XDOWN = 0x0080;
    const uint MOUSEEVENTF_XUP = 0x0100;
    const uint MOUSEEVENTF_WHEEL = 0x0800;
    const uint MOUSEEVENTF_HWHEEL = 0x01000;

    [DllImport("user32.dll")]
    static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags,
       UIntPtr dwExtraInfo);

    const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    const uint KEYEVENTF_KEYUP = 0x0002;

    Dictionary<char, byte> lettertobyte = new Dictionary<char, byte>();

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    public TwitchPlays()
    {
        char c = 'a';
        for (byte b = (byte)0x41; b <= (byte)0x5A; b++)
        {
            lettertobyte.Add(c, b);
            c = (char)((byte)c + 1);
            //Console.WriteLine("added " + (char)b);
        }

    }
    IntPtr _game;
    bool _amrunning { get { return _game == IntPtr.Zero; } }

    public void Play(string game)
    {
        Play(game, 1);
    }






    public async void RightClick()
    {


        mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, UIntPtr.Zero);
        System.Threading.Thread.Sleep(500);

        mouse_event(MOUSEEVENTF_RIGHTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, UIntPtr.Zero);
        System.Threading.Thread.Sleep(500);

    }


    public async void Play(string game, int index)
    {
        index--;

        string s = "";
        game = game.ToLower();
        IntPtr hWnd = IntPtr.Zero;
        foreach (Process pList in Process.GetProcesses())
        {
            if (pList.MainWindowTitle.ToLower().IndexOf(game) != -1)
            {
                index--;
                if (index > 0) continue;
                _game = pList.MainWindowHandle;
                Console.WriteLine("Got " + pList.MainWindowTitle);
                return;
            }
            s += "|" + pList.MainWindowTitle;
        }
        Console.WriteLine("No Window. " + s);

        SetForegroundWindow(_game);
        await Task.Delay(50);

        keybd_event(0x10, 0x45, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);

        await Task.Delay(50);
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);// IntPtr.Zero);
        await Task.Delay(50);


        keybd_event(0x10, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);// IntPtr.Zero);
        await Task.Delay(50);

    }

    [ViewerPermitted]
    public void Type(string command)
    {
        //ShowWindowAsync(_game, 1);
        IntPtr ptr = GetForegroundWindow();
        SetForegroundWindow(_game);
        SendKeys.Send(command);
        SetForegroundWindow(ptr);
        // ShowWindowAsync(_game, 2); //1 is normal. 2 is miniimzed. 3 is max
    }

    //for use with Do()
    //eg.
    //Input: "lmb.3000.12300"
    //Outputs: 
    //  new Input: 3000.12300
    //  parsedword: lmb
    private bool ParseCommand(ref string input, out string parsedword)
    {
        if (string.IsNullOrWhiteSpace(input)) { parsedword = "Failed"; return false; }
        int indexofdot = input.IndexOf('.');
        int indexofpipe = input.IndexOf('|');
        indexofdot = Math.Min(indexofdot, indexofpipe);
        if (indexofdot == -1)
            indexofdot = input.Length;
        parsedword = input.Substring(0, indexofdot);
        input = input.Substring(indexofdot + 1);
        return true;
    }

    /*[Help("Emulates commands. eg. 'lmb.3000|wait.2000' denotes we hold lmb for 3 second, and then we wait for 2 seconds.")]
    [ViewerPermitted]
    public async void Do(string command)
    {

        if (_game == IntPtr.Zero) return;
        SetForegroundWindow(_game);
        string[] commandsplit = command.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries);
        string nextcommand="";
        int time;
        foreach (string executing2 in commandsplit) //eg. "lmb.3000|wait.1000"
        {
            string executing=executing2;
            while (ParseCommand(ref executing, out nextcommand))
            {
                switch (nextcommand)
                {
                    case "wait": //wait.{time}
                        if (ParseCommand(ref executing,out nextcommand))
                            if (int.TryParse(nextcommand,out time))
                                await Task.Delay(time);
                        break;
                    case "lmb": //lmb.{time}
                        if (ParseCommand(ref executing,out nextcommand))
                            if (int.TryParse(nextcommand, out time))
                            {
                                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);// IntPtr.Zero);
                                await Task.Delay(10);
                                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);// IntPtr.Zero);
                            }
                        break;
                    case "rmb": //rmb.{time}
                        if (ParseCommand(ref executing,out nextcommand))
                            if (int.TryParse(nextcommand, out time))
                            {
                                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);// IntPtr.Zero);
                                await Task.Delay(time);
                                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);// IntPtr.Zero);
                            }
                        break;
                    case "type": //type.{charstring}
                        if (ParseCommand(ref executing, out nextcommand))
                        {
                            foreach (char c in nextcommand)
                            {
                                try
                                {
                                    keybd_event(lettertobyte[c], 0x45, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                                    await Task.Delay(10);
                                    keybd_event(lettertobyte[c], 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
                                }
                                catch { }
                            }
                        }
                        break;
                    default: //{key}.{time}

                        if (nextcommand.Length == 1)
                        {
                            char c = nextcommand[0];
                            if (ParseCommand(ref executing, out nextcommand))
                                if (int.TryParse(nextcommand, out time))
                                {
                                    keybd_event(lettertobyte[c], 0x45, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                                    await Task.Delay(10);
                                    keybd_event(lettertobyte[c], 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
                                }
                            
                        }
                        break;

                }
            }
            
            await Task.Delay(50);
        }
        //
    }*/


    [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpClassName,
        string lpWindowName);

    // Activate an application window.
    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

}