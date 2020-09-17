using System;
using System.Collections.Generic;
using Client.Networking;
using Client.UI;
using Server.Data;
using UnityEngine;

namespace Client.Core
{
    public class PlayersManager : MonoBehaviour
    {
        [SerializeField] private ClientNetworkManager m_ClientNetworkManager;
        [SerializeField] private ClientMenuUI m_ClientMenuUI;
        
        private Dictionary<string, Player> m_Players = new Dictionary<string, Player>();
        
        private void Awake()
        {
            if (m_ClientNetworkManager == null)
            {
                throw new NullReferenceException(
                    "The Client Network Manager is not referenced in the Players Manager!");
            }
        }

        private void OnEnable()
        {
            m_ClientNetworkManager.OnReceivePlayerRatingCallback += OnReceivePlayersRatings;
            m_ClientNetworkManager.OnReceiveNotificationCallback += OnReceiveNotification;
            m_ClientNetworkManager.OnPlayerRemovedFromLobbyCallback += OnPlayerRemovedFromLobby;
        }

        private void OnDisable()
        {
            m_ClientNetworkManager.OnPlayerRemovedFromLobbyCallback -= OnPlayerRemovedFromLobby;
            m_ClientNetworkManager.OnReceiveNotificationCallback -= OnReceiveNotification;
            m_ClientNetworkManager.OnReceivePlayerRatingCallback -= OnReceivePlayersRatings;
        }

        private void OnPlayerRemovedFromLobby(string playerName)
        {
            var playerListElement = m_ClientMenuUI.PlayerList.Elements[playerName];
            playerListElement.SetButtonAction(PlayerListElement.ButtonState.Join);
            m_Players[playerName].State = PlayerState.Active;
        }

        private void OnReceiveNotification(string notification)
        {
            m_ClientMenuUI.NotificationManager.titleObj.text = "Notification";
            m_ClientMenuUI.NotificationManager.descriptionObj.text = notification;
            m_ClientMenuUI.NotificationManager.OpenNotification();
        }

        private void OnReceivePlayersRatings(string playersRatingsJson)
        {
            var playerData = JsonUtility.FromJson<Player>(playersRatingsJson);
            m_Players.Add(playerData.Data.Name, playerData);
            
            var listElement = m_ClientMenuUI.PlayerList.AddPlayer(playerData);
            
            // This callback is cleared when the element of the list is removed
            listElement.OnButtonClickCallback += OnPlayerButtonClick;
        }

        private void OnPlayerButtonClick(PlayerListElement playerListElement)
        {
            var playerName = playerListElement.PlayerName;
            var playerData = m_Players[playerName];
            
            if (playerListElement.Action == PlayerListElement.ButtonState.Join)
            {
                m_ClientNetworkManager.SendStringMessage(playerName, CustomMsgType.PlayerJoinsLobby);
            }
            else if (playerListElement.Action == PlayerListElement.ButtonState.Drop)
            {
                m_ClientNetworkManager.SendStringMessage(playerName, CustomMsgType.PlayerLeavesLobby);    
            }
        }
    }
}