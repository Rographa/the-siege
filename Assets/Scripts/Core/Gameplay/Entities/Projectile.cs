using System;
using System.Collections;
using Core.Interfaces;
using UnityEngine;

namespace Core.Gameplay.Entities
{
    public class Projectile : MonoBehaviour, IDamager
    {
        private const float Lifetime = 10;
        [SerializeField] private LayerMask obstacleLayerMask;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float speed;
        [SerializeField] private bool piercingShot;
        [SerializeField] private Vector3 eulerOffset;
        [SerializeField] private float destroyDelay = 0f;
        public IDamageable Target { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        
        public void Initialize(Vector3 direction, float damage)
        {
            Damage = damage;
            if (direction == Vector3.zero) direction = transform.forward;
            if (Mathf.Abs(Vector3.Dot(direction, Vector3.up)) > 0.99f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.forward) * Quaternion.Euler(eulerOffset);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(eulerOffset);
            }
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
            
            Destroy(gameObject, Lifetime);
        }

        public void Attack()
        {
            Target.TakeDamage(Damage);
            if (!piercingShot)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & obstacleLayerMask.value) > 0)
            {
                rb.velocity = Vector3.zero;
                rb.Sleep();
                Destroy(gameObject, destroyDelay);
                return;
            }
            
            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null) return;

            Target = damageable;
            Attack();
        }
    }
}