using UnityEditor;

namespace Inventory
{
    //[CustomEditor(typeof(InventoryPanel))]
    public class InventoryPanelInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemHolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemButton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CloseButton"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Source"));
            EditorGUILayout.Space();

            if (serializedObject.FindProperty("Source").enumValueIndex == 0)
            {   //Player
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowWeight"));
                if (serializedObject.FindProperty("ShowWeight").boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("WeightText"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("WeightStringFormat"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowMoney"));
                if (serializedObject.FindProperty("ShowMoney").boolValue)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MoneyText"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowNoneItem"));
            }
            else
            {
                serializedObject.FindProperty("ShowWeight").boolValue = false;
                serializedObject.FindProperty("ShowMoney").boolValue = false;
                serializedObject.FindProperty("ShowNoneItem").boolValue = false;
            }

            if (serializedObject.FindProperty("Source").enumValueIndex == 1)
            {   //Loot
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TakeAllButton"));
            }
            else serializedObject.FindProperty("TakeAllButton").objectReferenceValue = null;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowObjectName"));
            if (serializedObject.FindProperty("ShowObjectName").boolValue)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectNameText"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnClick"));
            switch (serializedObject.FindProperty("OnClick").enumValueIndex)
            {
                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("DestInv"));
                    break;
                case 3:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("DragBox"));
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Tooltip"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TooltipFollowMouse"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}