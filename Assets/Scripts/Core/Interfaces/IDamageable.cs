using UnityEngine;

namespace Core.Interfaces
{
    public interface IDamageable
    {
        public bool IsDead { get; }
        public float Health { get; set; }
        public void TakeDamage(float damage);
        public void Die();
        public Transform GetTransform();
    }
}