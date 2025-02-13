using System;
using System.Collections.Generic;
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
        [SerializeField] private bool debug;
        [SerializeField] private int transmission = 1;
        [SerializeField] private List<float> speed;
        private void Update()
        {
            DebugInput();
        }

        void DebugInput()
        {
            if (!debug)
                return;
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            if (Input.GetKeyDown(KeyCode.E))
                SetTransmission(transmission + 1);
            if (Input.GetKeyDown(KeyCode.R))
                SetTransmission(transmission - 1);
            AddTurnForce(new Vector2(horizontal, vertical));
        }
        private void FixedUpdate()
        {
            currentTurn.x = Mathf.Clamp(currentTurn.x + turnForce.x, turnLimit.x, turnLimit.y);
            currentTurn.y = Mathf.Clamp(currentTurn.y + turnForce.y, turnLimit.x, turnLimit.y);
            smallDrill.transform.position -= smallDrill.transform.up * currentSpeed * Time.fixedDeltaTime;
            smallDrill.transform.localEulerAngles = new Vector3(currentTurn.x,0, currentTurn.y);
        }

        public void SetTransmission(int value)
        {
            transmission = value;
            transmission = Mathf.Clamp(transmission, 0, 3);
            currentSpeed = speed[transmission];
        }
        public void SetCurrentSpeed(float value)
        {
            currentSpeed = value;
        }
        public void AddTurnForce(Vector2 force)
        {
            turnForce += force * Time.fixedDeltaTime;
            turnForce = Vector2.Lerp(turnForce, Vector2.zero, Time.fixedDeltaTime);
        }
        
        
    }
}