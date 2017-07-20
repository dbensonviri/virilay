using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OverlayImages;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Viri.Lieutenants;
class MissionPoints//:Lieutenant
{
    
    
    Stack<MissionPointState> _states = new Stack<MissionPointState>(10);
    private MissionPointState _current;
    public MissionPointState Current
    {
        get { return _current; }
        set 
        { 
            _current = value;
            //this.Player1.Name = _current.SpyName;
            //this.Player2.Name = _current.SniperName;
            
        }
    }
    public MissionPoints()//:base("Player1","Player2")
    {
        /*OverlayImage oi=new OverlayImage();
        oi.TopMost=true;
        oi.SetBitmap("garden.png");
        */

        Current = new MissionPointState("Player1", "Player2", 0);
        _makeform();
        _updateform();
    }
   
    Label _label1;
    private void _makeform()
    {

        /*_label1 = new Label();
        Controls.Add(_label1);

        this.SuspendLayout();
        // 
        // MPForm
        // 
        
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(284, 261);
        this.Name = "MPForm";
        this.Text = "Mission=Points";
        this.ResumeLayout(false);*/

    }
    private void _updateform() { } 

    //[Help(true,"todo")]
    public string Start(string spyname,string snipername)
    {
        return Start(spyname, snipername, 6);
    }

    //[Help(true,"todo")]
    public string Start(string spyname, string snipername, int threshold)
    {
        _states.Clear();
        Current = new MissionPointState(spyname, snipername, threshold);
        _updateform();
        return Current.ToString();
    }

    //[Help(true,"todo")]
    public string Score(int newpoints)
    {
        Current.Points = newpoints;
        _updateform();
        return Current.ToString();
    }

    //[Help(true,"todo")]
    public string Threshold(int threshold)
    {
        Current.Threshold = threshold;
        _updateform();
        return Current.ToString();
    }

    //[Help(true,"todo")]
    public string Progress(uint points)
    {
        _states.Push(Current);
        StringBuilder sb = new StringBuilder(Current.CurrentSpy());
        sb.AppendFormat(" scored {0} points. ", (int)points);
        Current.Progress(points);
        _updateform();
        return sb.ToString() + Current.ToString();
    }

    //[Help(true,"todo")]
    public string Undo()
    {
        if (_states.Count != 0)
            Current = _states.Pop();
        _updateform();
        return Current.ToString() + ". Undos left: " + _states.Count;
    }


}


    class MissionPointState
    {
        public MissionPointState(string spyname, string snipername, int threshold)
        {
            SpyName = spyname;
            SniperName = snipername;
            Threshold = threshold;
            SpyQueue = new string[4] { spyname, snipername, snipername, spyname };
            Game = 0;
            Points = 0;
        }
        public int Points;
        public string SniperName;
        public string SpyName;
        public int Threshold;
        public int Game;

        public string[] SpyQueue; //SPY, SNIPER, SNIPER, SPY
        private int SpyQueueIndex
        {
            get { return Game % 4; }
        }

        public void Progress(uint points)
        {
            if (SpyQueueIndex == 0 || SpyQueueIndex == 3)
                Points += (int)points;
            else
                Points -= (int)points;
            Game++;
        }
        public string PreviousSpy() { return SpyQueue[(SpyQueueIndex + 3) % 4]; }
        public string CurrentSpy() { return SpyQueue[SpyQueueIndex]; }
        public string NextSpy() { return SpyQueue[(SpyQueueIndex + 1) % 4]; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int set = (int)(Math.Floor(Game / 2f) + 1);
            int round = Game % 2 + 1;
             

            sb.AppendFormat("Round {0} ({1}/{2}). ", Game+1,Points, Threshold);

            if (round == 1 && Threshold!=0)                //If it's the first round of the set, before anyone has a chance to play
            {                             //We should say who is spying, and tell him to pick a map. Or show the winner (if applicable)
                if (Points <= -Threshold)
                {
                    sb.AppendFormat("Victory goes to {0}! ", SniperName);
                    return sb.ToString();
                }
                else if (Points >= Threshold)
                {
                    sb.AppendFormat("Victory goes to {0}! ", SpyName);
                    return sb.ToString();
                }
                else
                {
                    sb.AppendFormat("{0} is spy, and picks a new map. ", SpyQueue[SpyQueueIndex]);
                    return sb.ToString();
                }
            }
            else
            {
                sb.AppendFormat("{0} is spy. ", SpyQueue[SpyQueueIndex]);
                return sb.ToString();
            }
        }
    }
    /*static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MissionPoints());

        }
    }*/