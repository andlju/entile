using System;

namespace Entile
{
    public class ToastMessageEventArgs : EventArgs
    {
        public ToastMessageEventArgs(string title, string body)
        {
            Title = title;
            Body = body;
        }

        public string Title { get; private set; }
        public string Body { get; private set; }
    }
}