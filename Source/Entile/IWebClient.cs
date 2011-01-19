using System;
using System.Collections.Generic;
using System.Net;

namespace Entile
{
    public interface IWebClient
    {
        void SendStringAsync(Uri uri, string body, string contentType);
        event EventHandler<SendCompletedEventArgs> SendCompleted;
    }
}