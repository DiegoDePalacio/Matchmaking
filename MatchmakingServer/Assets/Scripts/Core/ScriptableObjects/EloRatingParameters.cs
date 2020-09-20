using UnityEngine;

namespace MM.Server.Core
{
    [CreateAssetMenu(fileName = "EloRatingParameters", menuName = "ScriptableObjects/EloRatingParameters")]
    public class EloRatingParameters : ScriptableObject
    {
        public float ScaleFactor = 400;
        public float UpdateSpeed = 32;
    }    
}