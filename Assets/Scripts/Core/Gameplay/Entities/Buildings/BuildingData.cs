using Core.Gameplay.Entities.Components;
using UnityEngine;

namespace Core.Gameplay.Entities.Buildings
{
    [CreateAssetMenu(fileName = "Building Data", menuName = "Core/Gameplay/Entities/Buildings/Building Data")]
    public class BuildingData : EntityData
    {
        [SerializeField] private string buildingName;
        [SerializeField, TextArea(3, 3)] private string buildingDescription;
        [SerializeField] private int buildingCost;
        [SerializeField] private Vector3 buildingSize;
        [SerializeField] private ShooterType shooterType;
        [SerializeField] private MuzzleType muzzleType;
        [SerializeField] private Projectile projectilePrefab;

        public string Name => buildingName;
        public string Description => buildingDescription;
        public int Cost => buildingCost;
        public Vector3 Size => buildingSize;
        public ShooterType ShooterType => shooterType;
        public MuzzleType MuzzleType => muzzleType;
        public Projectile ProjectilePrefab => projectilePrefab;

    }
}