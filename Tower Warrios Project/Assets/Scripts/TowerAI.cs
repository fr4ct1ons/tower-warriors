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
    [SerializeField] private float timeForRegroup = 1.0f;
    [SerializeField] private int minFallenSwordsmen = 3;
    [SerializeField] private List<Swordsman> fallenSwordsmen;

    private bool regroupSwordsmen = false;


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
        }
        else if (Vector3.Distance(target.GetCaptain.transform.position, opponent.GetCaptain.transform.position) < minAttackDistance)
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
