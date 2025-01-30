using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _CODE
{
    public class FootstepController : MonoBehaviour
    {
        [SerializeField] private AudioSource footstepSource;
        [SerializeField] private List<AudioClip> grassSteps;
        [SerializeField] private List<AudioClip> metalSteps;
        [SerializeField] private AudioMixerGroup normalAudioGroup;
        [SerializeField] private AudioMixerGroup echoAudioGroup;
        
        public void OnFootstepFrame()
        {
            if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit))
            {
                footstepSource.pitch = Random.Range(.75f, 1.25f);
                if (hit.collider.CompareTag("Grass"))
                {
                    footstepSource.PlayOneShot(grassSteps[0],Random.Range(.5f, 1.0f));
                    footstepSource.outputAudioMixerGroup = normalAudioGroup;

                }
                else if (hit.collider.CompareTag("Metal"))
                {
                    footstepSource.PlayOneShot(metalSteps[0],Random.Range(.5f, 1.0f));
                    footstepSource.outputAudioMixerGroup = echoAudioGroup;
                }

            }
        }
    }
}