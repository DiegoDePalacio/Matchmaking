using System;
using MM.Server.Data;
using UnityEngine;
using TMPro;

namespace MM.Server.UI
{
    public class PlayerListElement : MonoBehaviour
    {
        // These fields are assigned in Editor, and then the warning CS0649 about the lack of assignation is disabled
        // Their assignation is checked on the Awake method and throwing an exception if are still null in runtime
#pragma warning disable 649
        [SerializeField] private TextMeshProUGUI m_PlayerName;
        [SerializeField] private TextMeshProUGUI m_Category;
        [SerializeField] private TextMeshProUGUI m_Rating;
#pragma warning restore 649

        private void Awake()
        {
            if (m_PlayerName == null)
                throw new NullReferenceException($"The Player Name text field was not assigned on the player list element {gameObject.name}");
            
            if (m_Category == null)
                throw new NullReferenceException($"The Category text field was not assigned on the player list element {gameObject.name}");
            
            if (m_Rating == null)
                throw new NullReferenceException($"The Rating text field was not assigned on the player list element {gameObject.name}");
        }

        public void SetPlayerData(Player player)
        {
            m_PlayerName.text = player.Data.Name;
            m_Category.text = player.Data.Category.ToString();
            m_Rating.text = player.Data.Rating.ToString();
        }
    }
}