using System;
using System.Collections.Generic;
using MM.Server.Data;
using MM.Server.Networking;
using MM.Server.UI;
using UnityEngine;

namespace MM.Server.Core
{
    public class MatchManager : MonoBehaviour
    {
        private enum MatchMakeState
        {
            NoConnection,
            NoTeamSize,
            NotEnoughPlayers,
            Ready
        }
        
        [SerializeField] private ServerNetworkManager m_ServerNetworkManager;
        [SerializeField] private ServerMenuUI m_ServerMenuUI;
        [SerializeField] private PlayersRatingsFromJson m_PlayersRatingsFromJson;
        [SerializeField] private EloRatingParameters EloRatingParameters;
        [SerializeField] private RatingConstants RatingConstants;
        
        private Dictionary<string, Player> m_PlayersData = new Dictionary<string, Player>();
        private List<Player> m_PlayesInLobby = new List<Player>();
        
        private MatchMakeState m_MatchMakeState = MatchMakeState.NoTeamSize;
        private float m_SimilarityActualMax = 0;
        private bool m_SimilarityChecked = false;
        
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
                    State = PlayerState.Inactive
                });
            }
        }

        private void Start()
        {
            RefreshState();
        }

        private void OnEnable()
        {
            m_ServerNetworkManager.OnClientConnectedCallback += OnClientConnected;
            m_ServerNetworkManager.OnPlayerJoinsCallback += OnPlayerJoins;
            m_ServerNetworkManager.OnPlayerLeavesCallback += OnPlayerLeaves;
            m_ServerNetworkManager.OnReceiveNotificationCallback += DisplayNotification;
            m_ServerNetworkManager.OnAllClientsDisconnectedCallback += OnAllClientsDiconnected;
            
            m_ServerMenuUI.MatchSizeInputField.onValueChanged.AddListener(OnTeamSizeChanged);
            m_ServerMenuUI.MatchmakingByCategory.switchButton.onClick.AddListener(OnMatchMakingTypeChanged);
            m_ServerMenuUI.Similarity.onPointerUp.AddListener(OnSimilarityChanged);
        }

        private void OnDisable()
        {
            m_ServerMenuUI.Similarity.onPointerUp.RemoveListener(OnSimilarityChanged);
            m_ServerMenuUI.MatchmakingByCategory.switchButton.onClick.RemoveListener(OnMatchMakingTypeChanged);
            m_ServerMenuUI.MatchSizeInputField.onValueChanged.RemoveListener(OnTeamSizeChanged);
            
            m_ServerNetworkManager.OnAllClientsDisconnectedCallback -= OnAllClientsDiconnected;
            m_ServerNetworkManager.OnReceiveNotificationCallback -= DisplayNotification;
            m_ServerNetworkManager.OnPlayerLeavesCallback -= OnPlayerLeaves;
            m_ServerNetworkManager.OnPlayerJoinsCallback -= OnPlayerJoins;
            m_ServerNetworkManager.OnClientConnectedCallback -= OnClientConnected;
        }

        private void OnAllClientsDiconnected()
        {
            RefreshState();
        }

        private void OnMatchMakingTypeChanged()
        {
            m_SimilarityActualMax = 0f;
            m_SimilarityChecked = false;
            RefreshState();
        }

        private void OnSimilarityChanged()
        {
            m_SimilarityChecked = false;
            RefreshState();
        }

        private void OnTeamSizeChanged(string _notUsed)
        {
            m_SimilarityChecked = false;
            RefreshState();
        }

        private void DisplayNotification(string notification)
        {
            m_ServerMenuUI.NotificationManager.title = "Alert";
            m_ServerMenuUI.NotificationManager.description = notification;
            m_ServerMenuUI.NotificationManager.UpdateUI();
            m_ServerMenuUI.NotificationManager.OpenNotification();
        }

        private void OnPlayerLeaves(string playerName)
        {
            if (m_ServerMenuUI.PlayerList.RemovePlayer(playerName))
            {
                m_ServerNetworkManager.SendMessageToAll(playerName, CustomMsgType.PlayerRemovedFromLobby);
            }

            m_PlayersData[playerName].State = PlayerState.Inactive;
            RefreshState();
        }

        private void OnPlayerJoins(string playerName)
        {
            var player = m_PlayersData[playerName];
            m_ServerMenuUI.PlayerList.AddPlayer(player);
            player.State = PlayerState.InLobby;
            m_SimilarityChecked = false;
            RefreshState();
        }

        private void OnClientConnected(int connId)
        {
            foreach (var player in m_PlayersData)
            {
                var playerRatingInJson = JsonUtility.ToJson(player.Value);
                m_ServerNetworkManager.SendMessageToClient(connId, playerRatingInJson, CustomMsgType.PlayerRating);
            }

            RefreshState();
        }

        private void RefreshState()
        {
            if (!m_ServerNetworkManager.AreClientsConnected)
            {
                m_MatchMakeState = MatchMakeState.NoConnection;
                m_ServerMenuUI.WarningText.text = "<sprite=12> Waiting for Client connection...";
                m_ServerMenuUI.SimilarityActualMax.text = string.Empty;
            }
            else if (m_ServerMenuUI.TeamSize < 1)
            {
                m_MatchMakeState = MatchMakeState.NoTeamSize;
                m_ServerMenuUI.WarningText.text = "<sprite=12> Please choose a Team Size";
                m_ServerMenuUI.SimilarityActualMax.text = string.Empty;
            }
            else if (m_ServerMenuUI.PlayerList.Elements.Count < (m_ServerMenuUI.TeamSize * 2))
            {
                m_MatchMakeState = MatchMakeState.NotEnoughPlayers;
                m_ServerMenuUI.WarningText.text = string.Empty;

                var missingPlayers = (m_ServerMenuUI.TeamSize * 2) - m_ServerMenuUI.PlayerList.Elements.Count;
                m_ServerMenuUI.SimilarityActualMax.text = $"<sprite=12> Not enough Players. \nMissing {missingPlayers}";
            }
            else
            {
                m_MatchMakeState = MatchMakeState.Ready;
                m_ServerMenuUI.WarningText.text = string.Empty;

                if (!m_SimilarityChecked)
                {
                    TryToMatchMake();
                }
            }
        }

        private void TryToMatchMake()
        {
            var playerCount = m_ServerMenuUI.PlayerList.Elements.Count;
            var matchSize = m_ServerMenuUI.TeamSize * 2;
            
            var playersInLobby = new List<PlayerBasicData>();
            foreach (var playerListElement in m_ServerMenuUI.PlayerList.Elements)
            {
                playersInLobby.Add(m_PlayersData[playerListElement.Key].Data);
            }
            
            // Sorting players by rating in descending order to start trying to find a good match for the players with better rating first
            // They're more likely to be more demanding in general, and they're also who are more likely to playing more our game
            playersInLobby.Sort((playerA, playerB) => playerB.Rating.CompareTo(playerA.Rating));
            
            // Only creating matches of players with closer rating between them 
            // Please read the "About grouping players for matchmaking" section of the README file for more information about it
            var maxSimilarityIterationCount = playerCount - matchSize + 1;

            var matchFound = false;
            
            // TODO: Only check the group of players that includes the new player added, to avoid checking combinations that we already
            // know that doesn't have enough similarity
            for (var i = 0; i < maxSimilarityIterationCount; ++i)
            {
                var playersOrderedByRating = playersInLobby.GetRange(i, matchSize);
                
                var similarity = (m_ServerMenuUI.MatchmakingByCategory.isOn
                    ? SimilarityByCategory.GetSimilarity(playersOrderedByRating, RatingConstants.MaxCategory)
                    : SimilarityByRating.GetSimilarity(playersOrderedByRating, RatingConstants.MaxRating));

                // If we found a very similar group of players to match
                if (similarity * 100 > m_ServerMenuUI.Similarity.currentValue)
                {
                    DisplayNotification($"A new game just started with a similarity of {similarity:P2}!");
                    matchFound = true;
                    CreateNewMatch(playersOrderedByRating);
                    m_SimilarityActualMax = 0f;
                    break;
                }
                else if (similarity > m_SimilarityActualMax)
                {
                    m_SimilarityActualMax = similarity;
                }
            }

            if (!matchFound)
            {
                m_SimilarityChecked = true;
            }
            
            RefreshSimilarityActualMaxText();
            RefreshState();
        }

        private void CreateNewMatch(List<PlayerBasicData> playersData)
        {
            var matchData = (m_ServerMenuUI.TeamSize > m_ServerMenuUI.MatchmakingParams.MaxTeamSizeForAccurateMatchmake
                ? MatchmakingBigTeams.ArrangePlayers(playersData)
                : MatchmakingSmallTeams.ArrangePlayers(playersData));

            var matchListElement = m_ServerMenuUI.MatchList.AddMatch(matchData);
                    
            // This callback is cleared as soon as the element list is reused
            matchListElement.OnTeamAWinsCallback += OnTeamAWins;

            for (var i = 0; i < playersData.Count; ++i)
            {
                var playerName = playersData[i].Name;
                m_PlayersData[playerName].State = PlayerState.Playing;
                m_ServerNetworkManager.SendMessageToAll(playerName, CustomMsgType.PlayerOnMatch);

                m_ServerMenuUI.PlayerList.RemovePlayer(playerName);
            }
        }
        
        private void OnTeamAWins(MatchListElement matchListElement)
        {
            var updatedPlayers = matchListElement.MatchData;

            UpdatePlayersRatings.UpdateRatingsTeamAWins(ref updatedPlayers, RatingConstants, EloRatingParameters);

            for (var i = 0; i < updatedPlayers.TeamA.Count; ++i)
            {
                UpdatePlayeDataAfterMatch(updatedPlayers.TeamA[i]);
                UpdatePlayeDataAfterMatch(updatedPlayers.TeamB[i]);
            }
            
            m_ServerMenuUI.MatchList.RemoveMatch(matchListElement);
        }

        private void UpdatePlayeDataAfterMatch(PlayerBasicData playerData)
        {
            var playerName = playerData.Name;
            m_PlayersData[playerName].Data = playerData;
            m_PlayersData[playerName].State = PlayerState.Inactive;

            var playerJson = JsonUtility.ToJson(playerData);
            m_ServerNetworkManager.SendMessageToAll(playerJson, CustomMsgType.PlayerUpdateAfterMatch);
        }
        
        private void RefreshSimilarityActualMaxText()
        {
            m_ServerMenuUI.SimilarityActualMax.text = $"Current Similarity by {(m_ServerMenuUI.MatchmakingByCategory.isOn ? "Category" : "Rating")}: {m_SimilarityActualMax:P2}";
        }

