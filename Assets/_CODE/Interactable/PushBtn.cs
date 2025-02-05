using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace _CODE.Interactable
{
    public class PushBtn : NetworkBehaviour
    {
        public UnityEvent OnActivate;
        public UnityEvent OnDeactivate;

        [SerializeField] private float distance;
        [SerializeField] private float time;

        private NetworkVariable<bool> pushed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private Sequence seq;

        private NetworkVariable<float> defaultPosY = new NetworkVariable<float>(0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (pushed.Value)
                OnActivate?.Invoke();
            pushed.OnValueChanged += TouchClient;
        }

        public override void OnNetworkDespawn()
        {
            pushed.OnValueChanged -= TouchClient;
        }

        private void Start()
        {
            defaultPosY.Value = transform.localPosition.x;
        }

        [Rpc(SendTo.Server)]
        public void TouchServerRpc()
        {
            if (!IsOwner)
                return;
            pushed.Value = !pushed.Value;
            Debug.Log("pushed: " +pushed.Value);
            
        }
        
        public void TouchClient(bool value, bool newValue)
        {
            // if (!IsOwner)
            //     return;
            if (seq != null)
                return;
            if (pushed.Value)
                PushOn();
            else
                PushOff();
        }

        public void PushOn()
        {
            seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMove(new Vector3(0, defaultPosY.Value + distance, 0), time));
            seq.OnComplete(() =>
            {
                // pushed.Value = true;
                seq = null;
                OnActivate?.Invoke();
            });
        }

        public void PushOff()
        {
            seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMove(new Vector3(0, defaultPosY.Value, 0), time));
            seq.OnComplete(() =>
            {
                // pushed.Value = false;
                seq = null;
                OnDeactivate?.Invoke();
            });
        }
    }
}