//For bitconverter
using TETCSharpClient;
using OverlayImages;
using TETCSharpClient.Data;
using System.Windows.Forms;
using System.Drawing;
using System;
using System.Runtime.InteropServices;

namespace Eyetribe
{
    public partial class Eyetribe : OverlayImage,IGazeListener
    {
        IntPtr hwnd;
        public Eyetribe(string file)
            : base(file+".png", 100)
        {
            GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);
            GazeManager.Instance.AddGazeListener(this);
            this.CanDrag(false);
            this.MouseEnter += Eyetribe_MouseEnter;
            Application.ApplicationExit += Application_ApplicationExit;
        }
        

        void Eyetribe_MouseEnter(object sender, EventArgs e)
        {
            Point p=Control.MousePosition;
            this.Location = new Point(p.X - this.Width / 2, p.Y - this.Height / 2);
        }

   
        void Application_ApplicationExit(object sender, EventArgs e)
        {
            GazeManager.Instance.Deactivate();
            this.Dispose();
            this.Close();
        }
        public void OnGazeUpdate(GazeData gazeData)
        {
            double gX = gazeData.SmoothedCoordinates.X;
            double gY = gazeData.SmoothedCoordinates.Y;

            if (gX == 0 && gY == 0) return;
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate 
                    {
                        double lerpX = Location.X + (gX - Location.X) * .45f;
                        double lerpY = Location.Y + (gY - Location.Y) * .45f;
                        this.Location = new Point((int)lerpX-this.Width/2, (int)lerpY-this.Height/2); 
                    }
                    ));
        }
    }
}
