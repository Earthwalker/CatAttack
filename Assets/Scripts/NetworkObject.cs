using UnityEngine;

public class NetworkObject : NetworkMonoBehaviour
{
	/// <summary>
	/// Called when the instance is created
	/// </summary>
	public virtual void Start()
	{
		// initialize all scripts on this object
		SendMessage("Initialize", networkView);

		// initialize all scripts on our children
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild(i).SendMessage("Initialize", networkView);

		Debug.Log(name + " initialized");
    }
}
