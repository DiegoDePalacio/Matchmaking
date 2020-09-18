using System;
using System.Linq;
using Server.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Server.UI
{
    public class MatchListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_FirstTeamMembers;
        [SerializeField] private TextMeshProUGUI m_SecondTeamMembers;
        [SerializeField] private Button m_FirstTeamWinsButton;
        [SerializeField] private Button m_SecondTeamWinsButton;

        private MatchData m_MatchData;
        
        // After a match ends, the winner is set always in the TeamA variable of the Match Data
        // This variable prevents to switch the teams more than once if the second team wins the match
        private bool m_MatchEnded = false;
        
        public MatchData MatchData => m_MatchData;

        public Action<MatchListElement> OnTeamAWinsCallback;
        
        private void OnEnable()
        {
            m_FirstTeamWinsButton.onClick.AddListener(OnFirstTeamWins);
            m_SecondTeamWinsButton.onClick.AddListener(OnSecondTeamWins);
        }

        private void OnDisable()
        {
            m_SecondTeamWinsButton.onClick.RemoveListener(OnSecondTeamWins);
            m_FirstTeamWinsButton.onClick.RemoveListener(OnFirstTeamWins);
        }

        private void OnFirstTeamWins()
        {
            m_MatchEnded = true;
            OnTeamAWinsCallback?.Invoke(this);
        }

        private void OnSecondTeamWins()
        {
            if (!m_MatchEnded)
            {
                // Switch the team order to have the winner on the Team A slot
                var temp = m_MatchData.TeamA;
                m_MatchData.TeamA = m_MatchData.TeamB;
                m_MatchData.TeamB = temp;
            }

            m_MatchEnded = true;
            OnTeamAWinsCallback?.Invoke(this);
        }

        public void SetMatch(MatchData matchData)
        {
            m_MatchData = matchData;
            m_MatchEnded = false;
            OnTeamAWinsCallback = null;
            
            var teamA = matchData.TeamA.Select(data => data.Name);
            var teamB = matchData.TeamB.Select(data => data.Name);

            m_FirstTeamMembers.text = string.Join(", ", teamA);
            m_SecondTeamMembers.text = string.Join(", ", teamB);
        }
    }
}