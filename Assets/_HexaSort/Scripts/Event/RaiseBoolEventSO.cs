using System;
using UnityEngine;

public class RaiseBoolEventSO : ScriptableObject
{
    public Action<bool> OnEventRaised;

    public void RaiseEvent(bool isTrigger)
    {
        OnEventRaised?.Invoke(isTrigger);
    }
}