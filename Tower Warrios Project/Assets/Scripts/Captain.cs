using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


public class Captain : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private float movementSmoothing = 0.05f;
    
    [Header("Tower variables")] 
    [SerializeField] private List<Swordsman> swordsmen;
    [SerializeField] private List<Swordsman> toAdd;
    [SerializeField] private float rotationAngle = 3.0f;
    [SerializeField] private float rotationMultiplier = 1.0f;
    [SerializeField] private float firstRotationSmoothing = 0.05f;
    [SerializeField] private float secondRotationSmoothing = 0.05f;
    [SerializeField] private float thirdRotationSmoothing = 0.05f;
    [SerializeField] private float rotationDivisor = 3.0f;
    [SerializeField] private float fallRotation = 20.0f;
    [SerializeField] private float characterHeight = 0.97f;
    [SerializeField] private int fallChance = 85;

    [Header("Others")] 
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private Captain opponent;
    
    [Space]
    [SerializeField] private bool isAI = false;
    
    [SerializeField] private UnityEvent defeatEvent;

    private PlayerInputs inputs;
    private float dir = 0.0f, lastDir = 0.0f;
    private Vector2 tempVelocity = Vector2.zero;
    private float tempAngle = 0.0f;
    private bool isCaptain = false;
    private float rotationValue = 0.0f;
    private float lastRotationValue = 0.0f;
    private bool isPending = false;
    private bool lostMatch = false;
    public bool wonMatch = false;

    // 2 = AI, 1 = Player
    public static int winningCaptain = 0; // TODO: Move to a GameManager or MatchManager.

    public delegate void Command();
    public delegate void CommandSwordsman(Swordsman character);
    public delegate void CommandFloat(float val);

    public event Command OnAttack;
    public event CommandSwordsman OnFall;
    public event CommandFloat OnMove;

    public List<Swordsman> Swordsmen => swordsmen;

    public bool IsAi => isAI;

    public List<Swordsman> ToAdd => toAdd;
    
    public Swordsman GetCaptain
    {
        get
        {
            if (swordsmen.Count > 0)
            {
                return swordsmen[0];
            }

            return null;
        }
        
    }

    private void Awake()
    {
        if (!isAI)
        {
            inputs = new PlayerInputs();
            inputs.Gameplay.DirectionMovement.performed += Move;
            inputs.Gameplay.DirectionMovement.canceled += Move;
            inputs.Gameplay.Attack.performed += Attack;
            inputs.Gameplay.Regroup.performed += TryRegroup;
            inputs.Gameplay.Jump.performed += Jump;
        }

        swordsmen[0].EnableCaptain();
        swordsmen[0].Initialize(this);
        swordsmen[0].Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        for (int i = 1; i < swordsmen.Count; i++)
        {
            swordsmen[i].DisableCaptain();
            swordsmen[i].Initialize(this);
        }
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        Jump();
    }
    
    public void Jump()
    {
        if (swordsmen[0].IsGrounded)
        {
            swordsmen[0].Rigidbody.AddForce(swordsmen[0].JumpForceVector);
            StartCoroutine(JumpCoroutine());
        }
    }

    private IEnumerator JumpCoroutine()
    {
        for (int i = 0; i < swordsmen.Count; i++) //TODO: Convert to event.
        {
            swordsmen[i].Anim.SetTrigger("Jump");
        }

        while (true)
        {
            swordsmen[0].Anim.SetFloat("YVelocity", swordsmen[0].Rigidbody.velocity.y);
            if (swordsmen[0].Rigidbody.velocity.y < 0)
            {
                for (int i = 1; i < swordsmen.Count; i++) //TODO: Convert to event.
                {
                    swordsmen[i].Anim.SetTrigger("ForceIdle");
                }
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void TryRegroup(InputAction.CallbackContext obj)
    {
        for (int i = 0; i < toAdd.Count; i++)
        {
            toAdd[i].Regroup(this);
        }
    }

    public void Attack(InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke();
    }
    
    private void Move(InputAction.CallbackContext obj)
    {
        lastDir = dir;
        dir = obj.ReadValue<float>();

        if(dir != 0)
            swordsmen[0].Anim.SetBool("Running", true);
        else
            swordsmen[0].Anim.SetBool("Running", false);
        
        OnMove?.Invoke(dir);
    }
    
    public void Move(float newDir)
    {
        lastDir = dir;
        dir = newDir;

        if(dir != 0)
            swordsmen[0].Anim.SetBool("Running", true);
        else
            swordsmen[0].Anim.SetBool("Running", false);
        
        OnMove?.Invoke(dir);
    }
    
    private void FixedUpdate()
    {
        Vector3 targetVelocity = Vector3.zero;
        
        if (Mathf.Abs(dir) > 0.05f)
        {
            targetVelocity = new Vector2(dir * movementSpeed, swordsmen[0].Rigidbody.velocity.y);
            swordsmen[0].Rigidbody.velocity =
                Vector2.SmoothDamp(swordsmen[0].Rigidbody.velocity, targetVelocity, ref tempVelocity, movementSmoothing);
                
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

        float currentRotValue;
        for (int i = 1; i < swordsmen.Count; i++)
        {
            currentRotValue = rotationValue * Mathf.Pow(rotationMultiplier, i);
            swordsmen[i].RotateTower(currentRotValue);
            
            float tempAngles = swordsmen[i].transform.localEulerAngles.z % 360.0f;
        
            if (tempAngles > 180)
                tempAngles = tempAngles - 360;

            if (Mathf.Abs(tempAngles) > fallRotation)
            {
                int random = Random.Range(1, 101);

                if (random > fallChance)
                {
                    ProcessFalloff(i);
                    break;
                }
            }
        }
        
        //if (aboveCharacter)
            //aboveCharacter.RotateTower(rotationValue);
        //rigidbody.AddForce(Vector2.right * (dir), ForceMode2D.Impulse);
    }

    public void ProcessFalloff(int i)
    {
        for (int j = swordsmen.Count - 1; j >= i; j--)
        {
            swordsmen[j].FallOff();
            OnFall?.Invoke(swordsmen[j]);
            swordsmen.RemoveAt(j);
        }
    }

    public void OnCaptainDeath()
    {
        if(swordsmen.Count >= 2)
            swordsmen[1].transform.SetParent(transform, true);
        
        swordsmen[0].transform.SetParent(null, true);
        swordsmen[0].FallOff();
        swordsmen.RemoveAt(0);
        
        if (swordsmen.Count > 0)
        {
            swordsmen[0].EnableCaptain();
            if (!isAI)
            {
                camera.Follow = swordsmen[0].CamView;
                camera.LookAt = swordsmen[0].CamView;
            }
        }
        else
        {
            OnDefeat();
        }
    }

    private void OnDefeat()
    {
        if(winningCaptain == 0)
        {
            winningCaptain = isAI ? 2 : 1;
            lostMatch = true;
            opponent.wonMatch = true;
            defeatEvent.Invoke();
        }
    }

    public void OnVictory()
    {

        if(!isAI)
            inputs.Disable();
        
        for (int i = 0; i < swordsmen.Count; i++)
        {
            swordsmen[i].Victory();
        }
    }

    private void OnEnable()
    {
        if(!isAI)
            inputs.Gameplay.Enable();
    }

    private void OnDisable()
    {
        if(!isAI)
            inputs.Gameplay.Disable();
    }

    public void Regroup(Swordsman newSwordsman)
    {
        swordsmen.Insert(1, newSwordsman);
        swordsmen[1].transform.SetParent(swordsmen[0].transform, false);
        swordsmen[1].transform.localPosition = new Vector3(0.0f, characterHeight, 0.0f);
        swordsmen[2].transform.SetParent(swordsmen[1].transform, false);
    }
}