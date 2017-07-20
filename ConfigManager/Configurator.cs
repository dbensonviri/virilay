using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Security;
namespace ConfigManager
{
    public partial class Configurator : Form
    {
        public config Config
        {
            //todo: lazy instantiation
            get
            {
                config ret = new config(new Point(StartX, StartY), (int)_startwidth.Value, _username.Text, _oauth1.Text, _oauth3.Text, (int)_opacity.Value, (int)_fadetime.Value,(int)_outputcount.Value, (_outputdirection.SelectedIndex == _outputdirection.FindStringExact("up")),(float) _fontsize.Value);
                
                return ret;
            }
            set
            {
                _username.Text = value.Username;

                _oauth1.Text = value.OAuth;
                _oauth3.Text=value.AccessToken;
                _fontsize.Value = (decimal)value.FontSize;
                _fadetime.Value = value.FadeTime;
                _opacity.Value = value.Opacity;
                _outputcount.Value = value.OutputCount;
                _outputdirection.SelectedIndex = _outputdirection.FindStringExact((value.OutputGoesUp ? "up" : "down"));
                StartX = value.StartX;
                StartY = value.StartY;
                _startwidth.Value = (int)value.StartWidth;
            }
        }
        private int StartX, StartY;
        public Configurator()
        {
            InitializeComponent();
            Config = config.FromFile("configv2.txt");
        }

        


        private void _savebutton_Click(object sender, EventArgs e)
        {

            Config.SaveToFile("config.txt");
            //Config = config.FromFile;
            Application.Exit();
        }

        private void _cancelbutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             
            System.Diagnostics.Process.Start("http://twitchapps.com/tmi"); 
            
        }

