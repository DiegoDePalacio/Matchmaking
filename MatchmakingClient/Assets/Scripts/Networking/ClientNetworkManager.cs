using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace MM.Client.Networking
{
// This class is using extensively the Unity Networking high level API classes that are deprecated and will be removed
// in the future. The new API is not yet ready and I didn't want to use a 3rd party library because of the time 
// limitations of this coding challenge. For that reason the CS0618 is disabled for the entire class.
#pragma warning disable 618    
    public class ClientNetworkManager : MonoBehaviour
    {
        [SerializeField] private ClientConfiguration m_ClientConfiguration = null;

        private NetworkClient m_Client = new NetworkClient();

        // The string in this callback is the PlayerBasicData in Json string (instead of PlayerBasicData) to not create an additional dependency in this class
        public Action<string> OnPlayerUpdateAfterMatchCallback;
        
        // The string in these callbacks is the Name of the Player
        public Action<string> OnReceivePlayerRatingCallback;
        public Action<string> OnPlayerRemovedFromLobbyCallback;
        public Action<string> OnPlayerOnMatchCallback;
        
        // The string in this callback is the body of the Notification
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

            m_Client.Connect(m_ClientConfiguration.ServerIP, m_ClientConfiguration.ServerPort);

            m_Client.RegisterHandler(MsgType.Connect, ClientConnected);
            m_Client.RegisterHandler(MsgType.Disconnect, ClientDisconnected);

            m_Client.RegisterHandler((short)CustomMsgType.Notification, OnReceiveNotificationMessage);
            m_Client.RegisterHandler((short)CustomMsgType.PlayerRating, OnReceivePlayerRatingMessage);
            m_Client.RegisterHandler((short)CustomMsgType.PlayerUpdateAfterMatch, OnReceivePlayerUpdateMessage);
            m_Client.RegisterHandler((short)CustomMsgType.PlayerRemovedFromLobby, OnPlayerRemovedFromLobby);
            m_Client.RegisterHandler((short)CustomMsgType.PlayerOnMatch, OnPlayerOnMatch);
        }

        private void ClientDisconnected(NetworkMessage networkMessage)
        {
            StringMessage messageContainer = new StringMessage
            {
                value = "Client Disconnected!"
            };

            OnReceiveNotificationCallback?.Invoke("Connection lost :( Closing the Client...");            
            Invoke("CloseClient", 5f);
            Debug.Log("ClientDisconnected");
        }

        private void CloseClient()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ClientConnected(NetworkMessage networkMessage)
        {
            StringMessage messageContainer = new StringMessage
            {
                value = "Client Connected!"
            };

            m_Client.Send((short)CustomMsgType.Notification, messageContainer);
            Debug.Log($"Successful connection with id {networkMessage.conn.connectionId}");
        }

        private void OnReceiveNotificationMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnReceiveNotificationCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnReceiveNotificationMessage {stringMsg.value}");
        }

        private void OnReceivePlayerRatingMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnReceivePlayerRatingCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnReceivePlayerRatingMessage {stringMsg.value}");
        }

        private void OnReceivePlayerUpdateMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnPlayerUpdateAfterMatchCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnReceivePlayerUpdateMessage {stringMsg.value}");
        }

        private void OnPlayerRemovedFromLobby(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnPlayerRemovedFromLobbyCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnPlayerRemovedFromLobby {stringMsg.value}");
        }

        private void OnPlayerOnMatch(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnPlayerOnMatchCallback?.Invoke(stringMsg.value);
            Debug.Log($"OnPlayerOnMatch {stringMsg.value}");
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
            SendStringMessage("Hello Server... Are you okay?", CustomMsgType.Notification);
        }
#endif        
#endregion
    }
#pragma warning restore 618
}