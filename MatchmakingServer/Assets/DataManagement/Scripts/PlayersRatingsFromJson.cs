using Server.Data;
using UnityEngine;

public class PlayersRatingsFromJson : MonoBehaviour
{
    [SerializeField] private TextAsset m_PlayersRatingsJson;
    public string PlayersRatingsJson => m_PlayersRatingsJson.text;

    public PlayersData GetData()
    {
        if (m_PlayersRatingsJson == null)
        {
            Debug.LogErrorFormat("No JSON file is attached to '{0}' to get the player's ratings!");
            return null;
        }

        return JsonUtility.FromJson<PlayersData>(m_PlayersRatingsJson.text);
    }
}