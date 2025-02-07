using _CODE.Player;
using ECM2;
using UnityEngine;
using UnityEngine.UIElements;

namespace _CODE
{
    public class AnimationController : MonoBehaviour
    {
        // Cached Character

        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int Ground = Animator.StringToHash("OnGround");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private Character _character;
        [SerializeField] private InteractionController _interactionController;

        private void Awake()
        {
            // Cache our Character

            _character = GetComponentInParent<Character>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // Get Character animator

            Animator animator = _character.GetAnimator();

            animator.SetBool("Sit", _interactionController.IsSit);
            // if (playerInput.Is)
            if (Input.GetKeyDown(KeyCode.Alpha1))
                animator.SetTrigger("Greetings");
            // Compute input move vector in local space
            animator.SetFloat("Speed", _character.GetSpeed());
            animator.SetBool(Crouch, _character.IsCrouched());
            animator.SetBool(Crouch, _character.IsCrouched());

            // Vector3 move = _character.GetMovementDirection();
            
            Vector3 move = transform.InverseTransformDirection(_character.GetMovementDirection());

            // Update the animator parameters

            float forwardAmount = move.z;

            animator.SetFloat(Forward, forwardAmount, 0.02f, deltaTime);
            animator.SetFloat(Turn, move.x, 0.02f, deltaTime);
            animator.SetBool(Ground, _character.IsGrounded());
        }
    }
}