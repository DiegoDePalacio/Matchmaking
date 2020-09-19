using System.Collections.Generic;
using MM.Server.Data;
using UnityEngine;

namespace MM.Server.UI
{
    public class PlayerList : ListWithPoolTemplate<PlayerListElement>
    {
        private Dictionary<string, PlayerListElement> m_Elements = new Dictionary<string, PlayerListElement>();
        public Dictionary<string, PlayerListElement> Elements => m_Elements;

        public PlayerListElement AddPlayer(Player player)
        {
            var playerName = player.Data.Name;
            
            if (m_Elements.ContainsKey(playerName))
            {
                return m_Elements[playerName];
            }
            else
            {
                var playerListElement = AddElement();
                m_Elements.Add(playerName, playerListElement);
                playerListElement.SetPlayerData(player);
                return playerListElement;
            }
        }

        public bool RemovePlayer(string playerName)
        {
            if (m_Elements.ContainsKey(playerName))
            {
                var playerListElement = m_Elements[playerName];
                RemoveElement(playerListElement);
                m_Elements.Remove(playerName);
                return true;
            }

            return false;
        }
        
#if UNITY_EDITOR
        // Unfortunately ContextMenu can't be included in generic classes
        [ContextMenu("Increase List Pool")]
        public void IncreasePool()
        {
            IncreasePoolInternal();
        }
        
#region Tests
        private Player m_TestDummyPlayer;

        [ContextMenu("[TEST] Add Dummy Player")]
        public void TestAddDummyPlayer()
        {
            m_TestDummyPlayer = new Player 
            {
                Data = new PlayerBasicData
                {
                    Name = "Arpad Elo", 
                    Category = 13,
                    Rating = 2899
                },
                State = PlayerState.InLobby
            };
            AddPlayer(m_TestDummyPlayer);
        }

        [ContextMenu("[TEST] Remove Dummy Player")]
        public void TestRemoveDummyPlayer()
        {
            RemovePlayer(m_TestDummyPlayer.Data.Name);
        }
#endregion
#endif
    }
}