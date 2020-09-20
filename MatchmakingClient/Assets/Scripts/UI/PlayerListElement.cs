using System;
using Michsky.UI.ModernUIPack;
using MM.Client.Data;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MM.Client.UI
{
    public class PlayerListElement : MonoBehaviour
    {
        public enum ButtonState
        {
            Join,
            Drop,
            Playing
        }

        // These fields are assigned in Editor, and then the warning CS0649 about the lack of assignation is disabled
        // Their assignation is checked on the Awake method and throwing an exception if are still null in runtime
#pragma warning disable 649
        [SerializeField] private TextMeshProUGUI m_PlayerName;
        public string PlayerName => m_PlayerName.text;
        
        [SerializeField] private TextMeshProUGUI m_Category;
        [SerializeField] private TextMeshProUGUI m_Rating;
        [SerializeField] private ButtonManagerBasic m_Button;
        [SerializeField] private Image m_ButtonImage;
#pragma warning restore 649
        
        [SerializeField] private Color m_ButtonJoinColor = Color.green;
        [SerializeField] private Color m_ButtonDropColor = Color.red;
        
        private ButtonState m_Action = ButtonState.Join;
        public ButtonState Action => m_Action;

        public Action<PlayerListElement> OnButtonClickCallback;

        private void Awake()
        {
            if (m_PlayerName == null)
                throw new NullReferenceException($"The Player Name text field was not assigned on the PlayListElement of {gameObject.name}");

            if (m_Rating == null)
                throw new NullReferenceException($"The Rating text field was not assigned on the PlayListElement of {gameObject.name}");

            if (m_Category == null)
                throw new NullReferenceException($"The Category text field was not assigned on the PlayListElement of {gameObject.name}");

            if (m_Button == null)
                throw new NullReferenceException($"The Action Button was not assigned on the PlayListElement of {gameObject.name}");

            if (m_ButtonImage == null)
                throw new NullReferenceException($"The Action Button Image was not assigned on the PlayListElement of {gameObject.name}");
        }

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
                case PlayerState.Inactive:
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
            OnButtonClickCallback?.Invoke(this);

            if (m_Action == ButtonState.Join)
            {
                SetButtonAction(ButtonState.Drop);
            }
            else if (m_Action == ButtonState.Drop)
            {
                SetButtonAction(ButtonState.Join);
            }                
        }

        public void SetButtonAction(ButtonState action)
        {
            m_Button.buttonText = action.ToString().ToUpper();
            m_Button.enabled = (action != ButtonState.Playing);
            m_ButtonImage.color = (action == ButtonState.Join ? m_ButtonJoinColor : m_ButtonDropColor);
            m_ButtonImage.enabled = m_Button.enabled;
            m_Button.UpdateUI();
            
            m_Action = action;
        }
    }
}