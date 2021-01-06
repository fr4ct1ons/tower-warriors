using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Global Bool", menuName = "Global Variables/Global Bool")]
public class GlobalBool : ScriptableObject
{
    public bool value;

    public void SetValue(bool newVal)
    {
        value = newVal;
    }

    public bool GetValue()
    {
        return value;
    }
}
