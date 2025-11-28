using Core.Gameplay.Entities.Components;
using Core.Gameplay.Managers;
using UnityEngine;

namespace Core.Gameplay.Entities.Buildings
{
    [CreateAssetMenu(fileName = "Building Data", menuName = "Core/Gameplay/Entities/Buildings/Building Data")]
    public class BuildingData : EntityData
    {
        [SerializeField] private string buildingName;
        [SerializeField, TextArea(3, 3)] private string buildingDescription;
        [SerializeField] private float buildingCost;
        [SerializeField] private Vector3 buildingSize = Vector3.one;
        [SerializeField] private ShooterType shooterType;
        [SerializeField] private MuzzleType muzzleType;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float priceIncreasePerBuilding;
        [SerializeField] private float upgradeBoost = 0.2f;
        
        public string Name => buildingName;
        public string Description => buildingDescription;
        public float Cost => GetCost();

        private float GetCost()
        {
            var buildingsPlaced = GameManager.GetBuildingCount(this); 
            var newCost = buildingCost +
                          (buildingCost *
                           priceIncreasePerBuilding *
                           buildingsPlaced);
            return newCost;
        }

        public Vector3 Size => buildingSize;
        public ShooterType ShooterType => shooterType;
        public MuzzleType MuzzleType => muzzleType;
        public Projectile ProjectilePrefab => projectilePrefab;
        public float PriceIncreasePerBuilding => priceIncreasePerBuilding;
    }
}