using System;
using System.Collections;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Gameplay.Entities.Units
{
    public class Unit : Entity
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private MeshRenderer meshRenderer;

        private UnitData _loadedData;
        private Coroutine _coroutine;

        public Unit Initialize(UnitData data)
        {
            if (IsInitialized) return this;
            _loadedData = data;
            Health = _loadedData.Health;
            Damage = _loadedData.Damage;
            AttackSpeed = _loadedData.AttackSpeed;
            agent.speed = _loadedData.MoveSpeed;
            Range = _loadedData.Range;
            //agent.stoppingDistance = _loadedData.Range;
            
            meshRenderer.material = new Material(meshRenderer.material)
            {
                color = _loadedData.Color
            };
            IsInitialized = true;
            return this;
        }

        public void SetTarget(IDamageable target)
        {
            Target = target;
            switch (Target)
            {
                case Entity entity:
                    var closestPoint = GetClosestPoint(entity);
                    if (agent.destination != closestPoint)
                    {
                        agent.SetDestination(closestPoint);
                    }
                    break;
                default:
                    if (agent.destination != Target.GetTransform().position)
                    {
                        agent.SetDestination(Target.GetTransform().position);
                    }
                    break;
            }
            

            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(MoveAndAttack());
        }

        private IEnumerator MoveAndAttack()
        {
            var entity = (Entity)Target;
            var attackCooldown = 1f / attributes.attackSpeed;
            while (!entity.IsDead)
            {
                while (GetDistance(entity) > Range || LastAttackTime > Time.time)
                {
                    yield return new WaitForEndOfFrame();
                }

                Attack();
                LastAttackTime = Time.time + attackCooldown;
            }
        }

        public override void Die()
        {
            base.Die();
            agent.isStopped = true;
            if (_coroutine != null) StopCoroutine(_coroutine);
            Destroy(gameObject, 0.2f);
        }

        public override void Attack()
        {
            Target.TakeDamage(attributes.damage);
        }
    }
}
