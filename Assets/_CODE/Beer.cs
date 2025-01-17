using UnityEngine;

namespace _CODE
{
    public class Beer : MonoBehaviour
    {
        [SerializeField] private BurpController burpController;
        public void Use()
        {
            
            burpController.Burp();
            Destroy(gameObject);
        }
    }
}