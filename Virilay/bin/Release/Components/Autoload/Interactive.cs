using Viri.Lieutenants; //let's us add [Help()] text and mark functions as [Privileged]
using System.Collections.Generic;
using System.Security.Cryptography;

using System.Threading.Tasks;
using System.Collections; //so we can use arraylist
using System.Text; //Lets us use StringBuilders
using System; //For bitconverter
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions; //so we can use REGEX to remove empty space
using System.Runtime.InteropServices; //sends keypresses
using OverlayImages;
using System.Net.NetworkInformation; //for pingz!
using System.Drawing.Imaging;
using System.Drawing;
using System.Timers;
public class Interactive
{
    public Interactive()
    {
    }

    int _splatcount = 0;



    bool _qsplat=false;

    [AdditionalParameters]
    public async void QSplat(AdditionalParameters ap)
    {
        _qsplat=!_qsplat;
        Console.WriteLine("Splatting: "+_qsplat);
        while(_qsplat)
        {
            for (int i = 0; i < r.Next(5); i++)
            {
                Splat(ap);
                await Task.Delay(200+r.Next(400));
            }
            await Task.Delay(5000+r.Next(5000));

        }
    }

    public class CryptoRandom : Random { private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider(); private byte[] _uint32Buffer = new byte[4]; public CryptoRandom() { } public CryptoRandom(Int32 ignoredSeed) { } public override Int32 Next() { _rng.GetBytes(_uint32Buffer); return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF; } public override Int32 Next(Int32 maxValue) { if (maxValue < 0) throw new ArgumentOutOfRangeException("maxValue"); return Next(0, maxValue); } public override Int32 Next(Int32 minValue, Int32 maxValue) { if (minValue > maxValue) throw new ArgumentOutOfRangeException("minValue"); if (minValue == maxValue) return minValue; Int64 diff = maxValue - minValue; while (true) { _rng.GetBytes(_uint32Buffer); UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0); Int64 max = (1 + (Int64)UInt32.MaxValue); Int64 remainder = max % diff; if (rand < max - remainder) { return (Int32)(minValue + (rand % diff)); } } } public override double NextDouble() { _rng.GetBytes(_uint32Buffer); UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0); return rand / (1.0 + UInt32.MaxValue); } public override void NextBytes(byte[] buffer) { if (buffer == null) throw new ArgumentNullException("buffer"); _rng.GetBytes(buffer); } }
    CryptoRandom r=new CryptoRandom();

    [ViewerPermitted]
    [AdditionalParameters]
    public async void Splat(AdditionalParameters ap)
    {
        if (_splatcount > 20) return;

        //r=1050
        //1050=baseradius * intervals
        int intervals = 9999;
        int radiusx = 1680 *  r.Next(intervals + 1) / intervals + 1, radiusy = 1050 * r.Next(intervals + 1) / intervals + 1;
        _splatcount++;
        OverlayImage oi = new OverlayImage("splat.png", 255);
//        oi.TopMost = true;
        {
            //oi.ShowInTaskbar = false;

            int x = 1680 / 2 + r.Next(radiusx / 2) - r.Next(radiusx / 2) - 168; 
            int y = 1050 / 2 + r.Next(radiusy / 2) - r.Next(radiusy / 2) - 149;
            
            oi.Location = new Point(x, y);
            await Task.Delay(1000+r.Next(3000));
            
            int speed = 10 + r.Next(40);
            for (int i = 0; i < r.Next(50) + 150; i++)
            {
                oi.Location = new Point(oi.Location.X, oi.Location.Y + 1);
                await Task.Delay(speed);
            }
        }
        oi.Close();
        _splatcount--;
    }





    public void Hi()
    {
        hi();
    }

    Task hi()
    {

        return Task.Run(async delegate
        {

            Console.WriteLine(":D");
            await Task.Delay(1000);
            Console.WriteLine(":D");
        });
    }


}