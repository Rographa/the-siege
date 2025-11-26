using System;
using System.Collections;
using Core.Interfaces;
using UnityEngine;

namespace Core.Gameplay.Entities
{
    public class Projectile : MonoBehaviour, IDamager
    {
        [SerializeField] private float speed;
        public IDamageable Target { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        
        private Rigidbody _rb;
        
        public void Initialize(Vector3 direction, float damage)
        {
            Damage = damage;
            if (direction == Vector3.zero) direction = transform.forward;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            _rb = GetComponent<Rigidbody>();
            if (_rb != null)
            {
                _rb.velocity = transform.forward * speed;
            }
            else
            {
                StartCoroutine(MoveRoutine());
            }
        }

        private IEnumerator MoveRoutine()
        {
            while (Target == null)
            {
                transform.Translate(transform.forward * (Time.fixedDeltaTime * speed), Space.Self);
                yield return new WaitForFixedUpdate();
            }
        }

        public void Attack()
        {
            Target.TakeDamage(Damage);
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null) return;

            Target = damageable;
            Attack();
        }
    }
}