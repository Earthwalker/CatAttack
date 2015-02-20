using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility class
/// 
/// Attached to: N/A
/// </summary>
public static class Utility
{
	/// <summary>
	/// Finds the first object at a position with one of the specified tags
	/// </summary>
	/// <returns>The object at the position</returns>
	/// <param name="pos">Position</param>
	/// <param name="tags">Tags</param>
	public static GameObject FindAtPositionWithTag(Vector3 pos, params string[] tags)
	{
		List<GameObject> objList = new List<GameObject>();
		
		for(int i=0; i<tags.Length; i++)
			objList.AddRange(GameObject.FindGameObjectsWithTag(tags[i]));
		
		foreach(GameObject obj in objList)
		{
			if(obj != null)
			{
                if (obj.transform.position == pos && (obj.renderer == null || obj.renderer.enabled))
					return obj;
			}
		}
		
		return null;
	}

    public static GameObject FindAtSnappedPositionWithTag(Vector3 pos, int snapSize, params string[] tags)
    {
        List<GameObject> objList = new List<GameObject>();

        for (int i = 0; i < tags.Length; i++)
            objList.AddRange(GameObject.FindGameObjectsWithTag(tags[i]));

        foreach (GameObject obj in objList)
        {
            if (obj != null)
            {
                if (Snap(obj.transform.position, snapSize) == pos && (obj.renderer == null || obj.renderer.enabled))
                    return obj;
            }
        }

        return null;
    }

	public static Color ColorFromHSV(float h, float s, float v, float a)
	{
		float r = v;
		float g = v;
		float b = v;

		if(s != 0)
		{
			float max = v;
			float dif = v * s;
			float min = v - dif;
			
			h *= 360f;
			
			if (h < 60f)
			{
				r = max;
				g = h * dif / 60f + min;
				b = min;
			}
			else if (h < 120f)
			{
				r = -(h - 120f) * dif / 60f + min;
				g = max;
				b = min;
			}
			else if (h < 180f)
			{
				r = min;
				g = max;
				b = (h - 120f) * dif / 60f + min;
			}
			else if (h < 240f)
			{
				r = min;
				g = -(h - 240f) * dif / 60f + min;
				b = max;
			}
			else if (h < 300f)
			{
				r = (h - 240f) * dif / 60f + min;
				g = min;
				b = max;
			}
			else if (h <= 360f)
			{
				r = max;
				g = min;
				b = -(h - 360f) * dif / 60 + min;
			}
			else
			{
				r = 0;
				g = 0;
				b = 0;
			}
		}
		
		return new Color(Mathf.Clamp01(r),Mathf.Clamp01(g),Mathf.Clamp01(b),a);
	}
	
	public static int GetIndexInVector(Vector3 pos, Vector3 size)
	{
        while (pos.x < 0)
            pos.x += size.x;
        while (pos.y < 0)
            pos.y += size.y;
        while (pos.z < 0)
            pos.z += size.z;

		return (int)((pos.x + (pos.y*size.x) + (pos.z*size.x*size.y)));
	}

	public static Vector3 GetVectorFromIndex(int ind, Vector3 size)
	{
		Vector3 vect = new Vector3();
		
		vect.z = Mathf.FloorToInt(ind / (size.x * size.y));
		ind -= (int)(vect.z * size.x * size.y);
		
		vect.y = Mathf.FloorToInt(ind / size.x);
		ind -= (int)(vect.y * size.x);
		
		vect.x = ind;

		return vect;
	}

	/// <summary>
	/// Gets the unbroken vertices of a box with the given bounds
	/// </summary>
	/// <returns>The unbroken box verts.</returns>
	/// <param name="box">Vertices</param>
	public static Vector3[] GetUnbrokenBoxVerts(Bounds box)
	{
		Vector3[] pointArray = new Vector3[16];
		
		//get all the points
		pointArray[0] = box.center  + new Vector3(box.extents.x, box.extents.y, box.extents.z);
		pointArray[1] = box.center + new Vector3(box.extents.x, box.extents.y, -box.extents.z);
		pointArray[2] = box.center + new Vector3(-box.extents.x, box.extents.y, -box.extents.z);
		pointArray[3] = box.center + new Vector3(-box.extents.x, box.extents.y, box.extents.z);
		pointArray[4] = box.center + new Vector3(box.extents.x, box.extents.y, box.extents.z);
		pointArray[5] = box.center + new Vector3(box.extents.x, -box.extents.y, box.extents.z);
		pointArray[6] = box.center + new Vector3(box.extents.x, -box.extents.y, -box.extents.z);
		pointArray[7] = box.center + new Vector3(box.extents.x, box.extents.y, -box.extents.z);
		pointArray[8] = box.center  + new Vector3(box.extents.x, -box.extents.y, -box.extents.z);
		pointArray[9] = box.center + new Vector3(-box.extents.x, -box.extents.y, -box.extents.z);
		pointArray[10] = box.center + new Vector3(-box.extents.x, box.extents.y, -box.extents.z);
		pointArray[11] = box.center + new Vector3(-box.extents.x, -box.extents.y, -box.extents.z);
		pointArray[12] = box.center + new Vector3(-box.extents.x, -box.extents.y, box.extents.z);
		pointArray[13] = box.center + new Vector3(-box.extents.x, box.extents.y, box.extents.z);
		pointArray[14] = box.center + new Vector3(-box.extents.x, -box.extents.y, box.extents.z);
		pointArray[15] = box.center + new Vector3(box.extents.x, -box.extents.y, box.extents.z);
		
		return pointArray;
	}

