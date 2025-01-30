using System;
using _CODE.Interactable;
using UnityEngine;

namespace _CODE
{
    public class Beer : InteractableItem
    {
        public override void Use(InteractionController interactionController)
        {
            interactionController.burpController.Burp();
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            OnHit(other);
        }
    }
}