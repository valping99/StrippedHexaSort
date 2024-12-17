using System;
using UnityEngine;

public class RaiseEventSO : ScriptableObject
{
    public Action OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
