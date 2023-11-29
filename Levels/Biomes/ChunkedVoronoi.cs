using Godot;
using System;
using System.Collections.Generic;
using Experiments.Levels.Biomes;

public partial class ChunkedVoronoi : Node
{
	public int RandomSeed { get; set; }
	public int ChunkWidth { get; set; } = 5;
	public int ChunkHeight { get; set; } = 5;
	public float MinPointDistance { get; set; } = 30f;
	public float MinVariationDistance { get; set; } = 0.3f;
	public float VoronoiTolerance { get; set; } = 0.3f;

	private uint GetRandomNumberFromCoordinates(Vector2 coordinates, long initialSeed)
	{
		var result = initialSeed;
		var generator = new RandomNumberGenerator();
		generator.Seed = (ulong)coordinates.X;
		result += generator.Randi();
		var yCoordinate = generator.Randi() + (int)coordinates.Y;
		generator.Seed = (ulong)yCoordinate;
		result += generator.Randi();
		generator.Seed = (ulong)result;
		return generator.Randi();
	}

	private Vector2[] GenerateChunkPoints(Vector2 coordinates)
	{
		return GenerateChunkPoints(coordinates, new Vector2(0, ChunkWidth), new Vector2(0, ChunkHeight));
	}

	private Vector2[] GenerateChunkPoints(Vector2 coordinates, Vector2 widthRange, Vector2 heightRange)
	{
		var localRandomSeed = GetRandomNumberFromCoordinates(coordinates, RandomSeed);
		var points = new List<Vector2>();
		for (var w = widthRange.X; w < widthRange.Y; w++)
		{
			for (var h = heightRange.X; h < heightRange.Y; h++)
			{
				var generator = new RandomNumberGenerator();
				generator.Seed = GetRandomNumberFromCoordinates(new Vector2(w, h), localRandomSeed);
				var newX = w * MinPointDistance + generator.RandfRange(-MinVariationDistance, MinVariationDistance) * MinPointDistance;
				var newY = h * MinPointDistance + generator.RandfRange(-MinVariationDistance, MinVariationDistance) * MinPointDistance;
				var newPoint = new Vector2(newX, newY);
				points.Add(newPoint);
			}
		}

		return points.ToArray();
	}

	private List<Tuple<Vector2, Vector2[]>> GenerateVoronoiChunk(Vector2 coordinates)
	{
		var initialPoints = GenerateChunkPoints(coordinates);
		var points = new List<Vector2>(initialPoints);

		for (var i = -1; i < 2; i++)
		{
			for(var j = -1; j < 2; j++)
			{
				if (i == 0 && j == 0) continue;
				
				var minX = 0.0f;
				var minY = 0.0f;
				var maxX = 1.0f;
				var maxY = 1.0f;
					
				if (i == -1)
				{
					minX = 1 - VoronoiTolerance;
				}

				if (i == 1)
				{
					maxX = VoronoiTolerance;
				}

				if (j == -1)
				{
					minY = 1 - VoronoiTolerance;
				}

				if (j == 1)
				{
					maxY = VoronoiTolerance;
				}

				var p1 = new Vector2(coordinates.X + i, coordinates.Y + j);
				var p2 = new Vector2(minX * ChunkWidth, maxX * ChunkWidth);
				var p3 = new Vector2(minY * ChunkHeight, maxY * ChunkHeight);
				var offset = new Vector2(i * ChunkWidth * MinPointDistance, j * ChunkHeight * MinPointDistance);
				var resultPoints = new Vector2[]
				{
					p1 + offset,
					p2 + offset,
					p3 + offset
				};

				points.AddRange(resultPoints);
			}
		}

		var delauney = Geometry2D.TriangulateDelaunay(points.ToArray());
		var triangleArray = new List<Vector2[]>();
		for (var t = 0; t < delauney.Length / 3; t++)
		{
			triangleArray.Add(new Vector2[] { points[delauney[t * 3]], points[delauney[t * 3 + 1]], points[delauney[t * 3 + 2]] });
		}

		var circumcenters = new List<Vector2>();
		for (var i = 0; i < triangleArray.Count; i++)
		{
			var triple = triangleArray[i];
			circumcenters.Add(GetCircumcenter(triple[0], triple[1], triple[2]));
		}

		var result = new List<Tuple<Vector2, Vector2[]>>();
		for (var i = 0; i < initialPoints.Length; i++)
		{
			var point = initialPoints[i];
			var verts = new List<Vector2>();
			for (var t = 0; t < triangleArray.Count; t++)
			{
				var triangle = triangleArray[t];
				if (point == triangle[0] || point == triangle[1] || point == triangle[2])
				{
					verts.Add(circumcenters[t]);
				}
			}
			
			var clockwise = ClockwisePoints(initialPoints[i], verts.ToArray());
			result.Add(new Tuple<Vector2, Vector2[]>(initialPoints[i], clockwise));
		}

		return result;
	}

