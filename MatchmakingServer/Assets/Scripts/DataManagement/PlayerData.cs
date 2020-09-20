using System;

namespace MM.Server.Data
{
    // IMPORTANT: Don't change this class, because needs to have the same structure as the JSON file where the data is extracted
    [Serializable]
    public class PlayerBasicData
    {
        public string Name;
        public int Category;
        public int Rating;
    }

    public enum PlayerState
    {
        Inactive,
        InLobby,
        Playing
    }
    
    [Serializable]
    public class Player
    {
        public PlayerBasicData Data;
        public PlayerState State;
    }
}