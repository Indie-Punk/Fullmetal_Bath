﻿using System;
using Unity.Netcode;
using UnityEngine;

namespace _CODE.Interactable
{
    public class SittingPlace : NetworkBehaviour
    {
        [SerializeField] private Transform sitPos;
        [SerializeField] private Transform standUpPos;
        [SerializeField] private SittingPlace leftPlace;
        [SerializeField] private SittingPlace rightPlace;
        [SerializeField] private Vector3 startAngle;
        [SerializeField] private Vector4 angleLock;

        public Vector3 SitPos => sitPos.position;
        public Vector3 StandUpPos => standUpPos.position;
        public bool IsBusy => _isBusy.Value;
        public Vector4 AngleLock => angleLock;
        public Vector3 StartAngle => startAngle;

        private NetworkVariable<bool> _isBusy = new NetworkVariable<bool>();

        [Rpc(SendTo.Server)]
        public void SeatControlRpc(bool value)
        {
            // if (!IsOwner)
            //     return;
            // if (_isBusy.Value)
            //     return;
            _isBusy.Value = value;
        }

        SittingPlace GetLeftPlace()
        {
            if (leftPlace != null && !leftPlace.IsBusy)
                return leftPlace;
            return null;
        }

        SittingPlace GetRightPlace()
        {
            if (rightPlace != null && !rightPlace.IsBusy)
                return rightPlace;
            return null;
        }

    }
}