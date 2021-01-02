using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StaticAnimationEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent deathEvent;

    public void PlayDeathEvent()
    {
        deathEvent.Invoke();
    }
}
