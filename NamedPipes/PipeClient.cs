using System.IO.Pipes;
using NamedPipes.Events;

namespace NamedPipes
{
    /// <summary>
    /// PipeClient
    /// </summary>
    public class PipeClient : PipeBase
    {
        /// <summary>
        /// Constructor, calls base
        /// </summary>
        /// <param name="pipeName">name</param>
        public PipeClient(string pipeName)
            :base(pipeName)
        {}

        /// <summary>
        /// Override Open
        /// </summary>
        public override async void Open()
        {
            if (PipeStream != null)
                return;

            PipeStream = CreateClient();
            RaiseConnectionStateChanged(ConnectionState.WaitingForConnection);

            await ((NamedPipeClientStream)PipeStream).ConnectAsync();
            await Receive();
            base.Open();
        }

        /// <summary>
        /// Create client
        /// </summary>
        /// <returns>NamedPipeClientStream</returns>
        private NamedPipeClientStream CreateClient()
        {
            return new NamedPipeClientStream(".",
                PipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);
        }
    }
}
