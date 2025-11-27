using System.Collections.Generic;
using Core.Gameplay.Entities.Buildings;
using UnityEngine;

namespace Core.UI
{
    public class CatalogUI : UIController
    {
        [SerializeField] private BuildingUI buildingUIPrefab;
        [SerializeField] private RectTransform buildingUIParent;

        private List<BuildingData> _catalog;
        public void SetCatalog(List<BuildingData> catalog)
        {
            _catalog = catalog;

            foreach (var buildingData in _catalog)
            {
                SpawnBuildingUI(buildingData);
            }
        }

        private void SpawnBuildingUI(BuildingData buildingData)
        {
            var obj = Instantiate(buildingUIPrefab, buildingUIParent);
            obj.Initialize(buildingData);
        }
    }
}