using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NamedPipes.Events;

namespace NamedPipes.UnitTest
{
    [TestClass]
    public class PipeTests
    {
        [TestMethod]
        public void CreatedCorrectly()
        {
            var server = new PipeServer("test-pipe");
            Assert.IsNotNull(server);
            Assert.AreEqual(typeof(PipeServer), server.GetType());
            Assert.AreEqual("test-pipe", server.PipeName);

            var client = new PipeClient("test-pipe");
            Assert.IsNotNull(client);
            Assert.AreEqual(typeof(PipeClient), client.GetType());
            Assert.AreEqual("test-pipe", client.PipeName);
        }

        [TestMethod]
        public void StateChangedCorrectly()
        {
            var client = new PipeClient("test-pipe");
            Assert.AreEqual(ConnectionState.Disconnected, client.ConnectionState);

            client.Open();
            Assert.AreEqual(ConnectionState.WaitingForConnection, client.ConnectionState);

            client.Close();
            Assert.AreEqual(ConnectionState.Disconnected, client.ConnectionState);
        }

        [TestMethod]
        public void RaisesEventsCorrectly()
        {
            int clientMessagesSent = 0;
            int serverMessagesReceived = 0;

            var client = new PipeClient("test-pipe");
            client.OnMessageSent += (sender, args) => clientMessagesSent++;

            var server = new PipeServer("test-pipe");
            server.OnMessageReceived += (sender, args) => serverMessagesReceived++;

            Assert.AreEqual(0, clientMessagesSent);
            Assert.AreEqual(0, serverMessagesReceived);

            server.Open();
            client.Open();
            Thread.Sleep(200);

            client.Send("test");
            Thread.Sleep(200);

            server.Close();
            client.Close();

            Assert.AreEqual(1, clientMessagesSent);
            Assert.AreEqual(1, serverMessagesReceived);
        }
    }
}
