using UnityEngine;

[CreateAssetMenu(fileName = "ServerConfiguration", menuName = "ScriptableObjects/ServerConfiguration", order = 1)]
public class ServerConfiguration : ScriptableObject
{
    public int ListeningPort = 8888;
    public int MaxConnections = 10000;
}