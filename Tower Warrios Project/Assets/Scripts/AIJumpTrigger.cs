using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIJumpTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Swordsman>(out Swordsman temp))
        {
            if (temp.Captain.IsAi)
            {
                temp.Captain.Jump();
            }
        }
    }
}
