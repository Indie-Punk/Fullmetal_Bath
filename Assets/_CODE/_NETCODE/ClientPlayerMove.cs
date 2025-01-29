using System.Collections.Generic;
using ECM2.Examples.FirstPerson;
using ECM2.Walkthrough.Ex92;
using Unity.Netcode;
using UnityEngine;

namespace _CODE._NETCODE
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField] private List<Renderer> playerMesh;
        [SerializeField] private GameObject cameraStaff;
        [SerializeField] private FirstPersonCharacterInput personController;
        [SerializeField] private AnimationController animController;

        private void Awake()
        {
            animController.enabled = false;
            personController.enabled = false;
            cameraStaff.SetActive(false);
            foreach (var mesh in playerMesh)
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner)
                return;
            foreach (var mesh in playerMesh)
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            animController.enabled = true;
            personController.enabled = true;
            cameraStaff.SetActive(true);
        }
    }
}