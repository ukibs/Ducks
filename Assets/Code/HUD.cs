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
	public Texture flashbangCoolDown;
	public Texture flashbang;
	public Texture claymoreCoolDown;
	public Texture claymore;
	public Texture trapCoolDown;
	public Texture trap;
	public Texture backgroundHud;
	public Texture totalLife;
	public Texture currentLife;

	private PlayerController player;
	private HealthController life;
    private WeaponController2 weapon;
	private Scene currentScene;

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerController> ();
		life = GetComponent<HealthController> ();
        weapon = GetComponent<WeaponController2>();
	}
	
	// Update is called once per frame
	void Update () {
		currentScene = SceneManager.GetActiveScene();

	}

	private void OnGUI()
	{
		if (currentScene.name != "SelectorOfMaps") {
		
			if (isLocalPlayer) {
				//Background
				GUI.DrawTexture (new Rect (0, Screen.height * 8.9f / 10 , Screen.width*1 , Screen.height*1), backgroundHud, ScaleMode.StretchToFill);

				//Bullets
				GUI.DrawTexture (new Rect (Screen.width * 7.1f / 10, Screen.height * 9f / 10, Screen.width / 17 , Screen.height/12f), imageBullet, ScaleMode.StretchToFill);
				GUI.Label (new Rect (Screen.width * 7.6f / 10, Screen.height * 9.3f / 10, Screen.width / 7, Screen.height / 9.5f), weapon.CurrentAmmo + "/" + weapon.MaxAmmo);

				//Recharge weapon
				GUI.DrawTexture (new Rect (Screen.width * 8.3f / 10, Screen.height * 9f / 10, Screen.width / 15 , Screen.height/12f), imageRecharge, ScaleMode.StretchToFill);
				GUI.Label (new Rect (Screen.width * 9.1f / 10, Screen.height * 9.3f / 10, Screen.width / 7, Screen.height / 9.5f), weapon.ReserveAmmo + "/" + weapon.MaxReserveAmmo);

				//Score
				GUI.Label (new Rect (Screen.width * 9 / 10, Screen.height * 0.7f / 10, Screen.width / 6, Screen.height / 9.5f), "Score: " + life.Score);

				//Blind screen
				if(player.getCooldown(4) != 0)
					GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black);

				for (int i = 0; i < 4; i++) 
				{
					if (player.getCooldown (i) != 0) {
						switch (i) {
						case 0:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), bombCooldown, ScaleMode.StretchToFill);
							break;
						case 1:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), claymoreCoolDown, ScaleMode.StretchToFill);
							break;
						case 2:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), trapCoolDown, ScaleMode.StretchToFill);
							break;
						case 3:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), flashbangCoolDown, ScaleMode.StretchToFill);
							break;
						}		
					} else {
						switch (i) {
						case 0:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), bomb, ScaleMode.StretchToFill);
							break;
						case 1:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), claymore, ScaleMode.StretchToFill);
							break;
						case 2:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), trap, ScaleMode.StretchToFill);
							break;
						case 3:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.5f / 10, Screen.height * 9 / 10, Screen.width / 10, Screen.height / 10), flashbang, ScaleMode.StretchToFill);
							break;
						}
					}
				}

				//Life
				GUI.DrawTexture(new Rect(Screen.width *3.5f/10, Screen.height*9.1f/10, Screen.width*3/10, Screen.height*0.8f/10), totalLife, ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(Screen.width *3.5f/10, Screen.height*9.1f/10, life.Health/100f * (Screen.width*3/10), Screen.height*0.8f/10), currentLife, ScaleMode.StretchToFill);
			}
		
		}

	}
}
