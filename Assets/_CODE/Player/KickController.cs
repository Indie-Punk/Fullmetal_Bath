using System;
using System.Collections;
using _CODE.Interactable;
using Unity.Netcode;
using UnityEngine;

namespace _CODE
{
    public class KickController : MonoBehaviour
    {
        [SerializeField] private Animator kickAnim;
        [SerializeField] private Animator thirdPerson;
        [SerializeField] private float kickForce = 20;
        [SerializeField] float kickCooldown = 1.25f;
        public bool kicked;
        float _kickTimer;



        public void Kick(PushableObject pushableObject)
        {
            if (_kickTimer > 0)
                return;
            _kickTimer = kickCooldown;
            kickAnim.SetTrigger("Kick");
            thirdPerson.SetTrigger("Kick");
            StartCoroutine(KickCoroutine(pushableObject));
            
        }

        IEnumerator KickCoroutine(PushableObject pushableObject)
        {
            yield return new WaitForSeconds(.45f);
            pushableObject?.PushRpc(transform.forward * kickForce);

        }

        private void Update()
        {
            _kickTimer -= Time.deltaTime;
        }
    }
}