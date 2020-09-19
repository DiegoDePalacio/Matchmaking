using UnityEngine;

namespace MM.Server.Core
{
    public enum MatchmakingType
    {
        ByRating = 0,
        ByCategory = 1
    }
    
    [CreateAssetMenu(fileName = "DefaultMatchmakingUIParams", menuName = "ScriptableObjects/DefaultMatchmakingUIParams", order = 1)]
    public class DefaultMatchmakingUIParams : ScriptableObject
    {
        public int TeamSize = 5;
        public MatchmakingType Matchmaking = MatchmakingType.ByRating;
        public float MinSimilarityToMatchmake = 0.85f;
    }    
}