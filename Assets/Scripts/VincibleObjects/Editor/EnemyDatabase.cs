using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDatabase : ScriptableObject
{
	[Serializable] private class EnemiesByName
	{
		[SerializeField] private Enemy[] m_enemies;
	}

	[SerializeField] private int[] m_levels;
	[SerializeField] private string[] m_names;
	[SerializeField] private EnemiesByName[] m_enemies;

	public Dictionary<(int, string), Enemy> LoadEnemies()
	{
		return null;
	}
}

//[CustomEditor(typeof(EnemyDatabase))]
public class EnemyDatabaseEditor : EditorWindow
{
	EnemyDatabase target { get; set; }
	SerializedObject serializedObject { get; set; }

	[MenuItem("Window/Enemy Database")]
	private static void Init()
	{
		// Get existing open window or if none, make a new one:
		EnemyDatabaseEditor window = (EnemyDatabaseEditor)GetWindow(typeof(EnemyDatabaseEditor));
		window.Show();
	}

	[MenuItem("Assets/Enemy Database")]
	private static void Create()
	{
		AssetCreator.CreateAsset<EnemyDatabase>("Enemy Database");
	}

	private void OnEnable()
	{
		var dbs = Resources.FindObjectsOfTypeAll<EnemyDatabase>();
		if (dbs.Length == 1) {
			target = dbs[0];
			serializedObject = new SerializedObject(dbs[0]);
		}
	}

	private void OnGUI()
	{
		int rows = serializedObject.FindProperty("m_levels").arraySize,
			columns = serializedObject.FindProperty("m_names").arraySize;
		float width = Mathf.Max(EditorGUIUtility.currentViewWidth / (rows + 2), EditorGUIUtility.fieldWidth);

		using (var scope = new GUILayout.HorizontalScope(EditorStyles.toolbar)) {
			GUILayout.Space(width);
			for (int i = 0; i < rows; i++)
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_levels").GetArrayElementAtIndex(i), new GUIContent("Level"));

			if (GUILayout.Button("+", EditorStyles.toolbarButton))
				serializedObject.FindProperty("m_levels").arraySize++;

			serializedObject.FindProperty("m_enemies").arraySize = rows;
		}

		for (int j = 0; j < columns; j++)
			using (var scope = new GUILayout.HorizontalScope()) {
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_names").GetArrayElementAtIndex(j), new GUIContent("Name"), GUILayout.Width(width));

				for (int i = 0; i < rows; i++) {
					serializedObject.FindProperty("m_enemies").GetArrayElementAtIndex(i).FindPropertyRelative("m_enemies").arraySize = columns;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_enemies").GetArrayElementAtIndex(i).FindPropertyRelative("m_enemies").GetArrayElementAtIndex(j));
				}
			}

		if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(width)))
			serializedObject.FindProperty("m_names").arraySize++;

		serializedObject.ApplyModifiedProperties();

		//for (var i = 0; i < columns; i++) {
		//	using (var scope = new GUILayout.HorizontalScope()) {
		//		for (int j = 0; j < rows; j++) {
		//			var tuple = (serializedObject.FindProperty("m_levels").GetArrayElementAtIndex(j).intValue,
		//						 serializedObject.FindProperty("m_names").GetArrayElementAtIndex(i).stringValue);
		//			SetEnemyObjectField((Enemy)EditorGUILayout.ObjectField(target.Enemies.ContainsKey(tuple) ? target.Enemies[tuple] : null, typeof(Enemy), false), tuple);
					

		//		}
		//		GUILayout.Space(width);
		//	}
		//}
	}

	private void SetEnemyObjectField(Enemy enemy, (int, string) tuple)
	{

	}
}