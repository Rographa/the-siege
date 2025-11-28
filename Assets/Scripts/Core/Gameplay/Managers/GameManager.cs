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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Core.Gameplay.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static event Action<int> OnWaveStarted;
        public static event Action<float> OnCurrencyChanged;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private NavMeshSurface surface;
        [SerializeField] private GameConfig config;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private Spawner spawner;
        [SerializeField] private Bastion bastion;
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private TextMeshProUGUI waveNumberText;
        [SerializeField] private UIController gameOverScreen;
        [SerializeField] private UIController pauseScreen;
        [SerializeField] private Button[] resumeButtons;
        [SerializeField] private Button[] restartButtons;
        [SerializeField] private Button[] quitButtons;
        
        private WaveData _currentWave;
        private List<Unit> _spawnedUnits;

        private int _waveNumber;
        private float _previousGameSpeed;
        private float _waveQuantityMultiplier;
        private float _waveDifficultyMultiplier;
        private float _currency;
        private bool _isGameOver;
        private bool _isPaused;

        private const string Wave = "Wave {0}";

        public static GameConfig GameConfig => Instance.config;
        public static float DifficultyMultiplier => Instance._waveDifficultyMultiplier;
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
                    InputManager.OnPauseInput += HandlePauseInput;
                    Building.OnBuildingInitialized += HandleBuildingInitialized;
                    OnCurrencyChanged += HandleCurrencyChanged;
                    bastion.OnDeath += HandleGameOver;
                    foreach (var resumeButton in resumeButtons)
                    {
                        resumeButton.onClick.AddListener(Resume);
                    }
                    
                    foreach (var restartButton in restartButtons)
                    {
                        restartButton.onClick.AddListener(Restart);    
                    }

                    foreach (var quitButton in quitButtons)
                    {
                        quitButton.onClick.AddListener(ReturnToMenu);
                    }
                    
                    break;
                case false:
                    InputManager.OnNormalSpeedInput -= HandleNormalSpeedInput;
                    InputManager.OnFastSpeedInput -= HandleFastSpeedInput;
                    InputManager.OnFastestSpeedInput -= HandleFastestSpeedInput;
                    InputManager.OnPauseInput -= HandlePauseInput;
                    Building.OnBuildingInitialized -= HandleBuildingInitialized;
                    OnCurrencyChanged -= HandleCurrencyChanged;
                    bastion.OnDeath -= HandleGameOver;
                    foreach (var resumeButton in resumeButtons)
                    {
                        resumeButton.onClick.RemoveAllListeners();
                    }
                    
                    foreach (var restartButton in restartButtons)
                    {
                        restartButton.onClick.RemoveAllListeners();    
                    }

                    foreach (var quitButton in quitButtons)
                    {
                        quitButton.onClick.RemoveAllListeners();
                    }
                    break;
            }
        }


        private void Resume()
        {
            SetPause(false);
        }

        private void SetPause(bool pause)
        {
            UnityEngine.Debug.Log($"Pause: {pause}");
            SetGameSpeed(pause ? 0 : _previousGameSpeed);
            pauseScreen.ShowHide(pause);
            _isPaused = pause;
        }

        private void Restart()
        {
            SetGameSpeed(1);
            var scene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(scene);
        }

        private void ReturnToMenu()
        {
            SetGameSpeed(1);
            SceneManager.LoadScene(0);
        }

        private void HandleGameOver(Entity _)
        {
            SetGameSpeed(0);
            _isGameOver = true;
            gameOverScreen.Show();
        }

        private void HandleCurrencyChanged(float _)
        {
            currencyText.SetText($"${Currency:0.00}");
        }
        
        
        private void HandlePauseInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            SetPause(!_isPaused);
        }

        private void HandleNormalSpeedInput(InputAction.CallbackContext context)
        {
            if (_isGameOver) return;
            SetGameSpeed(1);
        }

        private void HandleFastSpeedInput(InputAction.CallbackContext context)
        {
            if (_isGameOver) return;
            SetGameSpeed(2);
        }

        private void HandleFastestSpeedInput(InputAction.CallbackContext context)
        {
            if (_isGameOver) return;
            SetGameSpeed(4);
        }

        private void SetGameSpeed(float speed)
        {
            if (Time.timeScale > 0)
            {
                _previousGameSpeed = Time.timeScale;
            }

            Time.timeScale = speed;
        }

        private void HandleBuildingInitialized(Building building)
        {
            surface.BuildNavMesh();
        }

        private void StartGame()
        {
            bastion.Initialize(config.BastionData);
            _isGameOver = _isPaused = false;
            SetGameSpeed(1);
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
                waveNumberText.SetText(string.Format(Wave, _waveNumber.ToString()));
                OnWaveStarted?.Invoke(_waveNumber);
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
                var tmp = new List<WaveData>(config.WaveList);
                if (tmp.Count > 1)
                {
                    tmp.Remove(_currentWave);
                }
                return tmp[Random.Range(0, tmp.Count)];
            }
            return config.WaveList[Random.Range(0, config.WaveList.Count)];
        }

        public static int GetBuildingCount(BuildingData data)
        {
            if (Instance == null) return 0;
            return Instance.buildManager.GetBuildingCount(data);
        }
    }
}