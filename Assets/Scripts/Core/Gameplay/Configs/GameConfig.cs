using Core.Gameplay.Entities.Buildings;
using UnityEngine;

namespace Core.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Core/Gameplay/Configs/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("References")] 
        [SerializeField] private BastionData bastionData;
        
        [Header("Spawn Properties")]
        [SerializeField] private float spawnInterval = 0.5f;
        [SerializeField] private float spawnPosX = 45;
        [SerializeField] private float spawnPosY = 1;
        [SerializeField] private Vector2 spawnRangeZ = new(-10, 10);


        public BastionData BastionData => bastionData;
        public float SpawnInterval => spawnInterval;
        public float SpawnPosX => spawnPosX;
        public float SpawnPosY => spawnPosY;
        public Vector2 SpawnRangeZ => spawnRangeZ;
    }
}