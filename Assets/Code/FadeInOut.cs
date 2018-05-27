using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour {
	
	#region Public Attributes
	public Texture fadeTexture;
	public float fadeSpeed = 0.2f;
	//0 -> transparent, 1 -> opaque
	public float alpha = 1.0f;
	public int fadeDir = -1;
	#endregion

	#region Private Attributes
	private int drawDepth = -100;
	private Color fadeColor;
	#endregion
	
	#region MonoDevelop Methods	
	// Update is called once per frame
	void Update () 
	{
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01 (alpha);
		fadeColor = GUI.color;
		fadeColor.a = alpha;
	}

	//
	void onGUI()
	{
		GUI.color = fadeColor;
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeTexture);
	}
	#endregion
	
	#region User Methods
	//change the fade direction
	public void Switch()
	{
		fadeDir *= -1;
	}

	//get the current transparency
	public float GetAlpha()
	{
		return alpha;
	}
	#endregion
}
