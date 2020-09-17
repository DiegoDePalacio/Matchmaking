using System;
using Michsky.UI.ModernUIPack;
using Server.Data;
using UnityEngine;
using TMPro;

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
        public string PlayerName => m_PlayerName.text;
        
        [SerializeField] private TextMeshProUGUI m_Category;
        [SerializeField] private TextMeshProUGUI m_Rating;
        [SerializeField] private TextMeshProUGUI m_ButtonLabel;
        [SerializeField] private ButtonManagerBasic m_Button;
        
        private ButtonState m_Action = ButtonState.Join;
        public ButtonState Action => m_Action;

        public Action<PlayerListElement> OnButtonClickCallback;

        private void OnEnable()
        {
            m_Button.clickEvent.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            m_Button.clickEvent.RemoveListener(OnButtonClick);
        }

        public void SetPlayerData(Player playerData)
        {
            m_PlayerName.text = playerData.Data.Name;
            m_Category.text = playerData.Data.Category.ToString();
            m_Rating.text = playerData.Data.Rating.ToString();

            switch (playerData.State)
            {
                case PlayerState.Active:
                    SetButtonAction(ButtonState.Join);
                    break;
                case PlayerState.InLobby:
                    SetButtonAction(ButtonState.Drop);
                    break;
                case PlayerState.Playing:
                    SetButtonAction(ButtonState.Playing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnButtonClick()
        {
            if (OnButtonClickCallback != null)
                OnButtonClickCallback(this);
            
            if (m_Action == ButtonState.Join)
                SetButtonAction(ButtonState.Drop);
            else if (m_Action == ButtonState.Drop)
                SetButtonAction(ButtonState.Join);
        }

        public void SetButtonAction(ButtonState action)
        {
            switch (action)
            {
                case ButtonState.Join:
                    m_Button.buttonText = "JOIN";
                    m_Button.enabled = true;
                    break;
                case ButtonState.Drop:
                    m_Button.buttonText = "DROP";
                    m_Button.enabled = true;
                    break;
                case ButtonState.Playing:
                    m_Button.buttonText = "PLAYING";
                    m_Button.enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            m_Button.UpdateUI();
            m_Action = action;
        }
    }
}
