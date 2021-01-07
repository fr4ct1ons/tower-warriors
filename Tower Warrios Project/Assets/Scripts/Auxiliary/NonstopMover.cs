using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonstopMover : MonoBehaviour
{
    [Header("Locally moves an object in the specified speed.")]
    [SerializeField] private Vector3 unitsPerSecond;

    private void Update()
    {
        transform.localPosition += unitsPerSecond * Time.deltaTime;
    }
}
