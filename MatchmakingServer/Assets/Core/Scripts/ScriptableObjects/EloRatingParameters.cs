using UnityEngine;

namespace MM.Server.Core
{
    [CreateAssetMenu(fileName = "EloRatingParameters", menuName = "ScriptableObjects/EloRatingParameters", order = 1)]
    public class EloRatingParameters : ScriptableObject
    {
        public int ScaleFactor = 400;
        public int UpdateSpeed = 32;
    }    
}