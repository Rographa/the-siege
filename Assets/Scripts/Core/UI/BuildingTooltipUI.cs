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
        private bool _isHiding;

        public void SetBuilding(Building building, Vector2 screenPos, Camera gameCamera)
        {
            _isHiding = false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(screen, screenPos, gameCamera, out var pos);
            pos += offset;
            ((RectTransform)buildingUI.transform).anchoredPosition = pos;
            if (_currentBuilding == building) return;
            _currentBuilding = building;
            buildingUI.SetBuilding(building);
            if (!isActive) Show();
        }

        public void Clear()
        {
            _currentBuilding = null;
            if (isActive && !_isHiding)
            {
                _isHiding = true;
                Hide();
            }
        }
    }
}