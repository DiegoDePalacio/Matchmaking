using System.Collections.Generic;
using UnityEngine;

namespace MM.Server.Core
{
    [CreateAssetMenu(fileName = "RatingConstants", menuName = "ScriptableObjects/RatingConstants")]
    public class RatingConstants : ScriptableObject
    {
        public int MaxRating = 2900;
        public List<int> CategoryUpperLimit = new List<int>() {200, 400, 600, 800, 1000, 1200, 1400, 1600, 1800, 2000, 2200, 2400};

        public int MaxCategory => CategoryUpperLimit.Count + 1;

        public int GetCategory(int rating)
        {
            for (var i = 0; i < CategoryUpperLimit.Count; ++i)
            {
                if (rating < CategoryUpperLimit[i])
                {
                    return i + 1;
                }
            }

            return MaxCategory;
        }
    }    
}