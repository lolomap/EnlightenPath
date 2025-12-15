using UnityEngine;
namespace Spawnables.Data
{
    [CreateAssetMenu(fileName = "WaxEater", menuName = "Game/Monsters/WaxEater", order = 0)]
    public class WaxEaterSO : SpawnObjectSO
    {
        public int TilesToEat;
    }
}
