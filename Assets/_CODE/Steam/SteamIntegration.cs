using UnityEngine;

namespace _CODE.Steam
{
    public class SteamIntegration : MonoBehaviour
    {
        public static SteamIntegration Instance;

        public bool IsSteamOn;

        [SerializeField] private uint appID = 480;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            try
            {
                Steamworks.SteamClient.Init(appID);
                Debug.Log("Steam Initialized");
                PrintYourName();
                IsSteamOn = true;
                // SteamAchievements.Instance.InitializeAchievementProgress();
                // SteamAchievements.Instance.CheckForAllAchievements();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        private void Update()
        {
            Steamworks.SteamClient.RunCallbacks();
        }

        private void OnApplicationQuit()
        {
            Steamworks.SteamClient.Shutdown();
        }

        private void PrintYourName()
        {
            Debug.Log(Steamworks.SteamClient.Name);
        }
    }
}