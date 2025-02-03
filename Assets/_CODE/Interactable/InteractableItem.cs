using System;
using ECM2;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CODE.Interactable
{
    public abstract class InteractableItem : NetworkBehaviour
    {
        public Action OnTake;
        public Action OnDestroy;
        [SerializeField] AudioSource audioSource;
        [SerializeField] private AudioClip hitSfx;

        // [SerializeField] public Transform followPos;

        protected void OnHit(Collision other)
        {
            
            if (other.gameObject.GetComponent<CharacterMovement>())
                return;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(hitSfx,Random.Range(.5f,1f));
        }
        public abstract void Use(InteractionController interactionController);

        // private void FixedUpdate()
        // {
        //     if (followPos == null)
        //         return;
        //     transform.position = followPos.position;
        // }

        public virtual void Take()
        {
            OnTake?.Invoke();
        }
        
        public void Drop()
        {
            
        }
    }
}