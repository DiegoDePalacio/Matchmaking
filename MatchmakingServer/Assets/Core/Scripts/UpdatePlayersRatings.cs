using System.Collections.Generic;
using MM.Server.Data;
using UnityEngine;

namespace MM.Server.Core
{
    public static class UpdatePlayersRatings
    {
        private class WinningProbabilities
        {
            public List<float> Probability;

            public WinningProbabilities(int probabilitiesCount)
            {
                Probability = new List<float>(probabilitiesCount);
                
                for (var i = 0; i < probabilitiesCount; ++i)
                {
                    Probability.Add(0f);
                }
            }
        }
        
        public static void UpdateRatingsTeamAWins(ref MatchData matchData, RatingConstants ratingConstants, EloRatingParameters ratingParameters)
        {
            var teamSize = matchData.TeamA.Count;
            var teamAWinProb = new List<WinningProbabilities>(teamSize);
            var teamBWinProb = new List<WinningProbabilities>(teamSize);

            for (var i = 0; i < teamSize; ++i)
            {
                teamAWinProb.Add(new WinningProbabilities(teamSize));
                teamBWinProb.Add(new WinningProbabilities(teamSize));
            }
            
            // Calculate first the probabilities to win against any player of the other team
            for (var i = 0; i < matchData.TeamA.Count; ++i)
            {
                var teamAPlayer = matchData.TeamA[i];
                var teamAPlayerWinProb = teamAWinProb[i];
                
                for (var j = 0; j < matchData.TeamB.Count; ++j)
                {
                    var teamBPlayer = matchData.TeamB[j];
                    var teamBPlayerWinProb = teamBWinProb[j];

                    teamAPlayerWinProb.Probability[j] = (1f / (1f + Mathf.Pow(10, ((teamBPlayer.Rating - teamAPlayer.Rating) / ratingParameters.ScaleFactor))));
                    teamBPlayerWinProb.Probability[i] = 1f - teamAPlayerWinProb.Probability[j];
                }
            }

            UpdateRatings(true, matchData.TeamA, teamAWinProb, ratingConstants, ratingParameters.UpdateSpeed);
            UpdateRatings(false, matchData.TeamB, teamBWinProb, ratingConstants, ratingParameters.UpdateSpeed);
        }
        
        private static void UpdateRatings(bool hasWon, List<PlayerBasicData> players, List<WinningProbabilities> winProbs, RatingConstants ratingConstants, float updateSpeed)
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
                players[i].Category = ratingConstants.GetCategory(players[i].Rating);
            }
        }
    }
}