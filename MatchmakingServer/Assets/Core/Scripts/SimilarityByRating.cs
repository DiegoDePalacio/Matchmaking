using System;
using System.Collections.Generic;
using System.Linq;
using MM.Server.Data;

namespace MM.Server.Core
{
    public static class SimilarityByRating
    {
        public static float GetSimilarity(List<PlayerBasicData> playersToCompare, int maxRating)
        {
            var ratingSum = playersToCompare.Sum(player => player.Rating / (float) maxRating);
            var ratingAverage = ratingSum / playersToCompare.Count;

            var minSimilarity = float.MaxValue;

            for (var i = 0; i < playersToCompare.Count; ++i)
            {
                var playerSimilarity = 1 - Math.Abs((playersToCompare[i].Rating / (float) maxRating) - ratingAverage);

                if (playerSimilarity < minSimilarity)
                {
                    minSimilarity = playerSimilarity;
                }
            }

            return minSimilarity;
        }
    }
}