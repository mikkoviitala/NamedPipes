namespace NamedPipes.Messages
{
    /// <summary>
    /// Example message
    /// </summary>
    public class ExampleMessage : MessageBase
    {
        /// <summary>
        /// Needed for de/serialization
        /// </summary>
        public ExampleMessage()
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public ExampleMessage(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Id
        /// </summary>
        /// <remarks>
        /// Add some properties you need to pass around
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        /// <remarks>
        /// Needed to figure out how to deserialize this message.
        /// Use any string that is easy to understand but yet something you wouldn't send as "plain string"
        /// </remarks>
        public override string MessageType => "::example::";
    }
}
