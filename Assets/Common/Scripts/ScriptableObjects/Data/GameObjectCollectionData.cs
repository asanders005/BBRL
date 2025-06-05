using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject that stores a GameObject reference.
/// Can be created as an asset in the project and referenced by multiple scripts.
/// </summary>
[CreateAssetMenu(fileName = "GameObjectData", menuName = "Data/GameObjectCollectionData")]
public class GameObjectCollectionData : ScriptableObjectBase
{
	/// <summary>
	/// The GameObject reference stored in this ScriptableObject.
	/// Note: Since GameObject instances are scene-specific, this will typically
	/// be assigned at runtime rather than in the asset itself.
	/// </summary>
	[SerializeField] private List<GameObject> value;

	/// <summary>
	/// Public property to access or modify the stored GameObject reference.
	/// </summary>
	public List<GameObject> Value { get => value; }

	/// <summary>
	/// Implicit conversion operator that allows using this ScriptableObject directly as a GameObject.
	/// Example usage: GameObject obj = myGameObjectData; // instead of GameObject obj = myGameObjectData.value;
	/// Returns null if the variable is null or its value is null.
	/// </summary>
	/// <param name="variable">The GameObjectData object to convert</param>
	public static implicit operator List<GameObject>(GameObjectCollectionData variable)
	{
		return variable?.value ?? null;
	}

    public void AddGameObject(GameObject gameObject)
    {
        if (gameObject == null) return;
        if (value == null) value = new List<GameObject>();
        value.Add(gameObject);
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        if (gameObject == null || value == null) return;
        value.Remove(gameObject);
    }
}
