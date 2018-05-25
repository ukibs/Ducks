using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponController : NetworkBehaviour {
	#region Public Attributes

	public GameObject bulletPrefab;
	public Transform shootPoint;
	public const int maxWeaponAmmo = 10;
	public const int maxReserveAmmo = 20;

	#endregion

	#region Private Attributes

	private float fireRate = 0.5f;
	private float fireCooldown = 0.0f;

	#endregion

	public bool special = false;

	[SyncVar (hook = "OnStandardChange")]
	private int standardWeaponAmmo = maxWeaponAmmo;
	[SyncVar (hook = "OnStandardReserveChange")]
	private int standardReserveAmmo = maxReserveAmmo;

	[SyncVar (hook = "OnSpecialChange")]
	private int specialWeaponAmmo = maxWeaponAmmo;
	[SyncVar (hook = "OnSpecialReserveChange")]
	private int specialReserveAmmo = maxReserveAmmo;

	public int CurrentAmmo
	{
		get {
			if (special)
				return specialWeaponAmmo;
			else
				return standardWeaponAmmo;
		}
	}

	public int ReserveAmmo
	{
		get {
			if (special)
				return specialReserveAmmo;
			else
				return standardReserveAmmo;
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
	}
	
	// Update is called once per frame
	void Update () {
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

	void reload()
	{
		if (!isServer) {
			return;
		} else {
			if (CurrentAmmo == 0) {
				if (special) {
					int amountToReload = Mathf.Min (specialReserveAmmo, maxWeaponAmmo);
					specialWeaponAmmo = amountToReload;
					specialReserveAmmo -= amountToReload;
				} else {
					int amountToReload = Mathf.Min (standardReserveAmmo, maxWeaponAmmo);
					standardWeaponAmmo = amountToReload;
					standardReserveAmmo -= amountToReload;
				}
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

	[Command]
	public void CmdReload()
	{
		reload ();
	}
}
