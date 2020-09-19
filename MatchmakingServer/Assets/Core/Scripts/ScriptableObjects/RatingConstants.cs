using UnityEngine;

namespace MM.Server.Core
{
    [CreateAssetMenu(fileName = "RatingConstants", menuName = "ScriptableObjects/RatingConstants", order = 1)]
    public class RatingConstants : ScriptableObject
    {
        public int MaxRating = 2900;
        public int MaxCategory = 13;
    }    
}