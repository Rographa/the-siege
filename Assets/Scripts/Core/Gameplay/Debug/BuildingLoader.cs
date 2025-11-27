using Core.Gameplay.Entities.Buildings;
using UnityEngine;

namespace Core.Gameplay.Debug
{
    public class BuildingLoader : MonoBehaviour
    {
        public Building building;
        public BuildingData buildingData;

        private void OnValidate()
        {
            building = GetComponent<Building>();
        }

        private void Awake()
        {
            building.Initialize(buildingData);
        }
    }
}