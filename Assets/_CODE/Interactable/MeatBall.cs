using System;
using ECM2;
using ECM2.Examples.Glide;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CODE.Interactable
{
    public class MeatBall : InteractableItem
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private GameObject flyMeat;
        [SerializeField] private GameObject droppedMeat;
        [SerializeField] private GameObject decalPrefab;
        private bool onsleep;

        private void OnEnable()
        {
            OnTake += FlyMeat;
        }

        private void OnDisable()
        {
            OnTake -= FlyMeat;
        }

        public override void Use(InteractionController interactionController)
        {
            
        }

        // private void Update()
        // {
        //     if (rb.isKinematic)
        //         return;
        //     if (onsleep)
        //         return;
        //     if (rb.IsSleeping())
        //     {
        //         onsleep = true;
        //         DropMeat();
        //     }
        // }

        private void OnCollisionEnter(Collision other)
        {
            OnHit(other);
            Instantiate(decalPrefab, other.contacts[0].point,Quaternion.LookRotation(other.contacts[0].normal));
            if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, .2f))
            {
                
                DropMeat();
            }

            // rb.linearVelocity /= 2;
            // if (rb.linearVelocity.y < 1)
        }

        void DropMeat()
        {
            rb.isKinematic = true;
            // Debug.Log("DropMeat");
            transform.eulerAngles = Vector3.zero;
            flyMeat.SetActive(false);
            droppedMeat.SetActive(true);
        }

        void FlyMeat()
        {
            onsleep = false;
            // Debug.Log("FlyMeat");
            flyMeat.SetActive(true);
            droppedMeat.SetActive(false);
        }
    }
}