using Core.Gameplay.Entities;
using UnityEngine;

namespace Core.Gameplay.Units
{
    [CreateAssetMenu(menuName =  "Core/Gameplay/Units/Unit Data", fileName =  "UnitData")]
    public class UnitData : ScriptableObject
    {
        [Header("Gameplay Properties")] 
        [SerializeField] private EntityAttributes attributes;
        [SerializeField] private float moveSpeed;
        

        [Header("Visual Properties")]
        [SerializeField] private Color color;

        public float Health => attributes.health;
        public float Damage => attributes.damage;
        public float MoveSpeed => moveSpeed;
        public float AttackSpeed => attributes.attackSpeed;
        public float Range => attributes.range;
        public Color Color => color;
    }
}