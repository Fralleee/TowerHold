using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineMeshGenerator))]
public class SplineMeshGeneratorEditor : Editor
{
	SplineMeshGenerator _splineMeshGenerator;

	void OnEnable()
	{
		_splineMeshGenerator = (SplineMeshGenerator)target;

		Undo.undoRedoPerformed += OnUndoRedo;
	}

	void OnDisable()
	{
		Undo.undoRedoPerformed -= OnUndoRedo;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		serializedObject.Update();
		serializedObject.ApplyModifiedProperties();

		if (GUILayout.Button("Update Mesh"))
		{
			_splineMeshGenerator.UpdateMesh();
		}
	}

	void OnSceneGUI()
	{
		var basePosition = _splineMeshGenerator.transform.position;
		for (var i = 0; i < _splineMeshGenerator.SplinePoints.Count; i++)
		{
			// Handle for position
			EditorGUI.BeginChangeCheck();
			var newPos = Handles.PositionHandle(_splineMeshGenerator.SplinePoints[i].Position + basePosition, Quaternion.identity);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_splineMeshGenerator, "Move Path Point");
				_splineMeshGenerator.SplinePoints[i].Position = newPos - basePosition;
				_splineMeshGenerator.UpdateMesh();
			}

			// Handle for width
			EditorGUI.BeginChangeCheck();
			var newWidth = Handles.ScaleSlider(_splineMeshGenerator.SplinePoints[i].Width, _splineMeshGenerator.SplinePoints[i].Position + basePosition, Vector3.up, Quaternion.identity, HandleUtility.GetHandleSize(_splineMeshGenerator.SplinePoints[i].Position), 0.1f);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_splineMeshGenerator, "Adjust Width");
				_splineMeshGenerator.SplinePoints[i].Width = newWidth;
				_splineMeshGenerator.UpdateMesh();
			}
		}
		SceneView.RepaintAll();
	}

	void OnUndoRedo()
	{
		if (_splineMeshGenerator != null)
		{
			_splineMeshGenerator.UpdateMesh();
		}
	}
}