	private Vector2[] ClockwisePoints(Vector2 center, Vector2[] surrounding)
	{
		var result = new List<Vector2>();
		var angles = new List<float>();
		var sortedIndexes = new List<int>();

		for (var i = 0; i < surrounding.Length; i++)
		{
			angles.Add(center.AngleToPoint(surrounding[i]));
		}

		var remainingIdx = new List<int>();
		for (var a = 0; a < angles.Count; a++)
		{
			remainingIdx.Add(a);
		}

		for (var a = 0; a < angles.Count; a++)
		{
			var currentMin = Mathf.Pi;
			var currentTestIdx = 0;
			for (var r = 0; r < remainingIdx.Count; r++)
			{
				if (angles[remainingIdx[r]] < currentMin)
				{
					currentTestIdx = r;
					currentMin = angles[remainingIdx[r]];
				}
			}

			sortedIndexes.Add(remainingIdx[currentTestIdx]);
			remainingIdx.Remove(currentTestIdx);
		}

		for (var i = 0; i < sortedIndexes.Count; i++)
		{
			result.Add(surrounding[i]);			
		}
		
		return result.ToArray();
	}

	private Vector2 GetCircumcenter(Vector2 a, Vector2 b, Vector2 c)
	{
		var result = new Vector2();
		
		var midPointAB = new Vector2((a.X + b.X) / 2, (a.Y + b.Y) / 2);
		var slopePerpAB = -((b.X - a.X) / (b.Y - a.Y));

		var midpointAC = new Vector2((a.X + c.X) / 2, (a.Y + c.Y) / 2);
		var slopePerpAC = -((c.X - a.X) / (c.Y - a.Y));

		var bOfPerpAB = midPointAB.Y - (midPointAB.X * slopePerpAB);
		var bOfPerpAC = midpointAC.Y - (midpointAC.X * slopePerpAB);
		result.X = (bOfPerpAB - bOfPerpAC) / (slopePerpAB - slopePerpAC);
		result.Y = slopePerpAB * result.X + bOfPerpAB;
		return result;
	}

	public void DisplayVoronoiFromChunk(Vector2 coord)
	{
		DisplayVoronoiFromChunk(coord, new Color(1,1,1,1));
	}

	public void DisplayVoronoiFromChunk(Vector2 coord, Color pointColor)
	{
		var view = GetChild<VoronoiView>(0);
		view.RandomSeed = GetRandomNumberFromCoordinates(coord, RandomSeed);
		var voronoi = GenerateVoronoiChunk(coord);
		for (var i = 0; i < voronoi.Count; i++)
		{
			view.DisplayPolygon(
				new Vector2(coord.X * ChunkWidth * MinPointDistance, coord.Y * ChunkHeight * MinPointDistance),
				voronoi[i].Item2);
			view.DisplayPoints(
				new Vector2(coord.X * ChunkWidth * MinPointDistance, coord.Y * ChunkHeight * MinPointDistance),
				GenerateChunkPoints(coord),
				pointColor);
		}
	}
	
	public override void _Ready()
	{
		for (var i = 0; i < 10; i++)
		{
			for (var j = 0; j < 10; j++)
			{
				if (i == 5 && j == 5)
				{
					continue;
				}

				DisplayVoronoiFromChunk(new Vector2(i, j), new Color(0, i * 0.1f, j*0.1f));
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
