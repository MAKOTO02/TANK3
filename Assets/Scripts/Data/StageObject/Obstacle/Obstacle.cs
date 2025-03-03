using UnityEngine;
namespace TANK3.Data.StageObject
{
    [CreateAssetMenu(fileName = "NewObstacle", menuName = "Data/StageObject/Obstacle", order = 2)]
    public class Obstacle : StageObject
    {
        public bool isDestructible;

        public override void Interact()
        {
            // 障害物の行動（プレイヤーが接触した場合に破壊されるなど）
        }
    }
}
