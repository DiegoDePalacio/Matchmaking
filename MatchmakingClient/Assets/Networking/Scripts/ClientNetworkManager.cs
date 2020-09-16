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

        private void Awake()
        {
            if (m_ClientConfiguration == null)
                throw new NullReferenceException("There is no Server Configuration assigned!");
        }

        void Start()
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
            m_Client.RegisterHandler(CustomMsgType.String, ClientReceiveStringMessage);
        }

        private void ClientDisconnected(NetworkMessage netmsg)
        {
            Debug.Log("ClientDisconnected");
        }

        private void ClientConnected(NetworkMessage netmsg)
        {
            Debug.Log("ClientConnected " + netmsg.conn.connectionId);

            StringMessage messageContainer = new StringMessage
            {
                value = "Hello server!"
            };

            // Say hi to the server when connected
            m_Client.Send(CustomMsgType.String, messageContainer);
        }

        // Receive message
        void ClientReceiveStringMessage(NetworkMessage netMsg)
        {
            var stringMsg = netMsg.ReadMessage<StringMessage>();
            Debug.Log("Your message is " + stringMsg.value);
        }

        // Send message to Client
        public void OnClientSendStringMessage()
        {
            StringMessage stringMsg = new StringMessage
            {
                value = "Your message here, write what u wanna do"
            };
            m_Client.Send(CustomMsgType.String, stringMsg);
        }
        
#region Tests
#if UNITY_EDITOR
        [ContextMenu("[TEST] ClientSentStringMessage")]
        public void TestClientMessage()
        {
            OnClientSendStringMessage();
        }
#endif        
#endregion
    }
}