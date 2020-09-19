using System.Collections.Generic;
using MM.Server.Data;
using UnityEngine;

namespace MM.Server.Core
{
    public static class UpdatePlayersRatings
    {
        private struct WinningProbabilities
        {
            public List<float> Probability;
        }
        
        public static MatchData CalculateRatingsTeamAWins(MatchData matchData, float scaleFactor, int updateSpeed)
        {
            var teamSize = matchData.TeamA.Count;
            var teamAWinProb = new List<WinningProbabilities>(teamSize);
            var teamBWinProb = new List<WinningProbabilities>(teamSize);
            
            // Calculate first the probabilities to win against any player of the other team
            for (var i = 0; i < matchData.TeamA.Count; ++i)
            {
                var teamAPlayer = matchData.TeamA[i];
                var teamAPlayerWinProb = teamAWinProb[i];
                teamAPlayerWinProb.Probability = new List<float>(teamSize);
                
                for (var j = 0; j < matchData.TeamB.Count; ++j)
                {
                    var teamBPlayer = matchData.TeamB[j];
                    var teamBPlayerWinProb = teamBWinProb[j];

                    if (i == 0)
                    {
                        teamBPlayerWinProb.Probability = new List<float>();
                    }
                    
                    teamAPlayerWinProb.Probability[j] = (1f / (1f + Mathf.Pow(10, ((teamAPlayer.Rating - teamBPlayer.Rating) / scaleFactor))));
                    teamBPlayerWinProb.Probability[i] = 1f - teamAPlayerWinProb.Probability[j];
                }
            }

            UpdateRatings(true, matchData.TeamA, teamAWinProb, updateSpeed);
            UpdateRatings(false, matchData.TeamB, teamBWinProb, updateSpeed);

            return matchData;
        }

        private static void UpdateRatings(bool hasWon, List<PlayerBasicData> players, List<WinningProbabilities> winProbs, int updateSpeed)
        {
            for (var i = 0; i < players.Count; ++i)
            {
                var ratingPoints = 0f;
                var playerWinProb = winProbs[i];

                for (var j = 0; j < winProbs.Count; ++j)
                {
                    ratingPoints += updateSpeed * ((hasWon ? 1f : 0f) - playerWinProb.Probability[j]);
                }

                ratingPoints /= winProbs.Count;

                players[i].Rating += Mathf.RoundToInt(ratingPoints);
            }
        }
    }
}