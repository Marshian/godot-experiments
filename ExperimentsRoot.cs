using Experiments.UI;
using Godot;

namespace Experiments;

public partial class ExperimentsRoot : Node2D
{
	private ExperimentsMenu _experimentsMenu;
	private Node _levelTarget;
	private Node _currentLevel;

	public void OnSceneChange(string targetScene)
	{
		var nextScene = ((PackedScene)ResourceLoader.Load(targetScene)).Instantiate();
		_levelTarget.AddChild(nextScene);
		_currentLevel?.QueueFree();
		_currentLevel = nextScene;
	}
	
	public override void _Ready()
	{
		var menu = GetNode<ExperimentsMenu>("ExperimentsMenu");
		menu.SceneChange += OnSceneChange;
		_levelTarget = GetNode<Node>("LevelTarget");
	}

	public override void _Process(double delta)
	{
	}
}