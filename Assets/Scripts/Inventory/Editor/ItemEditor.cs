using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace InventoryManagement
{
	//[CustomEditor(typeof(BaseItem), true)]
	//public class ItemEditor : Editor
	//{
	//	private PreviewRenderUtility preview;
	//	private Vector2 drag;
	//	private float zoom;

	//	public override bool RequiresConstantRepaint()
	//	{
	//		return true;
	//	}

	//	protected virtual void OnEnable()
	//	{
	//		preview = new PreviewRenderUtility();
	//		preview.lights[0].transform.rotation = Quaternion.Euler(new Vector3(75, 180, 0));
	//	}

	//	public override GUIContent GetPreviewTitle()
	//	{
	//		return new GUIContent("Preview");
	//	}

	//	public override void OnPreviewGUI(Rect r, GUIStyle background)
	//	{
	//		if (serializedObject.FindProperty("_model")?.objectReferenceValue) {
	//			drag -= new Vector2((float)(0.0005 * EditorApplication.timeSinceStartup), 0);
	//			drag = Drag2D(drag, r);

	//			if (Event.current.type == EventType.Repaint) {
	//				preview.BeginPreview(r, background);

	//				GameObject model = (GameObject)serializedObject.FindProperty("_model").objectReferenceValue;
	//				Mesh mesh = model.GetComponentInChildren<SkinnedMeshRenderer>()?.sharedMesh ?? model.GetComponentInChildren<MeshFilter>()?.sharedMesh;
	//				Material material = model.GetComponentInChildren<SkinnedMeshRenderer>()?.sharedMaterial ?? model.GetComponentInChildren<MeshRenderer>()?.sharedMaterial;

	//				if (mesh) {
	//					float dist = 1.2f * (mesh.bounds.size.y / Mathf.Tan(preview.cameraFieldOfView * (Mathf.PI / 180)));

	//					preview.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
	//					preview.camera.transform.rotation = Quaternion.Euler(new Vector3(0, -drag.x, 0));
	//					preview.camera.transform.position = mesh.bounds.center + preview.camera.transform.forward * -dist;
	//					preview.camera.farClipPlane = dist * 1.5f;
	//				}

	//				preview.Render();
	//				Texture t = preview.EndPreview();

	//				GUI.DrawTexture(r, t);
	//			}
	//		}
	//		else
	//			EditorGUI.DropShadowLabel(r, "No Model Assigned");
	//	}

	//	public override bool HasPreviewGUI()
	//	{
	//		return true;
	//	}

	//	public override void OnInspectorGUI()
	//	{
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_name"));
	//		EditorStyles.textField.wordWrap = true;
	//		EditorGUILayout.LabelField("Description");
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_description"), GUIContent.none, GUILayout.MinHeight(100));

	//		EditorGUILayout.Space();
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_cost"));
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_icon"));
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_model"));

	//		serializedObject.ApplyModifiedProperties();
	//	}

	//	private void OnDisable()
	//	{
	//		preview.Cleanup();
	//	}

	//	private static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
	//	{
	//		int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
	//		Event current = Event.current;
	//		switch (current.GetTypeForControl(controlID)) {
	//			case EventType.MouseDown:
	//				if (position.Contains(current.mousePosition) && position.width > 50f) {
	//					GUIUtility.hotControl = controlID;
	//					current.Use();
	//					EditorGUIUtility.SetWantsMouseJumping(1);
	//				}
	//				break;
	//			case EventType.MouseUp:
	//				if (GUIUtility.hotControl == controlID) {
	//					GUIUtility.hotControl = 0;
	//				}
	//				EditorGUIUtility.SetWantsMouseJumping(0);
	//				break;
	//			case EventType.MouseDrag:
	//				if (GUIUtility.hotControl == controlID) {
	//					scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
	//					scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
	//					current.Use();
	//					GUI.changed = true;
	//				}
	//				break;
	//		}
	//		return scrollPosition;
	//	}
	//}

	//[CustomEditor(typeof(ArmourItem), true)]
	//public class ArmourEditor : ItemEditor
	//{
	//	private string[] phenoNames = System.Enum.GetNames(typeof(Character.Phenotype));
	//	private ReorderableList modelList;

	//	protected override void OnEnable()
	//	{
	//		base.OnEnable();

	//		serializedObject.FindProperty("_modelVariants").arraySize = phenoNames.Length;

	//		modelList = new ReorderableList(serializedObject, serializedObject.FindProperty("_modelVariants"), false, true, false, false);
	//		modelList.drawHeaderCallback += (rect) =>
	//		{
	//			GUI.Label(rect, "Model Variants");
	//		};
	//		modelList.drawElementCallback += (rect, index, isActive, isFocused) =>
	//		{
	//			rect.y += 2;
	//			rect.height -= 5;
	//			EditorGUI.PropertyField(rect, modelList.serializedProperty.GetArrayElementAtIndex(index), new GUIContent(phenoNames[index]));

	//			//set the default model property to the first index variant
	//			if (index == 0)
	//				serializedObject.FindProperty("_model").objectReferenceValue = modelList.serializedProperty.GetArrayElementAtIndex(0).objectReferenceValue;
	//		};
	//	}

	//	public override void OnInspectorGUI()
	//	{
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_name"));
	//		EditorStyles.textField.wordWrap = true;
	//		EditorGUILayout.LabelField("Description");
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_description"), GUIContent.none, GUILayout.MinHeight(100));

	//		EditorGUILayout.Space();
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_cost"));
	//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_icon"));

	//		EditorGUILayout.Space();
	//		modelList.DoLayoutList();

	//		serializedObject.ApplyModifiedProperties();
	//	}
	//}
}