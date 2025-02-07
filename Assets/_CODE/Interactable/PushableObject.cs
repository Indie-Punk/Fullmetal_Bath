using Unity.Netcode;
using UnityEngine;

namespace _CODE.Interactable
{
    public class PushableObject : NetworkBehaviour
    {
        private Rigidbody rb;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            rb = GetComponent<Rigidbody>();
        }

        [Rpc(SendTo.Server)]
        public virtual void PushRpc(Vector3 force)
        {
            if (!IsServer)
                return;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}