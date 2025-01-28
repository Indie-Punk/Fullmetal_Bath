using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _CODE._NETCODE
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button serverBtn;
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button clientBtn;
        [SerializeField] private GameObject camera;

        private void Awake()
        {
            serverBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
                camera.gameObject.SetActive(false);
            });
            
            hostBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
                camera.gameObject.SetActive(false);
            });
            clientBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
                camera.gameObject.SetActive(false);
            });
        }
    }
}