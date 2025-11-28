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
        [SerializeField] private ParticleSystem vfx;
        public IDamageable Target { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        
        public void Initialize(Vector3 direction, float damage)
        {
            Damage = damage;
            if (direction == Vector3.zero) direction = transform.forward;
            direction = direction.normalized;
            transform.forward = direction;
            transform.rotation *= Quaternion.Euler(eulerOffset);
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
                CheckVFX();
                Destroy(gameObject);
            }
        }

        private void CheckVFX()
        {
            if (vfx != null)
            {
                vfx.transform.parent = null;
                vfx.Stop();
                Destroy(vfx.gameObject, 1f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & obstacleLayerMask.value) > 0)
            {
                rb.velocity = Vector3.zero;
                rb.Sleep();
                CheckVFX();
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