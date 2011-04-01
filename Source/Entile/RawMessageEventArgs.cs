using System;

namespace Entile
{
    public class RawMessageEventArgs : EventArgs
    {
        public RawMessageEventArgs(byte[] body)
        {
            Body = body;
        }

        public byte[] Body { get; private set; }
    }
}