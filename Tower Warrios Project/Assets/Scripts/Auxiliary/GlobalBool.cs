using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Global Bool", menuName = "Global Variables/Global Bool")]
public class GlobalBool : ScriptableObject
{
    public bool value;
    public List<bool> values;

    public void SetValue(bool newVal)
    {
        value = newVal;
    }
    
    public void SetTrueByIndex(int index)
    {
        values[index] = true;
    }
    
    public void SetFalseByIndex(int index)
    {
        values[index] = false;
    }

    public bool GetValue()
    {
        return value;
    }
}
