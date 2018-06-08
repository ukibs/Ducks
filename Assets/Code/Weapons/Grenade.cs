using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grenade : NetworkBehaviour {

    public int damage;
    public int range;
	public GameObject owner;
    public GameObject explosionPrefab;

    private Effects effectManager;

    void Start()
    {
        effectManager = FindObjectOfType<Effects>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isServer)
        {
            RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, range, transform.forward);
            foreach (RaycastHit hit in hitInfo)
            {
                if (hit.transform.tag.Equals("Player") || hit.transform.tag.Equals("Enemy"))
                {
                    var health = hit.transform.GetComponent<HealthController>();
                    health.TakeDamage(damage, owner);
                }
            }
        }
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 3);
        effectManager.playEffect(1);
        Destroy(gameObject);
    }
}
