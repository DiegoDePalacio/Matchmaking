using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;

namespace Server.UI
{
    public class ServerMenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField m_MatchSize;
        public int MatchSize => int.Parse(m_MatchSize.text);

        [SerializeField] private SwitchManager m_SwitchManager;
        public SwitchManager SwitchManager => m_SwitchManager;

        [SerializeField] private RadialSlider m_Similarity;
        public RadialSlider Similarity => m_Similarity;
        
        [SerializeField] private NotificationManager m_NotificationManager;
        public NotificationManager NotificationManager => m_NotificationManager;

        [SerializeField] private PlayerList m_PlayerList;
        public PlayerList PlayerList => m_PlayerList;

        [SerializeField] private MatchList m_MatchList;
        public MatchList MatchList => m_MatchList;
    }
}