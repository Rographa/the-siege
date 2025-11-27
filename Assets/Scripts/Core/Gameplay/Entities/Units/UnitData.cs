using UnityEngine;

namespace Core.Gameplay.Entities.Units
{
    [CreateAssetMenu(menuName =  "Core/Gameplay/Units/Unit Data", fileName =  "UnitData")]
    public class UnitData : EntityData
    {
        [SerializeField] private float moveSpeed;

        public float MoveSpeed => moveSpeed;
    }
}