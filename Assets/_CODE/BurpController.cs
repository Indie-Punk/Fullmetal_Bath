using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace _CODE
{
    public class BurpController : MonoBehaviour
    {
        [SerializeField] Transform burpPoint;
        [SerializeField] GameObject burpPrefab;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip burpSfx;

        public void Burp()
        {
            Instantiate(burpPrefab, burpPoint.position, quaternion.identity).GetComponent<Rigidbody>()
                .AddForce(transform.forward * .2f, ForceMode.Impulse);
            audioSource.PlayOneShot(burpSfx);
        }
    }
}