	public static int[] GetEdgeIndices(Vector3 edge, Vector3 size)
	{
		if(edge == Vector3.zero)
			return new int[0];
		
		List<int> edgeInd = new List<int>();

		if(edge.x != 0)
		{
			if(edge.x < 0)
				edge.x = 0;
			
			for(int y=0; y<size.y; y++)
			{
				for(int z=0; z<size.z; z++)
					edgeInd.Add(Utility.GetIndexInVector(new Vector3(edge.x*(size.x-1),y,z), size));
			}
		}

		if(edge.y != 0)
		{
			if(edge.y < 0)
				edge.y = 0;
			
			for(int x=0; x<size.x; x++)
			{
				for(int z=0; z<size.z; z++)
					edgeInd.Add(Utility.GetIndexInVector(new Vector3(x,edge.y*(size.y-1),z), size));
			}
		}

		if(edge.z != 0)
		{
			if(edge.z < 0)
				edge.z = 0;
			
			for(int x=0; x<size.x; x++)
			{
				for(int y=0; y<size.y; y++)
					edgeInd.Add(Utility.GetIndexInVector(new Vector3(x,y,edge.z*(size.z-1)), size));
			}
		}

		return edgeInd.ToArray();
	}

    /// <summary>
    /// Snap a float to an int
    /// </summary>
    /// <param name="val"></param>
    /// <param name="snap"></param>
    /// <returns></returns>
    public static float Snap(float val, float snap)
    {
        return Mathf.RoundToInt(val / snap) * snap;
    }

    /// <summary>
    /// Snaps a Vector3 to an int
    /// </summary>
    /// <param name="val"></param>
    /// <param name="snap"></param>
    /// <returns></returns>
    public static Vector3 Snap(Vector3 val, float snap)
    {
        return new Vector3(Mathf.RoundToInt(val.x / snap),
                           Mathf.RoundToInt(val.y / snap),
                           Mathf.RoundToInt(val.z / snap)) * snap;
    }

    public static bool VectorGreater(Vector3 v1, Vector3 v2)
    {
        return v1.x > v2.x || v1.y > v2.y || v1.z > v2.z;
    }

    public static Vector3[] FindAdjacentEmptySpaces(Vector3 pos, int gridSize, params string[] tags)
    {
        List<Vector3> spacesList = new List<Vector3>();

        if (!FindAtPositionWithTag(new Vector3(pos.x - gridSize, pos.y, pos.z), tags))
            spacesList.Add(new Vector3(pos.x - gridSize, pos.y, pos.z));

        if (!FindAtPositionWithTag(new Vector3(pos.x + gridSize, pos.y, pos.z), tags))
            spacesList.Add(new Vector3(pos.x + gridSize, pos.y, pos.z));

        if (!FindAtPositionWithTag(new Vector3(pos.x, pos.y - gridSize, pos.z), tags))
            spacesList.Add(new Vector3(pos.x, pos.y - gridSize, pos.z));

        if (!FindAtPositionWithTag(new Vector3(pos.x, pos.y + gridSize, pos.z), tags))
            spacesList.Add(new Vector3(pos.x, pos.y + gridSize, pos.z));

        if (!FindAtPositionWithTag(new Vector3(pos.x, pos.y, pos.z - gridSize), tags))
            spacesList.Add(new Vector3(pos.x, pos.y, pos.z - gridSize));

        if (!FindAtPositionWithTag(new Vector3(pos.x, pos.y, pos.z + gridSize), tags))
            spacesList.Add(new Vector3(pos.x, pos.y, pos.z + gridSize));

        return spacesList.ToArray();
    }

    public static Vector3 Vector3FromString(string str)
    {
        string[] temp = str.Substring(1, str.Length - 2).Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);

        return new Vector3(x, y, z);
    }

	public static Quaternion QuaternionFromString(string str)
	{
		string[] temp = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(temp[0]);
		float y = float.Parse(temp[1]);
		float z = float.Parse(temp[2]);
		float w = float.Parse(temp[3]);

		return new Quaternion(x, y, z, w);
	}
}