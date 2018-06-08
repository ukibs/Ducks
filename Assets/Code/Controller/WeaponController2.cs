using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponController2 : NetworkBehaviour
{
    public GameObject [] weaponPrefabs;
    public GameObject[] bulletPrefab;
    public int [] maxWeaponAmmo;
    public int [] maxReserveAmmo;

    private List<GameObject> weapons;
    private int [] currentWeaponAmmo;
    private int [] currentReserveAmmo;
    [SyncVar(hook = "OnIndexChange")]
    private int currentWeaponIndex;

    private PlayerController player;

    #region Properties
    public int WeaponIndex
    {
        get { return currentWeaponIndex; }
    }
    public int CurrentAmmo
    {
        get { return currentWeaponAmmo[currentWeaponIndex]; }
    }

    public int ReserveAmmo
    {
        get { return currentReserveAmmo[currentWeaponIndex]; }
    }

    public int MaxAmmo
    {
        get { return maxWeaponAmmo[currentWeaponIndex]; }
    }
    public int MaxReserveAmmo
    {
        get { return maxReserveAmmo[currentWeaponIndex]; }
    }

    public Transform CurrentWeapon
    {
        get { return weapons[currentWeaponIndex].GetComponent<BaseWeapon>().shootPoint; }
    }

    public GameObject CurrentPrefab
    {
        get { return weaponPrefabs[WeaponIndex]; }
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        player = GetComponent<PlayerController>();
        currentWeaponAmmo = new int[weaponPrefabs.Length];
        currentReserveAmmo = new int[weaponPrefabs.Length];
        weapons = new List<GameObject>(weaponPrefabs.Length);
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            GameObject newWeapon = Instantiate(weaponPrefabs[i], player.weaponPoint);
            newWeapon.transform.localPosition = Vector3.zero;
            if (i > 0) newWeapon.SetActive(false);
            weapons.Add(newWeapon);
            currentWeaponAmmo[i] = maxWeaponAmmo[i];
            currentReserveAmmo[i] = maxReserveAmmo[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnIndexChange(int _currentWeaponIndex)
    {
        currentWeaponIndex = _currentWeaponIndex;
    }

    public void wasteBullet()
    {
        if (!isServer)
        {
            return;
        }
        else
        {
            if (CurrentAmmo > 0)
            {
                currentWeaponAmmo[currentWeaponIndex]--;
                RpcUpdateInfo(CurrentAmmo, ReserveAmmo, WeaponIndex);
            }
        }
    }

    void reload()
    {
        if (!isServer)
        {
            return;
        }
        else
        {
            if (CurrentAmmo == 0)
            {
                    int amountToReload = Mathf.Min(ReserveAmmo, MaxAmmo);
                    currentWeaponAmmo[currentWeaponIndex] = amountToReload;
                    currentReserveAmmo[currentWeaponIndex] -= amountToReload;
            }
            RpcUpdateInfo(CurrentAmmo, ReserveAmmo, WeaponIndex);
        }
    }

    public void addAmmo(int amount, int type)
    {
        if (!isServer)
        {
            return;
        }
        else
        {
                int dif = maxWeaponAmmo[type] - currentWeaponAmmo[type];

                if (dif >= amount)
                {
                    currentWeaponAmmo[type] += amount;
                    amount = 0;
                }
                else
                {
                    currentWeaponAmmo[type] += dif;
                    amount -= dif;
                }
                //add the rest of the bullets
                currentReserveAmmo[type] += amount;
                //check that it doesn't exceed the limit
                if (currentReserveAmmo[type] > maxReserveAmmo[type])
                    currentReserveAmmo[type] = maxReserveAmmo[type];
            RpcUpdateInfo(currentWeaponAmmo[type], currentReserveAmmo[type], type);
        }
    }

    [Command]
    public void CmdReload()
    {
        reload();
    }

    [Command]
    public void CmdShoot(Vector3 pointToLook)
    {
        // Check if a point has been recived
        Quaternion directionToShoot;
        if (pointToLook != null && pointToLook != Vector3.zero) {
            Vector3 pointDirection = pointToLook - CurrentWeapon.position;
            directionToShoot = Quaternion.LookRotation(pointDirection, Vector3.up);
        }
        // If no take de weapon point rotation
        else directionToShoot = CurrentWeapon.rotation;

        GameObject newBullet = Instantiate(bulletPrefab[currentWeaponIndex], CurrentWeapon.position, directionToShoot);
        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * 30f;
        newBullet.GetComponent<Bullet>().owner = gameObject;

        wasteBullet();
        NetworkServer.Spawn(newBullet);
        
        Destroy(newBullet, Constants.bulletTimeDestroy);
    }

    [Command]
    public void CmdChangeWeapon()
    {
        int lastIndex = currentWeaponIndex;
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count) currentWeaponIndex = 0;
        weapons[currentWeaponIndex].SetActive(true);

        RpcChangeWeapon(lastIndex, WeaponIndex);
    }

    [ClientRpc]
    private void RpcUpdateInfo(int cWA, int cRA, int index)
    {
        currentWeaponAmmo[index] = cWA;
        currentReserveAmmo[index] = cRA;
    }

    [ClientRpc]
    private void RpcChangeWeapon(int lIndex, int index)
    {
        weapons[lIndex].SetActive(false);
        weapons[index].SetActive(true);
        currentWeaponIndex = index;
    }
}
