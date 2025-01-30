using System;
using UnityEngine;

namespace _CODE
{
    public class SteeringWheel : MonoBehaviour
    {
        [SerializeField] private DrillSimulator drillSimulator;
        // [SerializeField] private force;
        private Vector2 rotForce;
        private void Update()
        {
            var rot = transform.localRotation;
            if (Input.GetKey(KeyCode.Keypad4))
                rotForce.x = 1;
            else if (Input.GetKey(KeyCode.Keypad6))
                rotForce.x = -1;
            else
                rotForce.x = 0;
            if (Input.GetKey(KeyCode.Keypad8))
                rotForce.y = 1;
            else if (Input.GetKey(KeyCode.Keypad2))
                rotForce.y = -1;
            else
                rotForce.y = 0;
            rotForce.Normalize();

            transform.localEulerAngles = new Vector3(0, rotForce.x * 20, rotForce.y * 20);
            drillSimulator.AddTurnForce(rotForce);
            Debug.Log(rotForce.normalized);
        }
    }
}