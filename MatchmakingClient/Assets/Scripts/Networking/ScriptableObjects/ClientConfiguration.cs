using UnityEngine;

namespace MM.Client.Networking
{
    [CreateAssetMenu(fileName = "ClientConfiguration", menuName = "ScriptableObjects/ClientConfiguration")]
    public class ClientConfiguration : ScriptableObject
    {
        public string ServerIP = "127.0.0.1"; // Local address by default
        public int ServerPort = 8888;
    }
}