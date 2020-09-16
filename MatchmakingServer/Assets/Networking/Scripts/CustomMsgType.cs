using UnityEngine.Networking;

namespace Server.Networking
{
    public class CustomMsgType
    {
        public static short String = MsgType.Highest + 1;
    }
}