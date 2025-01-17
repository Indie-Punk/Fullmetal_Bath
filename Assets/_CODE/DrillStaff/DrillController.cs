using System;
using System.Collections.Generic;
using DG.Tweening;
using MilkShake;
using MilkShake.Demo;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CODE
{
    public class DrillController : MonoBehaviour
    {
        [SerializeField] private DrillSimulator drillSimulator;
        [SerializeField] private ShakePreset shakePreset;
        

        [SerializeField] private AudioSource drillSource;
        [SerializeField] private List<ShakeInstance> shakeInstances;
        // private ShakeInstance shakeInstance;
        private bool active;
        
        public void Activate()
        {
            if (!active)
            {
                Shaker.ShakeAllSeparate(shakePreset, shakeInstances);
                
                DOTween.To(() => drillSource.volume, x => drillSource.volume = x, .5f, 2f);
                active = true;
                drillSimulator.SetCurrentSpeed(1);
            }
            else
            {
                foreach (var shakeInstance in shakeInstances)
                {
                    
                    shakeInstance.Stop(shakePreset.FadeOut, true);
                }
                drillSimulator.SetCurrentSpeed(0);
                active = false;
                DOTween.To(() => drillSource.volume, x => drillSource.volume = x, 0, 2f);
            }
        }
    }
}