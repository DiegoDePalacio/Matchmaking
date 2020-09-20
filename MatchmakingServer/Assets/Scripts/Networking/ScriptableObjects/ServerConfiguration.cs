using UnityEngine;

namespace MM.Server.Networking
{
    [CreateAssetMenu(fileName = "ServerConfiguration", menuName = "ScriptableObjects/ServerConfiguration")]
    public class ServerConfiguration : ScriptableObject
    {
        public int ListeningPort = 8888;
        public int MaxConnections = 10000;
    }    
}