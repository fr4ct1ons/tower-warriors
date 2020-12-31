using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class CollisionEvent2D : UnityEvent<Collision2D> {}

[Serializable] public class ColliderEvent2D : UnityEvent<Collider2D> {}

public class ColliderEvents : MonoBehaviour
{
    [SerializeField] private CollisionEvent2D onCollisionEnter2D;
    [SerializeField] private CollisionEvent2D onCollisionExit2D;
    
    [Space]
    
    [SerializeField] private ColliderEvent2D onTriggerEnter2D;
    [SerializeField] private ColliderEvent2D onTriggerExit2D;

    private void OnCollisionEnter2D(Collision2D other)
    {
        onCollisionEnter2D.Invoke(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        onCollisionExit2D.Invoke(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter2D.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        onTriggerExit2D.Invoke(other);
    }
}