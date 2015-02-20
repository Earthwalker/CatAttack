using UnityEngine;
using System.Reflection;

public class Console : MonoBehaviour
{
	public bool SetProperty(string name, object value)
	{
		FieldInfo field = GetType().GetField(name);

		if (field != null)
		{
			field.SetValue(this, value);
			return true;
		}
		else
			return false;
	}

	public object GetProperty(string name)
	{
		FieldInfo field = GetType().GetField(name);

		if (field != null)
			return field.GetValue(this);
		else
			return null;
	}
}