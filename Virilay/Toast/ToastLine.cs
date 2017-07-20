using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Virilay.IRC;

namespace Viri.ConsoleOut
{
    #region Toast
    class ToastLine : Form
    {
        public static int SecondsUntilFade=99;
        public static float OpacityMin = 10;
        //ToasterOut _owner;

        
        public TextBox Label;

        const int _fadetime = 33;
        private Timer _fadetimer = new Timer();

        void startfading()
        {
            _fadetimer.Interval = SecondsUntilFade * 1000;
            _fadetimer.Tick += fadestage1;
            _fadetimer.Start();
        }
        void fadestage1(object sender, EventArgs e)
        {
            _fadetimer.Interval = _fadetime;
            _fadetimer.Tick -= fadestage1;
            _fadetimer.Tick += fadestage2;
        }
        void fadestage2(object sender, EventArgs e)
        {
            Opacity -= .005f;
            if (Opacity <= OpacityMin)
            {
                Opacity = OpacityMin;
                _fadetimer.Stop();
                _fadetimer.Tick -= fadestage2;
            }
        }
        public void SetHeight(int height)
        {
            Height = height;
            Label.Height = height;
                
        }
        public void SetWidth(int width)
        {

            Width = width;
            Label.Width = width;

            Label.ClientSize = new Size(width, Height);
            ClientSize = new Size(width, Height);
        }
       
        protected override CreateParams CreateParams
        {
            get
            {
                // Turn on WS_EX_TOOLWINDOW style bit
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        public void SetText(string message,bool resetopacity)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate { SetHeight(TextRenderer.MeasureText(message, Label.Font, new Size(Label.Size.Width, 999), TextFormatFlags.WordBreak).Height); Label.Text = message; }));
                
                //if (resetopacity) ResetOpacity();
            
            }
            else {
                SetHeight(TextRenderer.MeasureText(message, Label.Font, new Size(Label.Size.Width, 999), TextFormatFlags.WordBreak).Height); 
                Label.Text = message; }
            
        }
  
        public void SetColor(Color c)
        {
            BackColor = c;
            Label.BackColor = c;
        }
        public void SetFontColor(Color c)
        {
            Label.ForeColor = c;
        }
        public void ResetOpacity()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    Opacity = 1; startfading();
                }));
            }
            else
            {
                Opacity = 1;
                startfading();
            
            }
        }
        ToasterOut _owner;
        public ToastLine(ToasterOut owner)
        {
            _owner = owner;
            #region window
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopLevel = true;
            TopMost = true;
            Opacity = 0;
            
            Show();
            #endregion

            #region label
            Label = new TextBox();
            Label.MouseClick += Label_MouseClick;
            Label.Height = Height ;
            Label.Width = Width;
            Label.BackColor = BackColor;
            Label.ReadOnly = true;
            Label.BorderStyle = 0;
            Label.MouseEnter += new EventHandler(_label_MouseEnter);
            Label.Multiline = true;
            Label.WordWrap = true;
            Controls.Add(Label);
            #endregion

        }

        void Label_MouseClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(Label.Text);
            Console.WriteLine("Copied to clipboard");
            _owner.Visible = false;
        }

        void _label_MouseEnter(object sender, EventArgs e)
        {
            if (Opacity<.9)
                Opacity = 0;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ToastLine
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(10, 10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToastLine";
            this.ResumeLayout(false);

        }

      


    }
    #endregion

}
