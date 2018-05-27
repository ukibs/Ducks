using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour {

    #region Public Attributes

    public AudioClip[] musicClips;

	#endregion

	#region Private Attributes
	private AudioSource aS;
	#endregion


	#region MonoDevelop Methods
	// Use this for initialization
	protected virtual void Start () {
		aS = GetComponent<AudioSource> ();
        PlayMusic();
	}
	#endregion

	#region User Methods

    void PlayMusic()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "InitialScene":
            case "SelectorOfMaps":
                aS.clip = musicClips[0];
                break;
            default:
                aS.clip = musicClips[1];
                break;
        }
        aS.Play();
    }

	#endregion
}
