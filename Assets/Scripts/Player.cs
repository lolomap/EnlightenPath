using System;
using EditorAttributes;
using Events;
using UnityEngine;
using Utilities;
using Zenject;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(LightSource))]
public class Player : MonoBehaviour
{
	public float Speed;
	public float StopDistance;

	private bool _isMoving;
	private Vector3 _direction;
	private Vector3 _targetPos;
	private Vector2Int _currentGridPos;
	private CharacterController _controller;

	[Inject] private EventBus _eventBus;
	[Inject] private MapManager _mapManager;
	[Inject] private LightManager _lightManager;
	[Inject] private GrandCandle _grandCandle;
	
	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_lightManager.RegisterLightSource(GetComponent<LightSource>());
	}
	
	private void Start()
	{
		transform.position = _mapManager.StartPos;
		_currentGridPos = _mapManager.WorldToGrid(transform.position);
		_mapManager.ConnectingSourceGridPos = _currentGridPos;
	}

	private void OnEnable()
	{
		_eventBus.ForceMove.EventRaised += MoveToGridPos;
	}

	private void OnDisable()
	{
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
		
		_eventBus.ToggleMovementUI.RaiseEvent(false);

		if (_mapManager.GetRoomInPos(gridPos) != null)
		{
			MoveToGridPos(gridPos);
			return;
		}
		
		MoveToDark(gridPos);
	}
	public void RequestMove(int direction) => RequestMove((Direction)direction); // Inspector fix

	private void MoveToDark(Vector2Int gridPos)
	{
		RoomSO nextRoom = _grandCandle.Pick(1, true)[0];

		while (
			!_mapManager.IsValidPlace(gridPos, nextRoom.Connections, _mapManager.ConnectingSourceGridPos) &&
			nextRoom.Direction != Direction.Count
			)
			MapManager.RotateRoom(nextRoom, (Direction)((int)nextRoom.Direction + 1));
		
		_mapManager.PlaceRoom(nextRoom, gridPos);
		
		_eventBus.ForceMove.RaiseEvent(gridPos);
	}
	
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
			_mapManager.ConnectingSourceGridPos = _currentGridPos;
			_lightManager.UpdateAllSources();
			
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