using System.Collections.Generic;
using Godot;

namespace Experiments.Levels.Biomes;

public partial class VoronoiView : Node2D
{
    public uint RandomSeed { get; set; }

    public void DisplayPoints(Vector2 offset, Vector2[] points)
    {
        DisplayPoints(offset, points, new Color(1, 1, 1, 1));
    }

    public void DisplayPoints(Vector2 offset, Vector2[] points, Color color)
    {
        foreach (var point in points)
        {
            var newPointPoly = new Polygon2D();
            newPointPoly.Position = point + offset;
            newPointPoly.Polygon = new Vector2[]
            {
                new Vector2(-2, -2),
                new Vector2(-2, 2),
                new Vector2(2, 2),
                new Vector2(2, -2)
            };
            newPointPoly.Color = color;
            AddChild(newPointPoly);
        }
    }
    
    public void DisplayPolygon(Vector2 offset, Vector2[] polygon)
    {
        var poly = new Polygon2D();
        var points = new List<Vector2>();
        foreach (var point in polygon)
        {
            points.Add(point + offset);
        }

        poly.Polygon = points.ToArray();
        var rng = new RandomNumberGenerator();
        rng.Seed = RandomSeed;
        poly.Color = new Color(rng.Randf(), rng.Randf(), rng.Randf(), 1);
        RandomSeed =  rng.Randi();
        
        AddChild(poly);
    }

    public override void _Ready()
    {
        base._Ready();
    }
}