using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Inventory
{
	public class InventoryInspector : Editor
	{
		private SerializedProperty _equipment;
		private SerializedProperty _maxweight;
		private SerializedProperty _money;

		private ReorderableList _equipmentList;
		private ReorderableList _itemList;

		private static GUIStyle _headerBackground;
		private static GUIStyle _elementBackground;

		public void OnEnable()
		{
			//_equipment = serializedObject.FindProperty("_serializedEquipment");
			//_maxweight = serializedObject.FindProperty("WeightLimit");
			//_money = serializedObject.FindProperty("Money");

			//if (_equipment.arraySize != InventoryUtility.EQUIPMENT_SLOT_COUNT)
			//	_equipment.arraySize = InventoryUtility.EQUIPMENT_SLOT_COUNT;
			//serializedObject.ApplyModifiedPropertiesWithoutUndo();


			//_itemList = new ReorderableList(serializedObject, serializedObject.FindProperty("_serializedItems"), true, true, true, true);
			//_itemList.drawHeaderCallback = (rect) => {
			//	Rect rr = new Rect(rect.xMin, rect.yMin, rect.xMin + 120, rect.height);
			//	Rect rc = new Rect(rect.xMin + 105, rect.yMin + 1, rect.xMin + 120, rect.height - 1);

			//	serializedObject.FindProperty("UseReferenceItemList").boolValue = !EditorGUI.Toggle(rr, "Item List", !serializedObject.FindProperty("UseReferenceItemList").boolValue);
			//	serializedObject.FindProperty("Capacity").intValue = EditorGUI.IntField(rc, new GUIContent("Capacity", "Leave at 0 for infinite"), serializedObject.FindProperty("Capacity").intValue);
			//};
			//_itemList.drawElementCallback = (rect, index, active, focused) => {
			//	SerializedProperty item = _itemList.serializedProperty.GetArrayElementAtIndex(index);

			//	rect.yMin += (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
			//	rect.height = EditorGUIUtility.singleLineHeight;
			//	rect.xMax -= 100;

			//	EditorGUI.PropertyField(rect, item.FindPropertyRelative("_tag"));

			//	rect.xMin = rect.xMax + 5;
			//	rect.xMax += 100;

			//	EditorGUI.PropertyField(rect, item.FindPropertyRelative("_quantity"));

			//	//Minimum quantity = 1
			//	item.FindPropertyRelative("_quantity").intValue = Mathf.Max(1, item.FindPropertyRelative("_quantity").intValue);
			//};
		}

		public override void OnInspectorGUI()
		{
			//GUILayout.Space(10);

			//_headerBackground = "RL Header";
			//_elementBackground = "RL Background";

			//EditorGUIUtility.labelWidth = 100;

			////Show the equipment list
			//EditorGUIUtility.labelWidth = 50;
			//_equipmentList.DoLayoutList();

			////Show the inventory's weight and money
			//EditorGUIUtility.labelWidth = 50;
			//GUILayout.BeginHorizontal();
			//GUILayout.Label("Weight: " + ((Inventory)target).Weight + "/");
			//_maxweight.intValue = EditorGUILayout.IntField(_maxweight.intValue);
			//EditorGUILayout.PropertyField(_money);
			//GUILayout.EndHorizontal();

			////If this inventory should use another inventory's item list do this
			//EditorGUIUtility.labelWidth = 50;
			//if (serializedObject.FindProperty("UseReferenceItemList").boolValue) {
			//	Rect rect = EditorGUILayout.GetControlRect();
			//	GUI.Label(rect, "", _headerBackground);
			//	rect.xMin += 6;
			//	rect.yMin += 1;
			//	serializedObject.FindProperty("UseReferenceItemList").boolValue = !EditorGUI.Toggle(rect, "Item List", !serializedObject.FindProperty("UseReferenceItemList").boolValue);

			//	rect = EditorGUILayout.GetControlRect(true, 30);

			//	GUI.Label(rect, "", _elementBackground);

			//	rect.xMin += 4;
			//	rect.xMax -= 4;
			//	rect.yMin += 5;
			//	rect.height = EditorGUIUtility.singleLineHeight;
			//	EditorGUIUtility.labelWidth = 0;

			//	serializedObject.FindProperty("ReferenceInventory").objectReferenceValue = EditorGUI.ObjectField(rect, "Use item list from", serializedObject.FindProperty("ReferenceInventory").objectReferenceValue, typeof(Inventory), true);
			//}
			////Otherwise, layout the item list
			//else
			//	_itemList.DoLayoutList();

			////Save changes
			//serializedObject.ApplyModifiedProperties();

			////If in game, add a button to deserialize changes
			//if (Application.isPlaying && GUILayout.Button("Update")) {
			//	((Inventory)target).LoadItemList();
			//}
		}
	}
}