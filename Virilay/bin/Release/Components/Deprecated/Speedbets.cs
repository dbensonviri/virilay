using System.Linq;
using Viri.Lieutenants; //let's us add [Help()] text and mark functions as [Privileged]
using System.Collections.Generic; 
using System.Collections; //so we can use arraylist
using System.Threading;
using System.Text; //Lets us use StringBuilders
using System; //For bitconverter
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions; //so we can use REGEX to remove empty space
public class Speedbets
{
    MessageHelper _mhlpr = new MessageHelper();
    int _sbihwnd;
    Process _p;
    
    public Speedbets()
    {
        _p = Process.Start("Components\\SpeedBets.exe");
        while((_sbihwnd=_mhlpr.getWindowId(null, "SpeedBetInterface"))==0)
            Thread.Sleep(50);
        Thread.Sleep(50);
    }
    ~Speedbets()
    {
        
        _p.CloseMainWindow();
    }

    uint _pump = 0;
    public void Pump(uint pump)
    {
        _pump = pump;
    }
    uint _economy = 20;  //the sum earning for each round will be AT LEAST 20
    public void MinEconomy(uint econ)
    {
        _economy = econ;
    }
    #region Moderator commands
    public void AddChoice(string choice)
    {
        int x;
        if (int.TryParse(choice,out x))
        {
            return;
        }
        choice = choice.ToLower();
        if (!_choices.ContainsKey(choice)) 
        {
            _choices.Add(choice,new List<Player>());
            //return "There are " + _choices.Count + " choices. Players bet " + String.Format("{0:0.00}", ((double)100 / (double)_choices.Count)) + "% of their cash";
        }
        //return "";
    }

    public void RemoveChoice(string choice)
    {
        choice = choice.ToLower();
        if (_choices.ContainsKey(choice))
        {
            foreach (Player p in _choices[choice]) p.Choice = "";
            _choices.Remove(choice);
          //  return "There are " + _choices.Count + " choices. Players bet " + String.Format("{0:0.00}", ((double)100 / (double)_choices.Count)) + "% of their cash";
        }
        //return "";
    }
    public string ClearBets()
    {
        foreach(List<Player>lp in _choices.Values)
        {
            foreach (Player p in lp)
                p.Choice = "";
            lp.Clear();
        }
        return "Everyone can vote again";
    }

