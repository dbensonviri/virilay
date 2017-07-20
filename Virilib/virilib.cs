using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
public static class Virilib
{
    #region File Writer
    public delegate void ReadFileCallback(List<string> line);

    /// <summary>
    /// Virilib.ReadFile((word) =>
    ///            {
    ///                foreach (string x in word) Console.Write(x+" ");
    ///            }
    ///           , "config.txt");
    /// </summary>
    /// <param name="rfc"></param>
    /// <param name="file"></param>
    public static bool ReadFile(string file, ReadFileCallback rfc)
    {
        if (!File.Exists(file)) return false;
        string Line = "";
        char[] sep = new char[] { ' ', '=','\t' };
        
        //sep[0] = ','; sep[1] = '='; sep[2] = ';'; sep[3] = ' '; sep[4] = '{'; sep[5] = '}'; sep[6] = ':';
        //sep.Concat(Environment.NewLine.ToCharArray());
        using (StreamReader Reader = new StreamReader(file))
        {
            Line = "A";
            while (Line != null)
            {
                Line = Reader.ReadLine();
                if (Line != null) if (Line != "") if (Line[0] != '#')
                        {
                            List<string> something = Line.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (something[something.Count - 1] == "") something.RemoveAt(something.Count - 1);
                            rfc(something);
                        }
            }

            Reader.Close();
            return true;
        }
    }
    public static List<string[]> ReadFile(string file)
    {
        StreamReader Reader;
        List<string[]> ret = new List<string[]>(); //ret[0]="pos","0","1"
        string Line = "";
        char[] sep = new char[7];

        sep[0] = ','; sep[1] = '='; sep[2] = ';'; sep[3] = ' '; sep[4] = '{'; sep[5] = '}'; sep[6] = ':';
        sep.Concat(Environment.NewLine.ToCharArray());

        using (Reader = new StreamReader(file))
        {
            Line = "A";
            while (Line != null)
            {
                Line = Reader.ReadLine();
                if (Line != null) if (Line != "") if (Line[0] != '#')
                        {
                            ret.Add(Line.Split(sep));
                        }
            }

            Reader.Close();
        }
        return ret;
    }
    public static string[] ParseString(string s)
    {
        char[] sep = new char[9];
        sep[0] = ','; sep[1] = '='; sep[2] = ';'; sep[3] = ' '; sep[4] = '{'; sep[5] = '}'; sep[6] = ':'; sep[7] = '\t'; sep[8] = '\n';
        sep.Concat(Environment.NewLine.ToCharArray());

        return s.Split(sep);

    }
    public static string ParseStringAndPop(ref string s)
    {
        char[] sep = new char[9];
        sep[0] = ','; sep[1] = '='; sep[2] = ';'; sep[3] = ' '; sep[4] = '{'; sep[5] = '}'; sep[6] = ':'; sep[7] = '\t'; sep[8] = '\n';
        sep.Concat(Environment.NewLine.ToCharArray());
        string ret = s.Split(sep)[0];

        s = s.Substring(ret.Length + 1);
        return ret;

    }

    /// <summary>
    /// returns true if timed out
    /// returns false if completed succcessfully
    /// </summary>
    /// <param name="action"></param>
    /// <param name="timeoutms"></param>
    /// <returns></returns>
    public static bool RunCodeWithTimeout(Action action,TimeSpan timeout,TimeSpan sleepinterval)
    {
        DateTime endtime = DateTime.Now + timeout;
        bool completed = false;
        ThreadStart start = new ThreadStart(() =>
        {
            action();//no writelines in here
            completed = true;
        });
        Thread thread = new Thread(start);
        thread.Start();
        while (true)
        {
            if (completed)
            {
                //Console.WriteLine("Done");
                return true;
            }
            else if (DateTime.Now > endtime)
            {
                //Console.WriteLine("Timed out");
                thread.Abort();
                return false;
            }
            Thread.Sleep(sleepinterval);
        }
            
           
    }

    public static void WriteFile(string file, String[] str)
    {
        using (StreamWriter Writer = new StreamWriter(file))
        {
            foreach (String x in str)
            {
                Writer.WriteLine(x);
            }
            Writer.Close();
        }
    }
    public static void WriteFile(string filename, List<string> str)
    {
        using (StreamWriter Writer = new StreamWriter(filename))
        {
            foreach (String x in str)
            {
                Writer.WriteLine(x);
                Console.WriteLine("wrote " + x);
            }
            Console.WriteLine();
            Writer.Close();
        }

    }

    #endregion
    public static string GetExternalIP()
    {
        try
        {
            string ret = new System.Net.WebClient().DownloadString("http://checkip.dyndns.org/");
            ret = new System.Text.RegularExpressions.Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Matches(ret)[0].ToString();
            return ret;
        }
        catch { return null; }
    }
    static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
 
    public  static int SecRandom(int range)
    {
        byte[] ary = new byte[4];
        rng.GetBytes(ary);
        return Math.Abs(BitConverter.ToInt32(ary,0))%range;
    }

 
}
