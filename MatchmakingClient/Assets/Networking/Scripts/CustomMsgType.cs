using UnityEngine.Networking;

namespace Client.Networking
{
    public class CustomMsgType
    {
        public static short String = MsgType.Highest + 1;
    }
}