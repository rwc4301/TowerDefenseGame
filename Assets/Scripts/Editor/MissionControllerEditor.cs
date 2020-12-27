using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mission
{
	[CustomEditor(typeof(MissionController))]
	public class MissionControllerEditor : Editor
	{
		private MissionController controller;

		private List<GameObject> staticObjects;
		private Mesh mesh;
		private Material material;
		private Sprite entryPointSprite, exitPointSprite;
		private GUIContent entryContent, exitContent; 
		SerializedProperty entryPoints, exitPoints, positionDictKeys, positionDictVals;

		private void OnEnable()
		{
			controller = (MissionController)target;
			staticObjects = new List<GameObject>();
			material = new Material(Shader.Find("SuperSystems/Wireframe-Transparent"));

			entryContent = new GUIContent(Resources.Load<Texture2D>("UI/Editor/enter"));
			exitContent = new GUIContent(Resources.Load<Texture2D>("UI/Editor/exit"));

			entryPoints = serializedObject.FindProperty("m_entryPoints");
			exitPoints = serializedObject.FindProperty("m_exitPoints");

			positionDictKeys = serializedObject.FindProperty("m_positionDictionary").FindPropertyRelative("keys");
			positionDictVals = serializedObject.FindProperty("m_positionDictionary").FindPropertyRelative("values");

			foreach (GameObject go in FindObjectsOfType(typeof(GameObject))) {
				StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(go);

				if ((flags & StaticEditorFlags.NavigationStatic) != 0) {
					staticObjects.Add(go);
				}
			}

		}

		private void OnSceneGUI()
		{
			foreach (GameObject go in staticObjects) {
				Bounds bounds = go.GetComponent<Terrain>().terrainData.bounds;

				int xMin = Mathf.FloorToInt(bounds.min.x), zMin = Mathf.FloorToInt(bounds.min.z);
				Vector3Int pos = new Vector3Int(xMin, 0, zMin);
				Quaternion rot = Quaternion.Euler(Vector3.left * 90);

				while (pos.z <= bounds.max.z) {
					while (pos.x <= bounds.max.x) {
						bool isSet = controller.GetCanBuildOnPosition(pos.x, pos.z);

						Handles.color = isSet ? Color.green : Color.red;
						if (Handles.Button(pos, rot, 0.5f, 0.5f, Handles.RectangleHandleCap)) {
							controller.SetCanBuildOnPosition(pos.x, pos.z, !isSet);
						}
						pos.x++;
					}
					pos.z++;
					pos.x = xMin;
				}
			}

			for (int i = 0; i < entryPoints.arraySize; i++) {
				entryPoints.GetArrayElementAtIndex(i).vector3Value = Handles.PositionHandle(entryPoints.GetArrayElementAtIndex(i).vector3Value, Quaternion.identity);
				Handles.Label(entryPoints.GetArrayElementAtIndex(i).vector3Value, entryContent, new GUIStyle { fixedWidth = 30 });
			}

			for (int i = 0; i < exitPoints.arraySize; i++) {
				exitPoints.GetArrayElementAtIndex(i).vector3Value = Handles.PositionHandle(exitPoints.GetArrayElementAtIndex(i).vector3Value, Quaternion.identity);
				Handles.Label(exitPoints.GetArrayElementAtIndex(i).vector3Value, exitContent, new GUIStyle { fixedWidth = 30 });
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}