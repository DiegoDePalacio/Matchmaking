using UnityEngine.Networking;

namespace Server.Networking
{
    public enum CustomMsgType : short
    {
        Notification = MsgType.Highest + 1,
        PlayerRating = MsgType.Highest + 2,
        PlayerUpdate = MsgType.Highest + 3,
        PlayerJoinsLobby = MsgType.Highest + 4,
        PlayerLeavesLobby = MsgType.Highest + 5,
        PlayerRemovedFromLobby = MsgType.Highest + 6
    }
}