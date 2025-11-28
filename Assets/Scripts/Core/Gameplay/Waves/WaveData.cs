using System;
using System.Collections.Generic;
using System.Linq;
using Core.Gameplay.Entities.Units;
using UnityEngine;

namespace Core.Gameplay.Waves
{
    [CreateAssetMenu(menuName = "Core/Gameplay/Waves/Wave Data", fileName =  "WaveData")]
    public class WaveData : ScriptableObject
    {
        public List<WaveNode> nodes;

        public Dictionary<UnitData, int> GetUnits(float multiplier = 1f)
        {
            var dict = new Dictionary<UnitData, int>();

            foreach (var node in nodes)
            {
                dict.Add(node.unit, Mathf.RoundToInt(node.quantity * multiplier));
            }

            return dict;
        }
    }

    [Serializable]
    public class WaveNode
    {
        public UnitData unit;
        public int quantity;
    }
}