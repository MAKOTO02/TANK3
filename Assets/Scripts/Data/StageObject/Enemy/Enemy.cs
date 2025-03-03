using System.Collections.Generic;
using UnityEngine;

namespace TANK3.Data.StageObject
{
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "Data/StageObject/Enemy", order = 1)]
    public class Enemy : StageObject
    {
        public string bulletId;
        public string vehicleControlId;
        public string cannonControlId;

        public override void Interact()
        {
            // DO NOTHING
        }
    }
}