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

    [Header("Tower variables")]
    [SerializeField] private Swordsman captain;
    [SerializeField] private Captain _captain;
    [SerializeField] private GameObject regroupCollider;

    private PlayerInputs inputs;
    private float dir = 0.0f, lastDir = 0.0f;
    private Vector2 tempVelocity = Vector2.zero;
    private float tempAngle = 0.0f;
    private bool isCaptain = false;
    private float rotationValue = 0.0f;
    private float lastRotationValue = 0.0f;
    private bool isPending = false, isOnTower = true, canRegroup = false;

    public Rigidbody2D Rigidbody
    {
        get => rigidbody;
        set => rigidbody = value;
    }

    public Animator Anim
    {
        get => anim;
        set => anim = value;
    }

    public SpriteRenderer Renderer
    {
        get => renderer;
        set => renderer = value;
    }

    public Swordsman Captain
    {
        get => captain;
        set => captain = value;
    }

    public bool CanRegroup
    {
        get => canRegroup;
        set => canRegroup = value;
    }

    private void Awake()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody2D>();
        if (!anim)
            anim = GetComponent<Animator>();
        if (!renderer)
            renderer = GetComponent<SpriteRenderer>();
    }

    public void Attack()
    {
        if(isOnTower)
            anim.SetTrigger("Attack");
    }

    public void EnableCaptain()
    {
        isCaptain = true;
    }

    public void DisableCaptain()
    {
        isCaptain = false;
    }

    private void Move(float dir)
    {
        if (isOnTower)
        {
            if (dir > 0)
                renderer.flipX = false;
            else if (dir < 0)
                renderer.flipX = true;
        }
    }

    public void RotateTower(float angles)
    {
        transform.localRotation = Quaternion.Euler(0, 0, angles);
    }

    public void FallOff()
    {
        rigidbody.simulated = true;
        transform.SetParent(null, true);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        isOnTower = false;
        regroupCollider.SetActive(true);
    }

    public void Initialize(Captain newCaptain)
    {
        _captain = newCaptain;
        _captain.OnMove += Move;
        _captain.OnAttack += Attack;
        isOnTower = true;
        regroupCollider.SetActive(false);
    }

    public void Regroup(Captain newTarget)
    {
        newTarget.Regroup(this);
        rigidbody.simulated = false;
        isOnTower = true;
    }

    public void TryEnableRegroup(Collider2D other)
    {
        var temp = other.GetComponentInParent<Captain>();
        if (temp)
        {
            canRegroup = true;
        }
    }
    
    public void TryDisableRegroup(Collider2D other)
    {
        var temp = other.GetComponentInParent<Captain>();
        if (temp)
        {
            canRegroup = false;
        }
    }
}
