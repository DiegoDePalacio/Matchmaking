using UnityEngine.Networking;

namespace MM.Client.Networking
{
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
}