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
	public MapManager Map;
	public List<LightSource> ExtraLight;

	private bool _isMoving;
	private Vector3 _direction;
	private Vector3 _targetPos;
	private CharacterController _controller;
	private MoveUI _moveUI;
	private LightSource _selfLight;
	
	public Vector2Int CurrentGridPos { get; private set; }

	[Inject] private EventBus _eventBus;
	
	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_moveUI = GetComponentInChildren<MoveUI>();
		_selfLight = GetComponent<LightSource>();
	}
	
	private void Start()
	{
		CurrentGridPos = Map.WorldToGrid(transform.position);
		_moveUI.Connections = Map.GetRoomInPos(Map.WorldToGrid(transform.position)).Connections;
	}

	private void OnEnable()
	{
		_eventBus.FogIsReady.EventRaised += _selfLight.UpdateLight;
	}

	private void OnDisable()
	{
		_eventBus.FogIsReady.EventRaised -= _selfLight.UpdateLight;
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
		
		Vector2Int gridPos = Map.WorldToGrid(transform.position);

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

		if (Map.GetRoomInPos(gridPos) != null)
		{
			MoveToGridPos(gridPos);
			return;
		}
		
		_eventBus.MovedToDark.RaiseEvent(gridPos);
	}
	public void RequestMove(int direction) => RequestMove((Direction)direction); // Inspector fix

	private void MoveToGridPos(Vector2Int gridPos)
	{
		_targetPos = Map.GridToWorld(gridPos);
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
			CurrentGridPos = Map.WorldToGrid(transform.position);
			_moveUI.Connections = Map.GetRoomInPos(CurrentGridPos).Connections;
			
			_selfLight.UpdateLight();
			foreach (LightSource lightSource in ExtraLight)
			{
				lightSource.UpdateLight();
			}
			
			return;
		}
		
		float halfWidth = Map.Width / 2f;
		float halfHeight = Map.Height / 2f;
		bool teleported = false;
		Vector3 teleportedPos = transform.position;
		if (transform.position.x > Map.MapCenter.x + halfWidth) { teleported = true; teleportedPos.x = Map.MapCenter.x - halfWidth; }
		if (transform.position.x < Map.MapCenter.x - halfWidth) { teleported = true; teleportedPos.x = Map.MapCenter.x + halfWidth; }
		if (transform.position.z > Map.MapCenter.z + halfHeight) { teleported = true; teleportedPos.z = Map.MapCenter.z - halfHeight; }
		if (transform.position.z < Map.MapCenter.z - halfHeight) { teleported = true; teleportedPos.z = Map.MapCenter.z + halfHeight; }

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
		transform.position = Map.GridToWorld(pos);
		_controller.enabled = true;
	}
	[Button] public void TeleportToGridPos(int x, int y) => TeleportToGridPos(new(x, y));
}