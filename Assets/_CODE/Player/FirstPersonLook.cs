using System;
using _CODE.Player;
using Cinemachine;
using ECM2.Examples.FirstPerson;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace _CODE
{
    public class FirstPersonLook : NetworkBehaviour
    {
        
        [Space(15.0f)]
        public bool invertLook = true;
        [Tooltip("Mouse look sensitivity")]
        public Vector2 mouseSensitivity = new Vector2(1.0f, 1.0f);
        
        [Space(15.0f)]
        [Tooltip("How far in degrees can you move the camera down.")]
        public float minPitch = -80.0f;
        [Tooltip("How far in degrees can you move the camera up.")]
        public float maxPitch = 80.0f;
        
        private FirstPersonCharacter _character;
        [SerializeField] private PlayerInput input;
        
        
        
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow.")]
        [SerializeField] private GameObject cameraTarget;

        [Space(15.0f)] [Tooltip("Cinemachine Virtual Camera positioned at desired crouched height.")]
        [SerializeField] private CinemachineVirtualCamera crouchedCamera;
        
        [SerializeField] private CinemachineVirtualCamera runningCamera;
        
        [FormerlySerializedAs("unCrouchedCamera")]
        [Tooltip("Cinemachine Virtual Camera positioned at desired un-crouched height.")]
        [SerializeField] private CinemachineVirtualCamera normalCamera;
        
        [Tooltip("Cinemachine Virtual Camera positioned at desired un-crouched height with fixed aim on look at")]
        [SerializeField] private CinemachineVirtualCamera lookedAtCamera;
        
        [Tooltip("Camera noise amplitude gain multiplier.")]
        [SerializeField] private float cameraNoiseAmplitudeMultiplier = 1f;

        public CinemachineVirtualCamera CurrentCamera => _character.IsCrouched() ? crouchedCamera : _isLookingAt ? lookedAtCamera : normalCamera;
        
        private CinemachineBasicMultiChannelPerlin _normalNoiseProfile;
        private CinemachineBasicMultiChannelPerlin _crouchedNoiseProfile;
        private CinemachineBasicMultiChannelPerlin _runNoiseProfile;
        private float _cameraTargetPitch;
        private bool _isLookingAt;
        
        private void Awake()
        {
            _character = GetComponent<FirstPersonCharacter>();
            if (_character == null)
            {
                Debug.LogError("Character component is missing on PlayerController.");
            }
            
            if (normalCamera != null)
            {
                _normalNoiseProfile = normalCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
            
            if (crouchedCamera != null)
            {
                _crouchedNoiseProfile = crouchedCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
            if (runningCamera != null)
            {
                _runNoiseProfile = runningCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
            
        }
        

        private void OnEnable()
        {
            input.OnRunning += OnRunning;
            input.OnUnRunning += OnUnRunning;
            _character.Crouched += OnCrouched;
            _character.UnCrouched += OnUnCrouched;
        }

        private void OnDisable()
        {
            input.OnRunning -= OnRunning;
            input.OnUnRunning -= OnUnRunning;
            _character.Crouched -= OnCrouched;
            _character.UnCrouched -= OnUnCrouched;
        }
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            Vector2 lookInput = new Vector2
            {
                x = Input.GetAxisRaw("Mouse X"),
                y = Input.GetAxisRaw("Mouse Y")
            };

            lookInput *= mouseSensitivity;

            _character.AddControlYawInput(lookInput.x);
            // _character.AddControlPitchInput(invertLook ? -lookInput.y : lookInput.y, minPitch, maxPitch);
            AddControlPitchInputRpc(lookInput.y);

        }

        [Rpc(SendTo.Everyone)]
        void AddControlPitchInputRpc(float lookInputY)
        {
            // if (OwnerClientId == 1)
            // {
            //     Debug.Log("client " + OwnerClientId + ": " + lookInputY);
            //     Debug.Log("client " + OwnerClientId + " minPitch: " + minPitch);
            //     Debug.Log("client " + OwnerClientId + " maxPitch: " + maxPitch);
            // }
            _character.AddControlPitchInput(invertLook ? -lookInputY : lookInputY, minPitch, maxPitch);
        }
        

        private void LateUpdate()
        {
            UpdateNoiseAmplitude();
        }

        // private void LateUpdate()
        // {
        //     HandleRotation();
        //     UpdateNoiseAmplitude();
        // }
        
        private void OnRunning()
        {
            crouchedCamera.Priority = 10;
            normalCamera.Priority = 10;
            runningCamera.Priority = 11;
        }

        /// <summary>
        /// When character un-crouches, toggle Crouched / UnCrouched cameras.
        /// </summary>
        private void OnUnRunning()
        {
            crouchedCamera.Priority = 10;
            normalCamera.Priority = 11;
            runningCamera.Priority = 10;
        }
        
        private void OnCrouched()
        {
            crouchedCamera.Priority = 11;
            normalCamera.Priority = 10;
            runningCamera.Priority = 10;
        }

        /// <summary>
        /// When character un-crouches, toggle Crouched / UnCrouched cameras.
        /// </summary>
        private void OnUnCrouched()
        {
            crouchedCamera.Priority = 10;
            normalCamera.Priority = 11;
            runningCamera.Priority = 10;
        }
        
        public float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public void ResetNoiseAmplitude()
        {
            if (_normalNoiseProfile == null && _crouchedNoiseProfile == null)
                return;
            
            _crouchedNoiseProfile.m_AmplitudeGain = 0f;
            _normalNoiseProfile.m_AmplitudeGain = 0f;
        }

        private void UpdateNoiseAmplitude()
        {
            if (_normalNoiseProfile == null && _crouchedNoiseProfile == null)
                return;

            float currentSpeed = _character.velocity.magnitude;
            
            if (_character.IsCrouched())
            {
                float speedRatio = currentSpeed / _character.maxWalkSpeedCrouched;
                _crouchedNoiseProfile.m_AmplitudeGain = 
                    EaseInCubic(0, _character.maxWalkSpeedCrouched, speedRatio) *
                    cameraNoiseAmplitudeMultiplier;
            }
            else if (input.IsRunning)
            {
                
                float speedRatio = currentSpeed / _character.maxWalkSpeed;
                _runNoiseProfile.m_AmplitudeGain =
                    EaseInCubic(0, _character.maxWalkSpeedCrouched, speedRatio) *
                    cameraNoiseAmplitudeMultiplier;
            }
            else
            {
                float speedRatio = currentSpeed / _character.maxWalkSpeed;
                _normalNoiseProfile.m_AmplitudeGain =
                    EaseInCubic(0, _character.maxWalkSpeedCrouched, speedRatio) *
                    cameraNoiseAmplitudeMultiplier;
            }

            if (!_character.IsOnGround())
            {
                _crouchedNoiseProfile.m_AmplitudeGain = 0;
                _normalNoiseProfile.m_AmplitudeGain = 0;
            }
        }
    }
}