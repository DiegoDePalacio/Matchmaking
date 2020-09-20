using System;
using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace MM.Client.UI
{
    public class ClientMenuUI : MonoBehaviour
    {
        // These fields are assigned in Editor, and then the warning CS0649 about the lack of assignation is disabled
        // Their assignation is checked on the Awake method and throwing an exception if are still null in runtime
#pragma warning disable 649
        [SerializeField] private PlayerList m_PlayerList;
        public PlayerList PlayerList => m_PlayerList;
        
        [SerializeField] private NotificationManager m_NotificationManager;
        public NotificationManager NotificationManager => m_NotificationManager;
#pragma warning restore 649

        private void Awake()
        {
            if (m_PlayerList == null)
                throw new NullReferenceException("The Player List component on the Client Menu UI was not assigned!");

            if (m_NotificationManager == null)
                throw new NullReferenceException("The Notification Manager on the Client Menu UI was not assigned!");
        }
    }
}