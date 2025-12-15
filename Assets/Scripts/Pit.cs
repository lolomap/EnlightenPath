using System;
using UnityEngine;
using Zenject;

public class Pit : MonoBehaviour
{
    protected Vector2Int _gridPos;
    
    [Inject] private MapManager _mapManager;

    private void Awake()
    {
        if (_mapManager != null)
            _gridPos = _mapManager.WorldToGrid(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player == null) return;
        OnFall(player);
    }

    protected virtual void OnFall(Player player)
    {
        player.FallToDark();
    }
}