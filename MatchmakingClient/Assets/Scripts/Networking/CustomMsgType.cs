using UnityEngine.Networking;

namespace MM.Client.Networking
{
// This enum is using extensively MsgType which is part of the Unity Networking high level API classes that are
// deprecated and will be removed in the future. The new API is not yet ready and I didn't want to use a 3rd party
// library because of the time limitations of this coding challenge. For that reason the CS0618 is disabled here.
#pragma warning disable 618    
    public enum CustomMsgType : short
    {
        Notification = MsgType.Highest + 1,
        PlayerRating = MsgType.Highest + 2,
        PlayerUpdateAfterMatch = MsgType.Highest + 3,
        PlayerJoinsLobby = MsgType.Highest + 4,
        PlayerLeavesLobby = MsgType.Highest + 5,
        PlayerRemovedFromLobby = MsgType.Highest + 6,
        PlayerOnMatch = MsgType.Highest + 7
    }
#pragma warning restore 618
}