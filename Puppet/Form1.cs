using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections; //so we can use arraylist
using System; //For bitconverter
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions; //so we can use REGEX to remove empty space
using System.Runtime.InteropServices; //sends keypresses
using OverlayImages;
using System.Net.NetworkInformation; //for pingz!
using System.Drawing.Imaging;
using System.Drawing;
using System.Timers;
using System.Threading;

using System.Windows.Forms;
namespace Puppet
{
    public partial class Form1 : Form
    {
        Thread _updatethread;
        Thread _simulatethread;
        int _destx;
        int _desty;

        public Form1()
        {
            InitializeComponent();
            TopMost = true;
            _updatethread = new Thread(new ThreadStart(_update));
            _updatethread.Start();

            _simulatethread = new Thread(new ThreadStart(_simulatemove));
            _simulatethread.Start();
        
        }

        int Lerp(int v1, int v2, float val)
        {
            return (int)(v1 + (v2 - v1) * val);
        }

        public void _update()
        {
            while (true)
            {
                this.Location = new Point(Lerp(this.Location.X,_destx,.1f),Lerp(this.Location.Y,_desty,.1f));
                //this.Cursor = new Cursor(Cursor.Current.Handle);
                //Cursor.Position = new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5);
                Thread.Sleep(30);
            }
        }

        void _simulatemove()
        {
            while (true)
            {
                Random r=new Random();
                MoveThis(r.Next(500), r.Next(500));
                Thread.Sleep(150);
            }
        }

        public void MoveThis(int x, int y)
        {
            _destx = x;
            _desty = y;
        }
    }
}
