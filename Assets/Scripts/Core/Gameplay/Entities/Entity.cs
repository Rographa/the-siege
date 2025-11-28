using System;
using Core.Interfaces;
using UnityEngine;

namespace Core.Gameplay.Entities
{
    public abstract class Entity : MonoBehaviour, IDamageable, IDamager
    {
        public Action<Entity> OnDeath;
        public event Action<Entity> OnDamageTaken;
        public EntityAttributes attributes = new();
        public Collider mainCollider;

        public float Health
        {
            get => attributes.health;
            set => attributes.health = value;
        }

        public float Damage
        {
            get => attributes.damage;
            set => attributes.damage = value;
        }

        public float AttackSpeed
        {
            get => attributes.attackSpeed;
            set => attributes.attackSpeed = value;
        }

        public float Range
        {
            get => attributes.range;
            set => attributes.range = value;
        }
        
        public IDamageable Target { get; set; }
        public bool IsDead => Health <= 0;
        protected float LastAttackTime;
        protected bool IsInitialized;
        
        public Transform GetTransform()
        {
            return transform;
        }

        public float GetDistance(Entity other)
        {
            if (IsDead) return Mathf.Infinity;
            var closestPoint = GetClosestPoint(other);
            var otherClosestPoint = other.GetClosestPoint(this);
            
            return Vector3.Distance(closestPoint, otherClosestPoint);
        }

        public Vector3 GetClosestPoint(Entity other)
        {
            if (other == null || other.mainCollider == null) return Vector3.zero;
            return other.mainCollider.ClosestPoint(transform.position);
        }
        
        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            Health -= damage;
            if (IsDead)
            {
                Die();
            }
            OnDamageTaken?.Invoke(this);
        }

        protected virtual void SetAttributes(EntityAttributes reference)
        {
            attributes = new EntityAttributes(reference);
        }

        public virtual void Die()
        {
            OnDeath?.Invoke(this);
        }

        public abstract void Attack();
    }
}