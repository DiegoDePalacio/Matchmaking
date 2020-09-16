using UnityEngine;

namespace Server.UI
{
    public class MatchList : ListWithPoolTemplate<MatchListElement>
    {
        public MatchListElement AddMatch(string firstTeamNames, string secondTeamNames)
        {
            var newMatch = AddElement();
            newMatch.FirstTeamNames = firstTeamNames;
            newMatch.SecondTeamNames = secondTeamNames;
            return newMatch;
        }

        public void RemoveMatch(MatchListElement matchListElement)
        {
            RemoveElement(matchListElement);
        }

#if UNITY_EDITOR
        // Unfortunately ContextMenu can't be included in generic classes
        [ContextMenu("Increase List Pool")]
        public void IncreasePool()
        {
            IncreasePoolInternal();
        }
        
#region Tests
        private MatchListElement m_TestDummyMatch;

        [ContextMenu("[TEST] Add Dummy Match")]
        public void TestAddDummyMatch()
        {
            m_TestDummyMatch = AddMatch("Sid Meier, John Romero, Satoshi Tajiri", "Markus Persson, Tim Schafer, Yu Suzuki");
        }

        [ContextMenu("[TEST] Remove Dummy Player")]
        public void TestDummyMatch()
        {
            RemoveMatch(m_TestDummyMatch);
        }
#endregion
#endif
    }
}
