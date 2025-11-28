using System;
using System.Collections;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Core.Gameplay.Entities.Units
{
    public class Unit : Entity
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private MeshRenderer meshRenderer;

        private UnitData _loadedData;
        private Coroutine _coroutine;
        private Coroutine _hitEffectCoroutine;

        public Unit Initialize(UnitData data, float difficultyMultiplier = 1f)
        {
            if (IsInitialized) return this;
            _loadedData = data;
            Health = _loadedData.Health * difficultyMultiplier;
            Damage = _loadedData.Damage * difficultyMultiplier;
            AttackSpeed = _loadedData.AttackSpeed * difficultyMultiplier;
            agent.speed = _loadedData.MoveSpeed * difficultyMultiplier;
            Range = _loadedData.Range;
            transform.localScale = Vector3.one * data.UnitSize;
            //agent.stoppingDistance = _loadedData.Range;
            
            meshRenderer.material = new Material(meshRenderer.material)
            {
                color = _loadedData.Color
            };
            IsInitialized = true;
            OnDamageTaken += HandleDamageTaken;
            return this;
        }

        private void HandleDamageTaken(Entity _)
        {
            if (_hitEffectCoroutine != null) StopCoroutine(_hitEffectCoroutine);
            _hitEffectCoroutine = StartCoroutine(HitEffect());
        }

        private IEnumerator HitEffect()
        {
            meshRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            meshRenderer.material.color = _loadedData.Color;
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

        public float GetReward(float multiplier = 1f)
        {
            return Random.Range(_loadedData.CurrencyRewardRange.x, _loadedData.CurrencyRewardRange.y) * multiplier;
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
            Target.TakeDamage(Damage);
        }
    }
}
