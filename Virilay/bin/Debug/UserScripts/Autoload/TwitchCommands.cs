        using System.Net;
using Viri.Lieutenants;
using System.Threading;
using System.Text;
using System; //For bitconverter
using System.IO;

public class TwitchCommands
{
    #region data
        private string _username;
    string authcode;

    #endregion
    #region asyncdata
    //all async fields prefixed with a<name>
    string a_newgame = "", a_newtitle = "";
    #endregion

    public TwitchCommands()
    {
        try
        {
            if (File.Exists("config.txt"))
                using (StreamReader Reader = new StreamReader("config.txt"))
                {
                    string file = Reader.ReadToEnd();
                    int index = file.IndexOf("ViriAuth=");
                    if (index != -1)
                    {
                        authcode = file.Substring(index + "ViriAuth=".Length);
                        authcode = authcode.Substring(0, authcode.IndexOf('\n'));
                        authcode = authcode.Trim();
                    } index = file.IndexOf("Username=");
                    if (index != -1)
                    {
                        _username = file.Substring(index + "Username=".Length);
                        _username = _username.Substring(0, _username.IndexOf('\n'));
                        _username = _username.Trim();
                    }
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        //if (authcode == null) Console.WriteLine("Did not get Viriauthcode. Will not be able to change title");
    }

    [ViewerPermitted]
    public void SetTitle(string title)
    {
        new Thread(delegate() { _twitchtitleset(title); }).Start();   
    }
    private void _twitchtitleset(string title)
    {
        a_newtitle = title;
        tempcode = authcode;
        this.PutHTTPReq("https://api.twitch.tv/kraken/channels/" + _username, "oauth_token=" + tempcode + "&channel[status]=" + a_newtitle.Replace("%", "%25").Replace("&", "%26").Replace("+", "%2B").Replace("/", "%2F").Replace(";", "%3B"));//+ "&channel[game]=" + a_newgame.Replace("%", "%25").Replace("&", "%26").Replace("+", "%2B").Replace("/", "%2F").Replace(";", "%3B"));
        this.GetHTTPReqA("https://api.twitch.tv/kraken/channels/" + _username, "?oauth_token=" + tempcode, new AsyncCallback(this.CheckGameAndTitle));
    }

    
    [ViewerPermitted]
    public void SetGame(string game)
    {
        new Thread(delegate() { _twitchset(game); }).Start();   
    }
    string tempcode = "";
    private void _twitchset(string game)
    {
        a_newgame = game.Replace("%", "%25").Replace("&", "%26").Replace("+", "%2B").Replace("/", "%2F").Replace(";", "%3B");

        tempcode = authcode;
        this.PutHTTPReq("https://api.twitch.tv/kraken/channels/" + _username, "oauth_token=" + tempcode + "&channel[game]=" + a_newgame);
        this.GetHTTPReqA("https://api.twitch.tv/kraken/channels/" + _username, "?oauth_token=" + tempcode, new AsyncCallback(this.CheckGameAndTitle));

        
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
    private void CheckGameAndTitle(IAsyncResult asynchronousResult)
    {
        //if (this.authorized != 1)
        //    return;
        string str = "NOT FOUND";
        string src;
        try
        {
            using (Stream responseStream = ((WebRequest)asynchronousResult.AsyncState).EndGetResponse(asynchronousResult).GetResponseStream())
            {
                TextReader textReader = (TextReader)new StreamReader(responseStream);
                str = textReader.ReadToEnd() + "\n";
                textReader.Close();
                responseStream.Close();
            }
            src = str.Trim(new char[1]
        {
          '{'
        }).Trim(new char[1]
        {
          '\n'
        }).Trim(new char[1]
        {
          '}'
        });
        }
        catch
        {
            return;
        }
        try
        {
            if ((a_newgame!="" && this.GetParam(src, "game", 0) == a_newgame) || (a_newtitle!="" && this.GetParam(src, "status", 0) == a_newtitle))
                Console.WriteLine("Success!");

            else Console.WriteLine("Failure :(");
            a_newgame = "";
            a_newtitle = "";
        }
        catch { }
    }
    #endregion
} 