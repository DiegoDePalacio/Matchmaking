using System.Collections.Generic;
using MM.Server.Data;

namespace MM.Server.Core
{
    // This implementation is intended to distribute players in matches with a big number of players
    // It uses the Greedy algorithm. This heuristic approach does not find a best solution, but it terminates
    // with an acceptable result in a reasonable number of steps. More about it in the section
    // "About splitting the players between teams" inside of the README file
    public static class MatchmakingBigTeams
    {
        public static MatchData ArrangePlayers(List<PlayerBasicData> playersOrderedByRating)
        {
            var matchData = new MatchData();
            var playersPerTeam = playersOrderedByRating.Count / 2;
            var teamATotalRating = 0;
            var teamBTotalRating = 0;

            for (var i = 0; i < playersOrderedByRating.Count; ++i)
            {
                var player = playersOrderedByRating[i];
                
                if (matchData.TeamA.Count == playersPerTeam)
                {
                    matchData.TeamB.Add(player);
                }
                else if (matchData.TeamB.Count == playersPerTeam)
                {
                    matchData.TeamA.Add(player);
                }
                else if (teamATotalRating < teamBTotalRating)
                {
                    matchData.TeamA.Add(player);
                    teamATotalRating += player.Rating;
                }
                else
                {
                    matchData.TeamB.Add(player);
                    teamBTotalRating += player.Rating;
                }
            }

            return matchData;
        }
    }
}