using UnityEngine;
using TMPro;

namespace Server.UI
{
    public class PlayerListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_PlayerName;

        public string PlayerName
        {
            get => m_PlayerName.text;
            set => m_PlayerName.text = value;
        }

        [SerializeField] private TextMeshProUGUI m_Category;

        public string Category
        {
            get => m_Category.text;
            set => m_Category.text = value;
        }

        [SerializeField] private TextMeshProUGUI m_Rating;

        public string Rating
        {
            get => m_Rating.text;
            set => m_Rating.text = value;
        }
    }
}