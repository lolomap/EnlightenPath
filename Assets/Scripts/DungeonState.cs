using System;
using System.Collections.Generic;
using Data;
using Events;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

[UsedImplicitly]
public class DungeonState : IInitializable, IDisposable
{
    private readonly HashSet<Color> _unlocked = new();
    
    [Inject] private DungeonConfig _config;
    [Inject] private EventBus _eventBus;
    [Inject] private GrandCandle _grandCandle;

    public void Initialize()
    {
        _eventBus.Unlocked.EventRaised += OnUnlocked;
        _eventBus.Lose.EventRaised += Lose;
        _grandCandle.Underflow += Lose;
    }
    
    public void Dispose()
    {
        _eventBus.Unlocked.EventRaised -= OnUnlocked;
        _eventBus.Lose.EventRaised -= Lose;
        _grandCandle.Underflow -= Lose;
    }

    private void Win()
    {
        Debug.LogWarning("WIN!");
    }
    
    private void Lose()
    {
        Debug.LogWarning("LOSE!");
    }

    private void OnUnlocked(Color color)
    {
        _unlocked.Add(color);
        if (_unlocked.Count >= _config.LockTypesToOpen)
            _grandCandle.Push(_config.TileToUnlock, GrandCandle.Origin.Random);
    }
}