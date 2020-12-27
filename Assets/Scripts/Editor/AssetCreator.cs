using System.IO;
using UnityEditor;
using UnityEngine;

class AssetCreator
{
	public static void CreateAsset<T>(string name) where T : ScriptableObject
	{
		var asset = ScriptableObject.CreateInstance<T>();

		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (path == "")
			path = "Assets";
		else if (Path.GetExtension(path) != "")
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

		path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

		AssetDatabase.CreateAsset(asset, path);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}

	public static void CreatePrefab<T>(string name) where T : MonoBehaviour
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (path == "")
			path = "Assets";
		else if (Path.GetExtension(path) != "")
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

		path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

		var asset = PrefabUtility.SaveAsPrefabAsset(new GameObject(name, typeof(T)), path);

		//AssetDatabase.CreateAsset(asset, path);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
}