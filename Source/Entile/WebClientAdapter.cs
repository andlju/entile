using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Entile
{
    public class SendCompletedEventArgs : EventArgs
    {
        public SendCompletedEventArgs(string result)
        {
            Error = null;
            Result = result;
        }
        public SendCompletedEventArgs(Exception error)
        {
            Error = error;
            Result = null;
        }

        public Exception Error { get; private set; }
        public string Result { get; private set; }
    }

    public class WebClientAdapter : IWebClient
    {
        private readonly WebClient _webClient;

        public WebClientAdapter()
        {
            _webClient = new WebClient();
        }

        public void SendStringAsync(Uri uri, string body, string contentType)
        {
            _webClient.Headers["content-type"] = contentType;
            _webClient.Headers["accept"] = contentType;
            _webClient.UploadStringCompleted += OnUploadStringCompleted;

            _webClient.UploadStringAsync(uri, "POST", body);
        }

        void OnUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Exception error = e.Error;
            if (error == null)
            {
                InvokeSendCompleted(new SendCompletedEventArgs(e.Result));
            } 
            else
            {
                InvokeSendCompleted(new SendCompletedEventArgs(error));
            }
        }

        public event EventHandler<SendCompletedEventArgs> SendCompleted;

        protected virtual void InvokeSendCompleted(SendCompletedEventArgs e)
        {
            EventHandler<SendCompletedEventArgs> handler = SendCompleted;
            if (handler != null) handler(this, e);
        }
    }
}