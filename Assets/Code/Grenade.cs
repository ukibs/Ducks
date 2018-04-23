using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public int damage;
    public int range;
    

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, range, transform.forward);
        foreach(RaycastHit hit in hitInfo)
        {
            if (hit.transform.tag.Equals("Player") || hit.transform.tag.Equals("Enemy"))
            {
                var health = hit.transform.GetComponent<HealthController>();
                health.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
