using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CODE.Stats
{
    

    public class StatsManager : NetworkBehaviour
    {
        private float hpRate;
        public NetworkVariable<float> hp = new NetworkVariable<float>(100
            , NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        private float staminaRate;
        public NetworkVariable<float> stamina = new NetworkVariable<float>(100
            , NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        
        private float hungerRate;
        public NetworkVariable<float> hunger = new NetworkVariable<float>(0
            , NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        
        private float temperatureRate;
        public NetworkVariable<float> temperature = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        
        private float shitRate;
        public NetworkVariable<float> shit = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        
        private float drunkRate;
        public NetworkVariable<float> drunk = new NetworkVariable<float>(100
            , NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        
        #region Getters
        
        public float GetHp()
        {
           
            return  hp.Value;
        }
        
        public float GetStamina()
        {
            return  stamina.Value;
        }
        
        public float GetHunger()
        {
            return hunger.Value;
        }
        
        public float GetTemperature()
        {
            return temperature.Value;
        }
        
        public float GetShit()
        {
            return shit.Value;
        }
        
        public float GetDrunk()
        {
            return drunk.Value;
        }
        #endregion
        
        // public override void OnNetworkSpawn()
        // {
        //     base.OnNetworkSpawn();
        //     SpawnRpc();
        // }
        //
        private void Start()
        {
            Spawn();
        }
        
        // [Rpc(SendTo.Server)]
        void Spawn()
        {
            if (!IsOwner)
                return;
            hpRate = 0;
            hp.Value = Random.Range(60, 100);
            staminaRate = 15;
            stamina.Value = 100;
            hungerRate = 1f;
            hunger.Value = Random.Range(60, 100);
            temperatureRate = 5f;
            temperature.Value = 0;
            shitRate = 1f;
            shit.Value = Random.Range(60, 100);
            drunkRate = -1;
            drunk.Value = Random.Range(60, 100);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Spawn();
            }
        }
        private void LateUpdate()
        {
            
            Tick(hp, hpRate);
            Tick(stamina, staminaRate, 100 - Mathf.Clamp(temperature.Value - 50, 0, 150));
            Tick(hunger, hungerRate);
            Tick(temperature, temperatureRate);
            Tick(shit, shitRate);
            Tick(drunk, drunkRate);
        }

        void Tick(NetworkVariable<float> stat, float speedRate, float max = 100)
        {
            if (!IsOwner)
                return;
            stat.Value = Mathf.Clamp(stat.Value + Time.deltaTime * speedRate, 0, max);
        }
        
        void UseStamina()
        {
        }
    }
}