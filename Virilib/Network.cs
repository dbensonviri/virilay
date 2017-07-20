using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Net;
using System.Collections.Generic;

namespace Viri
{
    class Network
    {
        //Socket _socket=default(Socket);
        //TcpClient _client = default(TcpClient);
        //TcpListener _server = default(TcpListener);
        /*
        public void Send(byte[] buffer)
        {
            int size = buffer.Length;
            int offset = 0;
            int startTickCount = Environment.TickCount;
            int sent = 0;  // how many bytes is already sent
            do
            {
                if (Environment.TickCount > startTickCount + 10000) //10s tiemout
                    throw new Exception("Timeout.");
                try
                {
                    sent += _socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (sent < size);
        }
        public byte[] Recieve()
        {
            byte[] _buffer = new byte[12];
            int offset = 0;
            int size = 12;//size of "Hello world!"
            int startTickCount = Environment.TickCount;
            int received = 0;  // how many bytes is already received
            do
            {
                if (Environment.TickCount > startTickCount + 10000)
                    throw new Exception("Timeout.");
                try
                {
                    received += _socket.Receive(_buffer, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (received < size);
            return _buffer;
        }
        */

    }
}
