using System.Collections;
using System.Collections.Generic;
using Core.Gameplay.Configs;
using Core.Gameplay.Entities.Buildings;
using Core.Gameplay.Entities.Units;
using Core.Gameplay.Units;
using Core.Gameplay.Waves;
using UnityEngine;
using Utilities;

namespace Core.Gameplay.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private Spawner spawner;
        [SerializeField] private Bastion bastion;
        [SerializeField] private List<WaveData> waveList;

        private WaveData _currentWave;
        private List<Unit> _spawnedUnits;
        
        protected override void Init()
        {
            base.Init();
            StartGame();
        }

        private void StartGame()
        {
            bastion.Initialize(config.BastionData);
            StartCoroutine(HandleWave());
        }

        private IEnumerator HandleWave()
        {
            _currentWave = GetWave();
            _spawnedUnits ??= new();
            var unitDict = _currentWave.GetUnits();
            foreach (var kvp in unitDict)
            {
                for (var i = 0; i < kvp.Value; i++)
                {
                    var unit = spawner.SpawnUnit(kvp.Key, GetRandomSpawnPosition());
                    unit.SetTarget(bastion);
                    _spawnedUnits.Add(unit);
                    yield return new WaitForSeconds(config.SpawnInterval);
                }
            }
        }

        private Vector3 GetRandomSpawnPosition()
        {
            return new Vector3(config.SpawnPosX, config.SpawnPosY,
                Random.Range(config.SpawnRangeZ.x, config.SpawnRangeZ.y));
        }

        private WaveData GetWave()
        {
            if (_currentWave != null)
            {
                var tmp = new List<WaveData>(waveList);
                if (tmp.Count > 1)
                {
                    tmp.Remove(_currentWave);
                }
                return tmp[Random.Range(0, tmp.Count)];
            }
            return waveList[Random.Range(0, waveList.Count)];
        }
    }
}