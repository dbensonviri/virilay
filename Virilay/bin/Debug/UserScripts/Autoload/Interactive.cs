using OverlayImages;
using Viri.Lieutenants; //let's us add [Help()] text and mark functions as [Privileged]
using System;
using System.Collections.Generic;
using System.Collections; //so we can use arraylist
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation; //for pingz!$
using System.Runtime.InteropServices; //sends keypresses
using System.Security.Cryptography;
using System.Threading;
using System.Text; //Lets us use StringBuilders
using System.Text.RegularExpressions; //so we can use REGEX to remove empty space
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;



public class Interactive
{
    int _splatcount = 0;
    bool _qsplat=false;
    System.Media.SoundPlayer _bananabreadsound = new System.Media.SoundPlayer(@"bananabread.wav");

    [ViewerPermitted]
    public void BB()
    {
        _bananabreadsound.Play();
    }

    [ViewerPermitted]
    public async void BB(int timestart,int timerange)
    {
        await Task.Delay((timestart + r.Next(timerange)) * 1000);
        _bananabreadsound.Play();
    }

    [AdditionalParameters]
    public async void QSplat(AdditionalParameters ap)
    {
        _qsplat=!_qsplat;
        Console.WriteLine("Splatting: "+_qsplat);
        while(_qsplat)
        {
            ap.Origin = "virifaux";
            Splat(ap);
            await Task.Delay(1000*r.Next(10)+r.Next(10000)*r.Next(3));
        }
    }


    //int[] offsets=new offsets[10]{0,}
    class _splatterval 
    { 
        public int Count; 
        public DateTime EndTime;
        public _splatterval(int count) 
        { 
            Count = count;
            EndTime = DateTime.Now-TimeSpan.FromSeconds(1);
        }
    }
    Dictionary<string, _splatterval> _splatters = new Dictionary<string, _splatterval>(); //people can send 15 in 15 seconds
    int _splatsonscreenmax = 60;
    int _individualsplats = 40;
    int _individualtimeout = 20; //seconds
    [ViewerPermitted]
    [AdditionalParameters]
    [Help("Shows viewer <3<3<3.")]
    public void Splat(AdditionalParameters ap)
    {
        Splat(ap, 1 + r.Next(3) * (r.Next(2) + 2), false);
    }
    [ViewerPermitted]
    [AdditionalParameters]
    [Help("Shows viewer <3<3<3.")]
    public void Heart(AdditionalParameters ap)
    {
        Splat(ap, 1 + r.Next(3) * (r.Next(2) + 2), true);
    }

    Random r = new Random();

    #pragma warning disable 4014 //ignores async warning
    [AdditionalParameters]
    async void Splat(AdditionalParameters ap,int count,bool isheart)
    {
        if (_splatcount > _splatsonscreenmax)
        {
            Console.WriteLine(_splatcount.ToString());
            return;
        }
        string name = ap.Origin;
        if (!_splatters.ContainsKey(name))
            _splatters.Add(name, new _splatterval(0));
        
        if (_splatters[name].EndTime < DateTime.Now)
        {
            _splatters[name].EndTime = DateTime.Now + TimeSpan.FromSeconds(_individualtimeout);
            _splatters[name].Count = 0;
        }
        else if (_splatters[name].Count >= _individualsplats && ap.Origin!="virifaux")
        {
            if (_splatters[name].Count>_individualsplats*3)
            {
                Console.WriteLine(name+" splat timed out for "+(_splatters[name].EndTime-DateTime.Now).TotalSeconds);
                _splatters[name].Count = _individualsplats;
            }
            return;
        }
        
        count = Math.Min(_individualsplats - _splatters[name].Count, count);
        _splatters[name].Count += count;
        if (count <= 0) return;

        bool shotgun = r.Next(3) >= 1;
        _splatcount += count;
        if (!isheart) count = 1;
        for(int i=0;i<count;i++)
        {
            int intervals = 200;
            int radiusx = 1050 * r.Next(intervals + 1) / intervals + 1, radiusy = 1050 * r.Next(intervals + 1) / intervals + 1;

            _splat(1620 / 2 + r.Next(radiusx / 2) - r.Next(radiusx / 2) - 168,
                    1050 / 2 + r.Next(radiusy / 2) - r.Next(radiusy / 2) - 149,
                    isheart
                    );

            if (shotgun)
                await Task.Delay(200 + r.Next(400));

        }
        await Task.Delay(6000);
        _splatcount-=count;
        
    }
    [ViewerPermitted]
    [AdditionalParameters]
    public void Shame(AdditionalParameters ap)
    {
        Splat(ap);
    }
    private async Task _splat(int x,int y,bool isheart)
    {

        OverlayImage oi;
        if (isheart)
        {
            oi = new OverlayImage("kiss.png", 255,false);
        }
        else oi = new OverlayImage("splat.png", 255,false);
        //        oi.TopMost = true;
        {
            
            oi.Location = new Point(x, y);
            await Task.Delay(1000 + r.Next(3000));

            int speed = 10 + r.Next(40);
            for (int i = 0; i < r.Next(60) + 60; i++)
            {
                oi.Location = new Point(oi.Location.X, oi.Location.Y + (isheart ? -1 : 1));
                await Task.Delay(speed);
            }
        }
        oi.Close();
    }

    
}