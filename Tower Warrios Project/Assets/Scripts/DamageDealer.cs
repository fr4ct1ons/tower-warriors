using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private UnityEvent onDamage;
    [SerializeField] private bool damageOnEnter, damageOnStay;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageOnEnter)
        {
            if (other.TryGetComponent<Swordsman>(out Swordsman temp))
            {
                temp.Health.Value -= damage;

            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (damageOnStay)
        {
            if (other.TryGetComponent<Swordsman>(out Swordsman temp))
            {
                temp.Health.Value -= damage;
            }
        }
    }
}