    public string Explain1()
    {
        return "Each round, you contribute a certain percent of your money to the pot. If you guess correctly, you get a portion of the pot (even if you have no money!). ";
    }
    public string Explain2()
    {
        return "The faster you bet, the more money you get back (if you guess right). A user who bets $3 first might get $14 back. If he waited 2 turns, he might get $5";
    }
    /*
    public string Options(int index)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder next = new StringBuilder();
        int lines = 0; 
        for (int i = index; i < numchoices;i++ )
        {
            string p = _choices.Keys[i];
            next.Length = 0; next.Capacity = 0;
            next.AppendFormat("{0} ", p);
            if ((sb.ToString() + next.ToString()).Length <= 19)
                sb.Append(next.ToString());
            
            else
            {
                lines++;
                if (lines == 4) break;
                else 
                sb.AppendFormat("{0} ", p);
            }
        }
        return sb.ToString();
    }*/
    public string Resolve(string answer)
    {

        StringBuilder sb = new StringBuilder();
        if (numchoices == 0) {Console.WriteLine("There were no choices. Use 'addchoice'"); return "";}
        if (!_choices.ContainsKey(answer)) { Console.WriteLine("That's not a valid choice"); return ""; }
        
        

        uint totalpayout = 0;
        List<Player> winners = _choices[answer];
        #region Update the payout pool
        foreach (Player p in _players.Values)
        {
            int val = p.GetBet(numchoices);
            p.PreviousPayout = -val;
            p.Points -= (uint)val;
            totalpayout += (uint)val;
            p.Choice = ""; //todo: --. Requires refactoring in !bet
        }  
        #endregion
        #region Check if everyone loses
        if (winners.Count == 0)
        {
            foreach (List<Player> lp in _choices.Values) lp.Clear();
            return "Everyone's a loser! Ahaha";
        }
        #endregion 
        #region Economy pump
        if (totalpayout < _economy || _pump!=0)                              //let's say we have 5 correct voters, and we want to pump $13 dollars into the economy
        {                                                                   //13/5=[2]. 13%5={3}   . We'll give [2]+1 to {3} people. Give [2] to the rest
            uint oldtotalpayout = totalpayout;
            if (totalpayout < _economy) totalpayout += (_economy-totalpayout) * (uint)numchoices;
            totalpayout += _pump;
            Console.WriteLine("Players bet "+oldtotalpayout+". Adding $"+(totalpayout-oldtotalpayout)+" to the pool.");
            _pump = 0;
        }
        #endregion

        //MYPAYOUT=    TOTALPAYOUT  *    (WINNERCOUNT-INDEXINWINNERS)/SUMSOFABOVE
        // 10     =        20       *       3/6
        // 6.6=6  =        20       *       2/6
        // 3.3=3  =        20       *       1/6
        uint[] numerators = new uint[winners.Count];
        uint numeratorsum = 0;
        for (int i = 0; i < winners.Count; i++)
        {
            uint val = (uint)(winners.Count - i);
            numeratorsum += val;    //3
            numerators[i] = val;    //{2,1}
        }
        uint sanitycheck_payout=0;
        for (int i = 0; i < winners.Count; i++)
        {
            numerators[i] = (uint)Math.Floor((double)totalpayout * numerators[i] / numeratorsum); //20*2/3=13.333 . 20*1/3=6.666
            sanitycheck_payout += numerators[i]; //19
        }
        int sanitycheck_index = 0;
        while (sanitycheck_payout<totalpayout)
        {
            sanitycheck_payout++;
            numerators[sanitycheck_index++]++; //if there's missing cash, we redistribute it 
        }
        //now, numerators are payouts
        for (int i = 0; i < winners.Count; i++)
        {
            winners[i].PreviousPayout += (int)(numerators[i]);
            winners[i].Points += numerators[i];                                  //now we distribute the payouts

        }

        
        uint moneyincirculation = 0;
        uint biggestpoints = 0;
        List<string> biggestname = new List<string>();
        foreach (Player p in _players.Values)
        {
            _mhlpr.sendWindowsStringMessage(_sbihwnd, 0, p.Name + " " + p.Points + " "+p.PreviousPayout);
            if (p.Points == biggestpoints) biggestname.Add(p.Name);
            else if (p.Points > biggestpoints)
            {
                biggestpoints = p.Points;
                biggestname.Clear();
                biggestname.Add(p.Name);
            }
            moneyincirculation += p.Points;
        
        }
       
        sb.AppendFormat("Total Payout: ${0}. Money in circulation: ${1}", totalpayout, moneyincirculation);
        Console.WriteLine(sb.ToString());sb.Length = 0; sb.Capacity = 0; //sb.Clear();   
        
        sb.AppendFormat("Payout: {0}={1}",winners[0].Name,numerators[0]);
        for (int i = 1; i < winners.Count;i++ )
        {
            Player p = winners[i];
            sb.AppendFormat(",{0}={1}", p.Name,numerators[i]);
        }

        if (biggestpoints != 0)
        {
            sb.AppendFormat(". Leaders ${0}: {1}",biggestpoints,biggestname[0]);
            for (int i = 1; i < biggestname.Count; i++)
                sb.AppendFormat(",{0}", biggestname[i]);
        }
        foreach (List<Player> lp in _choices.Values) lp.Clear();
        
        return sb.ToString();
        
            //do skew factor
            /*
             Suppose the totalpayout is 20. Three people guess correctly in order {3,1,2}
             The simple payout is {10,3.3,6.6}, based on how much each bet. 
             (3*20)/(3+1+2)=10
             Skewing it on speed, SkewFactor(x)=1/x, where x is place.For 3 places,SumSkews=1/1+1/2+1/3=1.83
             10*(1/1)/1.83=[5.46]
             3.3*(1/2)/1.83=[.9]
             6.6*(1/3)/1.83=[1.19]
             [Sum=7.55]
         
         

             foreach(player p in _players)
                totalpayout+=GetBet()
                p.PreviousPayout-=GetBet();
                if (amwinner)
                    SumContestants+=1/Index;
             foreach(palyer p in winners)
             {
                p.PreviousPayout+=TotalPayout * (Winners.Length-WInners.IndexOf(p)/SumContestants
             }
             PAYOUT=    TOTALPAYOUT  *    (WINNERCOUNT-INDEXINWINNERS)/SUMSOFABOVE
                                20          (3-0)/6
                                                        .4,.3,.2,.1              
              */

    }
    
    #endregion
    bool _allowrebetting = false;
    public string AllowRebetting(bool value)
    {
        _allowrebetting = !_allowrebetting;
        return "Rebetting is " + (value ? "allowed" : "not allowed");
    }
    
    [ViewerPermitted(NeedsOrigin = true)]
    public string Bet(string origin, string choice)
    {
        choice = choice.ToLower();
        origin = origin.ToLower();
        int tryparse;
        if (int.TryParse(choice,out tryparse))
        {
            if (tryparse-1<_choices.Keys.Count && tryparse-1>=0)
            choice=_choices.Keys.ToList()[tryparse-1];
        }
        if (!_choices.ContainsKey(choice))
            return "Not a valid choice ["+choice+"]";
        StringBuilder sb = new StringBuilder();

        if (!_players.ContainsKey(origin))
            _players.Add(origin, new Player(origin));
        if (_players[origin].Choice!="")
            return "Rebetting is not allowed!";
        
        sb.Append(Info(origin));
        _mhlpr.sendWindowsStringMessage(_sbihwnd, 0, "!BET "+choice);
            
        _players[origin].Choice = choice;
        _choices[choice].Add(_players[origin]);
        return sb.ToString();
    }
    [ViewerPermitted(NeedsOrigin = true)]
    public string Info(string origin)
    {
        origin = origin.ToLower();
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("You have ${2} (betting ${1}). Previous earnings: ${0}. ", _players[origin].PreviousPayout, Math.Floor((double)(_players[origin].Points) / (double)_choices.Count), _players[origin].Points);
        return sb.ToString();
    }


    int numchoices
    {
        get { return _choices.Keys.Count; }
    }
    Dictionary<string,List<Player>> _choices = new Dictionary<string,List<Player>>();
    Dictionary<string, Player> _players = new Dictionary<string, Player>();
    class Player
    {
        public int GetBet(int count)
        {
                return (int)Math.Ceiling((double)(Points) / (double)count);
            
        }
        public string Choice;
        public string Name;
        public uint Points;
        public int PreviousPayout;
        public Player(string name)
        {
            Choice = "";
            Name = name;
            Points = 0;
            PreviousPayout = 0;
        }
    }
} 