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

            // Compute input move vector in local space
            animator.SetFloat("Speed", _character.GetSpeed());
            Debug.Log(_character.GetSpeed());

            Vector3 move = _character.GetMovementDirection();

            // Update the animator parameters

            float forwardAmount = move.z;

            animator.SetFloat(Forward, forwardAmount, 0.1f, deltaTime);
            animator.SetFloat(Turn, move.x, 0.1f, deltaTime);
            // Vector3 move = transform.InverseTransformDirection(_character.GetMovementDirection());
            //
            // // Update the animator parameters
            //
            // float forwardAmount = _character.useRootMotion && _character.GetRootMotionController()
            //     ? move.z
            //     : Mathf.InverseLerp(0.0f, _character.GetMaxSpeed(), _character.GetSpeed());
            //
            // animator.SetFloat(Forward, forwardAmount, 0.1f, deltaTime);
            //
            // animator.SetBool(Ground, _character.IsGrounded());
            // animator.SetBool(Crouch, _character.IsCrouched());
            //
            // if (_character.IsFalling())
            //     animator.SetFloat(Jump, _character.GetVelocity().y, 0.1f, deltaTime);
            //
            // // Calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // // (This code is reliant on the specific run cycle offset in our animations,
            // // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            //
            // float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
            // float jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * forwardAmount;
            //
            // if (_character.IsGrounded())
            //     animator.SetFloat(JumpLeg, jumpLeg);
        }
    }
}