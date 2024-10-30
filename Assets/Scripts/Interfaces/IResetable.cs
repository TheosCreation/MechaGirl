using System;
using UnityEngine;

public abstract class IResetable : MonoBehaviour
{
    public abstract void Reset();

    protected virtual void OnEnable()
    {
        LevelManager.Instance.RegisterObject(this);
    }

    protected virtual void OnDisable()
    {
        LevelManager.Instance.Deregister(this);
    }
}