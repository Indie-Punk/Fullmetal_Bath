using Unity.Netcode;
using UnityEngine;

namespace _CODE.Interactable
{
    public class PushablePlayer : PushableObject
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        [Rpc(SendTo.Server)]
        public override void PushRpc(Vector3 force)
        {
            // if (!IsOwner)
            //     return;
            Debug.Log("pushing player " + force);
            transform.position += force;
        }
    }
}