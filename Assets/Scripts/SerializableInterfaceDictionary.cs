using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SerializableInterfaceList<T> : ISerializationCallbackReceiver, IList<GameObject>
{
	[SerializeField] private List<GameObject> values = new List<GameObject>();

	public GameObject this[int index] { get => values[index]; set => values[index] = value; }

	public int Count => values.Count;

	public bool IsReadOnly => false;

	public T GetT(int index)
	{
		return this[index].GetComponent<T>();
	}

	public void Add(GameObject item)
	{
		if (item != null && typeof(T).IsAssignableFrom(item.GetType()))
			values.Add(item);
		else
			Debug.LogError(string.Format("Object {0} does not implement interface {1}", item, typeof(T)));
	}

	public void Clear()
	{
		values.Clear();
	}

	public bool Contains(GameObject item)
	{
		return values.Contains(item);
	}

	public void CopyTo(GameObject[] array, int arrayIndex)
	{
		values.CopyTo(array, arrayIndex);
	}

	public IEnumerator<GameObject> GetEnumerator()
	{
		return values.GetEnumerator();
	}

	public int IndexOf(GameObject item)
	{
		return values.IndexOf(item);
	}

	public void Insert(int index, GameObject item)
	{
		values.Insert(index, item);
	}

	public bool Remove(GameObject item)
	{
		return values.Remove(item);
	}

	public void RemoveAt(int index)
	{
		values.RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return values.GetEnumerator();
	}

	public void OnAfterDeserialize()
	{
		for (int i = 0; i < values.Count; i++) {
			if (values[i] != null && !typeof(T).IsAssignableFrom(values[i].GetType())) {
				Debug.LogError(string.Format("Object {0} does not implement interface {1}", values[i], typeof(T)));
				values[i] = null;
			}
		}
	}

	public void OnBeforeSerialize()
	{
		for (int i = 0; i < values.Count; i++) {
			if (values[i] != null && values[i].GetComponent<T>() == null) {
				Debug.LogError(string.Format("Object {0} does not implement interface {1}", values[i], typeof(T)));
				values[i] = null;
			}
		}
	}
}

[Serializable]
public class SerializableInterfaceDictionary<TKey, TValue> : Dictionary<Object, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<Object> keys = new List<Object>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();

		foreach (KeyValuePair<Object, TValue> pair in this) {
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		if (keys.Count != values.Count)
			throw new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", keys.Count, values.Count));

		for (int i = 0; i < keys.Count; i++) {
			if (keys[i] != null && !this.ContainsKey(keys[i])) {
				if (!typeof(TKey).IsAssignableFrom(keys[i].GetType()))
					throw new ArgumentException(string.Format("Object {0} does not implement interface {1}", keys[i], typeof(TKey)));
				this.Add(keys[i], values[i]);
			}
		}
	}
}
