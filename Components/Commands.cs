using Viri.Lieutenants; //let's us add [Help()] text and mark functions as [Privileged]
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections; //so we can use arraylist
using NCalc; //I downloaded 'NCalc' from the internet so I use "Expression"!
using System.Text; //Lets us use StringBuilders
using System; //For bitconverter
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions; //so we can use REGEX to remove empty space
using System.Runtime.InteropServices; //sends keypresses
using OverlayImages;

public class Commands
{
            
    
    #region Standalone
    [ViewerPermitted] //anybody in chat can call this
    [Help("Displays the multitwitch.tv link of both players")]
    public string Twitch(string player1, string player2)
    {
        return "multitwitch.tv/" + player1 + "/" + player2 + "/";
    }
    #endregion
    //includes: Banner, Splat
    #region OverlayImages
    public List<string> peoplewhosplat = new List<string>();
    OverlayImage _oi;    
    [ViewerPermitted(NeedsOrigin = true)]
    public void Splat(string origin)
    {   //crazyfan123 might call !splat. In that case, it calls splat(virifan123)
        if (peoplewhosplat.Contains(origin)) return;
        peoplewhosplat.Add(origin);
        _oi = new OverlayImage();
        _oi.TopMost = true;
        _oi.SetBitmap("splat.png", 255);
        _oi.TopLevel = true;
        //todo: randomize splat image, splat goes downwards leavling trail of splat, splat disappears over time, apply disposals
    }

    public void Banner()
    {
        _oi = new OverlayImage();
        _oi.TopMost = true;
        _oi.SetBitmap("viribanner.png", 255);
        _oi.TopLevel = true;
    }
    #endregion


    
    //includes: OBS, TS, SpyParty. Launches a number of programs
    #region Programs

    public void Obs()
    {
        System.Diagnostics.Process.Start(@"C:\Program Files (x86)\OBS\OBS.exe");
    }
    public void TS()
    {
        System.Diagnostics.Process.Start(@"D:\Teamspeak\ts3client_win64.exe");
    }
    public void SpyParty()
    {
        System.Diagnostics.Process.Start(@"D:\Games\SpyParty\spyparty.exe");
    }

    #endregion
     
    //includes: Evaluate, Random, Roll, LongRoll, and various private methods
    #region Random, Math
    [ViewerPermitted]
    [Help("Does a math expression. For example: Evaluate 3%2 returns 1, 5&3 returns 1")]
    public object Evaluate(string equation)
    {
        return new Expression(equation).Evaluate();
    }


    Random _random = new Random();
    [ViewerPermitted]
    [Help("Returns a number from [min,max], inclusive")]
    public int Random(int min,int max)
    {
        return _random.Next(max - min+1) + min;
    }

    [Help("Rolls a die. eg. '1d6+2d3+3'")]
    [ViewerPermitted]
    public int Roll(string diceString)
    {
        string result = LongRoll(diceString); //eg. 1d6 = 4 = 4
        return int.Parse(result.Substring(result.LastIndexOf('=')+2)); //eg. 4
    }

