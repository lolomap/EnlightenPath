using System;
using Items.Data;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Pickable))]
    [RequireComponent(typeof(LightSource))]
    public class Candle : MonoBehaviour, ISpawnObject, IHandSlot
    {
        private LightSource _lightSource;

        private void Awake()
        {
            _lightSource = GetComponent<LightSource>();
        }
    }
}
