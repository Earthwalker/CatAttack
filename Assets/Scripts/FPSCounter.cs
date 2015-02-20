using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
	/// <summary>
	/// Update frequency
	/// </summary>
	public float Frequency = 0.5f;

	/// <summary>
	/// Current fps
	/// </summary>
	public float FPS { get; private set; }

	/// <summary>
	/// Called at creation
	/// </summary>
	void Start()
	{
		StartCoroutine(CalulateFPS());
	}

	/// <summary>
	/// Calculate and show fps
	/// </summary>
	/// <returns></returns>
	IEnumerator CalulateFPS()
	{
		while (true)
		{
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;

			for (float i = 0; i <= Frequency; i += Time.deltaTime)
				yield return 0;

			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;
			FPS = frameCount / timeSpan;

			// update our text
			GetComponent<Text>().text = string.Format("{0:0.} fps", FPS);
		}
	}
}