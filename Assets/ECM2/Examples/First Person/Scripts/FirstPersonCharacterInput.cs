using UnityEngine;

namespace ECM2.Examples.FirstPerson
{
    /// <summary>
    /// First person character input.
    /// </summary>
    
    public class FirstPersonCharacterInput : MonoBehaviour
    {
        [SerializeField] float maxWalkSpeed = 2;
        [SerializeField] float maxRunSpeed = 3;
        private Character _character;

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
            if (Input.GetKey(KeyCode.LeftShift) && inputMove.y >= 0)
            {
                _character.maxWalkSpeed = maxRunSpeed;
            }
            else
            {
                _character.maxWalkSpeed = maxWalkSpeed;
            }
            
            // Crouch input

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
                _character.Crouch();
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
                _character.UnCrouch();
            
            // Jump input
            
            if (Input.GetButtonDown("Jump"))
                _character.Jump();
            else if (Input.GetButtonUp("Jump"))
                _character.StopJumping();
        }
    }
}
