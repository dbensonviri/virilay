using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Viri.ConsoleOut
{
    class ToasterOut:TextWriter
    {
        ToastLine[] _toasts;
        int _messagesindex = 0;
        bool _toptobottom;
        Form _owner;
        public bool ChatInterrupts = true;
        
        public bool Visible
        {
            get {
                for (int i = 0; i < _toasts.Length; i++)
                    if (_toasts[i].Opacity > ToastLine.OpacityMin) return true;
                return false;
            }
            set
            {
                if (value)
                    ResetOpacity();
                else
                    for (int i = 0; i < _toasts.Length; i++)
                        _toasts[i].Opacity = ToastLine.OpacityMin;
            }
        }
        public ToasterOut(Form owner,int numtoasts,float fontsize)
        {
            _owner = owner;
            _toasts = new ToastLine[numtoasts];
            for (int i = 0; i < numtoasts; i++)
            {
                _toasts[i]=new ToastLine(this);

                _toasts[i].TopLevel = true;
                _toasts[i].TopMost = true;
                _toasts[i].Label.Font=new Font("Arial",fontsize);
                _toasts[i].SetHeight(0);
                _toasts[i].SetWidth(owner.Width);
                _toasts[i].Show();
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
       
        public override void WriteLine(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            _messagesindex = (_messagesindex + 1) % _toasts.Length;
            _toasts[_messagesindex].SetText(message, ChatInterrupts);
            _updateorder();

  
        }

        public override void WriteLine(object message)
        {
            WriteLine(message.ToString());
        }

        public void SetTopToBottom(bool value)
        {
            _toptobottom = value;
        }
        public void SetWidth(int width)
        {
            
            for (int i = 0; i < _toasts.Length; i++)
            {
                _toasts[i].Invoke((MethodInvoker)delegate
                {
                    //_toasts[i].ResetOpacity();
                    _toasts[i].Width = width;
                    _toasts[i].Label.Width = width;
                });
            }
           
            _updateorder();
           
        }
        public void ResetOpacity()
        {
            for (int i = 0; i < _toasts.Length; i++) _toasts[i].ResetOpacity();
        }
        private void _updateorder()
        {
            int y = 0;
            bool oddbackcolor = false;
            int _numtoasts = _toasts.Length;
            if (_toptobottom)
            {
                y = _owner.Height;
                for (int i = 0; i < _toasts.Length; i++)
                {

                    int calci = (_messagesindex + _numtoasts - i) % _numtoasts;
                    _toasts[calci].Invoke((MethodInvoker)delegate
                    {
                        _toasts[calci].Location = new Point(_owner.Left, _owner.Top + y);
                        y += _toasts[calci].Height;
                        if (oddbackcolor)
                            _toasts[calci].SetColor(Color.LightGoldenrodYellow);
                        else
                            _toasts[calci].SetColor(Color.White);
                        oddbackcolor = !oddbackcolor;
                    });
                }
            }
            else
            {
                Point Location = new Point(_owner.Left, _owner.Bottom - _owner.Height);
                y += _owner.Height; 

                for (int i = 0; i < _numtoasts; i++)
                {
                    int calci = (_messagesindex + _numtoasts - i) % _numtoasts;
                    _toasts[calci].Invoke((MethodInvoker)delegate
                    {
           
                        y += _toasts[calci].Height;
                        _toasts[calci].Location = new Point(_owner.Left, _owner.Bottom - y);
           
                        if (oddbackcolor)
                            _toasts[calci].SetColor(Color.LightGoldenrodYellow);
                        else
                            _toasts[calci].SetColor(Color.White);
                        oddbackcolor = !oddbackcolor;
                        if (calci==_messagesindex) _toasts[calci].ResetOpacity();
                    });
                }
            }
            

        }

    }
}
