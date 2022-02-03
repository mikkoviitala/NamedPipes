using System;
using NamedPipes.Messages;

namespace NamedPipes.TestClient
{
    /// <summary>
    /// Test application
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // Check input arguments
            SanityCheck(args);

            // Create pipe client/server
            var pipeType = args[0];
            var pipeName = args[1];

            IPipe pipe = pipeType.Equals("server", StringComparison.OrdinalIgnoreCase)
                ? (IPipe) new PipeServer(pipeName)
                : (IPipe) new PipeClient(pipeName);

            // Show some usage information
            ShowInstructions(pipe);

            // Register for events
            pipe.OnConnectionStateChanged += (sender, e) =>
                Console.WriteLine($"State > {e.ConnectionState.ToString()}");

            pipe.OnMessageSent += (sender, e) =>
                Console.WriteLine($"Send > {e.Message}");

            pipe.OnMessageReceived += (sender, e) =>
            {
                Console.WriteLine($"Received > {e.Message}");

                var parsed = MessageBase.Deserialize(e.Message);
                if (parsed is ExampleMessage example)
                {
                    Console.WriteLine($"Received > Type={example.GetType()}");
                    Console.WriteLine($"Received > Id={example.Id}");
                }
            };

            // Process user input
            string input;
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("example", StringComparison.OrdinalIgnoreCase))
                    pipe.Send(new ExampleMessage(Math.Abs((int) DateTime.Now.Ticks)));
                else if (input.Equals("open", StringComparison.OrdinalIgnoreCase))
                    pipe.Open();
                else if (input.Equals("close", StringComparison.OrdinalIgnoreCase))
                    pipe.Close();
                else if (!input.Equals("q", StringComparison.OrdinalIgnoreCase))
                    pipe.Send(input);

            } while (input != null && !input.Equals("q", StringComparison.OrdinalIgnoreCase));

            pipe.Dispose();
        }

        /// <summary>
        /// Check input arguments
        /// </summary>
        /// <param name="args"></param>
        private static void SanityCheck(string[] args)
        {
            if (args == null || args.Length != 2)
                OnArgsError();

            if (!args[0].Equals("server", StringComparison.InvariantCulture) &&
                !args[0].Equals("client", StringComparison.InvariantCulture))
                OnArgsError();

            if (string.IsNullOrWhiteSpace(args[1]))
                OnArgsError();
        }

        /// <summary>
        /// Report arguments error
        /// </summary>
        private static void OnArgsError()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("test server [pipe-name]");
            Console.WriteLine("test client [pipe-name]");
            Environment.Exit(-1);
        }

        /// <summary>
        /// Show usage
        /// </summary>
        /// <param name="pipe"></param>
        private static void ShowInstructions(IPipe pipe)
        {
            Console.WriteLine();
            Console.WriteLine($"Starting {pipe.GetType().Name} on pipe {pipe.PipeName}");
            Console.WriteLine();
            Console.WriteLine("> Type \"open\" and press ENTER to open connection");
            Console.WriteLine("> Type \"close\" and press ENTER to close connection");
            Console.WriteLine("> Type \"example\" and press ENTER to send example message");
            Console.WriteLine("> Type [any string] and press ENTER to send any string");
            Console.WriteLine("> Type \"q\" and press ENTER to quit");
            Console.WriteLine();
        }
    }
}
