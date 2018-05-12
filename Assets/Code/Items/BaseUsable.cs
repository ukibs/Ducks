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
}
