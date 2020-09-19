using System.Collections.Generic;
using MM.Server.Data;
using UnityEngine;

namespace MM.Server.Core
{
    public static class SimilarityByCategory
    {
        private struct CategoryCount
        {
            public int Category;
            public int Count;

            public void AddOne()
            {
                Count++;
            }
        }
        
        public static float GetSimilarity(List<PlayerBasicData> playersOrderedByCategory, int maxCategory)
        {
            var categoriesCount = GetCategoriesCount(playersOrderedByCategory);
            var playerCount = playersOrderedByCategory.Count;
            
            var minSimilarity = float.MaxValue;

            for (var i = 0; i < playersOrderedByCategory.Count; ++i)
            {
                var playerCategory = playersOrderedByCategory[i].Category;
                var playerSimilarity = 0f;
                
                for (var j = 0; j < categoriesCount.Count; ++j)
                {
                    playerSimilarity += (1 - Mathf.Abs(playerCategory - categoriesCount[j].Category)) *
                                        categoriesCount[j].Count;
                }

                playerSimilarity /= playerCount;

                if (playerSimilarity < minSimilarity)
                {
                    minSimilarity = playerSimilarity;
                }
            }
            
            return minSimilarity;
        }

        private static List<CategoryCount> GetCategoriesCount(List<PlayerBasicData> playersOrderedByCategory)
        {
            var categoriesCount = new List<CategoryCount>();
            var currentCategory = -1;
            var currentCategoryIndex = -1;
            
            // Gather the accumulate category data
            for (var i = 0; i < playersOrderedByCategory.Count; i++)
            {
                var playerCategory = playersOrderedByCategory[i].Category;
                
                if (playerCategory != currentCategory)
                {
                    categoriesCount.Add(new CategoryCount
                    {
                        Category = playerCategory,
                        Count = 1
                    });

                    currentCategory = playerCategory;
                    currentCategoryIndex = categoriesCount.Count - 1;
                }
                else
                {
                    categoriesCount[currentCategoryIndex].AddOne();
                }
            }

            return categoriesCount;
        }
    }
}