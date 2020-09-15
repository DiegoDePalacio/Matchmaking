using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Server : MonoBehaviour
{
    [SerializeField] private ServerConfiguration m_ServerConfiguration = null;
    
    private List<int> m_ConnectionIds = new List<int>();

    private void Awake()
    {
        if (m_ServerConfiguration == null)
            throw new NullReferenceException("There is no Server Configuration assigned!");
    }

    void Start ()
    {
        Application.runInBackground = true;
        
        // System handles
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);
         
        // Custom receiving message handle
        NetworkServer.RegisterHandler(CustomMsgType.String, OnServerReceiveStringMessage);
        
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
            Debug.Log ("No server created, could not listen to the port: " + m_ServerConfiguration.ListeningPort);
            return;
        }
    }

    private void OnApplicationQuit()
    {
        NetworkServer.Shutdown();
    }

    private void OnClientDisconnected(NetworkMessage netmsg)
    {
        Debug.Log("OnClientDisconnected");
    }

    private void OnClientConnected(NetworkMessage netMessage)
    {
        Debug.Log("OnClientConnected " + netMessage.conn.connectionId);

        // Send a thank you message to the client that just connected
        StringMessage messageContainer = new StringMessage
        {
            value = "Thanks for joining!"
        };
        m_ConnectionIds.Add(netMessage.conn.connectionId);
        
        // This sends a message to a specific client, using the connectionId
        NetworkServer.SendToClient(netMessage.conn.connectionId, CustomMsgType.String, messageContainer);
    }

    // Receive message
    void OnServerReceiveStringMessage(NetworkMessage netMsg)
    {
        var stringMsg = netMsg.ReadMessage<StringMessage>();
        Debug.Log("You string text: " + stringMsg.value);
    } 
 
    // Send message to specific Client
    public void OnServerSendStringMessage(int connectionId)
    {
        StringMessage stringMsg = new StringMessage
        {
            value = "Your message here, write what u wanna do"
        };
        NetworkServer.SendToClient(connectionId, CustomMsgType.String, stringMsg);
    }
#region QuickTests
    [ContextMenu("SendTestMessageToFirstConnectedClient")]
    public void SendTestMessageToFirstConnectedClient()
    {
        OnServerSendStringMessage(m_ConnectionIds[0]);
    }
#endregion
}