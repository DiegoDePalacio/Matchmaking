using System;
using Michsky.UI.ModernUIPack;
using MM.Server.Core;
using TMPro;
using UnityEngine;

namespace MM.Server.UI
{
    public class ServerMenuUI : MonoBehaviour
    {
        [SerializeField] private string m_WarningSpriteId = "<sprite=12>";
        public string WarningSpriteId => m_WarningSpriteId;

        // These fields are assigned in Editor, and then the warning CS0649 about the lack of assignation is disabled
        // Their assignation is checked on the Awake method and throwing an exception if are still null in runtime
#pragma warning disable 649
        [SerializeField] private MatchmakingParams m_MatchmakingParams;
        public MatchmakingParams MatchmakingParams => m_MatchmakingParams;
        
        [SerializeField] private TextMeshProUGUI m_WarningText;
        public TextMeshProUGUI WarningText => m_WarningText;
        
        [SerializeField] private TMP_InputField m_MatchSize;
        public TMP_InputField MatchSizeInputField => m_MatchSize;
        public int TeamSize => int.Parse(m_MatchSize.text);

        [SerializeField] private SwitchManager m_MatchmakingByCategory;
        public SwitchManager MatchmakingByCategory => m_MatchmakingByCategory;

        [SerializeField] private RadialSlider m_SimilaritySlider;
        public RadialSlider SimilaritySlider => m_SimilaritySlider;

        [SerializeField] private TextMeshProUGUI m_SimilarityRequired;
        public TextMeshProUGUI SimilarityRequired => m_SimilarityRequired;
        
        [SerializeField] private NotificationManager m_NotificationManager;
        public NotificationManager NotificationManager => m_NotificationManager;

        [SerializeField] private PlayerList m_PlayerList;
        public PlayerList PlayerList => m_PlayerList;

        [SerializeField] private MatchList m_MatchList;
        public MatchList MatchList => m_MatchList;
#pragma warning restore 649
        
        private void Awake()
        {
            CheckEditorInputs();
            
            m_MatchSize.text = (m_MatchmakingParams.TeamSize > 0 
                ? m_MatchmakingParams.TeamSize.ToString() 
                : string.Empty);

            m_MatchmakingByCategory.isOn = (m_MatchmakingParams.Matchmaking == MatchmakingType.ByCategory);

            // The slider values are in the range [0, 100]
            m_SimilaritySlider.currentValue = m_MatchmakingParams.MinSimilarityToMatchmake * 100;
        }

        private void CheckEditorInputs()
        {
            if (m_MatchmakingParams == null)
                throw new NullReferenceException("The Matchmaking Parameters on the Server Menu UI was not assigned!");
            
            if (m_WarningText == null)
                throw new NullReferenceException("The Warning text field on the Server Menu UI was not assigned!");

            if (m_MatchSize == null)
                throw new NullReferenceException("The Match Size text field on the Server Menu UI was not assigned!");

            if (m_MatchmakingByCategory == null)
                throw new NullReferenceException("The Matchmaking By Category switch on the Server Menu UI was not assigned!");

            if (m_SimilaritySlider == null)
                throw new NullReferenceException("The Similarity Slider on the Server Menu UI was not assigned!");

            if (m_SimilarityRequired == null)
                throw new NullReferenceException("The Similarity Required text field on the Server Menu UI was not assigned!");

            if (m_NotificationManager == null)
                throw new NullReferenceException("The Notification Manager on the Server Menu UI was not assigned!");

            if (m_PlayerList == null)
                throw new NullReferenceException("The Player List component on the Server Menu UI was not assigned!");

            if (m_MatchList == null)
                throw new NullReferenceException("The Match List component on the Server Menu UI was not assigned!");
        }
    }
}