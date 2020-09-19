﻿using System;
using Michsky.UI.ModernUIPack;
using MM.Server.Core;
using TMPro;
using UnityEngine;

namespace MM.Server.UI
{
    public class ServerMenuUI : MonoBehaviour
    {
        [SerializeField] private DefaultMatchmakingUIParams m_MatchmakingDefaults;
        
        [SerializeField] private TextMeshProUGUI m_WarningText;
        public TextMeshProUGUI WarningText => m_WarningText;
        
        [SerializeField] private TMP_InputField m_MatchSize;
        public TMP_InputField MatchSizeInputField => m_MatchSize;
        public int TeamSize => int.Parse(m_MatchSize.text);

        [SerializeField] private SwitchManager m_MatchmakingByCategory;
        public SwitchManager MatchmakingByCategory => m_MatchmakingByCategory;

        [SerializeField] private RadialSlider m_Similarity;
        public RadialSlider Similarity => m_Similarity;

        [SerializeField] private TextMeshProUGUI m_SimilarityActualMax;
        public TextMeshProUGUI SimilarityActualMax => m_SimilarityActualMax;
        
        [SerializeField] private NotificationManager m_NotificationManager;
        public NotificationManager NotificationManager => m_NotificationManager;

        [SerializeField] private PlayerList m_PlayerList;
        public PlayerList PlayerList => m_PlayerList;

        [SerializeField] private MatchList m_MatchList;
        public MatchList MatchList => m_MatchList;

        private void Awake()
        {
            if (m_MatchmakingDefaults != null)
            {
                m_MatchSize.text = (m_MatchmakingDefaults.TeamSize > 0 
                    ? m_MatchmakingDefaults.TeamSize.ToString() 
                    : string.Empty);

                m_MatchmakingByCategory.isOn = (m_MatchmakingDefaults.Matchmaking == MatchmakingType.ByCategory);

                m_Similarity.currentValue = m_MatchmakingDefaults.MinSimilarityToMatchmake;
            }
        }
    }
}