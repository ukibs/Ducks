using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigZagEnemy : EnemyControl {

    public float zigZagRate = 2;

    private float zigZagCounter;
    private int zigZagDirection = 1;

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        //
        float dt = Time.deltaTime;
        //
        zigZagCounter += dt;
        if(zigZagCounter >= zigZagRate)
        {
            zigZagDirection *= -1;
            zigZagCounter -= zigZagRate;
        }
        //
        cc.Move(transform.right * movementSpeed * dt * zigZagDirection);
	}
}
