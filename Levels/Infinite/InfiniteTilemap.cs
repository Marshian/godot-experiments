using Godot;

public partial class InfiniteTilemap : TileMap
{
	private FastNoiseLite moisture;
	private FastNoiseLite temperature;
	private FastNoiseLite altitude;

	private const int width = 64;
	private const int height = 48;
	
	public override void _Ready()
	{
		moisture = new FastNoiseLite()
		{
			Seed = (int)Time.GetUnixTimeFromSystem()
		};

		temperature = new FastNoiseLite()
		{
			Seed = (int)(moisture.GetNoise1D(moisture.Seed) * 10000.0f)
		};
		
		altitude = new FastNoiseLite()
		{
			Seed = (int)(temperature.GetNoise1D(moisture.Seed) * 10000.0f)
		};
	}

	public override void _Process(double delta)
	{
	}

	public void GenerateChunk(Vector2 position)
	{
		var tilePosition = LocalToMap(position);

		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				var cellPos = new Vector2I(tilePosition.X - (width / 2) + x, tilePosition.Y - (height / 2) + y);
				
				// GetNoise2D returns values from -1 to 1, remap to integers from 0 - 4 by adding 1 (0 - 2) and multiplying by 2 (0 - 4)
				var moist = (int)((moisture.GetNoise2D(cellPos.X, cellPos.Y) + 1) * 2);
				var temp = (int)((moisture.GetNoise2D(cellPos.X, cellPos.Y) + 1) * 2);
				var alt = (int)((moisture.GetNoise2D(cellPos.X, cellPos.Y) + 1) * 2);

				var terrainTile = GetTerrainTile(moist, temp, alt);
				
				SetCell(0, cellPos, 0, terrainTile);
			}
		}
	}

	private Vector2I GetTerrainTile(int moist, int temp, int alt)
	{
		if (alt < 2)
		{
			return new Vector2I(3, temp);
		}
		else
		{
			if (moist > 2)
			{
				moist = 2;
			}
		}
		
		return new Vector2I(moist, temp);
	}
}
