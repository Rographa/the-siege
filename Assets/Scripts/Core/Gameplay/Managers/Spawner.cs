using Core.Gameplay.Entities.Units;
using Core.Gameplay.Units;
using UnityEngine;

namespace Core.Gameplay.Managers
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject unitPrefab;

        public Unit SpawnUnit(UnitData data, Vector3 position)
        {
            return Instantiate(unitPrefab, position, Quaternion.identity, transform).GetComponent<Unit>().Initialize(data);
        }
    }
}
