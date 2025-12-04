using System;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class Player : MonoBehaviour
{
	public float Speed;
	public float StopDistance;
	public MapManager Map;

	public Button MoveDown;
	public Button MoveLeft;
	public Button MoveUp;
	public Button MoveRight;

	private bool _isMoving;
	private Vector3 _direction;
	private Vector3 _targetPos;
	private CharacterController _controller;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		UpdateUI();
	}

	private void Update()
	{
		Move();
	}
	
	[Button]
	private void MoveGrid(Direction direction)
	{
		if (_isMoving) return;
		
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
		
		_targetPos = Map.GridToWorld(gridPos);
		_direction = _targetPos - transform.position;
		_isMoving = true;
	}
	public void MoveGrid(int direction) => MoveGrid((Direction)direction); // Inspector fix

	private void Move()
	{
		if (!_isMoving) return;
		
		float distance = (_targetPos - transform.position).magnitude;
			
		if (distance <= StopDistance)
		{
			_isMoving = false;
			UpdateUI();
			return;
		}

		_controller.Move(_direction * (distance <= Speed ? StopDistance * Time.deltaTime : Speed * Time.deltaTime));
	}

	// TODO: move to UI script on EventBus
	private void UpdateUI()
	{
		List<Direction> connections = Map.GetRoomInPos(Map.WorldToGrid(transform.position))?.Connections ??
			new() { Direction.Down, Direction.Left, Direction.Up, Direction.Right };
		
		MoveDown.gameObject.SetActive(connections.Contains(Direction.Down));
		MoveLeft.gameObject.SetActive(connections.Contains(Direction.Left));
		MoveUp.gameObject.SetActive(connections.Contains(Direction.Up));
		MoveRight.gameObject.SetActive(connections.Contains(Direction.Right));
	}
}