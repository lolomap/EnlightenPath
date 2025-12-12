using UnityEngine;

namespace Items.Data
{
    [CreateAssetMenu(fileName = "Candle", menuName = "Game/Item/Candle", order = 0)]
    public class CandleSO : SpawnObjectSO
    {
        public bool IsFlaming;
    }
}
