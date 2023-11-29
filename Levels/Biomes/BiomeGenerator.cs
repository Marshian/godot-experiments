using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BiomeGenerator : Node2D
{
	public int NumberOfPoints { get; set; }
	private List<Vector2> _points;
	private int _paddingX = 20;
	private int _paddingY = 10;
	private double _minPointDistance = 50;
	private RandomNumberGenerator _rng;
	private List<Vector2> _centroids;

	private float MapToWindow(float value, int size, int padding)
	{
		if (2 * padding > size)
		{
			padding = 0;
		} 
		
		var range = size - 2 * padding;
		if (range < 1)
		{
			range = 1;
		}

		return (value * (range)) + padding;
	}

	private bool IsPointTooClose(Vector2 p)
	{
		return _points
			.Select(p.DistanceTo)
			.Any(d => d < _minPointDistance);
	}
	
	public void GeneratePoints()
	{
		var children = GetChildren();
		foreach (var child in children)
		{
			child.QueueFree();
		}

		var viewportSize = GetViewport().GetVisibleRect().Size;
		
		_points = new List<Vector2>();
		var step = 0;
		var maxSteps = 3000;

		while (_points.Count < 30 && step < maxSteps)
		{
			step++;
			var x = MapToWindow(_rng.Randf(), (int)viewportSize.X, _paddingX);
			var y = MapToWindow(_rng.Randf(), (int)viewportSize.Y, _paddingY);

			var newPoint = new Vector2(x, y);

			
			if (!IsPointTooClose(newPoint))
			{
				_points.Add(new Vector2(x, y));	
			}
		}

		GenerateDelaunay();
		DrawPoints();
		DrawCentroids();
	}

	private Vector2 GetCentroid(Vector2 a, Vector2 b, Vector2 c)
	{
		var x = (a.X + b.X + c.X) / 3.0f;
		var y = (a.Y + b.Y + c.Y) / 3.0f;

		return new Vector2(x, y);
	}

	public void GenerateDelaunay()
	{
		var delauney = Geometry2D.TriangulateDelaunay(_points.ToArray());
		_centroids = new List<Vector2>();
		
		for (var i = 0; i < delauney.Length; i += 3)
		{
			var v1 = _points[delauney[i]];
			var v2 = _points[delauney[i + 1]];
			var v3 = _points[delauney[i + 2]];
			
			var p = new Polygon2D();
			p.Color = new Color(_rng.Randf(), _rng.Randf(), _rng.Randf());
			p.Polygon = new Vector2[] { v1, v2, v3 };
			_centroids.Add(GetCentroid(v1, v2, v3));
			AddChild(p);
		}
	}
	
	private void DrawPoints()
	{
		var color = new Color(1, 1, 1);
		foreach (var point in _points)
		{
			DrawPoint(point, color);
		}
	}

	private void DrawCentroids()
	{
		var color = new Color(0.3f, 0.3f, 0.6f);
		foreach (var center in _centroids)
		{
			DrawPoint(center, color);
		}
	}

	private void DrawPoint(Vector2 p, Color c)
	{
		var poly = new Polygon2D();
		poly.Position = p;
		poly.Polygon = new Vector2[]
		{
			new Vector2(-2, -2),
			new Vector2(-2, 2),
			new Vector2(2, 2),
			new Vector2(2, -2)
		};
		poly.Color = c;
		AddChild(poly);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rng = new RandomNumberGenerator();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
