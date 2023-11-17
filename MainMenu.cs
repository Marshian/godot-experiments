using Godot;

public partial class MainMenu : Control
{
	private Button _exitButton;
	private Button _optionsButton;
	private Button _meshGenSceneButton;
	private Button _randomDungeonButton;
	private Button _martinButton;
	private Button _wailingButton;
	private Button _infiniteTileButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_meshGenSceneButton = GetNode<Button>("MarginContainer/HFlowContainer/MeshGen");
		if (_meshGenSceneButton != null)
		{
			_meshGenSceneButton.Pressed += () =>
			{
				GetTree().ChangeSceneToFile("Levels/mesh_gen.tscn");
			};
		}
		
		_martinButton = GetNode<Button>("MarginContainer/HFlowContainer/Martin");
		if (_martinButton != null)
		{
			_martinButton.Pressed += () =>
			{
				GetTree().ChangeSceneToFile("Levels/martin.tscn");
			};
		}

		_randomDungeonButton = GetNode<Button>("MarginContainer/HFlowContainer/RandomDungeon");
		if (_randomDungeonButton != null)
		{
			_randomDungeonButton.Pressed += () =>
			{
				GetTree().ChangeSceneToFile("Levels/random_dungeon_3d.tscn");
			};
		}
		
		_wailingButton = GetNode<Button>("MarginContainer/HFlowContainer/Wailing");
		if (_wailingButton != null)
		{
			_wailingButton.Pressed += () =>
			{
				GetTree().ChangeSceneToFile("Levels/Wailing.tscn");
			};
		}

		_infiniteTileButton = GetNode<Button>("MarginContainer/HFlowContainer/InfiniteTile");
		if (_infiniteTileButton != null)
		{
			_infiniteTileButton.Pressed += () =>
			{
				GetTree().ChangeSceneToFile("Levels/Infinite/Infinite.tscn");
			};
		}

		_exitButton = GetNode<Button>("MarginContainer/HFlowContainer/Exit");
		if (_exitButton != null)
		{
			_exitButton.Pressed += () => { GetTree().Quit(); };
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
