using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableInterfaceList<ICard>), true)]
public class SerializableListPropertyDrawer : PropertyDrawer
{
	ReorderableList list;

	void Initialize(SerializedProperty property, GUIContent label)
	{
		list = new ReorderableList(property.serializedObject, property.FindPropertyRelative("values"), true, true, true, true);
		list.drawElementCallback = DrawElement;
		list.drawHeaderCallback = DrawHeader;

		void DrawElement(Rect rect, int index, bool active, bool focused)
		{
			rect.yMin += 2;
			rect.yMax -= 3;

			EditorGUI.PropertyField(rect, list.serializedProperty.GetArrayElementAtIndex(index));
		}

		void DrawHeader(Rect rect)
		{
			GUI.Label(rect, label);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return list?.GetHeight() ?? 0;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (list == null)
			Initialize(property, label);

		list.DoList(position);
	}
}