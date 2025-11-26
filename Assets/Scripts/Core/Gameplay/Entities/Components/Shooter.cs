using System;
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
        [SerializeField] private GameObject prefab;
        [SerializeField] private ShooterType shooterType;
        [SerializeField] private MuzzleType muzzleType;
        [SerializeField] private List<Transform> muzzles;

        private Transform _currentMuzzle;
        private Entity _parent;

        public void Initialize(Entity parent)
        {
            _parent = parent;
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
            var muzzle = GetMuzzle();
            var position = muzzle.position;
            var direction = (target.GetTransform().position - position).normalized;
            
            var instance = Instantiate(prefab, position, Quaternion.LookRotation(direction)).GetComponent<Projectile>();
            instance.Initialize(direction, _parent.Damage);
        }
        
        private void Shoot_Ray(IDamageable target)
        {
            
        }

        private Transform GetMuzzle()
        {
            switch (muzzleType)
            {
                case MuzzleType.Sequenced:
                    if (_currentMuzzle == null)
                    {
                        _currentMuzzle = muzzles.FirstOrDefault();
                    }
                    else
                    {
                        var index = muzzles.IndexOf(_currentMuzzle) + 1;
                        if (index >= muzzles.Count) index = 0;
                        _currentMuzzle = muzzles[index];
                    }
                    break;
                case MuzzleType.Random:
                    if (_currentMuzzle == null)
                    {
                        _currentMuzzle = muzzles[Random.Range(0, muzzles.Count)];
                        break;
                    }

                    var tmp = muzzles.Where(x => x != _currentMuzzle).ToList();
                    if (tmp.Count > 0)
                    {
                        _currentMuzzle = tmp[Random.Range(0, tmp.Count)];
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _currentMuzzle;
        }
    }

    [Serializable]
    public enum ShooterType
    {
        Projectile,
        Ray
    }

    [Serializable]
    public enum MuzzleType
    {
        Sequenced,
        Random
    }
}