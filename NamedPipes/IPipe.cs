using System;
using NamedPipes.Events;
using NamedPipes.Messages;

namespace NamedPipes
{
    /// <summary>
    /// IPipe interface
    /// </summary>
    /// <remarks>
    /// Both server and client implement this interface for ease of use and better abstraction
    /// </remarks>
    public interface IPipe : IDisposable
    {
        /// <summary>
        /// Connection state changed event
        /// </summary>
        event EventHandler<ConnectionStateEventArgs> OnConnectionStateChanged;

        /// <summary>
        /// On message sent event
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageSent;

        /// <summary>
        /// On message received event
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageReceived;

        /// <summary>
        /// Open connection
        /// </summary>
        void Open();

        /// <summary>
        /// Close connection
        /// </summary>
        void Close();

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">string</param>
        void Send(string message);  

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Instance of an class inheriting from MessageBase</param>
        void Send(MessageBase message);

        /// <summary>
        /// Pipe name
        /// </summary>
        string PipeName { get; }

        /// <summary>
        /// Connection state
        /// </summary>
        ConnectionState ConnectionState { get; }
    }
}