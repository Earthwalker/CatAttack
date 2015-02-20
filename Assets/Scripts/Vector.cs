using UnityEngine;
using System;

[Serializable]
public struct Vector
{
	public float x;
	public float y;
	public float z;

	public Vector(float x, float y, float z=0)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector2 ToVector2()
	{
		return new Vector2(x, y);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y, z);
	}

	public new string ToString()
	{
		return "( " + x.ToString() + "," + y.ToString() + "," + z.ToString() + " )";
	}
}
