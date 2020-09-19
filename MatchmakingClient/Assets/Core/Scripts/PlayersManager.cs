using System;
using System.Collections.Generic;
using MM.Client.Networking;
using MM.Client.UI;
using Server.Data;
using UnityEngine;

namespace MM.Client.Core
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
            m_ClientNetworkManager.OnReceiveNotificationCallback += DisplayNotification;
            m_ClientNetworkManager.OnPlayerRemovedFromLobbyCallback += OnPlayerRemovedFromLobby;
            m_ClientNetworkManager.OnPlayerOnMatchCallback += OnPlayerOnMatch;
            m_ClientNetworkManager.OnPlayerUpdateAfterMatchCallback += OnPlayerUpdateAfterAMatch;
        }

        private void OnDisable()
        {
            m_ClientNetworkManager.OnPlayerUpdateAfterMatchCallback -= OnPlayerUpdateAfterAMatch;
            m_ClientNetworkManager.OnPlayerOnMatchCallback -= OnPlayerOnMatch;
            m_ClientNetworkManager.OnPlayerRemovedFromLobbyCallback -= OnPlayerRemovedFromLobby;
            m_ClientNetworkManager.OnReceiveNotificationCallback -= DisplayNotification;
            m_ClientNetworkManager.OnReceivePlayerRatingCallback -= OnReceivePlayersRatings;
        }

        private void OnPlayerUpdateAfterAMatch(string playerJson)
        {
            var playerData = JsonUtility.FromJson<PlayerBasicData>(playerJson);
            var playerName = playerData.Name;
            
            m_Players[playerName].Data = playerData;
            m_Players[playerName].State = PlayerState.Inactive;
            
            var playerListElement = m_ClientMenuUI.PlayerList.Elements[playerName];
            playerListElement.SetPlayerData(m_Players[playerName]);
            
            DisplayNotification("A match just finished and the players ratings was updated!");
        }

        private void OnPlayerOnMatch(string playerName)
        {
            var playerListElement = m_ClientMenuUI.PlayerList.Elements[playerName];
            playerListElement.SetButtonAction(PlayerListElement.ButtonState.Playing);
            m_Players[playerName].State = PlayerState.Playing;
        }

        private void OnPlayerRemovedFromLobby(string playerName)
        {
            var playerListElement = m_ClientMenuUI.PlayerList.Elements[playerName];
            playerListElement.SetButtonAction(PlayerListElement.ButtonState.Join);
            m_Players[playerName].State = PlayerState.Inactive;
        }

        private void DisplayNotification(string notification)
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