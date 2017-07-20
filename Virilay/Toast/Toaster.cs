using ConfigManager;
using System.Threading;
using System.Threading.Tasks;
using System;
//using CsGL.OpenGL;
using OverlayImages;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Net;
using System.Windows.Forms;
using Virilay.IRC;
using Viri.Lieutenants;
using System.Collections.Generic;

namespace Viri.ConsoleOut
{
    

    public partial class Virilay : Form
    {
        #region Data
        Viribot viribot = new Viribot();
        ToasterOut _toasterout;
        public TextBox _txtviribotsay;
        private Label _lblviewercount;
        Executor _lt;
        config _config;
        private PictureBox pictureBox1;
        TextWriter _oldout = Console.Out;
        #endregion

        #region Lifetime
        public Virilay()
        {
            

            if (!File.Exists("configv2.txt"))
            {
                MessageBox.Show("You need to apply twitch settings to Configurator. Please quit the program and run it");
                Application.Exit();
            }


            _config = config.FromFile("configv2.txt");
            
            #region Initialize Form
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            Text = "Toaster";
            Application.ApplicationExit += Application_ApplicationExit;
            GotFocus += Virilay_GotFocus;            
            ClientSize = new Size(_config.StartWidth, _lblviewercount.Location.Y + _lblviewercount.Size.Height +_lblviewercount.Height-4);
            Location = new Point(_config.StartX, _config.StartY);
            FormBorderStyle = FormBorderStyle.None;


   
            #endregion


            
            _toasterout = new ToasterOut(this, _config.OutputCount, _config.FontSize);
            _toasterout.SetTopToBottom(!_config.OutputGoesUp);

            ToastLine.SecondsUntilFade = _config.FadeTime;
            ToastLine.OpacityMin = ((float)_config.Opacity) / 100.0f;
            _toasterout.SetWidth(_config.StartWidth);
            Resize += new EventHandler(_resizetoaster);
            LocationChanged += new EventHandler(_locationchangedtoaster);
            _txtviribotsay.Focus();
            Show();

            _lt = new Executor(_config.Username,"");
            Console.SetOut(_toasterout);
            Console.SetError(_toasterout);
            Load += Virilay_Load;
            this.Activated += Virilay_Activated; //reset opacity
            this.Click += Virilay_Click;
            _ap = new AdditionalParameters();
            _ap.Origin = "";

            this.TopLevel = true;
            this.TopMost = true;
            _txtviribotsay.Font = new Font("Arial", _config.FontSize);
            _lblviewercount.Font = new Font("Arial", _config.FontSize);
            _txtviribotsay.KeyPress += new KeyPressEventHandler(this_KeyPress);
            _lblviewercount.Location = new Point(
                 _txtviribotsay.Location.X,
                 _txtviribotsay.Location.Y + _txtviribotsay.Size.Height+2 //+ (int)_config.FontSize
                 );

            pictureBox1.Location = new Point(
                 _txtviribotsay.Right-pictureBox1.Width,
                 _txtviribotsay.Location.Y + _txtviribotsay.Size.Height+2 //+ (int)_config.FontSize
                 );

            


            if (Directory.Exists("UserScripts\\Autoload"))
            {
                //todo:optimize
                string[] filePaths = Directory.GetFiles("UserScripts\\AutoLoad", "*.cs", SearchOption.AllDirectories);
                for (int i = 0; i < filePaths.Length; i++)
                    _lt.Compile(filePaths[i].Substring("UserScripts\\".Length, filePaths[i].LastIndexOf(".cs") - "UserScripts\\".Length));

            }
            if (!string.IsNullOrWhiteSpace(_config.Username))
            {
                viribot.OnMsg += _viribot_OnPrivMsg;
                Console.WriteLine(_config.Username);
                Console.WriteLine(_config.OAuth);
                viribot.Connect(_config.Username, "irc.twitch.tv", 6667, _config.Username, _config.OAuth, TimeSpan.FromSeconds(3));
                
            }
            else Console.WriteLine("Not launching twitch because username or password is empty.\n("+_config.Username+"/"+_config.OAuth+")");
            Console.SetError(viribot);
             
        }

