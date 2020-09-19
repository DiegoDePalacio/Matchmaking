using System.Collections.Generic;
using System.Linq;
using MM.Server.Data;
using UnityEngine;

namespace MM.Server.Core
{
    // This algorithm finds the fairest distribution, but can becomes expensive for big team sizes
    // A detailed explanation of the algorithm can be found on the section "About splitting the
    // players between teams" of the README file
    public static class MatchmakingSmallTeams
    {
        private struct ModifiedRating
        {
            public int Index;
            public int Rating;
        }
        
        public static MatchData ArrangePlayers(List<PlayerBasicData> playersOrderedByRating)
        {
            // Taking off the worst player and remove his rating from all players to search only
            // for n-1 players (where n is the players per team)
            MatchData matchData = new MatchData();

            var worstPlayerIndex = playersOrderedByRating.Count - 1;
            var worstPlayer = playersOrderedByRating[worstPlayerIndex];
            var worstRating = worstPlayer.Rating;
            
            var modifiedRatings = new List<ModifiedRating>();

            for (var i = 0; i < playersOrderedByRating.Count; ++i)
            {
                modifiedRatings.Add(new ModifiedRating
                {
                    Index = i,
                    Rating = playersOrderedByRating[i].Rating - worstRating
                });
            }
            
            modifiedRatings.Reverse();

            var remainingPlayersToAdd = modifiedRatings.Count / 2 - 1;
            modifiedRatings.RemoveAt(0);
            var halfTotalModifiedRatings = modifiedRatings.Sum(modifiedRating => modifiedRating.Rating) / 2f;
            var selectedIndices = new List<int>();

            // Calling a recursive method to find the combination of players whose cumulative rating
            // is the closest possible to half the sum of the ratings of all the players in the match 
            // (i.e. the fairest distribution of players)
            GetCloserSum(modifiedRatings, remainingPlayersToAdd, 0, halfTotalModifiedRatings, ref selectedIndices);

            matchData.TeamA.Add(worstPlayer);
            playersOrderedByRating.RemoveAt(worstPlayerIndex);

            for (var i = 0; i < playersOrderedByRating.Count; ++i)
            {
                if (selectedIndices.Contains(i))
                {
                    matchData.TeamA.Add(playersOrderedByRating[i]);
                }
                else
                {
                    matchData.TeamB.Add(playersOrderedByRating[i]);
                }
            }
            
            return matchData;
        }

        private static int GetCloserSum(List<ModifiedRating> fromList, int remainingElements, int accumulated, float target, ref List<int> selectedIndices)
        {
            remainingElements--;
            var stepsCount = fromList.Count - remainingElements;
            var distance = float.MaxValue;
            var bestIndices = new List<int>(selectedIndices);
            var originalAccumulated = accumulated;
            var newFromList = new List<ModifiedRating>(fromList);
            
            for (var i = 0; i < stepsCount; ++i)
            {
                var newSelectedIndices = new List<int>(selectedIndices);
                var newAccumulated = originalAccumulated + newFromList[0].Rating;
                
                newSelectedIndices.Add(newFromList[0].Index);
                newFromList.RemoveAt(0);

                if (remainingElements > 0)
                {
                    newAccumulated = GetCloserSum(newFromList, remainingElements, newAccumulated, target, ref newSelectedIndices);
                }
                
                var newDistance = Mathf.Abs(target - newAccumulated);
                
                if (newDistance < distance)
                {
                    distance = newDistance;
                    bestIndices = newSelectedIndices;
                    accumulated = newAccumulated;
                }
                // Because the next players to include in the possible combinations have a higher rating, the next
                // iterations in this for-loop can't find better solutions than those already found
                else if (newAccumulated > target)
                {
                    break;
                }
            }

            selectedIndices = bestIndices;
            return accumulated;
        }
    }
}