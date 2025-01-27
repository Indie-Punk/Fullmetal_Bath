using Unity.Netcode.Components;

namespace _CODE._NETCODE
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}