        void Virilay_Click(object sender, EventArgs e)
        {
            _toasterout.ResetOpacity();
        }

        void Virilay_GotFocus(object sender, EventArgs e)
        {
            _toasterout.Visible = true;
        }

        void Virilay_Load(object sender, EventArgs e)
        {

            
        }



        void Virilay_Activated(object sender, EventArgs e)
        {
            _toasterout.ResetOpacity();
        }

      

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            viribot.Dispose();
            Console.SetOut(_oldout);
            config c = config.FromFile("configv2.txt");
            c.StartWidth = Width;
            c.StartX = Location.X;
            c.StartY = Location.Y;
            _config.StartWidth = Width;
            _config.StartX = Location.X;
            _config.StartY = Location.Y;
            _config.SaveToFile("configv2.txt");
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x00A3) //disable maximize on doubleclick
            {
                m.Result = IntPtr.Zero;
                return;
            }
            if (m.Msg == 0x0084 /*WM_NCHITTEST*/)
            {
                m.Result = (IntPtr)2; //HTCLIENT
                return;
            }
            switch (m.Msg)
            {
                case MessageHelper.WM_USER:
                    MessageBox.Show("Message recieved: " + m.WParam + " - " + m.LParam);
                    break;
                case MessageHelper.WM_COPYDATA:
                    MessageHelper.COPYDATASTRUCT mystr = new MessageHelper.COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (MessageHelper.COPYDATASTRUCT)m.GetLParam(mytype);
                    viribot.Say(mystr.lpData);
                    break;
            }

