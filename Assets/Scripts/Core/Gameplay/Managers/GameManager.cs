using System;
using System.Collections;
using System.Collections.Generic;
using Core.Gameplay.Configs;
using Core.Gameplay.Entities;
using Core.Gameplay.Entities.Buildings;
using Core.Gameplay.Entities.Units;
using Core.Gameplay.Waves;
using Core.UI;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Random = UnityEngine.Random;

namespace Core.Gameplay.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static event Action<float> OnCurrencyChanged;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private NavMeshSurface surface;
        [SerializeField] private GameConfig config;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private Spawner spawner;
        [SerializeField] private Bastion bastion;
        [SerializeField] private List<WaveData> waveList;
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private BuildingTooltipUI buildingTooltip;
        [SerializeField] private LayerMask buildingLayerMask;

        private WaveData _currentWave;
        private List<Unit> _spawnedUnits;

        private int _waveNumber;
        private float _waveQuantityMultiplier;
        private float _waveDifficultyMultiplier;
        private float _currency;

        private float Currency
        {
            get => _currency;
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    OnCurrencyChanged?.Invoke(_currency);
                }
            }
        }
        
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
                    InputManager.OnPositionInput += HandlePositionInput;
                    Building.OnBuildingInitialized += HandleBuildingInitialized;
                    OnCurrencyChanged += HandleCurrencyChanged;
                    
                    break;
                case false:
                    InputManager.OnNormalSpeedInput -= HandleNormalSpeedInput;
                    InputManager.OnFastSpeedInput -= HandleFastSpeedInput;
                    InputManager.OnFastestSpeedInput -= HandleFastestSpeedInput;
                    Building.OnBuildingInitialized -= HandleBuildingInitialized;
                    InputManager.OnPositionInput -= HandlePositionInput;
                    OnCurrencyChanged -= HandleCurrencyChanged;
                    break;
            }
        }

        private void HandlePositionInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            TooltipRaycast(context.ReadValue<Vector2>());
        }

        private void TooltipRaycast(Vector2 screenPos)
        {
           var ray = gameCamera.ScreenPointToRay(screenPos);
           if (Physics.Raycast(ray, out var hit, 1000, buildingLayerMask, QueryTriggerInteraction.Ignore))
           {
               if (hit.transform.TryGetComponent(out Building building))
               {
                   UnityEngine.Debug.Log($"{building.LoadedData.Name}");
                   buildingTooltip.SetBuilding(building, screenPos, gameCamera);
               }
               else
               {
                   buildingTooltip.Clear();
               }
           }
           else
           {
               buildingTooltip.Clear();
           }
        }

        private void HandleCurrencyChanged(float _)
        {
            currencyText.SetText($"${Currency:0.00}");
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
            yield return new WaitForEndOfFrame();
            _spawnedUnits ??= new();
            _waveNumber = 0;
            _waveQuantityMultiplier = 1f;
            _waveDifficultyMultiplier = 1f;
            Currency = config.StartCurrency;
            
            while (_spawnedUnits.Count == 0)
            {
                UpdateProgression();
                _currentWave = GetWave();
                _waveNumber++;
                var unitDict = _currentWave.GetUnits(_waveQuantityMultiplier);
                foreach (var kvp in unitDict)
                {
                    for (var i = 0; i < kvp.Value; i++)
                    {
                        var unit = spawner.SpawnUnit(kvp.Key, GetRandomSpawnPosition(), _waveDifficultyMultiplier);
                        unit.SetTarget(bastion);
                        unit.OnDeath += HandleUnitDeath;
                        _spawnedUnits.Add(unit);
                        yield return new WaitForSeconds(config.SpawnInterval);
                    }
                }

                yield return new WaitUntil(() => _spawnedUnits.Count == 0);
            }
        }

        private void UpdateProgression()
        {
            if (_waveNumber == 0) return;
            if (_waveNumber % config.SpawnMultiplierInterval == 0)
            {
                _waveQuantityMultiplier += config.SpawnMultiplierIncrease;
            }
            if (_waveNumber % config.DifficultyMultiplierInterval == 0)
            {
                _waveDifficultyMultiplier += config.DifficultyMultiplierIncrease;
            }
        }

        private void HandleUnitDeath(Entity entity)
        {
            if (entity is not Unit unit) return;
            _spawnedUnits.Remove(unit);
            AddCurrency(unit.GetReward(_waveDifficultyMultiplier));
            unit.OnDeath -= HandleUnitDeath;
        }

        private void AddCurrency(float amount)
        {
            Currency += amount;
        }
        
        public bool HasCurrency(float amount) => _currency >= amount;

        public bool SpendCurrency(float amount)
        {
            if (!HasCurrency(amount)) return false;
            Currency -= amount;
            return true;
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

        public static int GetBuildingCount(BuildingData data)
        {
            if (Instance == null) return 0;
            return Instance.buildManager.GetBuildingCount(data);
        }
    }
}