using UnityEngine;
using System.Collections;

public class DebugHelper{

	public static bool useDebugDraw = true;
	public static bool doTests = true;

	public static void DrawLine(Vector3 start, Vector3 end){
		if (!useDebugDraw)
			return;
		Debug.DrawLine(start, end, Color.red);
	}

	public static void DrawCross(Vector3 p){
		if (!useDebugDraw)
			return;
			
		float offset = 5;

		Vector3 p1 = new Vector3(p.x, p.y, p.z-offset);
		Vector3 p2 = new Vector3(p.x, p.y, p.z+offset);

		Vector3 p3 = new Vector3(p.x-offset, p.y, p.z);
		Vector3 p4 = new Vector3(p.x+offset, p.y, p.z);

		Debug.DrawLine(p1, p2, Color.red);
		Debug.DrawLine(p3, p4, Color.red);
	}

	public static void Assert(bool condition, string message = "Error!"){
		if (condition)
			return;
		Debug.LogError(message);
	}

}