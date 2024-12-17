using System;
using UnityEngine;

public class RaiseClassEventSO<T> : ScriptableObject
{
    public Action<T> OnEventRaised;

    public void RaiseEvent(T value)
    {
        OnEventRaised?.Invoke(value);
    }
}
