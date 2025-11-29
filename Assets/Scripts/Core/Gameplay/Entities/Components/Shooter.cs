using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Gameplay.Entities.Buildings;
using Core.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Gameplay.Entities.Components
{
    public class Shooter : MonoBehaviour
    {
        private const float RayDuration = 0.1f;
        
        [SerializeField] private Projectile prefab;
        [SerializeField] private ShooterType shooterType;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private List<Transform> muzzles;

        private Transform _currentMuzzle;
        private Entity _parent;
        private Coroutine _vfxCoroutine;

        public void Initialize(Entity parent)
        {
            _parent = parent;

            if (parent is not Building building) return;
            if (building.LoadedData.ProjectilePrefab != null)
            {
                prefab = building.LoadedData.ProjectilePrefab;
            }
            shooterType = building.LoadedData.ShooterType;
            lineRenderer.startColor = lineRenderer.endColor = building.LoadedData.Color;
        }

        public void Shoot(IDamageable target)
        {
            switch (shooterType)
            {
                case ShooterType.Projectile:
                    Shoot_Projectile(target);
                    break;
                case ShooterType.Ray:
                    Shoot_Ray(target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Shoot_Projectile(IDamageable target)
        {
            var muzzle = GetMuzzle(target);
            var position = muzzle.position;
            var direction = (target.GetTransform().position - position).normalized;
            
            var instance = Instantiate(prefab, position, Quaternion.identity);
            instance.Initialize(direction, _parent.Damage);
        }
        
        private void Shoot_Ray(IDamageable target)
        {
            var muzzle = GetMuzzle(target);

            if (_vfxCoroutine != null) StopCoroutine(_vfxCoroutine);
            _vfxCoroutine = StartCoroutine(RayVFX(target.GetTransform().position, muzzle.position));
            
            target.TakeDamage(_parent.Damage);

        }

        private IEnumerator RayVFX(Vector3 from, Vector3 to)
        {
            lineRenderer.enabled = true;
            
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
            
            yield return new WaitForSeconds(RayDuration);

            lineRenderer.enabled = false;
        }

        private Transform GetMuzzle(IDamageable target)
        {
            return muzzles.OrderBy(x => Vector3.Distance(x.position, target.GetTransform().position)).First();
        }
    }

    [Serializable]
    public enum ShooterType
    {
        Projectile,
        Ray
    }
}