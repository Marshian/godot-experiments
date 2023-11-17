using Godot;
using System;

public partial class p_mona : CharacterBody3D
{
	private Node3D _armature; 
	private SpringArm3D _cameraSpring;
	private Node3D _cameraPivot;
	private Camera3D _camera;
	private AnimationTree _animationTree;
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float LerpValue = 0.15f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_armature = GetNode<Node3D>("Armature");
		_cameraPivot = GetNode<Node3D>("CameraPivot");
		_cameraSpring = GetNode<SpringArm3D>("CameraPivot/CameraSpring");
		_camera = GetNode<Camera3D>("CameraPivot/CameraSpring/Camera3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		var inputDir = Input.GetVector("left", "right", "forward", "back");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		direction = direction.Rotated(Vector3.Up, _cameraPivot.Rotation.Y);
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, LerpValue);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * Speed, LerpValue);
			var xz = MathF.Atan2(velocity.X, velocity.Z);
			var rotationLerp = Mathf.LerpAngle(_armature.Rotation.Y, xz, LerpValue);
			_armature.RotateY(rotationLerp - _armature.Rotation.Y);
		}
		else
		{
			
			velocity.X = Mathf.Lerp(velocity.X, 0f, LerpValue);
			velocity.Z = Mathf.Lerp(velocity.Z, 0f, LerpValue);
		}
		
		Velocity = velocity;
		MoveAndSlide();

		_animationTree.Set("parameters/BlendSpace1D/blend_position", Velocity.Length() / Speed);
	}

	public override void _UnhandledInput(InputEvent e)
	{
		if (Input.IsActionJustPressed("menu"))
		{
			GetTree().ChangeSceneToFile("MainMenu.tscn");
		}

		if (e is InputEventMouseMotion mouseMotionEvent)
		{
			_cameraPivot.RotateY(-mouseMotionEvent.Relative.X * 0.05f);
			var xRotation = Mathf.Clamp(mouseMotionEvent.Relative.Y * 0.05f, -MathF.PI / 4.0f, MathF.PI / 4.0f);
			_cameraSpring.RotateX(xRotation);
		}
		
		base._UnhandledInput(e);
	}
}
