using System;
using UnityEngine;

namespace _CODE
{
    public class DrillSimulator : MonoBehaviour
    {
        [SerializeField] private GameObject smallDrill;
        [SerializeField] private Vector2 turnLimit = new Vector2(-30, 30);
        [SerializeField] private float currentSpeed;
        [SerializeField] private Vector2 currentTurn;
        [SerializeField] private Vector2 turnForce;

        private void FixedUpdate()
        {
            currentTurn.x = Mathf.Clamp(currentTurn.x + turnForce.x, turnLimit.x, turnLimit.y);
            currentTurn.y = Mathf.Clamp(currentTurn.y + turnForce.y, turnLimit.x, turnLimit.y);
            smallDrill.transform.position += transform.up * currentSpeed * Time.fixedDeltaTime;
            smallDrill.transform.localEulerAngles = new Vector3(currentTurn.x,0, currentTurn.y);
        }

        public void SetCurrentSpeed(float speed)
        {
            currentSpeed = speed;
        }
        public void AddTurnForce(Vector2 force)
        {
            turnForce += force * Time.fixedDeltaTime;
            turnForce = Vector2.Lerp(turnForce, Vector2.zero, Time.fixedDeltaTime);
        }
        
        
    }
}