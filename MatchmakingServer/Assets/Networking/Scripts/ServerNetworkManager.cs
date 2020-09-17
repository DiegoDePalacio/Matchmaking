﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Server.Networking
{
    public class ServerNetworkManager : MonoBehaviour
    {
        [SerializeField] private ServerConfiguration m_ServerConfiguration = null;

        private List<int> m_ConnectionIds = new List<int>();

        public Action<int> OnClientConnectedCallback;
        
        // Sending the strings to not create an additional dependency in this class
        public Action<string> OnPlayerJoinsCallback;
        public Action<string> OnPlayerLeavesCallback;

        public Action<string> OnReceiveNotificationCallback;

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

            var ht = new HostTopology(config, m_ServerConfiguration.MaxConnections);

            if (!NetworkServer.Configure(ht))
            {
                Debug.LogError("No server created, error on the configuration definition");
                return;
            }

            if (!NetworkServer.Listen(m_ServerConfiguration.ListeningPort))
            {
                Debug.Log("No server created, could not listen to the port: " + m_ServerConfiguration.ListeningPort);
                return;
            }
        }

        private void OnApplicationQuit()
        {
            NetworkServer.Shutdown();
        }

        private void OnClientDisconnected(NetworkMessage networkMessage)
        {
            Debug.Log("OnClientDisconnected");
        }

        private void OnClientConnected(NetworkMessage networkMessage)
        {
            var connId = networkMessage.conn.connectionId;
            Debug.Log("OnClientConnected " + networkMessage.conn.connectionId);
            m_ConnectionIds.Add(connId);

            // This sends a message to a specific client, using the connectionId
           SendMessageToClient(connId, "Client connected!", CustomMsgType.Notification);

           OnClientConnectedCallback(connId);
           OnReceiveNotificationCallback("Client connected!");
        }

        private void OnReceiveNotificationMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            Debug.Log("OnReceiveNotificationMessage: " + stringMsg.value);
            OnReceiveNotificationCallback(stringMsg.value);
        }

        private void OnReceivePlayerJoinsLobbyMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnPlayerJoinsCallback(stringMsg.value);
            Debug.Log("OnReceivePlayerJoinsLobbyMessage: " + stringMsg.value);
        }

        private void OnReceivePlayerLeavesLobbyMessage(NetworkMessage networkMessage)
        {
            var stringMsg = networkMessage.ReadMessage<StringMessage>();
            OnPlayerLeavesCallback(stringMsg.value);
            Debug.Log("OnReceivePlayerLeavesLobbyMessage: " + stringMsg.value);
        }

        // Send message to specific Client
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
        
        // // This first version will only communicate with one client that is "handling" all the mockup clients at once
        // public void SendMessage(string message, CustomMsgType msgType)
        // {
        //     OnServerSendStringMessage(m_ConnectionIds[0], message, msgType);
        // }
        
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
}