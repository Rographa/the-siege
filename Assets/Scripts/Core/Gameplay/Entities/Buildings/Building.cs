using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Gameplay.Entities.Components;
using Core.Gameplay.Entities.Units;
using Core.Gameplay.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Random = UnityEngine.Random;

namespace Core.Gameplay.Entities.Buildings
{
    public class Building : Entity
    {
        public static event Action<Building> OnBuildingInitialized;
        [SerializeField] protected SphereCollider rangeCollider;
        [SerializeField] protected string enemyTag = "Enemy";
        [SerializeField] protected Shooter shooter;
        [SerializeField] protected Transform visuals;
        [GetComponent(true)] protected MeshRenderer MeshRenderer;

        protected List<Unit> EnemiesInRange = new();
        public int level;

        public BuildingData LoadedData
        {
            get;
            protected set;
        }

        private void OnEnable()
        {
            HandleSubscription(true);
        }

        private void OnDisable()
        {
            HandleSubscription(false);
        }

        private void HandleSubscription(bool subscribe)
        {
            switch (subscribe)
            {
                case true:
                    GameManager.OnWaveStarted += HandleWaveStarted;
                    break;
                case false:
                    GameManager.OnWaveStarted -= HandleWaveStarted;
                    break;
            }
        }

        private void HandleWaveStarted(int waveNumber)
        {
            Deteriorate();
        }

        private void Deteriorate()
        {
            var damage = Random.Range(LoadedData.DeteriorationRange.x, LoadedData.DeteriorationRange.y) * GameManager.DifficultyMultiplier;
            TakeDamage(damage);
        }


        public void Initialize(BuildingData data)
        {
            if (IsInitialized) return;
            ComponentInjector.InjectComponents(this);
            LoadedData = data;
            level = 1;
            MeshRenderer.material = new Material(data.Material)
            {
                color = data.Color
            };
            SetAttributes(data.Attributes);
            SetSize(data.Size);
            rangeCollider.isTrigger = true;
            shooter.Initialize(this);
            IsInitialized = true;
            OnBuildingInitialized?.Invoke(this);
            StartCoroutine(AttackEnemiesInRange());
        }

        public void LevelUp()
        {
            var newAttributes = new EntityAttributes()
            {
                attackSpeed = LoadedData.AttackSpeed +
                              LoadedData.AttackSpeed * LoadedData.UpgradeBoost.attackSpeed * level,
                damage = LoadedData.Damage + LoadedData.Damage * LoadedData.UpgradeBoost.damage * level,
                health = LoadedData.Health + LoadedData.Health * LoadedData.UpgradeBoost.health * level,
                range = LoadedData.Range + LoadedData.Range * LoadedData.UpgradeBoost.range * level,
            };
            SetAttributes(newAttributes);
            level++;
        }

        protected override void SetAttributes(EntityAttributes reference)
        {
            base.SetAttributes(reference);
            rangeCollider.radius = Range;
        }

        public float GetUpgradeCost()
        {
            return (LoadedData.BaseCost > 0 ? LoadedData.BaseCost : GameManager.GameConfig.UpgradeBaseCost) *
                   Mathf.Pow(GameManager.GameConfig.UpgradeGrowthRate, level - 1);
        }

        public override void Die()
        {
            base.Die();
            Destroy(gameObject);
        }

        private void SetSize(Vector3 size)
        {
            visuals.localScale = size;
            transform.position = new(transform.position.x, size.y / 2, transform.position.z);
        }

        private IEnumerator AttackEnemiesInRange()
        {
            while (!IsDead)
            {
                while (EnemiesInRange.Count > 0)
                {
                    while (LastAttackTime > Time.time)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    while (Target == null || Target.IsDead)
                    {
                        yield return new WaitForEndOfFrame();
                        GetNextTarget();
                    }
                    Attack();
                    LastAttackTime = Time.time + 1f / AttackSpeed;
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