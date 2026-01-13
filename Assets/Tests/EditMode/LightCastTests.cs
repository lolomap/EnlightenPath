using System.Collections.Generic;
using System.Reflection;
using Data;
using NUnit.Framework;
using UnityEngine;
using Utilities;

[TestFixture]
// ReSharper disable once CheckNamespace
public class LightCastTests
{
    private GameObject _testObj;
    private DungeonConfig _config;
    private LightManager _lightManager;
    private MapManager _mapManager;
    
    [SetUp]
    public void Setup()
    {
        _testObj = new();

        _config = ScriptableObject.CreateInstance<DungeonConfig>();
        _config.Height = 3;
        _config.Width = 6;

        _mapManager = _testObj.AddComponent<MapManager>();
        typeof(MapManager).GetField("_config", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_mapManager, _config);

        /********
         *_}O__#*
         *######* 
         *##_###*
         ********/
        RoomSO tRoom = ScriptableObject.CreateInstance<RoomSO>();
        tRoom.Connections = new()
        {
            Direction.Up, Direction.Down, Direction.Right
        };
        RoomSO xRoom = ScriptableObject.CreateInstance<RoomSO>();
        xRoom.Connections = new()
        {
            Direction.Up, Direction.Down, Direction.Right, Direction.Left
        };

        GridMap grid = new(6, 3);
        grid.Replace(1, 0, tRoom);
        grid.Replace(0, 0, xRoom);
        grid.Replace(2, 0, xRoom);
        grid.Replace(3, 0, xRoom);
        grid.Replace(4, 0, xRoom);
        grid.Replace(2, 2, xRoom);
        typeof(MapManager).GetField("_grid", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_mapManager, grid);

        _lightManager = _testObj.AddComponent<LightManager>();
        typeof(LightManager).GetField("_mapManager", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_lightManager, _mapManager);
        typeof(LightManager).GetField("_config", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_lightManager, _config);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_testObj);
        Object.DestroyImmediate(_config);
    }
    
    [Test]
    [Category("EditMode")]
    public void Intensity0()
    {
        List<Vector2Int> expected = new()
        {
            new(2, 0),
        };
        
        HashSet<Vector2Int> lighted = new();
        _lightManager.LightCast(new(2, 0), 0,
            (gridPos) =>
            {
                lighted.Add(gridPos);
            }
        );
        
        CollectionAssert.AreEquivalent(expected, lighted);
    }
    
    [Test]
    [Category("EditMode")]
    public void Intensity1()
    {
        List<Vector2Int> expected = new()
        {
            new(2, 0),
            new(3, 0),
            new(2, 2),
            new(1, 0),
            new(2, 1)
        };
        
        HashSet<Vector2Int> lighted = new();
        _lightManager.LightCast(new(2, 0), 1,
            (gridPos) =>
            {
                lighted.Add(gridPos);
            }
        );
        
        CollectionAssert.AreEquivalent(expected, lighted);
    }
    
    [Test]
    [Category("EditMode")]
    public void IntensityInf()
    {
        List<Vector2Int> expected = new()
        {
            new(2, 0),
            new(3, 0),
            new(2, 2),
            new(1, 0),
            new(2, 1),
            new(4, 0),
            new(5, 0),
            new(0, 0), //is blocked by wall from right T-room but lighted from left by loop
        };
        
        HashSet<Vector2Int> lighted = new();
        _lightManager.LightCast(new(2, 0), 999,
            (gridPos) =>
            {
                lighted.Add(gridPos);
            }
        );
        
        CollectionAssert.AreEquivalent(expected, lighted);
    }
    
    [Test]
    [Category("EditMode")]
    public void EmptyBlockingIntensity1()
    {
        List<Vector2Int> expected = new()
        {
            new(2, 0),
            new(3, 0),
            new(2, 2),
            new(1, 0)
        };
        
        HashSet<Vector2Int> lighted = new();
        _lightManager.LightCast(new(2, 0), 1,
            (gridPos) =>
            {
                lighted.Add(gridPos);
            },
            (_, room) => room == null
        );
        
        CollectionAssert.AreEquivalent(expected, lighted);
    }
    
    [Test]
    [Category("EditMode")]
    public void EmptyBlockingIntensityInf()
    {
        List<Vector2Int> expected = new()
        {
            new(2, 0),
            new(3, 0),
            new(2, 2),
            new(1, 0),
            new(4, 0),
            // (0, 0) is blocked by wall from right T-room
        };
        
        HashSet<Vector2Int> lighted = new();
        _lightManager.LightCast(new(2, 0), 999,
            (gridPos) =>
            {
                lighted.Add(gridPos);
            },
            (_, room) => room == null
        );
        
        CollectionAssert.AreEquivalent(expected, lighted);
    }
}
