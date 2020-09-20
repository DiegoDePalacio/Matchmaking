using UnityEngine;

namespace MM.Server.Core
{
    public enum MatchmakingType
    {
        ByRating = 0,
        ByCategory = 1
    }
    
    [CreateAssetMenu(fileName = "MatchmakingParams", menuName = "ScriptableObjects/MatchmakingParams")]
    public class MatchmakingParams : ScriptableObject
    {
        public int TeamSize = 5;
        public MatchmakingType Matchmaking = MatchmakingType.ByRating;
        public float MinSimilarityToMatchmake = 0.85f;

        [Range(0, 10)]
        public int MaxTeamSizeForAccurateMatchmake = 5;
    }    
}