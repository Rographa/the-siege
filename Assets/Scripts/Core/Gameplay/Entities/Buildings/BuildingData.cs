using UnityEngine;

namespace Core.Gameplay.Entities.Buildings
{
    [CreateAssetMenu(fileName = "Building Data", menuName = "Core/Gameplay/Entities/Buildings/Building Data")]
    public class BuildingData : ScriptableObject
    {
        public EntityAttributes attributes;

        public float Health => attributes.health;
        public float Damage => attributes.damage;
        public float AttackSpeed => attributes.attackSpeed;
        public float Range => attributes.range;
    }
}