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
    [SerializeField] private float minAttackDistance = 1.2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float timeForRegroup = 1.0f;
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private int minFallenSwordsmen = 3;
    [SerializeField] private List<Swordsman> fallenSwordsmen;
    [SerializeField] private int currentState;
    
    [Space]
    
    [SerializeField] private bool regroupSwordsmen = false;
    [SerializeField] private float currentCooldown = 0.0f;


    private void Awake()
    {
        if (!target)
        {
            Debug.LogError("Target not set! Disabling this component.");
            enabled = false;
            return;
        }
        if (!opponent)
        {
            Debug.LogError("Opponent not set! Disabling this component.");
            enabled = false;
            return;
        }
        
        target.OnFall += OnCharacterFall;
        currentCooldown = attackCooldown;
    }

    private void OnCharacterFall(Swordsman fallen)
    {
        fallenSwordsmen.Add(fallen);
        if (fallenSwordsmen.Count >= minFallenSwordsmen)
        {
            StartCoroutine(WaitBeforeRegrouping());
        }
    }

    private IEnumerator WaitBeforeRegrouping()
    {
        yield return new WaitForSeconds(timeForRegroup);
        regroupSwordsmen = true;
    }

    void Update()
    {
        currentCooldown += Time.deltaTime;
        
        if (regroupSwordsmen)
        {
            target.Move(fallenSwordsmen[0].transform.position.x > target.GetCaptain.transform.position.x ? 1.0f : -1.0f);
            Debug.Log("Moving towards swordsmen.");
            if (fallenSwordsmen[0].CanRegroup)
            {
                fallenSwordsmen[0].Regroup(target);
                fallenSwordsmen.RemoveAt(0);
                if (fallenSwordsmen.Count <= 0)
                {
                    regroupSwordsmen = false;
                }
            }

            currentState = 1;
        }
        else if (Vector3.Distance(target.GetCaptain.transform.position, opponent.GetCaptain.transform.position) < minAttackDistance)
        {
            if (currentCooldown > attackCooldown)
            {
                target.Attack(new InputAction.CallbackContext());
                target.Move(0.0f);
                Debug.Log("Attacking");
                currentCooldown = 0.0f;
                currentState = 2;
            }
        }
        else if (opponent.GetCaptain.transform.position.x < target.GetCaptain.transform.position.x) //Opponent at left
        {
            target.Move(-movementSpeed);
            Debug.Log("Left");
            currentState = 3;
        }
        else if (opponent.GetCaptain.transform.position.x > target.GetCaptain.transform.position.x) //Opponent at right
        {
            target.Move(movementSpeed);
            Debug.Log("Right");
            currentState = 4;
        }
        else
        {
            Debug.Log("None is true??????");
        }
    }
}
