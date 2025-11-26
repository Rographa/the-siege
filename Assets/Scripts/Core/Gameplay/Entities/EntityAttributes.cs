using System;
using UnityEngine;

namespace Core.Gameplay.Entities
{
    [Serializable]
    public class EntityAttributes
    {
        public float health;
        public float damage;
        public float attackSpeed;
        public float range;

        public EntityAttributes(){}

        public EntityAttributes(EntityAttributes other) : base()
        {
            health = other.health;
            damage = other.damage;
            attackSpeed = other.attackSpeed;
            range = other.range;
        }
    }
}