using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseUsable : NetworkBehaviour
{
    [Command]
    public virtual void CmdUse()
    {
        Debug.Log("Base Usable: This shouldn't appear");
    }

    private void OnTriggerEnter(Collider collision)
    {
        var hit = collision.gameObject;
        if (hit.CompareTag("Player"))
        {
            var player = hit.GetComponent<PlayerController>();
            player.CmdCollisionObject(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        var hit = collision.gameObject;
        if (hit.CompareTag("Player"))
        {
            var player = hit.GetComponent<PlayerController>();
            player.CmdCollisionObject(false);
        }
    }
}
