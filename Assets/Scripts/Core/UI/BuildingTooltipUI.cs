using Core.Gameplay.Entities.Buildings;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.UI
{
    public class BuildingTooltipUI : UIController
    {
        [SerializeField] private BuildingUI buildingUI;
        [SerializeField] private RectTransform screen;
        [SerializeField] private Vector2 offset;

        private Building _currentBuilding;

        public bool SetBuilding(Building building, Vector2 screenPos, Camera gameCamera)
        {

            RectTransformUtility.ScreenPointToLocalPointInRectangle(screen, screenPos, gameCamera, out var pos);
            pos += offset;
            ((RectTransform)buildingUI.transform).anchoredPosition = pos;
            if (_currentBuilding == building) return false;
            _currentBuilding = building;
            buildingUI.SetBuilding(building);
            return true;
        }

        public void Clear()
        {
            if (_currentBuilding != null)
            {
                buildingUI.Clear();
                _currentBuilding = null;
            }
        }

        public void UpdateData()
        {
            buildingUI.UpdateData(_currentBuilding);
        }
    }
}