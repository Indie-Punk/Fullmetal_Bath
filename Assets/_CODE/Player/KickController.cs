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
        [SerializeField] private Transform forcePoint;
        public bool kicked;
        float _kickTimer;


        private PushableObject _pushableObject;
        public void Kick(PushableObject pushableObject)
        {
            
            if (_kickTimer > 0)
                return;
            _kickTimer = kickCooldown;
            kickAnim.SetTrigger("Kick");
            thirdPerson.SetTrigger("Kick");
            _pushableObject = pushableObject;
            
        }

        public void KickImpact()
        {
            StartCoroutine(KickCoroutine());
        }
        
        IEnumerator KickCoroutine()
        {
            yield return new WaitForSeconds(.45f);
            _pushableObject?.PushRpc(forcePoint.forward * kickForce);

        }

        private void Update()
        {
            _kickTimer -= Time.deltaTime;
        }
    }
}