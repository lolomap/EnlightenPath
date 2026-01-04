using System;
using System.Collections.Generic;
using Data;
using Events;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        _eventBus.Win.EventRaised += Win;
        _eventBus.GameOver.EventRaised += GameOver;
        _grandCandle.Underflow += GameOver;
    }
    
    public void Dispose()
    {
        _eventBus.Unlocked.EventRaised -= OnUnlocked;
        _eventBus.Win.EventRaised -= Win;
        _eventBus.GameOver.EventRaised -= GameOver;
        _grandCandle.Underflow -= GameOver;
    }

    private void Win()
    {
        Debug.LogWarning("WIN!");
        Time.timeScale = 0;
    }
    
    private void GameOver()
    {
        Debug.LogWarning("LOSE!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnUnlocked(Color color)
    {
        _unlocked.Add(color);
        if (_unlocked.Count >= _config.LockTypesToOpen)
            _grandCandle.Push(_config.TileToUnlock, GrandCandle.Origin.Top); //TODO: Random
    }
}