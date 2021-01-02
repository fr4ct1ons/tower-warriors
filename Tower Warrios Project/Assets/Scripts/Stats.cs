using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StatEvent : UnityEvent<int> {}

[Serializable]
public class Stats
{
    [SerializeField] private int minValue = 0, maxValue = 100;
    [SerializeField] private int value = 100;
    [SerializeField] private StatEvent onUpdateValue;
    [SerializeField] private UnityEvent  onHitMinimum, onHitMaximum;

    public int MinValue => minValue;
    
    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            if (this.value <= minValue)
            {
                this.value = minValue;
                onHitMinimum.Invoke();
            }
            else if (this.value >= maxValue)
            {
                this.value = maxValue;
                onHitMaximum.Invoke();
            }
            
            onUpdateValue.Invoke(value);
        }
    }
}
