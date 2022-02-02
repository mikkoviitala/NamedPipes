namespace NamedPipes.Events
{
    /// <summary>
    /// Connection state enumeration
    /// </summary>
    public enum ConnectionState
    {
        Disconnected = 0,
        WaitingForConnection,
        Connected
    }
}