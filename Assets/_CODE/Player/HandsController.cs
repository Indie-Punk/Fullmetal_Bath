using System;
using _CODE.Player;
using UnityEngine;

namespace _CODE
{
    public class HandsController : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;

        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _anim.SetTrigger("Drink");
            }
            _anim.SetBool("Run", input.IsRunning);
            // if (input.IsRunning)
            // {
            //     
            // }
        }
    }
}