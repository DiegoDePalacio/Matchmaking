using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace MM.Server.Networking
{
// This class is using extensively the Unity Networking high level API classes that are deprecated and will be removed
// in the future. The new API is not yet ready and I didn't want to use a 3rd party library because of the time 
// limitations of this coding challenge. For that reason the CS0618 is disabled for the entire class.
#pragma warning disable 618    
    public class ServerNetworkManager : MonoBehaviour
    {
        [SerializeField] private ServerConfiguration m_ServerConfiguration = null;

        private List<int> m_ConnectionIds = new List<int>();
        public bool AreClientsConnected => (m_ConnectionIds.Count > 0);

        // The int on this callback is the connection ID
        public Action<int> OnClientConnectedCallback;
        
        // The string in these callbacks is the Name of the player
        public Action<string> OnPlayerJoinsCallback;
        public Action<string> OnPlayerLeavesCallback;

        // The string in this callback is the notification body
        public Action<string> OnReceiveNotificationCallback;

        public Action OnAllClientsDisconnectedCallback;
        
        private void Awake()
        {
            if (m_ServerConfiguration == null)
                throw new NullReferenceException("There is no Server Configuration assigned!");

            Application.runInBackground = true;
        }

        private void Start()
        {
            // System handles
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);

            // Custom receiving message handle
            NetworkServer.RegisterHandler((short)CustomMsgType.Notification, OnReceiveNotificationMessage);
            NetworkServer.RegisterHandler((short)CustomMsgType.PlayerJoinsLobby, OnReceivePlayerJoinsLobbyMessage);
            NetworkServer.RegisterHandler((short)CustomMsgType.PlayerLeavesLobby, OnReceivePlayerLeavesLobbyMessage);

            var config = new ConnectionConfig();
            config.AddChannel(QosType.Reliable);

            var hostTopology = new HostTopology(config, m_ServerConfiguration.MaxConnections);

            if (!NetworkServer.Configure(hostTopology))
            {
                Debug.LogError("No server created, error on the configuration definition");
                return;
            }

            if (!NetworkServer.Listen(m_ServerConfiguration.ListeningPort))
            {
                Debug.Log($"No server created, could not listen to the port: {m_ServerConfiguration.ListeningPort}");
                return;
            }
        }

        private void OnApplicationQuit()
        {
            NetworkServer.Shutdown();
        }

        private void OnClientDisconnected(NetworkMessage networkMessage)
        {
            var connectionId = networkMessage.conn.connectionId;
            m_ConnectionIds.Remove(connectionId);

            if (m_ConnectionIds.Count == 0)
            {
                OnAllClientsDisconnectedCallback();
                OnReceiveNotificationCallback?.Invoke("No clients connected!");
                Debug.Log("All clients was disconnected");
            }
            else
            {
                Debug.Log("A client was disconnected");
            }
        }

        private void OnClientConnected(NetworkMessage networkMessage)
        {
            var connectionId = networkMessage.conn.connectionId;
            m_ConnectionIds.Add(connectionId);

           SendMessageToClient(connectionId, "Client connected!", CustomMsgType.Notification);

           OnClientConnectedCallback?.Invoke(connectionId);
           OnReceiveNotificationCallback?.Invoke("Client connected!");
           Debug.Log($"OnClientConnected with connection ID: {networkMessage.conn.connectionId}");
        }

        private void OnReceiveNotificationMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnReceiveNotificationCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnReceiveNotificationMessage: {stringMsg.value}");
        }
        
        private void OnReceivePlayerJoinsLobbyMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnPlayerJoinsCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnReceivePlayerJoinsLobbyMessage: {stringMsg.value}");
        }

        private void OnReceivePlayerLeavesLobbyMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnPlayerLeavesCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnReceivePlayerLeavesLobbyMessage: {stringMsg.value}");
        }

        public void SendMessageToClient(int connectionId, string message, CustomMsgType msgType)
        {
            StringMessage stringMsg = new StringMessage
            {
                value = message
            };
            
            NetworkServer.SendToClient(connectionId, (short)msgType, stringMsg);
        }
        
        public void SendMessageToAll(string message, CustomMsgType msgType)
        {
            StringMessage stringMsg = new StringMessage
            {
                value = message
            };
            
            NetworkServer.SendToAll((short)msgType, stringMsg);
        }
        
#region Tests
#if UNITY_EDITOR
        [ContextMenu("[TEST] SendTestMessageToFirstConnectedClient")]
        public void TestSendMessageToFirstConnectedClient()
        {
            SendMessageToClient(m_ConnectionIds[0], "Hey Client... How are you doing?", CustomMsgType.Notification);
        }
#endif
#endregion
    }
#pragma warning restore 618    
}