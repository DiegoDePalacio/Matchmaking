using System.Collections;
using System.Collections.Generic;
using MM.Server.Core;
using MM.Server.Data;
using NUnit.Framework;
using UnityEngine.TestTools;
using RangeAttribute = NUnit.Framework.RangeAttribute;

namespace MM.Server.Tests.PlayMode
{
    public class SimilarityTests
    {
        // Checking that the Similarity by Category is working as expected (i.e. more similarity with players closer in category)
        // This test could be also a Edit mode test, because doesn't have to wait for the game loop to be executed to check
        // the results. However was included here as a matter of showing that I know how to implement PlayMode tests :)
        [UnityTest]
        public IEnumerator SimilarityTests_SimilarityByCategory_LessSimilarityBetweenSeparatedCatogories(
            [Range(1, 3, 1)] int categoryDifference)
        {
            var maxCategory = 13;

            var worstPlayerData = new PlayerBasicData
            {
                Category = 1
            };

            var averagePlayerData = new PlayerBasicData
            {
                Category = worstPlayerData.Category + categoryDifference
            };

            var bestPlayerData = new PlayerBasicData
            {
                Category = averagePlayerData.Category + categoryDifference
            };

            var worstPlayers = new List<PlayerBasicData>(new []
            {
                averagePlayerData,
                worstPlayerData
            });

            var worstPlayersSimilarity = SimilarityByCategory.GetSimilarity(worstPlayers, maxCategory);
            
            var oppositePlayers = new List<PlayerBasicData>( new []
            {
                bestPlayerData,
                worstPlayerData
            });

            var oppositePlayersSimilarity = SimilarityByCategory.GetSimilarity(oppositePlayers, maxCategory);
            
            var bestPlayers = new List<PlayerBasicData>( new []
            {
                bestPlayerData,
                averagePlayerData
            });
            
            var bestPlayersSimilarity = SimilarityByCategory.GetSimilarity(bestPlayers, maxCategory);
            
            Assert.IsTrue(oppositePlayersSimilarity < worstPlayersSimilarity);
            Assert.IsTrue(oppositePlayersSimilarity < bestPlayersSimilarity);
            yield return null;
        }
    }
}