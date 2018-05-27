using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour {

	#region Public Attributes
	public AudioClip [] effects;
	public AudioClip [] choque;
	#endregion

	#region Private Attributes
	private AudioSource aS;
	#endregion


	#region MonoDevelop Methods
	// Use this for initialization
	protected virtual void Start () 
	{
		aS = GetComponent<AudioSource> ();
    }
	#endregion

	#region User Methods
	private void playEffect(AudioClip clip)
	{
			aS.clip = clip;
			aS.Play ();
		
	}

	public void playEffect(int clip)
	{
			aS.clip = effects[clip];
			aS.Play ();
		
	}

	public void playChoque()
	{
		int i = Random.Range(0, choque.Length);
		playEffect (choque [i]);
    }
    #endregion
}
