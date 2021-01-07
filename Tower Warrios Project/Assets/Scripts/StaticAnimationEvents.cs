using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StaticAnimationEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent deathEvent;
    [SerializeField] private UnityEvent deathBeginEvent;
    [SerializeField] private UnityEvent attackEvent;

    public void PlayDeathEvent()
    {
        deathEvent.Invoke();
    }
    
    public void PlayDeathBeginEvent()
    {
        deathBeginEvent.Invoke();
    }
    
    public void PlayAttackEvent()
    { 
        attackEvent.Invoke();
    }
}
