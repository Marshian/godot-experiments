using Godot;
using System;

public partial class BiomeMap : Node2D
{
	private BiomeGenerator _generator;
	
	private void OnGenerateClicked()
	{
		_generator.GeneratePoints();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_generator = GetNode<BiomeGenerator>("BiomeGenerator");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
