using UnityEngine;

namespace Data
{
	public abstract class SpawnObjectSO : ScriptableObject
	{
		public GameObject Prefab;
		public float SpawnChance;
	}
}