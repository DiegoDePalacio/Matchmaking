using UnityEngine;

namespace Server.UI
{
    public class PlayerList : ListWithPoolTemplate<PlayerListElement>
    {
        public PlayerListElement AddPlayer(string name, int category, int rating)
        {
            var newPlayer = AddElement();
            newPlayer.PlayerName = name;
            newPlayer.Category = category.ToString();
            newPlayer.Rating = rating.ToString();
            return newPlayer;
        }

        public void RemovePlayer(PlayerListElement playerListElement)
        {
            RemoveElement(playerListElement);
        }

#if UNITY_EDITOR
        // Unfortunately ContextMenu can't be included in generic classes
        [ContextMenu("Increase List Pool")]
        public void IncreasePool()
        {
            IncreasePoolInternal();
        }
        
#region Tests
        private PlayerListElement m_TestDummyPlayer;

        [ContextMenu("[TEST] Add Dummy Player")]
        public void TestAddDummyPlayer()
        {
            m_TestDummyPlayer = AddPlayer("Arpad Elo", 13, 2899);
        }

        [ContextMenu("[TEST] Remove Dummy Player")]
        public void TestRemoveDummyPlayer()
        {
            RemovePlayer(m_TestDummyPlayer);
        }
#endregion
#endif
    }
}
