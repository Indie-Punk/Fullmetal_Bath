using System.Collections.Generic;
using Cinemachine;
using ECM2.Examples.FirstPerson;
using ECM2.Walkthrough.Ex92;
using Unity.Netcode;
using UnityEngine;

namespace _CODE._NETCODE
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField] private List<Renderer> playerMesh;
        [SerializeField] private Camera camera;
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private List<CinemachineVirtualCamera> cinemachineStaff;
        [SerializeField] private FirstPersonCharacterInput personController;
        [SerializeField] private AnimationController animController;
        [SerializeField] AudioListener audioListener;
        private void Awake()
        {
            animController.enabled = false;
            personController.enabled = false;
            camera.enabled = false;
            brain.enabled = false;
            audioListener.enabled = false;
            foreach (var c in cinemachineStaff)
            {
                c.enabled = false;
            }
            foreach (var mesh in playerMesh)
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner)
                return;
            brain.enabled = true;
            foreach (var mesh in playerMesh)
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            foreach (var c in cinemachineStaff)
            {
                c.enabled = true;
            }
            audioListener.enabled = true;
            animController.enabled = true;
            personController.enabled = true;
            camera.enabled = true;
        }
    }
}