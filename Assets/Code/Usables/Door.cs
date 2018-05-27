using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Door : BaseUsable
{
	public bool enter = false;

    
    public float journeyDuration = 0.5f;
	public Vector3 displacedPosition;
	private Vector3 originalPosition;
    // 0 closed, 1 opened
    private float status = 0.0f;
    private int direction = -1;

	void Start()
	{
		originalPosition = transform.position;
		displacedPosition += originalPosition;
	}

	// Update is called once per frame
	void Update ()
	{
        if (isServer)
        {
            status += direction * Time.deltaTime / journeyDuration;
            status = Mathf.Clamp01(status);

            transform.position = Vector3.Lerp(originalPosition, displacedPosition, status);
            
        }
	}

    [Command]
    public void CmdSwitchDirection()
    {
        direction *= -1;
    }

    [Command]
    public override void CmdUse()
    {
        base.CmdUse();
        direction *= -1;
    }
}
