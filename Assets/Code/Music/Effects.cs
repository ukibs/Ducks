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
    //protected GameObject gameManager;
    //protected GameManager gmScript;
    //protected GameManagerSingleton gameManagerSingleton;
	#endregion


	#region MonoDevelop Methods
	// Use this for initialization
	protected virtual void Start () 
	{
        //gmScript = FindObjectOfType<GameManager>();
        //gameManagerSingleton = GameManagerSingleton.instance;
		aS = GetComponent<AudioSource> ();
        //aS.volume = gameManagerSingleton.Volume;
        //Debug.Log("Current volume: " + aS.volume);
    }

	// Update is called once per frame
	protected virtual void Update () 
	{

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
