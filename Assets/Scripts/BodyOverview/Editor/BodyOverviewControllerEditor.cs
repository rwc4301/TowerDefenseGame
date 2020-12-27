using BodyOverview;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(BodyOverviewController))]
public class BodyOverviewControllerEditor : Editor
{
	ReorderableList wBoundsList;
	List<BoxBoundsHandle> handles;

	private void OnEnable()
	{
		wBoundsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_woundBounds"));
		wBoundsList.elementHeight *= 3;
		wBoundsList.drawHeaderCallback = (rect) => { GUI.Label(rect, "Wounds Bounding Boxes"); };
		wBoundsList.drawElementCallback = (rect, index, active, focused) =>
		{
			if (handles.Count <= index)
				handles.Add(new BoxBoundsHandle {
					center = wBoundsList.serializedProperty.GetArrayElementAtIndex(index).boundsValue.center,
					size = wBoundsList.serializedProperty.GetArrayElementAtIndex(index).boundsValue.size,
					axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y
				});
			
			EditorGUI.PropertyField(rect, wBoundsList.serializedProperty.GetArrayElementAtIndex(index));

			handles[index].wireframeColor = handles[index].handleColor = active ? Color.yellow : Color.white;
			handles[index].center = wBoundsList.serializedProperty.GetArrayElementAtIndex(index).boundsValue.center;
			handles[index].size = wBoundsList.serializedProperty.GetArrayElementAtIndex(index).boundsValue.size;
		};

		handles = new List<BoxBoundsHandle>(wBoundsList.serializedProperty.arraySize);
	}

	public override bool RequiresConstantRepaint()
	{
		return true;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		wBoundsList.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI()
	{
		for (int i = 0; i < handles.Count; i++) {
			SerializedProperty p = wBoundsList.serializedProperty.GetArrayElementAtIndex(i);
			handles[i].DrawHandle();

			p.boundsValue = new Bounds(handles[i].center, handles[i].size);
		}

		serializedObject.ApplyModifiedProperties();

	}
}
