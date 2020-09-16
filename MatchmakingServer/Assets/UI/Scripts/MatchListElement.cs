using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Server.UI
{
    public class MatchListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_FirstTeamMembers;
        public string FirstTeamNames
        {
            get => m_FirstTeamMembers.text;
            set => m_FirstTeamMembers.text = value;
        }

        [SerializeField] private TextMeshProUGUI m_SecondTeamMembers;
        public string SecondTeamNames
        {
            get => m_SecondTeamMembers.text;
            set => m_SecondTeamMembers.text = value;
        }

        [SerializeField] private Button m_FirstTeamWinsButton;
        public Button FirstTeamWinsButton => m_FirstTeamWinsButton;

        [SerializeField] private Button m_SecondTeamWinsButton;
        public Button SecondTeamWinsButton => m_SecondTeamWinsButton;
    }
}