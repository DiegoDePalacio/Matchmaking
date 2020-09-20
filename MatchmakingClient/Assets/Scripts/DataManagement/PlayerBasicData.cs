using System;

namespace MM.Client.Data
{
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