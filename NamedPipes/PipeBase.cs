using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using NamedPipes.Events;
using NamedPipes.Messages;

namespace NamedPipes
{
    /// <summary>
    /// PipeBase
    /// Base PipeStream implementation, shared by client and server
    /// </summary>
    public abstract class PipeBase : IPipe
    {
        public event EventHandler<ConnectionStateEventArgs> OnConnectionStateChanged;
        public event EventHandler<MessageEventArgs> OnMessageSent;
        public event EventHandler<MessageEventArgs> OnMessageReceived;
        private bool _disposed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pipeName"></param>
        protected PipeBase(string pipeName)
        {
            if (string.IsNullOrWhiteSpace(pipeName))
                throw new ArgumentException("pipeName must be defined", nameof(pipeName));

            PipeName = pipeName;
        }

        /// <summary>
        /// Pipe name
        /// </summary>
        public string PipeName { get; }

        /// <summary>
        /// Connection state
        /// </summary>
        public ConnectionState ConnectionState { get; protected set; } = ConnectionState.Disconnected;

        /// <summary>
        /// Pipe stream
        /// </summary>
        protected PipeStream PipeStream { get; set; }

        /// <summary>
        /// Open connection
        /// </summary>
        public virtual void Open()
        {
            ConnectionStateChanged();
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            if (PipeStream == null)
                return;

            try
            {
                if (PipeStream is NamedPipeServerStream server)
                    server.Disconnect();
            }
            catch (InvalidOperationException)
            {}

            PipeStream.Close();
            PipeStream = null;
            ConnectionStateChanged();
        }

        /// <summary>
        /// Send message (plain string)
        /// </summary>
        public void Send(string message)
        {
            if (PipeBroken() || string.IsNullOrWhiteSpace(message))
                return;

            try
            {
                var writer = new StreamWriter(PipeStream) {AutoFlush = true};
                writer.WriteLine(message);
                PipeStream.WaitForPipeDrain();

                OnMessageSent?.Invoke(this, new MessageEventArgs(message));
            }
            catch (IOException)
            {
                ConnectionStateChanged();
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        public void Send(MessageBase message)
        {
            if (PipeBroken() || message == null)
                return;

            var knownMessage = $"{message.MessageType}{MessageBase.Serialize(message)}";
            Send(knownMessage);
        }

        /// <summary>
        /// Implement IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
                return;

            PipeStream?.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Receive message
        /// </summary>
        protected async Task Receive()
        {
            await Task.Factory.StartNew(async () =>
            {
                var reader = new StreamReader(PipeStream);
                while (PipeStream.IsConnected)
                {
                    try
                    {
                        var message = await reader.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(message))
                            OnMessageReceived?.Invoke(this, new MessageEventArgs(message));
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }
                Reset();
            });
        }

        /// <summary>
        /// Raise connection changed event
        /// </summary>
        protected void RaiseConnectionStateChanged(ConnectionState connectionState)
        {
            ConnectionState = connectionState;
            OnConnectionStateChanged?.Invoke(this, new ConnectionStateEventArgs(connectionState));
        }

        /// <summary>
        /// Reset connection
        /// </summary>
        private void Reset()
        {
            Close();
            Open();
        }

        /// <summary>
        /// Check connection state and raise event
        /// </summary>
        private void ConnectionStateChanged()
        {
            bool isConnected = !PipeBroken();

            var connectionState = isConnected
                ? ConnectionState.Connected
                : ConnectionState.Disconnected;

            RaiseConnectionStateChanged(connectionState);
        }

        /// <summary>
        /// Check if pipe is broken
        /// </summary>
        /// <remarks>
        /// Stream's IsConnected value is not updated when client/server drops unexpectedly.
        /// Connection state can then be determined while trying to send a message and IOException is thrown
        /// </remarks>
        private bool PipeBroken()
        {
            return PipeStream == null || !PipeStream.IsConnected;
        }
    }
}
