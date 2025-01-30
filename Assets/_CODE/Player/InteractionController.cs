using System;
using _CODE.Interactable;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace _CODE
{
    public class InteractionController : NetworkBehaviour
    {
        [SerializeField] NetworkBehaviour _networkBehaviour;
        [SerializeField] public BurpController burpController;
        [SerializeField] private LayerMask interactionLayerMask;
        [SerializeField] private Transform takePos;
        // [SerializeField] private Beer beer;
        [SerializeField] private InteractableItem interactItem;
        [SerializeField] private float pushForce;
        [SerializeField] private float torqueForce;

        [Rpc(SendTo.Server)]
        private void MoveToRpc()
        {
            interactItem.transform.position = takePos.position;
            interactItem.transform.rotation = takePos.rotation;
        }
        private void Update()
        {
            if (!_networkBehaviour.IsOwner)
                return;
            // Debug.Log("is owner " +IsOwner);
            // if (!IsOwner)
            //     return;
            Interaction();
            if (interactItem != null)
            {
                MoveToRpc();
            }
            else
            {
                Searching();
            }
        }

        private void Interaction()
        {
            if (interactItem != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Use");
                    interactItem.Use(this);
                    // interactItem = null;
                }
                else if (Input.GetKeyDown(KeyCode.G))
                {
                    Debug.Log("Drop");
                    interactItem.GetComponent<Collider>().enabled = true;
                    var rbItem = interactItem.GetComponent<Rigidbody>();
                    rbItem.isKinematic = false;
                    rbItem.AddForce(transform.forward * pushForce, ForceMode.Impulse);
                    rbItem.AddTorque(transform.up * torqueForce, ForceMode.Impulse);
                    interactItem = null;
                }
            }

        }

        private void Searching()
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10, interactionLayerMask))
            {
                
                if (hit.transform.gameObject.TryGetComponent<InteractionBtn>(out var btn))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                        btn.Touch();
                }
                else if (hit.transform.gameObject.TryGetComponent<InteractableItem>(out var item))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("Take");
                        item.Take();
                        interactItem = item;
                        interactItem.GetComponent<Collider>().enabled = false;
                        interactItem.GetComponent<Rigidbody>().isKinematic = true;
                        interactItem.transform.localPosition = Vector3.zero;
                        interactItem.OnDestroy += () => { interactItem = null; };
                    }
                }

                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow); 
                // Debug.Log("Did Hit"); 
            }
            else
            { 
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white); 
                // Debug.Log("Did not Hit"); 
            }
        }
    }
}