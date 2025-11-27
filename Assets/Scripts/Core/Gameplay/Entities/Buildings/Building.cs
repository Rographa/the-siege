using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Gameplay.Entities.Components;
using Core.Gameplay.Entities.Units;
using UnityEngine;
using Utilities;

namespace Core.Gameplay.Entities.Buildings
{
    public class Building : Entity
    {
        public static event Action<Building> OnBuildingInitialized;
        [SerializeField] protected SphereCollider rangeCollider;
        [SerializeField] protected string enemyTag = "Enemy";
        [SerializeField] protected Shooter shooter;
        [GetComponent] protected MeshRenderer MeshRenderer;

        protected List<Unit> EnemiesInRange = new();

        public BuildingData LoadedData
        {
            get;
            protected set;
        }
        
        
        public void Initialize(BuildingData data)
        {
            if (IsInitialized) return;
            ComponentInjector.InjectComponents(this);
            LoadedData = data;
            MeshRenderer.material = new Material(data.Material)
            {
                color = data.Color
            };
            SetAttributes(data.Attributes);
            SetSize(data.Size);
            rangeCollider.radius = Range;
            rangeCollider.isTrigger = true;
            shooter.Initialize(this);
            IsInitialized = true;
            OnBuildingInitialized?.Invoke(this);
            StartCoroutine(AttackEnemiesInRange());
        }

        private void SetSize(Vector3 size)
        {
            transform.localScale = size;
            transform.position = new(transform.position.x, size.y / 2, transform.position.z);
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
            EnemiesInRange.RemoveAll(x => x == null || x.IsDead);
            if (EnemiesInRange.Count == 0) return;

            var list = EnemiesInRange.OrderBy(GetDistance).ToList();

            foreach (var unit in list)
            {
                if (unit.IsDead) continue;
                Target = unit;
                break;
            }

            if (Target == null)
            {
                Target = list.FirstOrDefault();
            }
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
            unit.OnDeath += HandleUnitDeath;
        }

        private void HandleUnitDeath(Entity entity)
        {
            if (entity is not Unit unit) return;

            EnemiesInRange.Remove(unit);
            unit.OnDeath -= HandleUnitDeath;
        }

        protected void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(enemyTag)) return;
            var unit = other.GetComponent<Unit>();
            if (unit == null || !EnemiesInRange.Contains(unit)) return;
            EnemiesInRange.Remove(unit);
            unit.OnDeath -= HandleUnitDeath;
        }
    }
}