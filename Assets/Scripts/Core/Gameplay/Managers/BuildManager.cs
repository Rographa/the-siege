using System;
using System.Collections.Generic;
using Core.Gameplay.Entities.Buildings;
using Core.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Gameplay.Managers
{
    public class BuildManager : MonoBehaviour
    {
        [SerializeField] private Color availableColor;
        [SerializeField] private Color unavailableColor;
        [SerializeField] private Building buildingPrefab;
        [SerializeField] private LayerMask obstacleLayerMask;
        [SerializeField] private LayerMask buildableLayerMask;
        [SerializeField] private Transform ghostBuilding;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private List<BuildingData> buildingCatalog;
        [SerializeField] private CatalogUI catalogUI;

        private BuildingData _selectedBuilding;
        private Vector2 _lastPositionInput;
        private BoxCollider _ghostBuildingCollider;
        private MeshRenderer _ghostBuildingMeshRenderer;

        private Dictionary<BuildingData, List<Building>> _placedBuildings = new();
        private void Initialize()
        {
            _ghostBuildingCollider = ghostBuilding.GetComponent<BoxCollider>();
            _ghostBuildingMeshRenderer = ghostBuilding.GetComponent<MeshRenderer>();
            catalogUI.SetCatalog(buildingCatalog);
        }
        
        private void OnEnable()
        {
            Initialize();
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
                    InputManager.OnPositionInput += HandlePositionInput;
                    InputManager.OnConfirmInput += HandleConfirmInput;
                    InputManager.OnCancelInput += HandleCancelInput;
                    InputManager.OnBuildModeInput += HandleBuildModeInput;
                    BuildingUI.OnSelected += HandleOnSelectBuilding;
                    break;
                case false:
                    InputManager.OnPositionInput -= HandlePositionInput;
                    InputManager.OnConfirmInput -= HandleConfirmInput;
                    InputManager.OnCancelInput -= HandleCancelInput;
                    InputManager.OnBuildModeInput -= HandleBuildModeInput;
                    BuildingUI.OnSelected -= HandleOnSelectBuilding;
                    break;
            }
        }

        private void HandleCancelInput(InputAction.CallbackContext context)
        {
            if (_selectedBuilding == null || !context.performed) return;
            _selectedBuilding = null;
            catalogUI.Show();
            UpdateGhostBuilding();
        }

        private void HandleConfirmInput(InputAction.CallbackContext context)
        {
            if (!context.performed || _selectedBuilding == null) return;

            if (CanPlace() && GameManager.Instance.SpendCurrency(_selectedBuilding.Cost))
            {
                PlaceBuilding();
            }
        }

        private void PlaceBuilding()
        {
            var building = Instantiate(buildingPrefab, ghostBuilding.position, ghostBuilding.rotation);
            
            if (!_placedBuildings.ContainsKey(_selectedBuilding))
            {
                _placedBuildings.Add(_selectedBuilding, new ());    
            }
            _placedBuildings[_selectedBuilding].Add(building);
            building.Initialize(_selectedBuilding);
            _selectedBuilding = null;
            UpdateGhostBuilding();
        }

        public bool CanPlace()
        {
            return Physics.OverlapBox(ghostBuilding.position, ghostBuilding.localScale / 2, ghostBuilding.rotation,
                obstacleLayerMask, QueryTriggerInteraction.Ignore).Length == 0;
        }

        public int GetBuildingCount(BuildingData data)
        {
            if (!_placedBuildings.ContainsKey(data)) return 0;
            return _placedBuildings[data].Count;
        }

        private void HandlePositionInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            _lastPositionInput = context.ReadValue<Vector2>();
            UpdateGhostBuilding();
        }

        private void HandleBuildModeInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            catalogUI.Toggle();
        }
        
        private void HandleOnSelectBuilding(BuildingData data)
        {
            _selectedBuilding = data;
            catalogUI.Hide();
        }

        private void UpdateGhostBuilding()
        {
            if (_selectedBuilding == null)
            {
                ghostBuilding.gameObject.SetActive(false);
                return;
            }
            ghostBuilding.gameObject.SetActive(true);
            ghostBuilding.localScale = _selectedBuilding.Size;
            var offset = Vector3.zero;
            offset.y = ghostBuilding.localScale.y / 2f;
            var ray = gameCamera.ScreenPointToRay(_lastPositionInput);
            if (Physics.Raycast(ray, out var hit, 1000, buildableLayerMask))
            {
                ghostBuilding.position = offset + hit.point;
            }

            _ghostBuildingMeshRenderer.material.color = CanPlace() && GameManager.Instance.HasCurrency(_selectedBuilding.Cost) ? availableColor : unavailableColor;
        }
    }
}