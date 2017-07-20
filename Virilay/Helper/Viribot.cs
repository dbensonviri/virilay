using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Security;
namespace Virilay.IRC
{
    /* Usage:
     * //Initialization
     *      Viribot viribot=new Viribot();
     *      viribot.OnMsg+=new Viribot.d_msg(_onprivmsg);
     *      viribot.Connect("#somechannel","irc.freenode.net",6667,"someusername","somepassword",TimeSpan.FromSeconds(3));
     *      viribot.Disconnect(); //when done with the session
     *      viribot.Dispose();    //when you want to free resources
     *      
     * //Normal usage
     *      viribot.Say("Hello world!");
     *      
     * void _onprivmsg(ircmsg msg)
     * {
     *      if (msg.Function=="PRIVMSG")
     *          Console.WriteLine(msg.Origin + " said " + msg.FullTail);
     *      else if (msg.Function=="JOIN")
     *          Console.WriteLine(msg.Origin + " joined channel");
     * }
     * 
     * //TODO:
     *      Make ircmsg.Functon into enumerated type.
     *      Make FullTailinto a better name
     */
    /// <summary>
    /// Usage:
    ///    Initialization
    ///        Viribot viribot=new Viribot();
    ///        viribot.OnMsg+= Viribot_OnMsg; 
    ///        viribot.Connect("#somechannel","irc.freenode.net",6667,"someusername","somepassword",TimeSpan.FromSeconds(3));
    ///        
    /// </summary>
    class Viribot : TextWriter, IDisposable
    {
        #region Definitions
        public enum status { Disconnected, Connecting, Connected };
        public delegate void GotMessage(ircmsg the_msg);
        public event GotMessage OnMsg;

        #endregion

        #region Public properties

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
       
        public uint ViewerCount { get; private set; } //note: not accurate. just approximates based on who joins and leaves
        public status Status { get; private set; } //e.g. status.Connected
        public string Channel { get; private set; } //e.g. "#virifaux"
        public string UserName { get; private set; } //e.g."virifaux"
        #endregion
        #region Internal variables
        TcpClient _irc;

        //Disposables
        Thread _updateloopthread;
        NetworkStream _stream;
        StreamWriter _writer;
        StreamReader _reader;
        #endregion


        #region Life and death
        public Viribot()
        {
        }

        /// <summary>
        /// Tries to connect to IRC chat with the settings
        /// </summary>
        /// <param name="channel">e.g. "mychat" or "#mychat"</param>
        /// <param name="server">e.g. "irc.twitch.tv" or "irc.freenode.net"</param>
        /// <param name="port">pick a random port. e.g. 6667. TODO: optional with random good port</param>
        /// <param name="username"></param>
        /// <param name="oauth"></param>
        /// <param name="timeout">Timespan to giveup</param>
        public void Connect(string channel, string server, int port, string username, string oauth, TimeSpan timeout)
        {
            UserName = username;
            ViewerCount = 0;
            if (channel[0] != '#') channel = "#" + channel;
            Channel = channel.Substring(1);
            Status = status.Connecting;
            try
            {
                _irc = new TcpClient(server, port);

                _stream = _irc.GetStream();
                _reader = new StreamReader(_stream);
                _writer = new StreamWriter(_stream);

                _writer.WriteLine("PASS " + oauth);
                _writer.WriteLine("NICK " + username);
                _writer.WriteLine("USER " + username + " " + username + " " + username + " :" + username);

                _writer.WriteLine("JOIN #" + Channel);
                _writer.Flush();
                _updateloopthread = new Thread(_updateloop);
                _updateloopthread.Start();
                _updateloopthread.Priority = ThreadPriority.Lowest;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Closes the stream connection, readers, and writers
        /// </summary>
        public void Disconnect()
        {
            if (Status != status.Disconnected)
            {
                if (_writer != null)
                {
                    _writer.Close();
                    _writer.Dispose();
                }
                if (_reader != null)
                {
                    _reader.Close();
                    _reader.Dispose();
                }
                if (_irc != null)
                {
                    _irc.Close();
                    _irc.GetStream().Close();
                }
                if (_stream != null)
                {
                    _stream.Close();
                }
                Status = status.Disconnected;
            }

        }
        /*If extending this class, it should also include the following code*/
        bool _disposed; //when false, it means this is going out of {}. when true, Disposed() was called.  //consider adding "if _disposed throw new ObjectDisposedException(...); at the beginning of all functions?????
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                //dispose all disposables
                if (_stream != null) _stream.Dispose();
                if (_writer != null) _writer.Dispose();
                if (_reader != null) _reader.Dispose();
            }
            //do something about objects that dont have dispose
            if (_updateloopthread != null) _updateloopthread.Abort();
            /*
                if (ptr!=IntPtr.Zero)
                    {
                    CloseHandle(ptr);
                    //Marshal.FreeCoTaskMem(ptr);//for  Marshal.StringToCoTaskMemAuto("") IntPtrs
                    //Console.WriteLine("{0}: Unmanaged memory freed  at {1:x16}",this.GetType().Name,ptr.ToInt64());
                    ptr=IntPtr.Zero;
                    }
             */

            _disposed = true;

            base.Dispose(disposing); //when inheriting
        }
      
