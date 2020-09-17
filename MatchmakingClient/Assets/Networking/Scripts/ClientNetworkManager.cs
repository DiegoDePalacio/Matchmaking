using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Client.Networking
{
    public class ClientNetworkManager : MonoBehaviour
    {
        [SerializeField] private ClientConfiguration m_ClientConfiguration = null;

        private NetworkClient m_Client = new NetworkClient();
        private int m_ServerConnectionId = -1;

        // Sending the strings to not create an additional dependency in this class
        public Action<string> OnReceivePlayerRatingCallback;
        public Action<string> OnPlayerRemovedFromLobbyCallback;
        
        public Action<string> OnReceiveNotificationCallback;
        
        private void Awake()
        {
            if (m_ClientConfiguration == null)
                throw new NullReferenceException("There is no Server Configuration assigned!");

            Application.runInBackground = true;
        }

        private void Start()
        {
            var config = new ConnectionConfig();
            config.AddChannel(QosType.Reliable);

            m_Client.Configure(config, 1);

            // Connecting...
            m_Client.Connect(m_ClientConfiguration.ServerIP, m_ClientConfiguration.ServerPort);

            // System handles
            m_Client.RegisterHandler(MsgType.Connect, ClientConnected);
            m_Client.RegisterHandler(MsgType.Disconnect, ClientDisconnected);

            // Custom receiving message handle
            m_Client.RegisterHandler((short)CustomMsgType.Notification, OnReceiveNotificationMessage);
            m_Client.RegisterHandler((short)CustomMsgType.PlayerRating, OnReceivePlayerRatingMessage);
            m_Client.RegisterHandler((short)CustomMsgType.PlayerUpdate, OnReceivePlayerUpdateMessage);
            m_Client.RegisterHandler((short)CustomMsgType.PlayerRemovedFromLobby, OnPlayerRemovedFromLobby);
        }

        private void ClientDisconnected(NetworkMessage networkMessage)
        {
            Debug.Log("ClientDisconnected");

            StringMessage messageContainer = new StringMessage
            {
                value = "Client Disconnected!"
            };

            // Say hi to the server when connected
            m_Client.Send((short)CustomMsgType.Notification, messageContainer);
        }

        private void ClientConnected(NetworkMessage networkMessage)
        {
            Debug.Log("ClientConnected " + networkMessage.conn.connectionId);

            StringMessage messageContainer = new StringMessage
            {
                value = "Client Connected!"
            };

            m_Client.Send((short)CustomMsgType.Notification, messageContainer);
        }

        private void OnReceiveNotificationMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            Debug.Log("OnReceiveNotificationMessage " + stringMsg.value);
            OnReceiveNotificationCallback(stringMsg.value);
        }

        private void OnReceivePlayerRatingMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            Debug.Log("OnReceivePlayerRatingMessage " + stringMsg.value);
            OnReceivePlayerRatingCallback(stringMsg.value);
        }

        private void OnReceivePlayerUpdateMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            Debug.Log("OnReceivePlayerUpdateMessage " + stringMsg.value);
        }

        // Send message to Client
        private void OnPlayerRemovedFromLobby(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            Debug.Log("OnPlayerRemovedFromLobby " + stringMsg.value);
            OnPlayerRemovedFromLobbyCallback(stringMsg.value);
        }

        public void SendStringMessage(string message, CustomMsgType msgType)
        {
            StringMessage stringMsg = new StringMessage
            {
                value = message
            };
            m_Client.Send((short)msgType, stringMsg);
        }
        
#region Tests
#if UNITY_EDITOR
        [ContextMenu("[TEST] ClientSentStringMessage")]
        public void TestClientMessage()
        {
            SendStringMessage("Hello Server... Are you fine?", CustomMsgType.Notification);
        }
#endif        
#endregion
    }
}