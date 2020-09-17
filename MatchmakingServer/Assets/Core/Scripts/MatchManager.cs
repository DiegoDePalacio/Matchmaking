using System;
using System.Collections.Generic;
using Server.Data;
using Server.Networking;
using Server.UI;
using UnityEngine;

namespace Server.Core
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] private ServerNetworkManager m_ServerNetworkManager;
        [SerializeField] private ServerMenuUI m_ServerMenuUI;
        [SerializeField] private PlayersRatingsFromJson m_PlayersRatingsFromJson;

        private Dictionary<string, Player> m_PlayersData = new Dictionary<string, Player>();

        private void Awake()
        {
            if (m_ServerNetworkManager == null)
                throw new NullReferenceException(
                    "There is no reference to the Server Network Manager on the MatchManager!");

            if (m_ServerMenuUI == null)
                throw new NullReferenceException("There is no reference to the Server Menu UI on the MatchManager!");

            if (m_PlayersRatingsFromJson == null)
                throw new NullReferenceException(
                    "There is no reference to the JSON file where the player ratings are located!");

            var playersBasicData = m_PlayersRatingsFromJson.GetData().Players;
            var playersBasicDataList = new List<PlayerBasicData>(playersBasicData);
            playersBasicDataList.Sort((playerA, playerB) => playerB.Rating.CompareTo(playerA.Rating));

            for (var i = 0; i < playersBasicDataList.Count; ++i)
            {
                m_PlayersData.Add(playersBasicDataList[i].Name, new Player
                {
                    Data = playersBasicDataList[i],
                    State = PlayerState.Active
                });
            }
            
        }

        private void OnEnable()
        {
            m_ServerNetworkManager.OnClientConnectedCallback += OnClientConnected;
            m_ServerNetworkManager.OnPlayerJoinsCallback += OnPlayerJoins;
            m_ServerNetworkManager.OnPlayerLeavesCallback += OnPlayerLeaves;
            m_ServerNetworkManager.OnReceiveNotificationCallback += OnReceiveNotification;
        }

        private void OnDisable()
        {
            m_ServerNetworkManager.OnReceiveNotificationCallback -= OnReceiveNotification;
            m_ServerNetworkManager.OnPlayerLeavesCallback -= OnPlayerLeaves;
            m_ServerNetworkManager.OnPlayerJoinsCallback -= OnPlayerJoins;
            m_ServerNetworkManager.OnClientConnectedCallback -= OnClientConnected;
        }

        private void OnReceiveNotification(string notification)
        {
            m_ServerMenuUI.NotificationManager.titleObj.text = "Notification";
            m_ServerMenuUI.NotificationManager.descriptionObj.text = notification;
            m_ServerMenuUI.NotificationManager.OpenNotification();
        }

        private void OnPlayerLeaves(string playerName)
        {
            if (m_ServerMenuUI.PlayerList.RemovePlayer(playerName))
            {
                m_ServerNetworkManager.SendMessageToAll(playerName, CustomMsgType.PlayerRemovedFromLobby);
            }

            m_PlayersData[playerName].State = PlayerState.Active;
        }

        private void OnPlayerJoins(string playerName)
        {
            var player = m_PlayersData[playerName];
            m_ServerMenuUI.PlayerList.AddPlayer(player);
            player.State = PlayerState.InLobby;
        }

        private void OnClientConnected(int connId)
        {
            foreach (var player in m_PlayersData)
            {
                var playerRatingInJson = JsonUtility.ToJson(player.Value);
                m_ServerNetworkManager.SendMessageToClient(connId, playerRatingInJson, CustomMsgType.PlayerRating);
            }
        }

        private void Start()
        {
            // m_PlayersData.Sort((playerA, playerB) => playerB.Rating.CompareTo(playerA.Rating));
            //
            // for (var i = 0; i < m_PlayersData.Count; ++i)
            // {
            //     var player = m_PlayersData[i];
            //     
            //     m_ServerMenuUI.PlayerList.AddPlayer
            //     (
            //         name: player.Name,
            //         category: player.Category,
            //         rating: player.Rating
            //     );
            // }
        }
    }
}