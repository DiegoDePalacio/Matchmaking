using System.Collections.Generic;
using MM.Server.Data;
using UnityEngine;

namespace MM.Server.UI
{
    public class MatchList : ListWithPoolTemplate<MatchListElement>
    {
        public MatchListElement AddMatch(MatchData matchData)
        {
            var newMatch = AddElement();
            newMatch.SetMatch(matchData);
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
            m_TestDummyMatch = AddMatch(new MatchData
            {
                TeamA = new List<PlayerBasicData>(new PlayerBasicData[]
                {
                    new PlayerBasicData
                    {
                        Category = 13,
                        Name = "Sid Meier",
                        Rating = 2899
                    },
                    new PlayerBasicData
                    {
                        Category = 12,
                        Name = "John Romero",
                        Rating = 2350
                    },
                    new PlayerBasicData
                    {
                        Category = 13,
                        Name = "Satoshi Tajiri",
                        Rating = 2850
                    }
                }),
                TeamB = new List<PlayerBasicData>(new PlayerBasicData[]
                {
                    new PlayerBasicData
                    {
                        Category = 13,
                        Name = "Markus Persson",
                        Rating = 2800
                    },
                    new PlayerBasicData
                    {
                        Category = 12,
                        Name = "Tim Schafer",
                        Rating = 2399
                    },
                    new PlayerBasicData
                    {
                        Category = 13,
                        Name = "Yu Suzuki",
                        Rating = 2888
                    }
                })
            });
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
