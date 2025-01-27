using Unity.Netcode.Components;

namespace _CODE._NETCODE
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}