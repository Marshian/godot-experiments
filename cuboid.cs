using Godot;
using System.Collections.Generic;

public partial class cuboid : MeshInstance3D
{
	private static List<Vector3> _makeCuboidVerts(float w, float h, float l)
	{
		var centerOffset = new Vector3(w / 2.0f, h / 2.0f, -l / 2.0f);
		var vertices = new List<Vector3>
		{
			(new Vector3(0, 0, 0)) - centerOffset,
			(new Vector3(w, 0, 0)) - centerOffset,
			(new Vector3(w, 0, -l)) - centerOffset,
			(new Vector3(0, 0, -l)) - centerOffset,
			(new Vector3(0, h, 0)) - centerOffset,
			(new Vector3(w, h, 0)) - centerOffset,
			(new Vector3(w, h, -l)) - centerOffset,
			(new Vector3(0, h, -l)) - centerOffset
		};

		return vertices;
	}

	private static List<Vector2> _makeCuboidUvs()
	{
		return new List<Vector2>
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(1, 1),
		};
	}

	private static List<int> _makeCuboidIndices()
	{
		return new List<int>
		{
			0, 1, 2,
			0, 2, 3,

			0, 4, 5,
			1, 0, 5,

			3, 7, 4,
			0, 3, 4,

			2, 6, 7,
			3, 2, 7,

			1, 5, 6,
			2, 1, 6,

			7, 6, 5,
			7, 5, 4
		};
	}

	private static List<Vector3> _makeCuboidNormals(float w, float h, float l, List<Vector3> vertices)
	{
		var centerOffset = new Vector3(w / 2.0f, l / 2.0f, -h / 2.0f);
/*
		var normals = new List<Vector3>
		{
			new Vector3(-1, -1, -1),
			new Vector3(1, -1, -1),
			new Vector3(1, -1, 1),
			new Vector3(-1, -1, 1),
			new Vector3(-1, 1, -1),
			new Vector3(1, 1, -1),
			new Vector3(1, 1, 1),
			new Vector3(-1, 1, 1)
		};
*/

		var normals = new List<Vector3>
		{
			(vertices[0]).Normalized(),
			(vertices[1]).Normalized(),
			(vertices[2]).Normalized(),
			(vertices[3]).Normalized(),
			(vertices[4]).Normalized(),
			(vertices[5]).Normalized(),
			(vertices[6]).Normalized(),
			(vertices[7]).Normalized(),
		};

		return normals;
	}

	private static Godot.Collections.Array _makeCuboidArray(float w, float h, float l)
	{
		var surfaceArray = new Godot.Collections.Array();
		surfaceArray.Resize((int)Mesh.ArrayType.Max);
		var vertices = _makeCuboidVerts(w, h, l);
		var uvs = _makeCuboidUvs();
		var normals = _makeCuboidNormals(w, h, l, vertices);
		var indices = _makeCuboidIndices();

		surfaceArray[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

		return surfaceArray;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var arrayMesh = Mesh as ArrayMesh;
		var surfaceArray = _makeCuboidArray(1, 0.5f, 0.25f);
		arrayMesh?.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotateZ((float)(delta));
	}
}
