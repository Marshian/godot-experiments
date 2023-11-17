using Godot;
using System;
using System.Security.Cryptography;
using RandomNumberGenerator = Godot.RandomNumberGenerator;

public partial class random_dungeon_3d : Node3D
{
	private Timer _timer;
	private StaticBody3D _staticBody;
	private MeshInstance3D _meshInstance;
	private RandomNumberGenerator _rng;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rng = new Godot.RandomNumberGenerator();
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnTimerTimeout;
		GenerateMesh(1, 2, 3);
	}

	private void OnTimerTimeout()
	{
		var l = _rng.RandfRange(1, 10);
		var w = _rng.RandfRange(1, 10);
		var h = _rng.RandfRange(1, 10);
		
		GenerateMesh(l, w, h);
		_staticBody.Position = new Vector3(0, 0, -1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_staticBody.Rotation += new Vector3((float)delta, 0, 0);
	}

	private void GenerateMesh(float l, float w, float h)
	{
		if (IsInstanceValid(_staticBody) && _staticBody != null)
		{
			_staticBody.QueueFree();
			_staticBody = null;
		}
		
		_staticBody = new StaticBody3D();
		_meshInstance = new MeshInstance3D();
		_meshInstance.Mesh = new BoxMesh();	
		_meshInstance.Scale = new Vector3(l, w, h);
		_staticBody.AddChild(_meshInstance);
		AddChild(_staticBody);
	}
}