    /// <summary>
    /// Pass a die roll string in any standard d20-type format, including 
    /// parenthetical rolls, and it will output a string breaking down each 
    /// roll as well as summing the total. This works very nicely. The code was
    /// hacked from java code from Malar's RPG Dice Version 0.9 By Simon Cederqvist 
    /// (simon.cederqvist(INSERT AT HERE) gmail.com). 13.March.2007
    /// http://users.tkk.fi/~scederqv/Dice/
    /// His same license still applies to this code. But if you figure out how to
    /// make a million dollars off this code, then you're smarter than me and 
    /// you deserve to keep it.  On the other hand, share and share alike. 
    /// </summary>
    /// <param name="diceString">This is a standard d20 die roll string, parenthesis 
    /// are allowed. Example Input: (2d8+9)+(3d6+1)-10</param>
    /// <returns>A string breaking down and summing the roll. Example Output: 
    /// (2d8+9)+(3d6+1)-10 = 7+4+9+5+1+2+1-10 = 20+9-10 = 19</returns>
    [ViewerPermitted]
    [HelpAttribute("Rolls a die and shows the math. For example, LongRoll(\"3d6+2\")")]
    public string LongRoll(string diceString)
    {
        StringBuilder finalResultBuilder = new StringBuilder();
        string tempString = "";
        int intermediateTotal = 0;
        ArrayList sums = new ArrayList();
        ArrayList items = new ArrayList();
        ArrayList dice = new ArrayList();
        int totals = 0;
        bool collate = false;
        bool positive = true;
        string validChars = "1234567890d";
        char[] diceCharArray = diceString.ToLower().ToCharArray();

        for (int i = 0; i < diceString.Length; i++)
        {
            switch (diceCharArray[i])
            {
                case '+':
                    {
                        if (tempString.Length < 1)
                        {
                            positive = true;
                            break;
                        }
                        dice = calcSubStringRoll(tempString);
                        for (int j = 0; j < dice.Count; j++)
                        {
                            if (!positive)
                            {
                                items.Add(-1 * Convert.ToInt32(dice[j].ToString()));
                                intermediateTotal += (-1 * Convert.ToInt32(dice[j].ToString()));
                            }
                            else
                            {
                                items.Add(Convert.ToInt32(dice[j].ToString()));
                                intermediateTotal += (Convert.ToInt32(dice[j].ToString()));
                            }
                        }
                        if (!collate)
                        {
                            sums.Add(intermediateTotal);
                            intermediateTotal = 0;
                        }
                        positive = true;
                        tempString = "";
                        break;
                    }
                case '-':
                    {
                        if (tempString.Length < 1)
                        {
                            positive = false;
                            break;
                        }
                        dice = calcSubStringRoll(tempString);
                        for (int j = 0; j < dice.Count; j++)
                        {
                            if (!positive)
                            {
                                items.Add(-1 * Convert.ToInt32(dice[j].ToString()));
                                intermediateTotal += (-1 * Convert.ToInt32(dice[j].ToString()));
                            }
                            else
                            {
                                items.Add(Convert.ToInt32(dice[j].ToString()));
                                intermediateTotal += (Convert.ToInt32(dice[j].ToString()));
                            }
                        }
                        if (!collate)
                        {
                            sums.Add(intermediateTotal);
                            intermediateTotal = 0;
                        }
                        positive = false;
                        tempString = "";
                        break;
                    }
                case '(': collate = true; break;
                case ')': collate = false; break;
                default:
                    {
                        if (validChars.Contains("" + diceCharArray[i]))
                            tempString += diceCharArray[i];
                        break;
                    }
            }
        }

        // And once more for the remaining text
        if (tempString.Length > 0)
        {
            dice = calcSubStringRoll(tempString);
            for (int j = 0; j < dice.Count; j++)
            {
                if (!positive)
                {
                    items.Add(-1 * Convert.ToInt32(dice[j].ToString()));
                    intermediateTotal += (-1 * Convert.ToInt32(dice[j].ToString()));
                }
                else
                {
                    items.Add(Convert.ToInt32(dice[j].ToString()));
                    intermediateTotal += (Convert.ToInt32(dice[j].ToString()));
                }
            }
            sums.Add(intermediateTotal);
            intermediateTotal = 0;
        }

        //// Print it all.
        finalResultBuilder.Append(diceString + " = ");
        /*if (items.Count == 1)
        {
            finalResultBuilder.Append(items.GetRange(0, 1).ToString() + ".");
            return finalResultBuilder.ToString();
        }*/
        for (int i = 0; i < items.Count; i++)
        {
            if (Convert.ToInt32(items[i].ToString()) > 0 && i > 0)
                finalResultBuilder.Append("+" + items[i].ToString());
            else
                finalResultBuilder.Append(items[i].ToString());
        }

        if (sums.Count > 1 && items.Count > sums.Count)
        { // Don't print just one, or items again.
            finalResultBuilder.Append(" = ");
            for (int i = 0; i < sums.Count; i++)
            {
                if (Convert.ToInt32(sums[i].ToString()) > 0 && i > 0)
                    finalResultBuilder.Append("+" + sums[i].ToString());
                else
                    finalResultBuilder.Append(sums[i].ToString());
            }
        }
        for (int i = 0; i < sums.Count; i++)
            totals += Convert.ToInt32(sums[i]);
        
        finalResultBuilder.Append(" = " + totals);
        
        return finalResultBuilder.ToString();
    }

    /// <summary>
    /// This function merely breaks down the *basic* die roll string
    /// into the requsite integers. It is used by the above Roll(string) 
    /// method. 
    /// </summary>
    /// <param name="s">A simple die roll string, such as 3d6. Nothing more.</param>
    /// <returns>Returns an ArrayList of int's containing the various die
    /// rolls as passed in as a parameter.</returns>
    ArrayList calcSubStringRoll(string s)
    {
        int x, d;
        ArrayList dice = new ArrayList();
        if (s.Contains("d"))
        {
            x = Convert.ToInt32(s.Split('d')[0]);
            d = Convert.ToInt32(s.Split('d')[1]);

            // I loop here so that each roll is added to the ArrayList, and 
            // therefore works properly with the code I hacked from java above. 
            for (int i = 0; i < x; i++)
                dice.Add(Roll(1,d,0));
        }
        else
            dice.Add(Convert.ToInt32(s));

        return dice;
    }

    /// <summary>
    /// Rolls the specified number of die each with the specified number of
    /// sides and returns the numeric result as a string. I had to introduce a 
    /// call to Thread.Sleep() so that the random num gen would seed differently on 
    /// each iteration. 
    /// </summary>
    /// <param name="numberOfDice">The number of die to roll.</param>
    /// <param name="numberOfSides">The number of faces on each dice rolled.</param>
    /// <param name="rollMod"></param>
    /// <returns>A string containing the result of the roll.</returns>
    string Roll(int numberOfDice, int numberOfSides, int rollMod)
    {
        // don't allow a Number of Dice less than or equal to zero
        if (numberOfDice <= 0)
            throw new ApplicationException("Number of die must be greater than zero.");

        // don't allow a Number of Sides less than or equal to zero
        if (numberOfSides <= 0)
            throw new ApplicationException("Number of sides must be greater than zero.");

        //// Create the string builder class used to build the string
        //// we return with the result of the die rolls.
        //// See: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfsystemtextstringbuilderclasstopic.asp
        //StringBuilder result = new StringBuilder();

        // Declare the integer in which we will keep the total of the rolls
        int total = 0;

        // repeat once for each number of dice
        for (int i = 0; i < numberOfDice; i++)
        {
            // Create the random class used to generate random numbers.
            // See: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfSystemRandomClassTopic.asp

            // Get a pseudo-random result for this roll
            int roll = _random.Next(1, numberOfSides);

            // Add the result of this roll to the total
            total += roll;

            //// Add the result of this roll to the string builder
            //result.AppendFormat("Dice {0:00}:\t{1}\n", i + 1, roll);
        }

        return (total + rollMod).ToString();

        //// Add a line to the result to seperate the rolls from the total
        //result.Append("\t\t--\n");

        //// Add the total to the result
        //result.AppendFormat("TOTAL:\t\t{0}\n", total);

        //// Now that we've finished building the result, get the string
        //// that we've been building and return it.
        //return result.ToString();            

    }
    #endregion


} 