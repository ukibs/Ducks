using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponController : NetworkBehaviour {
	#region Public Attributes

	public GameObject bulletPrefab;
	public Transform shootPoint;
	public int maxWeaponAmmo = 10;
	public int maxReserveAmmo = 20;

	#endregion

	#region Private Attributes

	private float fireRate = 0.5f;
	private float fireCooldown = 0.0f;

	//[SyncVar(hook = "OnChangeAmmo")]
	//private int currentWeaponAmmo;
	//private int currentReserveAmmo;

	#endregion

	public bool special = false;

	[SyncVar (hook = "OnStandardChange")]
	private int standardWeaponAmmo;
	[SyncVar (hook = "OnStandardReserveChange")]
	private int standardReserveAmmo;

	[SyncVar (hook = "OnSpecialChange")]
	private int specialWeaponAmmo;
	[SyncVar (hook = "OnSpecialReserveChange")]
	private int specialReserveAmmo;

	public int CurrentAmmo
	{
		get {
			if (special)
				return specialWeaponAmmo;
			else
				return standardWeaponAmmo;
		}
	}

	void OnStandardChange(int _standardWeaponAmmo)
	{
		standardWeaponAmmo = _standardWeaponAmmo;
	}

	void OnStandardReserveChange(int _standardReserveAmmo)
	{
		standardReserveAmmo = _standardReserveAmmo;
	}

	void OnSpecialChange(int _specialWeaponAmmo)
	{
		specialWeaponAmmo = _specialWeaponAmmo;
	}

	void OnSpecialReserveChange(int _specialReserveAmmo)
	{
		specialReserveAmmo = _specialReserveAmmo;
	}

	// Use this for initialization
	void Start () {
		standardWeaponAmmo = 10;
		specialWeaponAmmo = 10;
	}
	
	// Update is called once per frame
	void Update () {
		if (isServer) {
			Debug.Log(gameObject.name + " ServerAmmo " + CurrentAmmo);
		}
		if (isLocalPlayer) {
			Debug.Log(gameObject.name + " ammo " + CurrentAmmo);
		}
	}

	public void wasteBullet()
	{
		if (!isServer) {
			return;
		} else {
			if (special) {
				if (specialWeaponAmmo > 0) {
					specialWeaponAmmo--;
				}
			} else {
				if (standardWeaponAmmo > 0) {
					standardWeaponAmmo--;
				}
			}
		}
	}

	public void reload()
	{
		if (!isServer) {
			return;
		} else {
			if (special) {
				int amountToReload = Mathf.Min (specialWeaponAmmo, maxWeaponAmmo);
				specialWeaponAmmo = amountToReload;
				specialWeaponAmmo -= amountToReload;
			} else {
				int amountToReload = Mathf.Min (standardWeaponAmmo, maxWeaponAmmo);
				standardWeaponAmmo = amountToReload;
				standardWeaponAmmo -= amountToReload;
			}
		}
	}

	public void addAmmo(int amount)
	{	
		if (!isServer) {
			return;
		} else {
			if (special) {
				int dif = maxWeaponAmmo - CurrentAmmo;

				if (dif >= amount) {
					specialWeaponAmmo += amount;
					amount = 0;
				} else {
					specialWeaponAmmo += dif;
					amount -= dif;
				}
				//add the rest of the bullets
				specialReserveAmmo += amount;
				//check that it doesn't exceed the limit
				if (specialReserveAmmo > maxReserveAmmo)
					specialReserveAmmo = maxReserveAmmo;
			} else {
				int dif = maxWeaponAmmo - CurrentAmmo;

				if (dif >= amount) {
					standardWeaponAmmo += amount;
					amount = 0;
				} else {
					standardWeaponAmmo += dif;
					amount -= dif;
				}
				//add the rest of the bullets
				standardReserveAmmo += amount;
				//check that it doesn't exceed the limit
				if (standardReserveAmmo > maxReserveAmmo)
					standardReserveAmmo = maxReserveAmmo;
			}
		}
	}


}
