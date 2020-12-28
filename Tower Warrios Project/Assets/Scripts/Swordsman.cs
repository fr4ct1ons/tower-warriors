using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;

public class Swordsman : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private float movementSmoothing = 0.05f;
    
    [Header("Tower variables")]
    [SerializeField] private Swordsman aboveCharacter;
    [SerializeField] private Swordsman belowCharacter;
    [SerializeField] private Swordsman captain;
    [SerializeField] private float rotationAngle = 3.0f;
    [SerializeField] private float rotationMultiplier = 1.0f;
    [SerializeField] private float rotationSmoothing = 0.05f;

    private PlayerInputs inputs;
    private float dir = 0.0f, lastDir = 0.0f;
    private Vector2 tempVelocity = Vector2.zero;
    private float tempAngle = 0.0f;
    private bool isCaptain = false;
    private float rotationValue = 0.0f;
    private bool isPending = false;

    private void Awake()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody2D>();
        
        inputs = new PlayerInputs();
        inputs.Gameplay.DirectionMovement.performed += Move;
        inputs.Gameplay.DirectionMovement.canceled += Move;
        
        if (!belowCharacter)
        {
            captain = this;
            EnableCaptain();
        }
        else
        {
            DisableCaptain();
        }
    }

    private void EnableCaptain()
    {
        inputs.Enable();
        isCaptain = true;
    }

    private void DisableCaptain()
    {
        inputs.Disable();
        isCaptain = false;
    }

    private void Move(InputAction.CallbackContext obj)
    {
        lastDir = dir;
        dir = obj.ReadValue<float>();
    }

    private void Update()
    {
        //transform.position += Vector3.right * (dir * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (isCaptain)
        {
            Vector3 targetVelocity = Vector3.zero;
            //rigidbody.MovePosition(rigidbody.position + Vector2.right * (dir * Time.fixedDeltaTime));
            if (Mathf.Abs(dir) > 0.05f)
            {
                targetVelocity = new Vector2(dir * movementSpeed, rigidbody.velocity.y);
                rigidbody.velocity =
                    Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref tempVelocity, movementSmoothing);
                rotationValue = Mathf.SmoothDampAngle(rotationValue, rotationAngle * dir, ref tempAngle, rotationSmoothing);
                isPending = true;
            }
            else
            {
                if (Mathf.Abs(rotationValue - rotationAngle * (-lastDir)) > Mathf.Abs(rotationAngle * (-lastDir)/3.0f) && isPending)
                {
                    rotationValue = Mathf.SmoothDampAngle(rotationValue, rotationAngle * (-lastDir), ref tempAngle,
                        rotationSmoothing);
                }
                else
                {
                    isPending = false;
                    rotationValue = Mathf.SmoothDampAngle(rotationValue, 0.0f, ref tempAngle, rotationSmoothing * 1.7f);
                }

            }

            if (aboveCharacter)
                aboveCharacter.RotateTower(rotationValue);
            //rigidbody.AddForce(Vector2.right * (dir), ForceMode2D.Impulse);
        }
    }

    public void RotateTower(float angles)
    {
        transform.localRotation = Quaternion.Euler(0, 0, angles);
        if(aboveCharacter)
            aboveCharacter.RotateTower(angles * rotationMultiplier);
    }
    
    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
}
