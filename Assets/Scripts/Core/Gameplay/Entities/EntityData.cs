using UnityEngine;

namespace Core.Gameplay.Entities
{
    public abstract class EntityData : ScriptableObject
    {
        [SerializeField] protected EntityAttributes attributes;
        [SerializeField] protected Color color;
        [SerializeField] protected Material material;
        
        public EntityAttributes Attributes => attributes;
        public float Health => attributes.health;
        public float Damage => attributes.damage;
        public float AttackSpeed => attributes.attackSpeed;
        public float Range => attributes.range;
        public Color Color => color;
        public Material Material => material;
    }
}