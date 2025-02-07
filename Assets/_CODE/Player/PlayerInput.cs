using _CODE.Stats;
using ECM2;
using UnityEngine;

namespace _CODE.Player
{
    /// <summary>
    /// First person character input.
    /// </summary>
    
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] float maxWalkSpeed = 2;
        [SerializeField] float maxRunSpeed = 3;
        private Character _character;
        private StatsManager _statsManager;
        private InteractionController _interactionController;
        bool isRunning;
        bool isCrouch;

        public bool IsRunning => isRunning;

        private void Awake()
        {
            _interactionController = GetComponent<InteractionController>();
            _statsManager = GetComponent<StatsManager>();
            _character = GetComponent<Character>();
        }

        private void Update()
        {
            // Movement input, relative to character's view direction
            StandUp();

            if (_interactionController.IsSit)
            {
                _character.SetMovementDirection(Vector3.zero);
                return;
            }
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
            Sprint(inputMove);
            
            // Crouch input
            Crouch();
            
            // Jump input
            Jump();
        }

        void StandUp()
        {
            if (Input.GetKeyDown(KeyCode.X))
                _interactionController.StandUp();
        }
        void Crouch()
        {
            
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
                _character.Crouch();
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
                _character.UnCrouch();
        }
        
        void Sprint(Vector2 inputMove)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && _statsManager.stamina.Value > 20)
            {
                isRunning = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || _statsManager.stamina.Value <= 1 || inputMove.y < 0 || _character.IsCrouched())
            {
                isRunning = false;
            }
            if (isRunning)
            {
                _statsManager.stamina.Value -= Time.deltaTime * 40;
                _character.maxWalkSpeed = maxRunSpeed;
            }
            else if (!isRunning)
            {
                _character.maxWalkSpeed = maxWalkSpeed;
            }
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
