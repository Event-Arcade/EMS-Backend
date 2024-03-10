

using System.Collections.Concurrent;

public class SharedDb
{
    private readonly ConcurrentDictionary<string, UserConnection> _connections = new ();
    public ConcurrentDictionary<string, UserConnection> connections => _connections;


}