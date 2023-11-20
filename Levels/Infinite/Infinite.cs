using Godot;

public partial class Infinite : Node2D
{
	private InfiniteTilemap _infiniteTilemap;
	private CBTam _cbTam;
	
	public override void _Ready()
	{
		_infiniteTilemap = GetNode<InfiniteTilemap>("TileMap");
		_cbTam = GetNode<CBTam>("CBTam");
		var tamCam = GetNode<Camera2D>("CBTam/Camera2D");
		tamCam?.MakeCurrent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_cbTam == null)
		{
			return;
		}

		_infiniteTilemap?.GenerateChunk(_cbTam.Position);
	}
}
