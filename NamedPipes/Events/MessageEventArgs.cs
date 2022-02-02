using System;

namespace NamedPipes.Events
{
    /// <summary>
    /// Message event arguments
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
