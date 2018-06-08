using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour {
	#region Cooldown
	public const float bulletCooldown = 0.2f;
	public const int grenadeCooldown = 5;
	public const int blindGrenadeCooldown = 3;
	public const int trampExplosiveCooldown = 2;
	public const int trampInmovilCooldown = 4;
	#endregion

	#region Duration
	public const int blindGrenadeDuration = 3;
	#endregion

	#region Time Destroy
	public const int bulletTimeDestroy = 4;
	public const int grenadeTimeDestroy = 6;
	public const int blindGrenadeTimeDestroy = 3;
	public const int trampExplosiveTimeDestroy = 20;
	public const int trampInmovilTimeDestroy = 30;
	public const int lifeTimeDestroy = 10;
	public const int weaponTimeDestroy = 7;
	#endregion
}
