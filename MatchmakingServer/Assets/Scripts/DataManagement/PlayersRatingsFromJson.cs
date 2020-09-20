using System;
using UnityEngine;

namespace MM.Server.Data
{
    public class PlayersRatingsFromJson : MonoBehaviour
    {
        // This field is assigned in Editor, therefore the warning CS0649 about the lack of assignation is disabled
        // Its assignation is checked on the Awake method and throwing an exception if is still null in runtime
#pragma warning disable 649
        [SerializeField] private TextAsset m_PlayersRatingsJson;
#pragma warning restore 649
        public string PlayersRatingsJson => m_PlayersRatingsJson.text;

        private void Awake()
        {
            if (m_PlayersRatingsJson == null)
                throw new NullReferenceException($"The Player's Ratings JSON text asset was not assigned on the PlayersRatingsFromJson component of {gameObject.name}");
        }

        public PlayersData GetData()
        {
            if (m_PlayersRatingsJson == null)
            {
                Debug.LogErrorFormat($"No JSON file is attached to '{gameObject.name}' to get the player's ratings!");
                return null;
            }

            return JsonUtility.FromJson<PlayersData>(m_PlayersRatingsJson.text);
        }
    }
}