        ~Viribot()
        {
            Dispose(false);
        }

        #endregion

        #region Public methods
        

        
        public override void WriteLine(string message)
        {
            Say(message);
        }
        public void Say(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return;

            if (Status == status.Connected)
            {
                _writer.WriteLine("PRIVMSG #" + Channel + " :" + msg);
                _writer.Flush();
            }
            OnMsg(new ircmsg(UserName, msg, "PRIVMSG"));

        }
        #endregion
        #region Private methods
        void _updateloop()
        {
            string msg;
            while (true)
            {
                try
                {

                    while ((msg = _reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(msg)) continue;
                        ircmsg imsg = new ircmsg(msg);
                        //Console.WriteLine(msg);
                        //Console.WriteLine(imsg.Origin + " tail:" + imsg.FullTail + " fun:" + imsg.Function);
                        //if (Status!=status.Connected) Console.WriteLine(msg);
                        if (msg[0] != ':')
                        {
                            if (msg.StartsWith("PING"))
                            {
                                _writer.WriteLine("PONG " + msg.Substring(5) + "\r\n");
                                _writer.Flush();
                                break;
                            }
                        }
                        else
                        {

                            if (imsg.Function == "JOIN")
                            {
                                try
                                {
                                    if (UserName.ToLower() == imsg.Origin.ToLower())
                                    {
                                        Status = status.Connected;
                                        Console.WriteLine("Connected to " + imsg.Params[1] + "!");
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine(UserName == null);
                                }
                                ViewerCount++;
                            }
                            else if (imsg.Function == "PART")
                            {
                                ViewerCount--;
                            }

                            if (Status == status.Connected)
                            {
                                OnMsg(imsg);
                            }
                        }



                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }
        }

        #endregion
    }

    #region Definitions
    public struct ircmsg
    {
        #region fields
        private readonly string _origin;
        private readonly List<string> _params;
        private readonly string _fulltail;
        private readonly List<string> _tail;


        /// <summary>
        /// From who? 
        /// e.g. "Virifaux"
        /// </summary>
        public string Origin { get { return _origin; } }

        /// <summary>
        /// What type of msg?
        /// e.g. "PRIVMSG", "JOIN", "PART"
        /// </summary>
        public string Function { get { return _params[0]; } }
        /// <summary>
        /// Parameters. Params[0] is the function. The others are arguments.
        /// eg. Params = {"JOIN","#virifaux"}
        /// </summary>
        public List<string> Params { get { return _params; } }

        /// <summary>
        /// eg. {"I","am","a","cat"}
        /// </summary>
        public List<string> Tail { get { return _tail; } }        //{"I","am","a","cat"}. Space delimited.
        public string FullTail { get { return _fulltail; } } //"I am a cat"

        #endregion
        #region constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin">W</param>
        /// <param name="tail"></param>
        /// <param name="function"></param>
        public ircmsg(string origin, string tail, params string[] function)
        {
            _origin = origin;
            _fulltail = tail;
            _tail = new List<string>(_fulltail.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            _params = new List<string>(function);
        }

        /// <summary>
        /// Accepts a standard format string. e.g.
        ///     :notviri!44058b80@gateway/web/freenode/ip.68.5.139.128 PRIVMSG #virifaux :nou
        ///     :test!~test@test.com PRIVMSG #channel :Hi!
        ///     :virifaux!virifaux@virifaux.tmi.twitch.tv JOIN #virifaux
        /// 
        /// Standard format:
        ///     :USERNAME!USERNAME@USERNAME.JUNK PARAMS :TAIL
        /// </summary>
        /// <param name="msg"></param>
        public ircmsg(string msg)
        {

            //Console.WriteLine("<" + msg + ">");
            _fulltail = "";
            _tail = new List<string>();
            _params = new List<string>();

            List<string> msgstack = new List<string>(msg.Split(' ')); //{ notviri!408/ip68 , PRIVMSG , #virifaux , :tail tail tail}. 
            if (msgstack[0].IndexOf('!') == -1)
                _origin = "NONE";
            else
                _origin = msg.Substring(1,msgstack[0].IndexOf('!')-1);
            //_origin = msg.Substring(1, msg.IndexOf('!') );
            int index=1;
            for (; index < msgstack.Count;index++ )
            {
                if (msgstack[index][0] == ':') 
                {
                    _fulltail = msg.Substring(msg.IndexOf(':', 1) + 1);
                    _tail.Add(msgstack[index].Substring(1));
                    for(;index<msgstack.Count;index++)
                    {
                        _tail.Add(msgstack[index]);
                    }
                    break;
                }
               
                _params.Add(msgstack[index]);
            }
    
        }

        #endregion

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_fulltail)) return "";
            StringBuilder sb = new StringBuilder(_origin);

            if (_fulltail.Length > 4 && _fulltail.Substring(0, 4) == "/me ")
            {
                sb.Append(" ");
                sb.Append(_fulltail.Substring(4));
                return sb.ToString();
            }
            else if (char.IsControl(_fulltail[0])) //assume it's a me command
            {
                sb.Append(" ");
                sb.Append(_fulltail.Substring(8));
                return sb.ToString();
            }
            sb.Append(": ");
            sb.Append(_fulltail);
            return sb.ToString();
        }
    }
    #endregion
}