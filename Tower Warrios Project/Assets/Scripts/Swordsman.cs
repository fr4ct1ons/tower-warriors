using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class Swordsman : MonoBehaviour
{
    [Header("Components")]    
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private Animator anim;
    [SerializeField] private new SpriteRenderer renderer;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private float movementSmoothing = 0.05f;
    
    [Header("Tower variables")]
    [SerializeField] private Swordsman aboveCharacter;
    [SerializeField] private Swordsman belowCharacter;
    [SerializeField] private Swordsman captain;
    [SerializeField] private float rotationAngle = 3.0f;
    [SerializeField] private float rotationMultiplier = 1.0f;
    [FormerlySerializedAs("rotationSmoothing")] [SerializeField] private float firstRotationSmoothing = 0.05f;
    [SerializeField] private float secondRotationSmoothing = 0.05f;
    [SerializeField] private float thirdRotationSmoothing = 0.05f;
    [SerializeField] private float rotationDivisor = 3.0f;
    [SerializeField] private float fallRotation = 20.0f;

    private PlayerInputs inputs;
    private float dir = 0.0f, lastDir = 0.0f;
    private Vector2 tempVelocity = Vector2.zero;
    private float tempAngle = 0.0f;
    private bool isCaptain = false;
    private float rotationValue = 0.0f;
    private float lastRotationValue = 0.0f;
    private bool isPending = false;

    public Swordsman BelowCharacter
    {
        get => belowCharacter;
        set => belowCharacter = value;
    }

    public Swordsman AboveCharacter
    {
        get => aboveCharacter;
        set => aboveCharacter = value;
    }

    public Swordsman Captain
    {
        get => captain;
        set => captain = value;
    }

    private void Awake()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody2D>();
        if (!anim)
            anim = GetComponent<Animator>();
        if (!renderer)
            renderer = GetComponent<SpriteRenderer>();
        
        inputs = new PlayerInputs();
        inputs.Gameplay.DirectionMovement.performed += Move;
        inputs.Gameplay.DirectionMovement.canceled += Move;
        inputs.Gameplay.Attack.performed += Attack;
        
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

    private void Attack(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Attack");
    }

    private void EnableCaptain()
    {
        inputs.Gameplay.Enable(); //NOTE: Does not appear to be fully working. Must rely on checks of the isCaptain variable.
        isCaptain = true;
    }

    private void DisableCaptain()
    {
        inputs.Gameplay.Disable(); //NOTE: Does not appear to be fully working. Must rely on checks of the isCaptain variable.
        isCaptain = false;
    }

    private void Move(InputAction.CallbackContext obj)
    {
        lastDir = dir;
        dir = obj.ReadValue<float>();

        if(isCaptain)
            anim.SetBool("Running", true);
        
        if (dir > 0)
            renderer.flipX = false;
        else if (dir < 0)
            renderer.flipX = true;
        else
        {
            if(isCaptain)
                anim.SetBool("Running", false);
        }

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
                
                lastRotationValue = rotationValue;
                rotationValue = Mathf.SmoothDampAngle(rotationValue, rotationAngle * dir, ref tempAngle, firstRotationSmoothing);
                isPending = true;
            }
            else
            {
                if (isPending)
                {
                    rotationValue = Mathf.SmoothDampAngle(rotationValue, (lastRotationValue * (-1)), ref tempAngle,
                        secondRotationSmoothing);
                    if (lastRotationValue * (-1) > 0.0f)
                    {
                        if (rotationValue > (lastRotationValue * (-1))/rotationDivisor)
                            isPending = false;
                    }
                    else if (lastRotationValue * (-1) < 0.0f)
                    {
                        if (rotationValue < (lastRotationValue * (-1))/rotationDivisor)
                            isPending = false;
                    }
                }
                else
                {
                    isPending = false;
                    rotationValue = Mathf.SmoothDampAngle(rotationValue, 0.0f, ref tempAngle, thirdRotationSmoothing);
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

        float tempAngles = transform.localEulerAngles.z % 360.0f;
        
        if (tempAngles > 180)
            tempAngles = tempAngles - 360;

        if (Mathf.Abs(tempAngles) > fallRotation/* || Mathf.Abs(transform.localEulerAngles.z) < 360.0f - fallRotation*/)
        {
            FallOff();
        }
    }

    public void FallOff()
    {
        rigidbody.simulated = true;
        transform.SetParent(null, true);
        belowCharacter.AboveCharacter = null;
        belowCharacter = null;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        if(aboveCharacter)
            aboveCharacter.FallOff(); //TODO: Stop using recurrence.
    }

    private void OnEnable()
    {
        inputs.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputs.Gameplay.Disable();
    }
}
