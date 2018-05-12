using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseWeapon : NetworkBehaviour {

    #region Public Attributes

    public GameObject bulletPrefab;
    public Transform shootPoint;
    public int maxWeaponAmmo = 10;
    public int maxReserveAmmo = 20;

    #endregion

    #region Private Attributes

    private float fireRate = 0.5f;
    private float fireCooldown = 0.0f;

    private int currentWeaponAmmo;
    private int currentReserveAmmo;

    #endregion

    #region Properties

    public int CurrentWeaponAmmo { get { return currentWeaponAmmo; } }

    public int CurrentReserveAmmo { get { return currentReserveAmmo; } }

    #endregion

    // Use this for initialization
    void Start () {
        currentWeaponAmmo = maxWeaponAmmo;
        currentReserveAmmo = maxReserveAmmo;
	}
	
	// Update is called once per frame
	void Update () {
        float dt = Time.deltaTime;

        if (fireCooldown < fireRate) fireCooldown += dt;
    }

    //
    public bool OrderFire()
    {
		if (Input.GetMouseButtonDown (0) && fireCooldown >= fireRate && currentWeaponAmmo > 0)
        {

            // CmdFire();
            fireCooldown = 0.0f;
            currentWeaponAmmo--;
            return true;
        }
        return false;
    }

    //
    public void Reload()
    {
		//Ese max devuelve el valor más alto
        int amountToReload = Mathf.Max(currentReserveAmmo, maxWeaponAmmo);
        currentWeaponAmmo = amountToReload;
        currentReserveAmmo -= amountToReload;
    }

	public void addAmmo(int amount)
	{
		int dif = maxWeaponAmmo - CurrentWeaponAmmo;
		if (dif >= amount) {
			currentWeaponAmmo += amount;
			amount = 0;
		} else {
			currentWeaponAmmo += dif;
			amount -= dif;
		}

		//add the rest of the bullets
		currentReserveAmmo += amount;
		//check that it doesn't exceed the limit
		if (currentReserveAmmo > maxReserveAmmo)
			currentReserveAmmo = maxReserveAmmo;
	}
}
