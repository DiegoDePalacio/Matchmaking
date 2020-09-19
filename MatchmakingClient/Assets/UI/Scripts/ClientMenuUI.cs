using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace MM.Client.UI
{
    public class ClientMenuUI : MonoBehaviour
    {
        [SerializeField] private PlayerList m_PlayerList;
        public PlayerList PlayerList => m_PlayerList;
        
        [SerializeField] private NotificationManager m_NotificationManager;
        public NotificationManager NotificationManager => m_NotificationManager;
    }
}
