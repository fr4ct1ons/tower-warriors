using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoolButtonEnabler : MonoBehaviour
{
    [SerializeField] private GlobalBool valueToCheck;
    [SerializeField] private bool fallbackValue;
    [SerializeField] private Button target;

    private void Awake()
    {
        if (!target)
            target = GetComponent<Button>();

        if (!valueToCheck)
        {
            Debug.LogWarning("Value to check was not set. Setting button interactable to Fallback Value and disabling this component.", gameObject);
            target.interactable = fallbackValue;
            enabled = false;
        }
    }

    private void Start()
    {
        target.interactable = valueToCheck.GetValue();
    }
}
