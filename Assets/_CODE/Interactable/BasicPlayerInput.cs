using ECM2;
using UnityEngine;

namespace _CODE.Interactable
{
    public class BasicPlayerInput : MonoBehaviour
    {
        [SerializeField] float maxWalkSpeed = 2;
        [SerializeField] float maxRunSpeed = 3;
        private Character _character;
        bool isRunning;
        bool isCrouch;

        public bool IsRunning => isRunning;

        private void Awake()
        {
            _character = GetComponent<Character>();
        }

        private void Update()
        {
            // Movement input, relative to character's view direction
            Vector2 inputMove = new Vector2()
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };
            
            Vector3 movementDirection =  Vector3.zero;
            
            movementDirection += _character.GetRightVector() * inputMove.x;
            movementDirection += _character.GetForwardVector() * inputMove.y;

            _character.SetMovementDirection(movementDirection);
            
            // Run input
            // Sprint(inputMove);
            
            // Crouch input
            Crouch();
            
            // Jump input
            Jump();
        }

        void Crouch()
        {
            
            if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C)) && !isRunning)
                _character.Crouch();
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
                _character.UnCrouch();
        }

        void Jump()
        {

            if (Input.GetButtonDown("Jump"))
                _character.Jump();
            else if (Input.GetButtonUp("Jump"))
                _character.StopJumping();
        }
    }
}