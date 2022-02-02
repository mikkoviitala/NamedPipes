using System.IO.Pipes;
using NamedPipes.Events;

namespace NamedPipes
{
    /// <summary>
    /// PipeServer
    /// </summary>
    public class PipeServer : PipeBase
    {
        /// <summary>
        /// Constructor, calls base
        /// </summary>
        /// <param name="pipeName">name</param>
        public PipeServer(string pipeName)
            :base(pipeName)
        {}
        
        /// <summary>
        /// Override Open
        /// </summary>
        public override async void Open()
        {
            if (PipeStream != null)
                return;

            PipeStream = CreateServer();
            RaiseConnectionStateChanged(ConnectionState.WaitingForConnection);

            await ((NamedPipeServerStream)PipeStream).WaitForConnectionAsync();
            await Receive();
            base.Open();
        }

        /// <summary>
        /// Create server
        /// </summary>
        /// <returns>NamedPipeServerStream</returns>
        private NamedPipeServerStream CreateServer()
        {
            return new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
        }
    }
}