#if UNITY_EDITOR
#region Tests
        [ContextMenu("[TEST] Test Matchmaking Small Teams")]
        public void TestMatchmakingSmallTeams()
        {
            var playerList = new List<PlayerBasicData>(new PlayerBasicData[]
            {
                new PlayerBasicData
                {
                    Category = 13,
                    Name = "Sid Meier",
                    Rating = 1 // 2899
                },
                new PlayerBasicData
                {
                    Category = 12,
                    Name = "John Romero",
                    Rating = 4 // 2350
                },
                new PlayerBasicData
                {
                    Category = 13,
                    Name = "Satoshi Tajiri",
                    Rating = 8 // 2850
                },
                new PlayerBasicData
                {
                    Category = 13,
                    Name = "Markus Persson",
                    Rating = 9 // 2800
                },
                new PlayerBasicData
                {
                    Category = 12,
                    Name = "Tim Schafer",
                    Rating = 11 // 2399
                },
                new PlayerBasicData
                {
                    Category = 13,
                    Name = "Yu Suzuki",
                    Rating = 12 // 2888
                }
            });

            playerList.Sort((playerA, playerB) => playerB.Rating.CompareTo(playerA.Rating));
            MatchData matchData = MatchmakingSmallTeams.ArrangePlayers(playerList);
            
            Debug.Log("TeamA");
            foreach (var playerBasicData in matchData.TeamA)
            {
                Debug.Log(playerBasicData.Rating);
            }
            
            Debug.Log("TeamB");
            foreach (var playerBasicData in matchData.TeamB)
            {
                Debug.Log(playerBasicData.Rating);
            }
        }
#endregion
#endif        
    }
}