using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Gameplay.Entities.Components;
using Core.Gameplay.Entities.Units;
using UnityEngine;

namespace Core.Gameplay.Entities.Buildings
{
    public class Building : Entity
    {
        [SerializeField] protected SphereCollider rangeCollider;
        [SerializeField] protected string enemyTag = "Enemy";
        [SerializeField] protected Shooter shooter;

        protected List<Unit> EnemiesInRange = new();
        public void Initialize(BuildingData data)
        {
            SetAttributes(data.attributes);
            rangeCollider.radius = Range;
            rangeCollider.isTrigger = true;
            shooter.Initialize(this);
            StartCoroutine(AttackEnemiesInRange());
        }

        private IEnumerator AttackEnemiesInRange()
        {
            var attackCooldown = 1f / attributes.attackSpeed;
            while (true)
            {
                while (EnemiesInRange.Count > 0)
                {
                    while (Target == null)
                    {
                        GetNextTarget();
                        yield return new WaitForEndOfFrame();
                    }

                    while (LastAttackTime > Time.time)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    
                    Attack();
                    LastAttackTime = Time.time + attackCooldown;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void GetNextTarget()
        {
            if (EnemiesInRange.Count == 0) return;
            Target = EnemiesInRange.Where(e => !e.IsDead).OrderBy(GetDistance).First();
        }

        public override void Die()
        {
            throw new System.NotImplementedException();
        }

        public override void Attack()
        {
            if (Target == null || Target.IsDead)
            {
                if (EnemiesInRange.Count == 0) return;
                GetNextTarget();
            }
            shooter.Shoot(Target);
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(enemyTag)) return;
            var unit = other.GetComponent<Unit>();
            if (unit == null || EnemiesInRange.Contains(unit)) return;
            EnemiesInRange.Add(unit);
        }

        protected void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(enemyTag)) return;
            var unit = other.GetComponent<Unit>();
            if (unit == null || !EnemiesInRange.Contains(unit)) return;
            EnemiesInRange.Remove(unit);
        }
    }
}