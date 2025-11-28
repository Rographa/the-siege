using System;
using System.Collections.Generic;
using Core.Gameplay.Entities.Buildings;
using Core.Gameplay.Managers;
using UnityEngine;

namespace Core.UI
{
    public class CatalogUI : UIController
    {
        [SerializeField] private BuildingUI buildingUIPrefab;
        [SerializeField] private RectTransform buildingUIParent;

        private List<BuildingData> _catalog;
        private Dictionary<BuildingData, BuildingUI> _catalogUIDict;

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
                    Building.OnBuildingInitialized += UpdateBuildingCost;
                    break;
                case false:
                    Building.OnBuildingInitialized -= UpdateBuildingCost;
                    break;
            }
        }

        private void UpdateBuildingCost(Building building)
        {
            if (building.LoadedData is BastionData) return;
            var buildingUI = _catalogUIDict[building.LoadedData];
            if (buildingUI == null) return;

            
            buildingUI.UpdateCost(building.LoadedData.Cost);

        }

        public void SetCatalog(List<BuildingData> catalog)
        {
            _catalog = catalog;
            _catalogUIDict = new();
            foreach (var buildingData in _catalog)
            {
                SpawnBuildingUI(buildingData);
            }
        }

        private void SpawnBuildingUI(BuildingData buildingData)
        {
            var obj = Instantiate(buildingUIPrefab, buildingUIParent);
            obj.Initialize(buildingData);
            _catalogUIDict.Add(buildingData, obj);
        }
    }
}