using Server.Data;
using UnityEngine;
using TMPro;

namespace Server.UI
{
    public class PlayerListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_PlayerName;
        [SerializeField] private TextMeshProUGUI m_Category;
        [SerializeField] private TextMeshProUGUI m_Rating;

        public void SetPlayerData(Player player)
        {
            m_PlayerName.text = player.Data.Name;
            m_Category.text = player.Data.Category.ToString();
            m_Rating.text = player.Data.Rating.ToString();
        }
    }
}