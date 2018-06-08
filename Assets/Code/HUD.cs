using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HUD : NetworkBehaviour {

    private Texture imageBullet;
    private Texture imageRecharge;
    private Texture black;
    private Texture bomb;
    private Texture bombCooldown;
    private Texture flashbangCoolDown;
    private Texture flashbang;
    private Texture claymoreCoolDown;
    private Texture claymore;
    private Texture trapCoolDown;
    private Texture trap;
    private Texture backgroundHud;
    private Texture totalLife;
    private Texture currentLife;
    private Texture pointer;

	private PlayerController player;
	private HealthController life;
    private WeaponController2 weapon;
	private Scene currentScene;

    //0 -> transparent, 1 -> opaque
    public float alpha = 0.0f;
    private int drawDepth = -100;
    private Color fadeColor;

    // Use this for initialization
    void Start () {
        pointer = Resources.Load("Textures/crosshair") as Texture;
        currentLife = Resources.Load("Textures/green") as Texture;
        totalLife = Resources.Load("Textures/red") as Texture;
        backgroundHud = Resources.Load("Textures/backgroundHUD") as Texture;
        trap = Resources.Load("Textures/trampa") as Texture;
        trapCoolDown = Resources.Load("Textures/trampaDesact") as Texture;
        claymore = Resources.Load("Textures/Claymore") as Texture;
        claymoreCoolDown = Resources.Load("Textures/ClaymoreDesact") as Texture;
        flashbang = Resources.Load("Textures/Aturdidiora") as Texture;
        flashbangCoolDown = Resources.Load("Textures/AturdidioraDesact") as Texture;
        imageBullet = Resources.Load("Textures/bullet") as Texture;
        imageRecharge = Resources.Load("Textures/recharge") as Texture;
        black = Resources.Load("Textures/silver-metal-texture-vector") as Texture;
        bomb = Resources.Load("Textures/granada") as Texture;
        bombCooldown = Resources.Load("Textures/granadaDesact") as Texture;

        player = GetComponent<PlayerController> ();
		life = GetComponent<HealthController> ();
        weapon = GetComponent<WeaponController2>();

        currentScene = SceneManager.GetActiveScene();
    }
	
	// Update is called once per frame
	void Update () {
		

        alpha = player.getCooldown(4)/Constants.blindGrenadeCooldown;
        alpha = Mathf.Clamp01(alpha);
        fadeColor = GUI.color;
        fadeColor.a = alpha;
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

                //Pointer
                GUI.DrawTexture(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100), pointer, ScaleMode.StretchToFill);

				//Collision elevator or doors
				if (player.CollisionObject)
				{
					GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, Screen.width / 15, Screen.height / 12f), "Press E to use");
				} 
					
				for (int i = 0; i < 4; i++) 
				{
					if (player.getCooldown (i) != 0) 
					{
						switch (i) 
						{
						case 0:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.6f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), bombCooldown, ScaleMode.StretchToFill);
							break;
						case 1:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.7f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), claymoreCoolDown, ScaleMode.StretchToFill);
							break;
						case 2:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.7f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), trapCoolDown, ScaleMode.StretchToFill);
							break;
						case 3:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.7f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), flashbangCoolDown, ScaleMode.StretchToFill);
							break;
						}		
					} 
					else 
					{
						switch (i) 
						{
						case 0:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.6f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), bomb, ScaleMode.StretchToFill);
							GUI.Label (new Rect (Screen.width * i * 1.2f /  10, Screen.height * 9.5f / 10, Screen.width / 17, Screen.height / 9.5f), "Righ Mouse");
							break;
						case 1:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.7f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), claymore, ScaleMode.StretchToFill);
							GUI.Label (new Rect (Screen.width * i * 1.3f  / 10, Screen.height * 9.5f / 10, Screen.width / 17, Screen.height / 9.5f), "1");
							break;
						case 2:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.7f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), trap, ScaleMode.StretchToFill);
							GUI.Label (new Rect (Screen.width * i * 1f  / 10, Screen.height * 9.5f / 10, Screen.width / 17, Screen.height / 9.5f), "2");
							break;
						case 3:
							GUI.DrawTexture (new Rect (Screen.width * i * 0.7f / 10, Screen.height * 9 / 10, Screen.width / 17, Screen.height / 10), flashbang, ScaleMode.StretchToFill);
							GUI.Label (new Rect (Screen.width * i * 0.9f  / 10, Screen.height * 9.5f / 10, Screen.width / 17, Screen.height / 9.5f), "3");
							break;
						}
					}
				}

				//Life
				GUI.DrawTexture(new Rect(Screen.width *3.5f/10, Screen.height*9.1f/10, Screen.width*3/10, Screen.height*0.8f/10), totalLife, ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(Screen.width *3.5f/10, Screen.height*9.1f/10, life.Health/100f * (Screen.width*3/10), Screen.height*0.8f/10), currentLife, ScaleMode.StretchToFill);

                //Blind screen
                if (player.getCooldown(4) != 0)
                {
                    GUI.color = fadeColor;
                    GUI.depth = drawDepth;
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black);

                }
            }
		
		}

	}
}
