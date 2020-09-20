using System;
using System.Collections.Generic;
using MM.Client.Data;
using UnityEngine;

namespace MM.Client.UI
{
    public class PlayerList : MonoBehaviour
    {
        [Tooltip(
            "Number of list elements to add into the pool by using the 'Increase List Pool' option on the Context Menu")]
        [SerializeField]
        private int m_PoolChunkSize = 10;

        // This field is assigned in Editor, therefore the warning CS0649 about the lack of assignation is disabled
        // Its assignation is checked on the Awake method and throwing an exception if is still null in runtime
#pragma warning disable 649
        [SerializeField] private PlayerListElement m_ElementPrefab;
#pragma warning restore 649
        
        [SerializeField] private List<PlayerListElement> m_AvailableElements = new List<PlayerListElement>();
        [SerializeField] private List<PlayerListElement> m_UsedElements = new List<PlayerListElement>();

        // Names of the players are the keys
        private Dictionary<string, PlayerListElement> m_Elements = new Dictionary<string, PlayerListElement>();
        public Dictionary<string, PlayerListElement> Elements => m_Elements;

        private void Awake()
        {
            if (m_ElementPrefab == null)
                throw new NullReferenceException($"The Element Prefab was not assigned on the Player List of {gameObject.name}");
        }

        public PlayerListElement AddPlayer(Player playerData)
        {
            if (m_AvailableElements.Count == 0)
            {
                Debug.LogWarning(
                    $"The pool for the PlayerList on {gameObject.name} was not populated with enough element in Editor-time. " +
                    "Consider increasing the pool for avoiding Instantiation of its elements in runtime.");
                IncreaseListPool();
            }

            var playerName = playerData.Data.Name;
            if (m_Elements.ContainsKey(playerName))
            {
                return m_Elements[playerName];
            }
            else
            {
                var playerListElement = m_AvailableElements[0];
                m_AvailableElements.RemoveAt(0);
                m_Elements.Add(playerData.Data.Name, playerListElement);

                playerListElement.SetPlayerData(playerData);
                m_UsedElements.Add(playerListElement);
                playerListElement.gameObject.SetActive(true);

                return playerListElement;
            }
        }

        public void RemovePlayer(PlayerListElement playerToRemove)
        {
            if (playerToRemove == default(PlayerListElement))
            {
                Debug.LogError($"Trying to remove from the PlayerList {gameObject.name} the Player '{playerToRemove.PlayerName}' but is not the list!");
                return;
            }

            var playerName = playerToRemove.PlayerName;

            if (m_Elements.ContainsKey(playerName))
            {
                m_Elements.Remove(playerName);
                playerToRemove.OnButtonClickCallback = null;
                m_UsedElements.Remove(playerToRemove);
                playerToRemove.gameObject.SetActive(false);
                m_AvailableElements.Add(playerToRemove);
            }
        }

        [ContextMenu("Increase List Pool")]
        public void IncreaseListPool()
        {
            for (var i = 0; i < m_PoolChunkSize; ++i)
            {
                var newElement = Instantiate(m_ElementPrefab, transform);
                m_AvailableElements.Add(newElement);
                newElement.gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
#region Tests
        private PlayerListElement m_TestDummyPlayerListElement;

        [ContextMenu("[TEST] Add Dummy Player")]
        public void TestAddDummyPlayer()
        {
            m_TestDummyPlayerListElement = AddPlayer(new Player
            {
                Data = new PlayerBasicData
                {
                    Name = "Arpad Elo", 
                    Category = 13,
                    Rating = 2899
                },
                State = PlayerState.Inactive
            });
        }

        [ContextMenu("[TEST] Remove Dummy Player")]
        public void TestRemoveDummyPlayer()
        {
            RemovePlayer(m_TestDummyPlayerListElement);
        }
#endregion        
#endif
    }
}