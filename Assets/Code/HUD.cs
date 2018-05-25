using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HUD : NetworkBehaviour {

	public Texture imageBullet; 
	public Texture imageRecharge;
	public Texture black;
	public Texture bomb;
	public Texture bombCooldown;
	public Texture backgroundHud;
	public Texture totalLife;
	public Texture currentLife;

	private PlayerController player;
	private HealthController life;
	private Scene currentScene;
	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerController> ();
		life = GetComponent<HealthController> ();
	}
	
	// Update is called once per frame
	void Update () {
		currentScene = SceneManager.GetActiveScene();

	}

	void NesxtScene(){
		//NetworkManager.ServerChangeScene ("TestScene");
	}

	private void OnGUI()
	{
		if (currentScene.name != "SelectorOfMaps") {
		
			if (isLocalPlayer) {
				// Weapon info
				BaseWeapon weaponData = player.CurrentWeapon;
				//Background
				GUI.Label (new Rect (0, Screen.height * 8.9f / 10 , Screen.width * 100, Screen.width*1), backgroundHud);
				//Bullets
				GUI.Label (new Rect (Screen.width * 7.1f / 10, Screen.height * 9.2f / 10, 150, 30), imageBullet);
				GUI.Label (new Rect (Screen.width * 7.6f / 10, Screen.height * 9.3f / 10, 150, 30), weaponData.CurrentWeaponAmmo + "/" + weaponData.maxWeaponAmmo);
				//Recharge weapon
				GUI.Label (new Rect (Screen.width * 8.3f / 10, Screen.height * 9.2f / 10, 150, 30), imageRecharge);
				GUI.Label (new Rect (Screen.width * 9.1f / 10, Screen.height * 9.3f / 10, 100, 20), weaponData.CurrentReserveAmmo + "/" + weaponData.maxReserveAmmo);

				//GUI.Label(new Rect(10, 10, 350, 20), "State: " + player.State + ", movement state: " + player.);

				//Score
				GUI.Label (new Rect (Screen.width * 9 / 10, Screen.height * 0.7f / 10, 100, 20), "Score: " + player.Score);

				//Blind screen
				if(player.getCooldown(4) != 0)
					GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black);

				for (int i = 0; i < 4; i++) 
				{
					if (player.getCooldown (i) != 0) 
					{
						GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), bombCooldown, ScaleMode.StretchToFill);
					}
					else
						GUI.DrawTexture (new Rect (Screen.width * i * 0.5f/ 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), bomb, ScaleMode.StretchToFill);
				}

				//Life
				GUI.DrawTexture(new Rect(Screen.width *3.5f/10, Screen.height*9.1f/10, Screen.width*3/10, Screen.height*0.8f/10), totalLife, ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(Screen.width *3.5f/10, Screen.height*9.1f/10, life.Health/100f * (Screen.width*3/10), Screen.height*0.8f/10), currentLife, ScaleMode.StretchToFill);
			}
		
		}

	}
}
