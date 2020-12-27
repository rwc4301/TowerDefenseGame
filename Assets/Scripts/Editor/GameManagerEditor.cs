using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
	//ReorderableList list;

	//public void OnEnable()
	//{
	//	list = new ReorderableList(serializedObject, serializedObject.FindProperty("cards").FindPropertyRelative("keys"));
	//	list.drawElementCallback = DrawElement;
	//	list.onAddCallback = OnAdd;

	//	void OnAdd(ReorderableList list)
	//	{
	//		serializedObject.FindProperty("cards").FindPropertyRelative("keys").arraySize++;
	//		serializedObject.FindProperty("cards").FindPropertyRelative("values").arraySize++;
	//		Debug.Log(serializedObject.FindProperty("cards").FindPropertyRelative("keys").arraySize);
	//	}

	//	void DrawElement(Rect rect, int index, bool active, bool focused)
	//	{
	//		rect.yMin += (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
	//		rect.height = EditorGUIUtility.singleLineHeight;
	//		rect.xMax -= 100;

	//		EditorGUI.PropertyField(rect, serializedObject.FindProperty("cards").FindPropertyRelative("keys").GetArrayElementAtIndex(index));

	//		rect.xMin = rect.xMax + 5;
	//		rect.xMax += 100;

	//		EditorGUI.PropertyField(rect, serializedObject.FindProperty("cards").FindPropertyRelative("values").GetArrayElementAtIndex(index));
	//	}
	//}

	public override void OnInspectorGUI()
	{
		GUILayout.Label("UI Panels", EditorStyles.boldLabel);
		DrawPropertiesExcluding(serializedObject, "m_Script");

		serializedObject.ApplyModifiedProperties();
	}
}