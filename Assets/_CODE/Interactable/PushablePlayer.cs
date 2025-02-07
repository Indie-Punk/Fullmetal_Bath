using ECM2;
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

        [Rpc(SendTo.Everyone)]
        public override void PushRpc(Vector3 force)
        {
            // if (!IsOwner)
            //     return;
            Debug.Log("pushing player " + force);
            // var rb = GetComponent<Rigidbody>();
            // rb.AddForce(force, ForceMode.Impulse);
            GetComponent<CharacterMovement>().velocity += force;
        }
    }
}