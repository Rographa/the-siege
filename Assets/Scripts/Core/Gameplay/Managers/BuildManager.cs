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
        [SerializeField] private Building buildingPrefab;
        [SerializeField] private LayerMask buildableLayer;
        [SerializeField] private Transform ghostBuilding;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private List<BuildingData> buildingCatalog;
        [SerializeField] private CatalogUI catalogUI;

        private BuildingData _selectedBuilding;
        private Vector2 _lastPositionInput;
        private void Initialize()
        {
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

            PlaceBuilding();
        }

        private void PlaceBuilding()
        {
            var building = Instantiate(buildingPrefab, ghostBuilding.position, ghostBuilding.rotation);
            building.Initialize(_selectedBuilding);

            _selectedBuilding = null;
            UpdateGhostBuilding();
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
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, buildableLayer))
            {
                ghostBuilding.position = offset + hit.point;
            }
        }
    }
}