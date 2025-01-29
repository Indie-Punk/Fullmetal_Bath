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

            if (Input.GetKeyDown(KeyCode.Keypad1))
                animator.SetTrigger("Greetings");
            // Compute input move vector in local space
            animator.SetFloat("Speed", _character.GetSpeed());
            animator.SetBool(Crouch, _character.IsCrouched());
            animator.SetBool(Crouch, _character.IsCrouched());
            Debug.Log(_character.GetSpeed());

            // Vector3 move = _character.GetMovementDirection();
            Vector3 move = transform.InverseTransformDirection(_character.GetMovementDirection());

            // Update the animator parameters

            float forwardAmount = move.z;

            animator.SetFloat(Forward, forwardAmount, 0.1f, deltaTime);
            animator.SetFloat(Turn, move.x, 0.1f, deltaTime);
            animator.SetBool(Ground, _character.IsGrounded());
            // Vector3 move = transform.InverseTransformDirection(_character.GetMovementDirection());
            //
            // // Update the animator parameters
            //
            // float forwardAmount = _character.useRootMotion && _character.GetRootMotionController()
            //     ? move.z
            //     : Mathf.InverseLerp(0.0f, _character.GetMaxSpeed(), _character.GetSpeed());
            //
            //
            // animator.SetBool(Ground, _character.IsGrounded());
            // animator.SetBool(Crouch, _character.IsCrouched());
            //
            // if (_character.IsFalling())
            //     animator.SetFloat(Jump, _character.GetVelocity().y, 0.1f, deltaTime);
            //
        }
    }
}