using System;
using ECM2;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CODE.Interactable
{
    public abstract class InteractableItem : MonoBehaviour
    {
        public Action OnTake;
        public Action OnDestroy;
        [SerializeField] AudioSource audioSource;
        [SerializeField] private AudioClip hitSfx;


        protected void OnHit(Collision other)
        {
            
            if (other.gameObject.GetComponent<CharacterMovement>())
                return;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(hitSfx,Random.Range(.5f,1f));
        }
        public abstract void Use(InteractionController interactionController);
        
        public virtual void Take()
        {
            OnTake?.Invoke();
        }
        
        public void Drop()
        {
            
        }
    }
}