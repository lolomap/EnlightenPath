using System;
using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "DungeonConfig", menuName = "Game/DungeonConfig", order = 0)]
	public class DungeonConfig : ScriptableObject
	{
		public int Width;
		public int Height;

		public GrandCandleConfig GrandCandle;
	}
}