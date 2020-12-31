using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerAI : MonoBehaviour
{
    [SerializeField] private Captain target;
    [SerializeField] private Captain opponent;
    
    [Header("AI Variables")]
    [SerializeField] private float minAttackDistance = 1.5f;
    
    private void Awake()
    {
        if (!target)
        {
            Debug.LogError("Target not set! Disabling this component.");
            enabled = false;
        }
        if (!opponent)
        {
            Debug.LogError("Opponent not set! Disabling this component.");
            enabled = false;
        }
    }

    void Update()
    {
        
        if (Vector3.Distance(target.GetCaptain.transform.position, opponent.GetCaptain.transform.position) < minAttackDistance)
        {
            target.Attack(new InputAction.CallbackContext());
            target.Move(0.0f);
            Debug.Log("Attacking");
        }
        else if (opponent.transform.position.x < target.transform.position.x) //Opponent at left
        {
            target.Move(-1.0f);
            Debug.Log("Left");
        }
        else if (opponent.transform.position.x < target.transform.position.x) //Opponent at right
        {
            target.Move(1.0f);
            Debug.Log("Right");
        }
    }
}
