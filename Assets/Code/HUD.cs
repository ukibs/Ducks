using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HUD : NetworkBehaviour {

	public Texture imageBullet; 
	public Texture imageRecharge;
	public Texture black;

	private PlayerController player;

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void NesxtScene(){
		//NetworkManager.ServerChangeScene ("TestScene");
	}

	private void OnGUI()
	{
		if (isLocalPlayer) {
			// Weapon info
			BaseWeapon weaponData = player.CurrentWeapon;
			//Bullets
			GUI.Label (new Rect (Screen.width * 7.8f / 10, Screen.height * 9f / 10, 150, 30), imageBullet);
			GUI.Label (new Rect (Screen.width * 8.1f / 10, Screen.height * 9.1f / 10, 150, 30), weaponData.CurrentWeaponAmmo + "/" + weaponData.maxWeaponAmmo);
			//Recharge weapon
			GUI.Label (new Rect (Screen.width * 8.5f / 10, Screen.height * 9 / 10, 150, 30), imageRecharge);
			GUI.Label (new Rect (Screen.width * 9.1f / 10, Screen.height * 9.1f / 10, 100, 20), weaponData.CurrentReserveAmmo + "/" + weaponData.maxReserveAmmo);

			//GUI.Label(new Rect(10, 10, 350, 20), "State: " + player.State + ", movement state: " + player.);

			//Score
			GUI.Label (new Rect (Screen.width * 9 / 10, Screen.height * 0.7f / 10, 100, 20), "Score: " + player.Score);

			//Blind screen
			if(player.State == PlayerStates.Blind)
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black);
		}
	}
}
