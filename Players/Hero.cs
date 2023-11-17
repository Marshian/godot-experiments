using Godot;
using System;

public partial class Hero : CharacterBody2D
{
	private AnimationPlayer _animationPlayer;
	private AnimationTree _animationTree;
	private AnimationNodeStateMachinePlayback _animationState;

	public const float Friction = 500f;
	public const float Acceleration = 240.0f;
	public const float MaxSpeed = 80f;
	

	public Hero() { }

	public override void _Ready()
	{
		base._Ready();
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		var direction = Input.GetVector("left", "right", "up", "down");
		direction = direction.Normalized();
		
		if (direction != Vector2.Zero)
		{
			_animationTree.Set("parameters/Idle/blend_position", direction);
			_animationTree.Set("parameters/Run/blend_position", direction);
			_animationState.Travel("Run");
			velocity = Velocity + direction * Acceleration * (float)delta;
			if (velocity.Length() > MaxSpeed)
			{
				velocity = velocity.Normalized() * MaxSpeed;
			}
		}
		else 
		{
			_animationState.Travel("Idle");
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Friction * (float)delta);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Friction * (float)delta);
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
