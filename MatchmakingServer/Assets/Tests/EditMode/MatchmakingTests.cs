using System.Collections.Generic;
using System.Linq;
using MM.Server.Core;
using MM.Server.Data;
using NUnit.Framework;

namespace MM.Server.Tests.EditMode
{
    public class MatchmakingTests
    {
        [Test]
        public void Matchmaking_MatchmakingSmallTeams_BestDistributionFound()
        {
            // Verifying the right assignation to the teams
            // 1914 + 2120 + 1846 = 1880 + 2131 + 1869 = 5880
            var playerList = new List<PlayerBasicData>(new PlayerBasicData[]
            {
                new PlayerBasicData
                {
                    Category = 10,
                    Name = "Sid Meier",
                    Rating = 1914
                },
                new PlayerBasicData
                {
                    Category = 10,
                    Name = "John Romero",
                    Rating = 1880
                },
                new PlayerBasicData
                {
                    Category = 10,
                    Name = "Satoshi Tajiri",
                    Rating = 1869
                },
                new PlayerBasicData
                {
                    Category = 11,
                    Name = "Markus Persson",
                    Rating = 2131
                },
                new PlayerBasicData
                {
                    Category = 11,
                    Name = "Tim Schafer",
                    Rating = 2120
                },
                new PlayerBasicData
                {
                    Category = 10,
                    Name = "Yu Suzuki",
                    Rating = 1846
                }
            });

            playerList.Sort((playerA, playerB) => playerB.Rating.CompareTo(playerA.Rating));
            MatchData matchData = MatchmakingSmallTeams.ArrangePlayers(playerList);

            var teamARatingSum = matchData.TeamA.Sum(player => player.Rating);
            var teamBRatingSum = matchData.TeamB.Sum(player => player.Rating);
            Assert.IsTrue(teamARatingSum == teamBRatingSum);
        }
    }
}