using System;
using UnityEngine;

namespace _CODE
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private LayerMask interactionLayerMask;
        [SerializeField] private Transform takePos;
        [SerializeField] private Beer beer;

        private void Update()
        {
            if (beer != null && Input.GetKeyDown(KeyCode.E))
            {
                beer.Use();
                beer = null;
                return;
            }
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10, interactionLayerMask))

            {

                if (hit.transform.gameObject.TryGetComponent<Beer>(out var b))
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        beer = b;
                        beer.GetComponent<Rigidbody>().isKinematic = true;
                        beer.GetComponent<Collider>().enabled = false;
                        beer.transform.parent = takePos.transform;
                        beer.transform.localPosition = Vector3.zero;
                    }
                }

                if (hit.transform.gameObject.TryGetComponent<InteractionBtn>(out var btn))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                        btn.Touch();
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