using Godot;

namespace Experiments.UI;

public partial class ExperimentsMenu : CanvasLayer
{
	private bool _paused;
	public bool IsPaused
	{
		get => _paused;
		set { 
			_paused = value;
			GetTree().Paused = value;
			Visible = _paused;
		}
	}
	
	
	//
	//  Signals
	//
	private void OnResumeButtonDown()
	{
		IsPaused = false;
	}

	private void OnExitButtonDown()
	{
		GetTree().Quit();
	}

	private void OnMartinButtonDown()
	{
		EmitSignal(SignalName.SceneChange, "res://Levels/Martin.tscn");
		IsPaused = false;
	}

	private void OnWailingButtonDown()
	{
		EmitSignal(SignalName.SceneChange, "res://Levels/wailing.tscn");
		IsPaused = false;
	}

	private void OnRandomDungeonButtonDown()
	{
		EmitSignal(SignalName.SceneChange, "res://Levels/random_dungeon_3d.tscn");
		IsPaused = false;
	}

	private void OnMeshButtonDown()
	{
		EmitSignal(SignalName.SceneChange, "res://Levels/mesh_gen.tscn");
		IsPaused = false;
	}

	private void OnInfiniteTileButtonDown()
	{
		EmitSignal(SignalName.SceneChange, "res://Levels/Infinite/Infinite.tscn");
		IsPaused = false;
	}

	[Signal]
	public delegate void SceneChangeEventHandler(string targetScene);

	//
	// Overrides
	//
	public override void _Ready()
	{
		IsPaused = true;
	}
	
	public override void _Process(double delta)
	{
	}

	public override void _UnhandledInput(InputEvent e)
	{
		if (e.IsActionPressed("menu"))
		{
			IsPaused = !_paused;
		}
	}
}