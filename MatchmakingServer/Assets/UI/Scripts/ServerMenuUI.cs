using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;

namespace Server.UI
{
    public class ServerMenuUI : MonoBehaviour
    {
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
    }
}