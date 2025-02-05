using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _CODE.Stats
{
    public class UIDebugStats : MonoBehaviour
    {
        [SerializeField] private StatsManager statsManager;
        [SerializeField] private Slider hp;
        [SerializeField] private Slider stamina;
        [SerializeField] private Slider hunger;
        [SerializeField] private Slider temperature;
        [SerializeField] private Slider shit;
        [SerializeField] private Slider drunk;

        // public override void OnNetworkSpawn()
        // {
        //     base.OnNetworkSpawn();
        //     hp.value = statsManager.GetHp();
        //     stamina.value = statsManager.GetStamina();
        //     hunger.value = statsManager.GetHunger();
        //     temperature.value = statsManager.GetTemperature();
        //     shit.value = statsManager.GetShit();
        //     drunk.value = statsManager.GetDrunk();
        // }

        private void Update()
        {
            if (!statsManager)
                return;
            hp.value = statsManager.GetHp();
            stamina.value = statsManager.GetStamina();
            hunger.value = statsManager.GetHunger();
            temperature.value = statsManager.GetTemperature();
            shit.value = statsManager.GetShit();
            drunk.value = statsManager.GetDrunk();
        }
    }
}