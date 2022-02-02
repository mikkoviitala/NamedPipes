using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NamedPipes.Messages
{
    /// <summary>
    /// MessageBase
    /// </summary>
    /// <remarks>
    /// Base implementation for all message types you want to support
    /// Any new message must inherit from this common base and provide unique MessageType
    /// </remarks>
    public abstract class MessageBase
    {
        private static readonly Dictionary<string, Type> MessageTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Message type (abstract)
        /// </summary>
        public abstract string MessageType { get; }

        /// <summary>
        /// Serialize (static)
        /// </summary>
        public static string Serialize(MessageBase message)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    var serializer = new XmlSerializer(message.GetType());
                    serializer.Serialize(writer, message);
                }

                stream.Position = 0;
                using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Deserialize (static)
        /// </summary>
        public static object Deserialize(string message)
        {
            object deserialized = null;

            try
            {
                var messageType = GetMessageType(message);
                if (messageType != null)
                {
                    var serializer = new XmlSerializer(messageType.Item2);
                    var serialized = message.Substring(messageType.Item1.Length);

                    using (TextReader reader = new StringReader(serialized))
                        deserialized = serializer.Deserialize(reader);
                }
            }
            catch // Let it fail
            {}

            return deserialized;
        }

        /// <summary>
        /// Figure out message type from input string
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static Tuple<string, Type> GetMessageType(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return null;

            if (!MessageTypes.Any())
            {
                var messageBase = typeof(MessageBase);
                var messageClasses = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type =>
                        type.IsClass &&
                        type != messageBase &&
                        messageBase.IsAssignableFrom(type))
                    .ToList();

                foreach (var messageClass in messageClasses)
                {
                    var messageInstance = Activator.CreateInstance(messageClass);
                    var typeProperty = messageInstance.GetType().GetProperty("MessageType");

                    if (typeProperty == null)
                        continue;

                    var messageType = (string) typeProperty.GetValue(messageInstance, null);
                    if (string.IsNullOrWhiteSpace(messageType))
                        continue;

                    MessageTypes.Add(messageType, messageClass);
                }
            }

            var key = MessageTypes.Keys.FirstOrDefault(str => message.StartsWith(str, StringComparison.InvariantCulture));

            return key != null
                ? new Tuple<string, Type>(key, MessageTypes[key])
                : null;
        }
    }
}