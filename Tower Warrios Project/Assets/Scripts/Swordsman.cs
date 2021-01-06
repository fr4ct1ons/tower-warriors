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
    [SerializeField] private Captain captain;
    [SerializeField] private GameObject regroupCollider;

    [Header("Stats")]
    [SerializeField] private int damage;
    [SerializeField] private Stats health;

    [Header("Others")]
    [SerializeField] private ColliderEvents attackCollider;
    [SerializeField] private float attackEnableTime = 0.25f;
    [SerializeField] private GameObject regroupInfo;
    [SerializeField] private Vector2 jumpForceVector;

    private PlayerInputs inputs;
    private float dir = 0.0f, lastDir = 0.0f;
    private Vector2 tempVelocity = Vector2.zero;
    private float tempAngle = 0.0f;
    private bool isCaptain = false;
    private float rotationValue = 0.0f;
    private float lastRotationValue = 0.0f;
    private bool isPending = false, isOnTower = true, canRegroup = false;
    private bool isGrounded = false;

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

    public Captain Captain
    {
        get => captain;
        set => captain = value;
    }

    public bool CanRegroup
    {
        get => canRegroup;
        set => canRegroup = value;
    }

    public Stats Health => health;

    public bool IsGrounded => isGrounded;

    public Vector2 JumpForceVector => jumpForceVector;
    
    private void Awake()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody2D>();
        if (!anim)
            anim = GetComponent<Animator>();
        if (!renderer)
            renderer = GetComponent<SpriteRenderer>();
    }

    public void EnableGrounded(Collider2D target)
    {
        anim.SetBool("IsGrounded", true);
        isGrounded = true;
    }

    public void DisableGrounded(Collider2D target)
    {
        anim.SetBool("IsGrounded", false);
        isGrounded = false;
    }

    public void OnAttackColliderDetect(Collider2D target)
    {
        Debug.Log($"Detected {target.gameObject}", target.gameObject);
        if (target.TryGetComponent<Swordsman>(out Swordsman temp))
        {
            Debug.Log("Detected enemy.");
            temp.health.Value -= damage;
        }
    }

    public void PlayHitAnimation()
    {
        if(health.Value > health.MinValue)
            anim.SetTrigger("TakeDamage");
    }

    public void Die()
    {
        anim.SetTrigger("Death");
        Debug.Log("Death!!!");
    }

    public void OnDeath()
    {
        int index = captain.Swordsmen.FindIndex(swordsman => swordsman.GetHashCode() == this.GetHashCode());
        if (index == -1)
        {
            Debug.LogError($"The swordsman {gameObject} is not on the captain {captain.gameObject} list.", gameObject);
            Destroy(gameObject);
            return;
        }
        else if (index == 0)
        {
            captain.OnCaptainDeath();
            Destroy(gameObject);
            return;
        }
        captain.ProcessFalloff(index);
        Destroy(gameObject);
    }

    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        if (isOnTower)
        {
            anim.SetTrigger("Attack");
            attackCollider.gameObject.SetActive(true);
            yield return new WaitForSeconds(attackEnableTime);
            attackCollider.gameObject.SetActive(false);
        }
    }

    public void EnableCaptain()
    {
        isCaptain = true;
        anim.SetFloat("IsCaptain", 1.0f);
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void DisableCaptain()
    {
        isCaptain = false;
        anim.SetFloat("IsCaptain", 0.0f);
        rigidbody.bodyType = RigidbodyType2D.Kinematic;

    }

    private void Move(float dir)
    {
        if (isOnTower)
        {
            if (dir > 0)
            {
                renderer.flipX = false;
                attackCollider.transform.localPosition = new Vector3(Mathf.Abs(attackCollider.transform.localPosition.x), 
                    attackCollider.transform.localPosition.y, 
                    attackCollider.transform.localPosition.z);
            }
            else if (dir < 0)
            {
                renderer.flipX = true;
                attackCollider.transform.localPosition = new Vector3(-Mathf.Abs(attackCollider.transform.localPosition.x), 
                    attackCollider.transform.localPosition.y, 
                    attackCollider.transform.localPosition.z);
            }
        }
    }

    public void RotateTower(float angles)
    {
        transform.localRotation = Quaternion.Euler(0, 0, angles);
    }

    public void FallOff()
    {
        rigidbody.bodyType  = RigidbodyType2D.Dynamic;
        transform.SetParent(null, true);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        isOnTower = false;
        regroupCollider.SetActive(true);
        anim.SetTrigger("Falloff");
    }

    public void Initialize(Captain newCaptain)
    {
        captain = newCaptain;
        captain.OnMove += Move;
        captain.OnAttack += Attack;
        isOnTower = true;
        regroupCollider.SetActive(false);
    }

    public void Regroup(Captain newTarget)
    {
        newTarget.Regroup(this);
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        rigidbody.velocity = Vector2.zero;
        isOnTower = true;
        regroupCollider.SetActive(false);
        anim.SetTrigger("Regroup");
    }

    public void TryEnableRegroup(Collider2D other)
    {
        var temp = other.GetComponentInParent<Captain>();
        if (temp)
        {
            canRegroup = true;
            if (!temp.IsAi)
            {
                regroupInfo.SetActive(true);
                temp.ToAdd.Add(this);
            }
        }
    }
    
    public void TryDisableRegroup(Collider2D other)
    {
        var temp = other.GetComponentInParent<Captain>();
        if (temp)
        {
            canRegroup = false;
            if (!temp.IsAi)
            {
                regroupInfo.SetActive(false);
                temp.ToAdd.Remove(this);
            }
        }
    }

    public void Victory()
    {
        anim.SetTrigger("Victory");
    }
}
