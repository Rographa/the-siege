using System;
using System.Collections;
using System.Collections.Generic;
using Core.Gameplay.Configs;
using Core.Gameplay.Entities;
using Core.Gameplay.Entities.Buildings;
using Core.Gameplay.Entities.Units;
using Core.Gameplay.Waves;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Random = UnityEngine.Random;

namespace Core.Gameplay.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private NavMeshSurface surface;
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

        private void OnEnable()
        {
            HandleSubscription(true);
        }

        private void OnDisable()
        {
            HandleSubscription(false);
        }

        private void HandleSubscription(bool subscribe)
        {
            switch (subscribe)
            {
                case true:
                    InputManager.OnNormalSpeedInput += HandleNormalSpeedInput;
                    InputManager.OnFastSpeedInput += HandleFastSpeedInput;
                    InputManager.OnFastestSpeedInput += HandleFastestSpeedInput;
                    Building.OnBuildingInitialized += HandleBuildingInitialized;
                    break;
                case false:
                    InputManager.OnNormalSpeedInput -= HandleNormalSpeedInput;
                    InputManager.OnFastSpeedInput -= HandleFastSpeedInput;
                    InputManager.OnFastestSpeedInput -= HandleFastestSpeedInput;
                    Building.OnBuildingInitialized -= HandleBuildingInitialized;
                    break;
            }
        }

        private void HandleNormalSpeedInput(InputAction.CallbackContext context)
        {
            SetGameSpeed(1);
        }

        private void HandleFastSpeedInput(InputAction.CallbackContext context)
        {
            SetGameSpeed(2);
        }

        private void HandleFastestSpeedInput(InputAction.CallbackContext context)
        {
            SetGameSpeed(4);
        }

        private void SetGameSpeed(float speed)
        {
            Time.timeScale = speed;
        }

        private void HandleBuildingInitialized(Building building)
        {
            surface.BuildNavMesh();
        }

        private void StartGame()
        {
            bastion.Initialize(config.BastionData);
            StartCoroutine(HandleWave());
        }

        private IEnumerator HandleWave()
        {
            _spawnedUnits ??= new();
            var waveNumber = 0;
            var waveMultiplier = 1f;
            while (_spawnedUnits.Count == 0)
            {
                if (waveNumber > 0 && waveNumber % config.SpawnMultiplierInterval == 0)
                {
                    waveMultiplier += config.SpawnMultiplierIncrease;
                }
                _currentWave = GetWave();
                waveNumber++;
                var unitDict = _currentWave.GetUnits(waveMultiplier);
                foreach (var kvp in unitDict)
                {
                    for (var i = 0; i < kvp.Value; i++)
                    {
                        var unit = spawner.SpawnUnit(kvp.Key, GetRandomSpawnPosition());
                        unit.SetTarget(bastion);
                        unit.OnDeath += HandleUnitDeath;
                        _spawnedUnits.Add(unit);
                        yield return new WaitForSeconds(config.SpawnInterval);
                    }
                }

                yield return new WaitUntil(() => _spawnedUnits.Count == 0);
            }
        }

        private void HandleUnitDeath(Entity entity)
        {
            if (entity is not Unit unit) return;
            _spawnedUnits.Remove(unit);
            unit.OnDeath -= HandleUnitDeath;
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