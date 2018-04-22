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
		if (!Input.GetMouseButtonDown (0) && fireCooldown >= fireRate && currentWeaponAmmo > 0)
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
        int amountToReload = Mathf.Max(currentReserveAmmo, maxWeaponAmmo);
        currentWeaponAmmo = amountToReload;
        currentReserveAmmo -= amountToReload;
    }

    
}
