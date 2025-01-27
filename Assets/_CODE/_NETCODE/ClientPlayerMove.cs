using System.Collections.Generic;
using ECM2.Walkthrough.Ex92;
using Unity.Netcode;
using UnityEngine;

namespace _CODE._NETCODE
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> cameraStaff;
        [SerializeField] private ThirdPersonController personController;
        [SerializeField] private AnimationController animController;
        private void Awake()
        {
            animController.enabled = false;
            personController.enabled = false;
            foreach (var staff in cameraStaff)
            {
                staff.SetActive(false);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                animController.enabled = true;
                personController.enabled = true;
                foreach (var staff in cameraStaff)
                {
                    staff.SetActive(true);
                }
            }
        }
    }
}