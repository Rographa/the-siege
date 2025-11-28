using System;
using Core.Gameplay.Entities.Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class BuildingUI : MonoBehaviour
    {
        public static event Action<BuildingData> OnSelected;
        
        private const string Damage = "Damage: ";
        private const string AttackSpeed = "Attack Speed: ";
        private const string Range = "Range: ";
        private const string Health = "Health: ";

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buildingName;
        [SerializeField] private TextMeshProUGUI buildingDescription;
        [SerializeField] private TextMeshProUGUI buildingDamage;
        [SerializeField] private TextMeshProUGUI buildingAttackSpeed;
        [SerializeField] private TextMeshProUGUI buildingRange;
        [SerializeField] private TextMeshProUGUI buildingHealth;
        [SerializeField] private TextMeshProUGUI buildingCost;
        [SerializeField] private Image[] backgrounds;

        private BuildingData _buildingData;
        
        public void Initialize(BuildingData data)
        {
            _buildingData = data;
            buildingName.SetText(data.Name);
            buildingDescription.SetText(data.Description);
            buildingDamage.SetText($"{Damage}{data.Damage:N1}");
            buildingAttackSpeed.SetText($"{AttackSpeed}{data.AttackSpeed:N1}");
            buildingRange.SetText($"{Range}{data.Range:N1}");
            buildingHealth.SetText($"{Health}{data.Health:N1}");
            UpdateCost(data.Cost);
            foreach (var background in backgrounds)
            {
                background.color = data.Color;    
            }
            button.onClick.AddListener(() => OnSelected?.Invoke(_buildingData));
        }

        public void UpdateCost(float newCost)
        {
            buildingCost.SetText($"${newCost:0.00}");
        }

        public void SetBuilding(Building building)
        {
            _buildingData = building.LoadedData;
            buildingName.SetText(_buildingData.Name);
            buildingDescription.SetText(_buildingData.Description);
            buildingDamage.SetText($"{Damage}{building.Damage:N1}");
            buildingAttackSpeed.SetText($"{AttackSpeed}{building.AttackSpeed:N1}");
            buildingRange.SetText($"{Range}{building.Range:N1}");
            buildingHealth.SetText($"{Health}{building.Health:N1}");
            buildingCost.transform.parent.gameObject.SetActive(false);
            foreach (var background in backgrounds)
            {
                background.color = _buildingData.Color;    
            }
            button.enabled = false;
        }
    }
}