            base.WndProc(ref m);
        }
        #endregion

        #region Viribot Events
        void _viribot_OnPrivMsg(ircmsg themsg)
        {
            
            //Console.WriteLine(themsg.Function + " " + themsg.Origin + " " + themsg.FullTail);
            this.Invoke((MethodInvoker)(() =>
            {
               
                if (themsg.Function == "PRIVMSG")
                {
                    if (string.IsNullOrWhiteSpace(themsg.FullTail)) return;

                    TryExecute(themsg);
                }
                else if (themsg.Function == "JOIN")
                {
                    if (_entertain)
                    {
                        //Console.WriteLine(themsg.Origin + " joined");
                        if (Virilib.SecRandom(2) == 1)
                            Console.Error.WriteLine("Ugh, it's " + themsg.Origin);
                        else
                            Console.Error.WriteLine("<3 <3 <3 It's " + themsg.Origin);
                        _lblviewercount.Text = "Viewers: " + viribot.ViewerCount;
                    }
                }
                else if (themsg.Function == "PART")
                {
                    if (_entertain)
                    {
                        Console.WriteLine(themsg.Origin + " left");
                        _lblviewercount.Text = "Viewers: " + viribot.ViewerCount;
                    }
                }
            }));
        }
        #endregion

        #region Designer
        //return true if owner should say the command
        //situations where we return true:
        //  if we couldn't execute the command

        public void Say(string say)
        {
            viribot.Say(say);
        }

        int _numkappa = 0;
        AdditionalParameters _ap;
        private void TryExecute(ircmsg msg)
        {
            if (!_entertain) return;
            string text = msg.FullTail;
            string origin = msg.Origin;

            if (msg.Tail.Count>=1 && msg.Tail[0].Length>=2 && msg.Tail[0].Substring(1)=="bb")
            {

            }
            else
                Console.WriteLine(msg);
            
            if (msg.Tail.Count>=1)
            {

                for(int i=0;i<msg.Tail.Count;i++)
                    if (msg.Tail[i] == "Kappa" || msg.Tail[i] == "KappaClaus" || msg.Tail[i] == "KappaPride" || msg.Tail[i] == "KappaRoss" || msg.Tail[i] == "KappaWealth" || msg.Tail[i] == "Keepo")
                    {
                        _numkappa++;
                        Virilib.WriteFile("kappacount.txt", new string[] { _numkappa.ToString() });
                    }
                    
            }

            StringBuilder sb=new StringBuilder();
            if (!string.IsNullOrWhiteSpace(msg.Origin))
                sb.AppendFormat("{0}: ",msg.Origin);
            if (text[0] == '!' && text.Length>1)
            {
                _ap.Origin = msg.Origin;
                object[] message;
                // await Task.Run<Lieutenant.executereturn>(() => _lt.Execute(text.Substring(1), msg.Origin));
                _lt.Execute(text.Substring(1),_ap,out message);
                if (message.Length>=1  && message[0]!=null && (!string.IsNullOrWhiteSpace(message[0].ToString())))
                {
                    sb.Append(message[0]);
                    viribot.Say(sb.ToString());
                
                }


                _ap.Origin = "";

            }
            
        }

        bool _entertain = true;
        void this_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (e.KeyChar == (char)13) //enter
            {
                TextBox tb = (sender as TextBox);
                string text = tb.Text;
                if (string.IsNullOrEmpty(text)) return;
                tb.ResetText();
                e.Handled = true;

                if (text.Length >= 6 && text.Substring(0, 6) == "!join ") //todo: do better
                {
                    string channel=text.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries)[1];
                    Process.Start(new ProcessStartInfo("explorer.exe", "http://twitch.tv/" + channel));
                    viribot.Connect(channel, "irc.twitch.tv", 6667, _config.Username, _config.OAuth,TimeSpan.FromSeconds(10));
                }
                else if (text.Length >= "!entertain".Length && text.Substring(0, "!entertain".Length) == "!entertain")
                {
                    _entertain = !_entertain;
                    Console.WriteLine("Entertain: " + _entertain + ". Will " + (_entertain ? "" : "not ") + "execute commands");
                }
                else if (text.Length >= "!interrupt".Length && text.Substring(0, "!interrupt".Length) == "!interrupt")
                {
                    _toasterout.ChatInterrupts = !_toasterout.ChatInterrupts;
                    Console.WriteLine("Interrupt: " + _toasterout.ChatInterrupts + ". Chats are " + (_toasterout.ChatInterrupts ? "" : "not ") + "immediately visible");
                }
                else if (text.Length >= "!center".Length && text.Substring(0, "!center".Length) == "!center")
                {
                    Location = Cursor.Position;
                }
                else if (viribot!=null) viribot.Say(text);
            }
            else if (e.KeyChar == (char)27) //escape
            {
                _toasterout.Visible = false;
    
            }
        }

        //for alias joins
        Dictionary<string, filerow> _filerow = new Dictionary<string, filerow>();
        struct filerow
        {
            public string Alias;
            public int Port;
            public string Server;
            public string Channel;
            public string User;
            public string Password;
            public filerow(string alias, int port, string server, string channel, string user, string password)
            {
                Alias = alias;
                Port = port;
                Server = server;
                Channel = channel;
                User = user;
                Password = password;
            }
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Virilay));
            this._txtviribotsay = new System.Windows.Forms.TextBox();
            this._lblviewercount = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // _txtviribotsay
            // 
            this._txtviribotsay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtviribotsay.Location = new System.Drawing.Point(12, 12);
            this._txtviribotsay.Margin = new System.Windows.Forms.Padding(0);
            this._txtviribotsay.Name = "_txtviribotsay";
            this._txtviribotsay.Size = new System.Drawing.Size(278, 20);
            this._txtviribotsay.TabIndex = 0;
            // 
            // _lblviewercount
            // 
            this._lblviewercount.AutoSize = true;
            this._lblviewercount.Location = new System.Drawing.Point(12, 43);
            this._lblviewercount.Margin = new System.Windows.Forms.Padding(0);
            this._lblviewercount.Name = "_lblviewercount";
            this._lblviewercount.Size = new System.Drawing.Size(56, 13);
            this._lblviewercount.TabIndex = 1;
            this._lblviewercount.Text = "Viewers: 0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(270, 36);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Virilay
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(302, 61);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this._lblviewercount);
            this.Controls.Add(this._txtviribotsay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Virilay";
            this.Text = "Virilay";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Virilay_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        void _resizetoaster(object sender, EventArgs e)
        {
            _toasterout.SetWidth(Width);
 

        }

        void _locationchangedtoaster(object sender, EventArgs e)
        {
            _toasterout.SetWidth(Width);

        }


        #endregion

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Virilay_MouseDown(object sender, MouseEventArgs e)
        {
            _toasterout.SetWidth(Width);
        }
    }

}
