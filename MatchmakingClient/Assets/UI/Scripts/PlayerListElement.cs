using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Client.UI
{
    public class PlayerListElement : MonoBehaviour
    {
        public enum ButtonState
        {
            Join,
            Drop,
            Playing
        }

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

        [SerializeField] private TextMeshProUGUI m_ButtonLabel;
        [SerializeField] private Button m_Button;

        public void SetButtonState(ButtonState buttonState)
        {
            switch (buttonState)
            {
                case ButtonState.Join:
                    m_ButtonLabel.text = "JOIN";
                    m_Button.enabled = true;
                    break;
                case ButtonState.Drop:
                    m_ButtonLabel.text = "DROP";
                    m_Button.enabled = true;
                    break;
                case ButtonState.Playing:
                    m_ButtonLabel.text = "PLAYING";
                    m_Button.enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonState), buttonState, null);
            }
        }
    }
}
