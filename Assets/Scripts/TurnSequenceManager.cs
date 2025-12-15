using System.Collections.Generic;
using Spawnables.Data;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[CreateAssetMenu(fileName = "TurnSequence", menuName = "Game/TurnSequence")]
public class TurnSequenceManager : ScriptableObject
{
    public List<UnityEvent> Sequence = new();
    
    [Inject] private MapManager _mapManager;
    [Inject] private LightManager _lightManager;

    public void Turn()
    {
        foreach (UnityEvent action in Sequence)
        {
            action.Invoke();
        }
    }

    public void TriggerAllMonsters()
    {
        foreach (IMonsterTrigger monster in _mapManager.Monsters)
        {
            monster.UpdateTrigger();
        }
    }

    public void UpdateLight()
    {
        _lightManager.UpdateAllSources();
    }
}