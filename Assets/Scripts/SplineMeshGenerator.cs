using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class SplineMeshGenerator : MonoBehaviour
{
	public enum UnwrapMode
	{
		XZWorldSpace = 0,
		River = 1
	}

	public List<SplinePoint> SplinePoints = new List<SplinePoint>();
	MeshFilter _meshFilter;

	MeshCollider _meshCollider;

	public int Subdivisions = 3;

	public float VertDistance = 0.2f;

	public int EndCapSubdivisions = 3;

	public float WidthWhobble = 0.5f;

	public float PositionWhobble = 0.5f;

	public UnwrapMode UvUnwrapMode;

	public float UvScale = 0.01f;

	public bool TopNormalsAlwaysFaceDirectlyUp;

	public Vector3 ExtrudeDownOffset;

	public bool Vertical;

	public bool FlipNormals;

	public void UpdateMesh()
	{
		if (SplinePoints.Count < 2)
		{
			return;
		}

		_meshFilter = GetComponent<MeshFilter>();
		_meshCollider = GetComponent<MeshCollider>();
		var mesh = new Mesh();
		var list = new List<Vector3>();
		var uvs = new List<Vector2>();
		var list2 = new List<int>();
		var list3 = new List<int>();
		var pathOriginal = new List<SplinePoint>();
		InterpolateModifier(SplinePoints, pathOriginal, Subdivisions);
		if (VertDistance > 0.1f)
		{
			ResampleModifier(ref pathOriginal, VertDistance, VertDistance / 10f);
		}
		AddRoundedEndsModifier(pathOriginal, EndCapSubdivisions);
		WhobbleModifier(pathOriginal, WidthWhobble, PositionWhobble);
		var num = 0f;
		for (var j = 0; j < pathOriginal.Count; j++)
		{
			if (pathOriginal.Count <= 1)
			{
				break;
			}
			var forwards = GetForwards(j, pathOriginal);
			var vector = Quaternion.Euler(0f, 90f, 0f) * forwards * pathOriginal[j].Width;
			var vector2 = Quaternion.Euler(0f, -90f, 0f) * forwards * pathOriginal[j].Width;
			if (Vertical)
			{
				vector = Vector3.up * pathOriginal[j].Width;
				vector2 = Vector3.down * pathOriginal[j].Width;
			}
			AddUnwrappedVertTop(pathOriginal[j].Position + vector, list, uvs, num, 0f - pathOriginal[j].Width);
			AddUnwrappedVertTop(pathOriginal[j].Position + vector2, list, uvs, num, pathOriginal[j].Width);
			if (j > 0)
			{
				num += (pathOriginal[j].Position - pathOriginal[j - 1].Position).magnitude;
			}
		}
		var count = list.Count;
		if (ExtrudeDownOffset.y != 0f)
		{
			num = 0f;
			for (var k = 0; k <= 1; k++)
			{
				var num2 = 0;
				for (var l = k; l < count - 2; l += 2)
				{
					AddUnwrappedVertTop(list[l], list, uvs, num, 0f);
					AddUnwrappedVertTop(list[l] + ExtrudeDownOffset, list, uvs, num, 0f - ExtrudeDownOffset.magnitude);
					num += (pathOriginal[num2 + 1].Position - pathOriginal[num2].Position).magnitude;
					num2++;
					AddUnwrappedVertTop(list[l + 2], list, uvs, num, 0f);
					AddUnwrappedVertTop(list[l + 2] + ExtrudeDownOffset, list, uvs, num, 0f - ExtrudeDownOffset.magnitude);
				}
			}
		}
		var normals = new Vector3[list.Count];
		for (var m = 0; m < pathOriginal.Count - 1; m++)
		{
			if (pathOriginal.Count <= 1)
			{
				break;
			}
			var num3 = m * 2;
			ConnectTriangle(num3 + 2, num3 + 1, num3, list2, normals, list, TopNormalsAlwaysFaceDirectlyUp, Vertical);
			ConnectTriangle(num3 + 1, num3 + 2, num3 + 3, list2, normals, list, TopNormalsAlwaysFaceDirectlyUp, Vertical);
		}
		if (ExtrudeDownOffset.y != 0f)
		{
			var num4 = count;
			for (var n = 0; n < count - 2; n += 2)
			{
				ConnectTriangle(num4, num4 + 1, num4 + 2, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
				ConnectTriangle(num4 + 3, num4 + 2, num4 + 1, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
				num4 += 4;
			}
			var num5 = num4;
			for (var num6 = 1; num6 < count - 2; num6 += 2)
			{
				ConnectTriangle(num4 + 2, num4 + 1, num4, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
				ConnectTriangle(num4 + 1, num4 + 2, num4 + 3, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
				num4 += 4;
			}
			ConnectTriangle(num5, count + 1, count, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
			ConnectTriangle(count + 1, num5, num5 + 1, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
			ConnectTriangle(num5 - 2, num5 - 1, num4 - 2, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
			ConnectTriangle(num4 - 1, num4 - 2, num5 - 1, list3, normals, list, makeNormalsFaceUp: false, showFacesFacedDownwards: true);
		}
		mesh.subMeshCount = (ExtrudeDownOffset.y == 0f) ? 1 : 2;
		mesh.SetVertices(list);
		mesh.SetTriangles(list2, 0);
		if (ExtrudeDownOffset.y != 0f)
		{
			mesh.SetTriangles(list3, 1);
		}
		mesh.SetUVs(0, uvs);
		mesh.SetNormals(normals);
		mesh.RecalculateBounds();
		if ((bool)_meshFilter)
		{
			_meshFilter.sharedMesh = mesh;
		}
		if ((bool)_meshCollider)
		{
			_meshCollider.sharedMesh = mesh;
		}
	}

	public void ConnectTriangle(int vertA, int vertB, int vertC, List<int> tris, Vector3[] normals, List<Vector3> verts, bool makeNormalsFaceUp, bool showFacesFacedDownwards)
	{
		if (FlipNormals)
		{
			var num = vertA;
			vertA = vertC;
			vertC = num;
		}
		var normalized = Vector3.Cross(verts[vertA] - verts[vertB], verts[vertC] - verts[vertB]).normalized;
		if (normalized.y > 0f || showFacesFacedDownwards)
		{
			tris.Add(vertC);
			tris.Add(vertB);
			tris.Add(vertA);
			if (makeNormalsFaceUp)
			{
				normals[vertC] = Vector3.up;
				normals[vertB] = Vector3.up;
				normals[vertA] = Vector3.up;
			}
			else
			{
				normals[vertC] = normalized;
				normals[vertB] = normalized;
				normals[vertA] = normalized;
			}
		}
	}

	public void AddUnwrappedVertTop(Vector3 pos, List<Vector3> verts, List<Vector2> uvs, float distanceTraveled, float width)
	{
		verts.Add(pos);
		if (UvUnwrapMode == UnwrapMode.XZWorldSpace)
		{
			pos = transform.localToWorldMatrix.MultiplyPoint(pos);
			uvs.Add(new Vector2(pos.x * UvScale, pos.z * UvScale));
		}
		else if (UvUnwrapMode == UnwrapMode.River)
		{
			uvs.Add(new Vector2(distanceTraveled * UvScale, width * UvScale));
		}
	}

	public void InterpolateModifier(List<SplinePoint> pathIn, List<SplinePoint> pathOut, int subdivisions)
	{
		for (var i = 0; i < pathIn.Count - 1; i++)
		{
			for (var j = 0; j < subdivisions; j++)
			{
				var num = j / (float)subdivisions;
				var magnitude = (pathIn[i + 1].Position - pathIn[i].Position).magnitude;
				var forwards = GetForwards(i, pathIn);
				var forwards2 = GetForwards(i + 1, pathIn);
				var a = pathIn[i].Position + (magnitude * num * forwards);
				var b = pathIn[i + 1].Position - ((1f - num) * magnitude * forwards2);
				var position = Vector3.Lerp(a, b, Mathf.SmoothStep(0f, 1f, num));
				var width = Mathf.SmoothStep(pathIn[i].Width, pathIn[i + 1].Width, num);
				var item = new SplinePoint(position, width);
				pathOut.Add(item);
			}
		}
		pathOut.Add(pathIn[^1]);
	}

	public void ResampleModifier(ref List<SplinePoint> pathOriginal, float maxDistance, float stepSize = 0.1f)
	{
		if (pathOriginal.Count < 2)
		{
			return;
		}
		var list = new List<SplinePoint>
		{
			pathOriginal[0]
		};
		var num = 0f;
		for (var i = 0; i < pathOriginal.Count - 1; i++)
		{
			var num2 = (int)Mathf.Ceil((pathOriginal[i].Position - pathOriginal[i + 1].Position).magnitude / stepSize);
			for (var j = 0; j < num2; j++)
			{
				num += stepSize;
				if (num >= maxDistance)
				{
					var t = j / (float)num2;
					var position = Vector3.Lerp(pathOriginal[i].Position, pathOriginal[i + 1].Position, t);
					var width = Mathf.Lerp(pathOriginal[i].Width, pathOriginal[i + 1].Width, t);
					num = 0f;
					list.Add(new SplinePoint(position, width));
				}
			}
		}
		list.Add(pathOriginal[^1]);
		pathOriginal = list;
	}

	public void WhobbleModifier(List<SplinePoint> path, float widthAmount, float posAmount)
	{
		UnityEngine.Random.InitState(path.Count * 7);
		for (var i = 0; i < path.Count; i++)
		{
			path[i].Position += new Vector3(UnityEngine.Random.value - 0.5f, 0f, UnityEngine.Random.value - 0.5f) * posAmount;
			path[i].Width *= 1f + ((UnityEngine.Random.value - 0.5f) * widthAmount);
		}
	}

	public void AddRoundedEndsModifier(List<SplinePoint> pathModify, int interpolations)
	{
		var position = pathModify[0].Position;
		var vector = -GetForwards(0, pathModify);
		var width = pathModify[0].Width;
		var position2 = pathModify[^1].Position;
		var forwards = GetForwards(pathModify.Count - 1, pathModify);
		var width2 = pathModify[^1].Width;
		for (var i = 1; i < interpolations; i++)
		{
			var f = i / (float)interpolations;
			f = Mathf.Pow(f, 0.5f);
			var width3 = width * (1f - Mathf.Pow(f, 3f));
			var num = width * f;
			pathModify.Insert(0, new SplinePoint(position + (num * vector), width3));
		}
		for (var j = 1; j < interpolations; j++)
		{
			var f2 = j / (float)interpolations;
			f2 = Mathf.Pow(f2, 0.5f);
			var width4 = width2 * (1f - Mathf.Pow(f2, 3f));
			var num2 = width2 * f2;
			pathModify.Add(new SplinePoint(position2 + (num2 * forwards), width4));
		}
	}

	Vector3 GetForwards(int i, List<SplinePoint> path, bool xzPlaneOnly = true)
	{
		var result = (i == 0) ? (path[i + 1].Position - path[i].Position).normalized : ((i != path.Count - 1) ? (path[i + 1].Position - path[i - 1].Position).normalized : (path[i].Position - path[i - 1].Position).normalized);
		if (xzPlaneOnly)
		{
			result = new Vector3(result.x, 0f, result.z).normalized;
		}
		return result;
	}

	public void Nullify()
	{
		var list = new List<Vector3>();
		for (var i = 0; i < transform.childCount; i++)
		{
			var child = transform.GetChild(i);
			list.Add(child.transform.position);
		}
		transform.position = Vector3.zero;
		for (var j = 0; j < transform.childCount; j++)
		{
			transform.GetChild(j).transform.position = list[j];
		}
	}
}
