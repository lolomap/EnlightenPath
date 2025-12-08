using System;
using System.Collections.Generic;
using EditorAttributes;
using Events;
using UI;
using UnityEngine;
using Utilities;
using Zenject;

public class Player : MonoBehaviour
{
	public float Speed;
	public float StopDistance;
	public List<LightSource> ExtraLight;

	private bool _isMoving;
	private Vector3 _direction;
	private Vector3 _targetPos;
	private Vector2Int _currentGridPos;
	private CharacterController _controller;
	private MoveUI _moveUI;
	private LightSource _selfLight;

	[Inject] private EventBus _eventBus;
	[Inject] private MapManager _mapManager;
	
	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_moveUI = GetComponentInChildren<MoveUI>();
		_selfLight = GetComponent<LightSource>();
	}
	
	private void Start()
	{
		_currentGridPos = _mapManager.WorldToGrid(transform.position);
		_mapManager.SetConnectingSource(_currentGridPos);
		_moveUI.Connections = _mapManager.GetRoomInPos(_mapManager.WorldToGrid(transform.position)).Connections;
	}

	private void OnEnable()
	{
		_eventBus.FogIsReady.EventRaised += _selfLight.UpdateLight;
		_eventBus.ForceMove.EventRaised += MoveToGridPos;
	}

	private void OnDisable()
	{
		_eventBus.FogIsReady.EventRaised -= _selfLight.UpdateLight;
		_eventBus.ForceMove.EventRaised -= MoveToGridPos;
	}

	private void Update()
	{
		Move();
	}
	
	[Button]
	private void RequestMove(Direction direction)
	{
		if (_isMoving) return;
		
		_moveUI.Toggle(false);
		
		Vector2Int gridPos = _mapManager.WorldToGrid(transform.position);

		switch (direction)
		{
			case Direction.Down:
				gridPos.y -= 1;
				break;
			case Direction.Left:
				gridPos.x -= 1;
				break;
			case Direction.Up:
				gridPos.y += 1;
				break;
			case Direction.Right:
				gridPos.x += 1;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
		}

		if (_mapManager.GetRoomInPos(gridPos) != null)
		{
			MoveToGridPos(gridPos);
			return;
		}
		
		_eventBus.MovedToDark.RaiseEvent(gridPos);
	}
	public void RequestMove(int direction) => RequestMove((Direction)direction); // Inspector fix

	private void MoveToGridPos(Vector2Int gridPos)
	{
		_targetPos = _mapManager.GridToWorld(gridPos);
		_direction = _targetPos - transform.position;
		_isMoving = true;
	}
	
	private void Move()
	{
		if (!_isMoving) return;
		
		float distanceToTarget = (_targetPos - transform.position).magnitude;
			
		if (distanceToTarget <= StopDistance)
		{
			_isMoving = false;
			_currentGridPos = _mapManager.WorldToGrid(transform.position);
			_mapManager.SetConnectingSource(_currentGridPos);
			_moveUI.Connections = _mapManager.GetRoomInPos(_currentGridPos).Connections;
			
			_selfLight.UpdateLight();
			foreach (LightSource lightSource in ExtraLight)
			{
				lightSource.UpdateLight();
			}
			
			return;
		}
		
		float halfWidth = _mapManager.Width / 2f;
		float halfHeight = _mapManager.Height / 2f;
		bool teleported = false;
		Vector3 teleportedPos = transform.position;
		if (transform.position.x > _mapManager.MapCenter.x + halfWidth)
		{ teleported = true; teleportedPos.x = _mapManager.MapCenter.x - halfWidth; }
		if (transform.position.x < _mapManager.MapCenter.x - halfWidth)
		{ teleported = true; teleportedPos.x = _mapManager.MapCenter.x + halfWidth; }
		if (transform.position.z > _mapManager.MapCenter.z + halfHeight)
		{ teleported = true; teleportedPos.z = _mapManager.MapCenter.z - halfHeight; }
		if (transform.position.z < _mapManager.MapCenter.z - halfHeight)
		{ teleported = true; teleportedPos.z = _mapManager.MapCenter.z + halfHeight; }

		if (teleported)
		{
			_controller.enabled = false;

			Vector3 deltaToTarget = _targetPos - transform.position;
			transform.position = teleportedPos + _direction * (Speed * Time.deltaTime);
			_targetPos = transform.position + deltaToTarget;
			
			_controller.enabled = true;
		}
		else
			_controller.Move(_direction * (distanceToTarget <= Speed ? StopDistance * Time.deltaTime : Speed * Time.deltaTime));
	}

	private void TeleportToGridPos(Vector2Int pos)
	{
		_controller.enabled = false;
		transform.position = _mapManager.GridToWorld(pos);
		_controller.enabled = true;
	}
	[Button] public void TeleportToGridPos(int x, int y) => TeleportToGridPos(new(x, y));
}