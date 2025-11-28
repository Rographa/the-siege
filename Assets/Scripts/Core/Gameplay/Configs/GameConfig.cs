using System.Collections.Generic;
using Core.Gameplay.Entities.Buildings;
using Core.Gameplay.Waves;
using UnityEngine;

namespace Core.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Core/Gameplay/Configs/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("References")] 
        [SerializeField] private BastionData bastionData;
        [SerializeField] private List<WaveData> waveList;
        
        [Header("Spawn Properties")]
        [SerializeField] private float spawnInterval = 0.5f;
        [SerializeField] private float spawnPosX = 45;
        [SerializeField] private float spawnPosY = 1;
        [SerializeField] private Vector2 spawnRangeZ = new(-10, 10);


        [Header("Progression")] 
        [SerializeField] private float startCurrency = 50;
        [SerializeField] private int spawnMultiplierInterval = 5;
        [SerializeField] private float spawnMultiplierIncrease = 0.5f;
        [SerializeField] private int difficultyMultiplierInterval = 10;
        [SerializeField] private float difficultyMultiplierIncrease = 1f;
        [SerializeField] private float upgradeBaseCost = 80;
        [SerializeField] private float upgradeGrowthRate = 1.5f;

        public BastionData BastionData => bastionData;
        public List<WaveData> WaveList => waveList;
        public float SpawnInterval => spawnInterval;
        public float SpawnPosX => spawnPosX;
        public float SpawnPosY => spawnPosY;
        public Vector2 SpawnRangeZ => spawnRangeZ;
        public int SpawnMultiplierInterval => spawnMultiplierInterval;
        public float SpawnMultiplierIncrease => spawnMultiplierIncrease;
        public int DifficultyMultiplierInterval => difficultyMultiplierInterval;
        public float DifficultyMultiplierIncrease => difficultyMultiplierIncrease;
        public float StartCurrency => startCurrency;
        public float UpgradeBaseCost => upgradeBaseCost;
        public float UpgradeGrowthRate => upgradeGrowthRate;
    }
}