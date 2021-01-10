using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIJumpTrigger : MonoBehaviour
{
    public enum Direction
    {
        None, Left, Right, Both
    }

    [SerializeField] private Direction allowedDirection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(allowedDirection == Direction.None)
            return;
        
        
        if (other.TryGetComponent<Swordsman>(out Swordsman temp))
        {
            if (temp.Captain.IsAi)
            {
                switch (allowedDirection)
                {
                    case Direction.Both:
                        temp.Captain.Jump();
                        break;
                    case Direction.Left:
                        if(temp.Flipped)
                            temp.Captain.Jump();
                        break;
                    case Direction.Right:
                        if(!temp.Flipped)
                            temp.Captain.Jump();
                        break;
                }
            }
        }
    }
}
