using System;
using DG.Tweening;
using NUnit.Framework.Constraints;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class InteractionBtn : NetworkBehaviour
{
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    [SerializeField] private BtnType btnType;
    [SerializeField] private float distance;
    [SerializeField] private float time;
    
    private NetworkVariable<bool> pushed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    private Sequence seq;
    private NetworkVariable<float> defaultPosY = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (pushed.Value && btnType == BtnType.Touch)
            OnActivate?.Invoke();
    }

    private void Start()
    {
        defaultPosY.Value = transform.localPosition.x;
    }

    enum BtnType
    {
        Click,
        Touch
    };

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void TouchRpc()
    { 
        if (seq != null)
            return;
        switch (btnType)
        {
            case BtnType.Click:
                Click();
                break;
            case BtnType.Touch:
                if (!pushed.Value)
                    PushOn();
                else
                    PushOff();
                break;
        }
    }

    void Click()
    {
        seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(new Vector3(defaultPosY.Value + distance,0,0), time));
        seq.Append(transform.DOLocalMove(new Vector3(defaultPosY.Value,0,0), time));
        seq.OnComplete(() =>
        {
            seq = null;
            OnActivate?.Invoke();
        });
    }
    
    public void PushOn()
    {
        seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(new Vector3(defaultPosY.Value + distance,0,0), time));
        seq.OnComplete(() =>
        {
            pushed.Value = true;
            seq = null;
            OnActivate?.Invoke();
        });
    }

    public void PushOff()
    {
        seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(new Vector3(defaultPosY.Value,0,0), time));
        seq.OnComplete(() =>
        {
            pushed.Value = false;
            seq = null;
            OnDeactivate?.Invoke();
        });
    }
}