        //todo: securestring decryption
        private string _clientid = "8raxdz0rci7506jyv5j51l7vle0326z";
        private string _clientsecret = "pgqgcvxto6ldcupnl7aa4d1q390ihe8";
        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id=8raxdz0rci7506jyv5j51l7vle0326z&redirect_uri=http://localhost&scope=chat_login+channel_read+channel_commercial+channel_editor");
        
        }

 


        #region dashboard
        private String GetParam(String src, String paramname, int SkipMatches)
        {
            // This is my somewhat useless JSON parsing thing. .NET has its own JSON parsing, but I didn't find
            // out about that until after I had made this one, and then I didn't feel like figuring out how a proper
            // JSON parsing... thing would work, so I just left this as it is. Feel free to replace it.
            int strpos = 0, curcite = 0;
            int pstart = 0, plen = 0;

            int skipParam = SkipMatches;

            String pValue;
            String pName = "NOT FOUND";

            int getPName = 1, getPValue = 0, pValueType = 0;
            int vstart = 0, vlen = 0;

            while (strpos < src.Length)
            {
                if (getPName == 1)
                {
                    if (src[strpos] == '\"')
                    {
                        if (curcite == 0)
                        {
                            pstart = strpos + 1; curcite = 1;
                        }
                        else if (curcite == 1)
                        {
                            plen = strpos - pstart;
                            pName = src.Substring(pstart, plen);
                            if (src[strpos + 2] == '{')
                                curcite = 0;
                            else
                            {
                                getPValue = 1;
                                getPName = 0;
                                if (src[strpos + 2] == '\"')
                                {
                                    pValue = "";
                                    strpos += 3;
                                    vstart = strpos;
                                    vlen = 0;
                                    pValueType = 1;
                                }
                                else
                                {
                                    pValue = "";
                                    strpos += 2;
                                    pValueType = 2;
                                    vstart = strpos;
                                    vlen = 0;
                                }
                            }
                        }
                    }
                }
                if (getPValue == 1)
                {
                    if (pValueType == 1)
                    {
                        bool flimps = true;
                        while (flimps)
                        {
                            vlen++;
                            strpos++;
                            if (src[strpos] == '\"')
                            {
                                if (src[strpos - 1] != '\\') flimps = false;
                            }
                            if (strpos >= src.Length - 1)
                            {
                                flimps = false;
                            }
                        }
                        pValue = src.Substring(vstart, vlen);
                        if (pName == paramname && skipParam == 0)
                            return pValue;
                        else if (pName == paramname && skipParam > 0)
                        {
                            skipParam--;
                        }
                        getPName = 1; getPValue = 0; curcite = 0;
                    }
                    else if (pValueType == 2)
                    {
                        while (src[strpos] != ',' && src[strpos] != '}' && strpos < src.Length - 1)
                        {
                            vlen++;
                            strpos++;
                        }
                        pValue = src.Substring(vstart, vlen);
                        if (pName == paramname)
                            return pValue;
                        getPName = 1; getPValue = 0; curcite = 0;
                    }
                }
                strpos++;
            }
            return "NOT FOUND";
        }
        private string GetHTTPReq(string Request, string Reqparams)
        {
            string str = "NOTHING";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Request + Reqparams);
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                httpWebRequest.Accept = "application/vnd.twitchtv.v2+json";
                httpWebRequest.Method = "GET";
                using (Stream responseStream = httpWebRequest.GetResponse().GetResponseStream())
                {
                    TextReader textReader = (TextReader)new StreamReader(responseStream);
                    str = textReader.ReadToEnd() + "\n";
                    textReader.Close();
                    responseStream.Close();
                }
            }
            catch
            {
                // return "Error during HTTP request: " + ex.Message;
            }
            return str;
        }
        private string PutHTTPReq(string Request, string Reqparams)
        {
            string str = "NOTHING";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Request);
                byte[] bytes = new UTF8Encoding().GetBytes(Reqparams);
                httpWebRequest.Accept = "application/vnd.twitchtv.v2+json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = (long)bytes.Length;
                using (Stream requestStream = ((WebRequest)httpWebRequest).GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                using (Stream responseStream = httpWebRequest.GetResponse().GetResponseStream())
                {
                    TextReader textReader = (TextReader)new StreamReader(responseStream);
                    str = textReader.ReadToEnd() + "\n";
                    textReader.Close();
                    responseStream.Close();
                }
            }
            catch
            {
                //  this.toolStripStatusLabel1.Text = "Update failed: " + ex.Message;
            }
            return str;
        }
        private void GetHTTPReqA(string Request, string Reqparams, AsyncCallback callbackFunction)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Request + Reqparams);
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                httpWebRequest.Accept = "application/vnd.twitchtv.v2+json";
                httpWebRequest.Method = "GET";
                httpWebRequest.BeginGetResponse(new AsyncCallback(callbackFunction.Invoke), (object)httpWebRequest);
            }
            catch { }
        }
            
        #endregion

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                string s = @"client_id=" + _clientid
                            + "&client_secret=" + _clientsecret
                            + "&grant_type=authorization_code"
                            + "&redirect_uri=" + "http://localhost"
                            + "&code=" + _oauth2.Text;
                byte[] bytes = new ASCIIEncoding().GetBytes(s);

                #region (client_id,client_secret,client_auth) => access_token
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/oauth2/token");
                httpWebRequest.Accept = "application/vnd.twitchtv.v2+json";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = (long)bytes.Length;
                using (Stream requestStream = ((WebRequest)httpWebRequest).GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                WebResponse response = httpWebRequest.GetResponse();

                string str2; //{"access_token":"8adfkjl.......","scope":["channel_read","channel_editor","channel_commercial","channel_stream","chat_login"]}
                using (Stream responseStream = response.GetResponseStream())
                {
                    TextReader textReader = (TextReader)new StreamReader(responseStream, Encoding.UTF8);
                    str2 = textReader.ReadToEnd().Trim();
                    textReader.Close();
                    responseStream.Close();
                }
                Console.WriteLine(str2);
                str2 = str2.Substring(str2.IndexOf("access_token") + "access_token...".Length); //8adfkl........",scope":[...
                _oauth3.Text = str2.Substring(0, str2.IndexOf('\"'));
                #endregion
            }
            catch(Exception r) { Debug.Assert(false, "Did not properly set Access Token! "+r.ToString()); }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("This was developed by Virifaux (viriapps@gmail.com, twitch.tv/virifaux). This was offered for free somewhere on the internet.");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=9SLXKBHKJJE9Y&lc=US&item_name=Virilay&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Oauth (Chat): Click the button. Click through the buttons. Copy and paste the entire 'oauth:randomletters' text into the box\n\nOauth (SetGame): Click the first get button. Copy the 'code' part in the url. For instance, if code=111111111, you'll copypaste 111111111.  If you have trouble with url, paste into a new webbrowser, or clear your cookies\n\nThen, click the second get button. Text should appear in the box.\n\nSave when you are done. You can now run Virilay");
        }


    }

    public class config
    {
        //public fields for serializing with reflection
        //reflection SetValue does not work on structs

        //readonly properties
        public float FontSize;// { get; internal set; }
        public string Username;// { get; internal set; }
        public string OAuth;// { get; internal set; }
        public string AccessToken;// { get; internal set; }
        public int Opacity;// { get; internal set; }
        public int FadeTime;// { get; internal set; }
        public int OutputCount;// { get; internal set; }
        public bool OutputGoesUp;// { get; internal set; }

        //public properties
        public int StartX;// { get; set; }
        public int StartY;// { get; set; }
        public int StartWidth;// { get; set; }


        public config(Point loc, int w, string username,string oauth,string accesstoken, int opac, int fade, int opcount, bool opup,float fontsize)
        {
            StartX = loc.X;
            StartY = loc.Y;
            StartWidth = w;
            Username = username;
            OAuth = oauth;
            AccessToken = accesstoken;
            Opacity = opac;
            FadeTime = fade;
            OutputCount = opcount;
            OutputGoesUp = opup;
            FontSize = fontsize;
        }
        public static config Default
        {
            get
            {
                Rectangle r = Screen.PrimaryScreen.WorkingArea;
                Point loc = new Point(r.Right - 300, r.Bottom - 320);
                return new config(loc, 300,"default","default","default", 20, 7, 8, true,10);
            }
        }

        //todo: use File, instead of string filename
        public static config FromFile(string filename)
        {
            config ret = config.Default;
            ReflectionSerializer vs = new ReflectionSerializer();
            vs.ReadFields<config>(filename, ret);
            return ret;            
        }

        //todo: use File instead of string filename
        public void SaveToFile(string filename)
        {
            try
            {
                ReflectionSerializer vs = new ReflectionSerializer();
                vs.WriteFields(filename, this);       
            }
            catch
            {
                Debug.Assert(false, "Unable to serialize. Requires .Net Frameword 4.5: http://www.microsoft.com/en-us/download/details.aspx?id=30653");
            }
        }
    }
 
}
