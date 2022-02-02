using System;

namespace NamedPipes.Events
{
    /// <summary>
    /// Connection state event arguments
    /// </summary>
    public class ConnectionStateEventArgs : EventArgs
    {
        public ConnectionStateEventArgs(ConnectionState connectionState)
        {
            ConnectionState = connectionState;
        }

        public ConnectionState ConnectionState { get; }
    }
}