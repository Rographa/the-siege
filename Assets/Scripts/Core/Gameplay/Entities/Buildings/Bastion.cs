using Core.Gameplay.Configs;
using Core.Interfaces;
using UnityEngine;

namespace Core.Gameplay.Entities.Buildings
{
    public class Bastion : Building
    {
        private BastionData _loadedData;

        public override void Die()
        {
            throw new System.NotImplementedException();
        }
    }
}