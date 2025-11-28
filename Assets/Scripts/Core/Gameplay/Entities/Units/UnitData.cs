using UnityEngine;

namespace Core.Gameplay.Entities.Units
{
    [CreateAssetMenu(menuName =  "Core/Gameplay/Units/Unit Data", fileName =  "UnitData")]
    public class UnitData : EntityData
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Vector2 currencyRewardRange;

        public float MoveSpeed => moveSpeed;
        public Vector2 CurrencyRewardRange => currencyRewardRange;

    }
}