using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {
[Serializable]
public class Movement {
    [SerializeField]
    private int speed;
    public int Speed { get => this.speed; }

    [SerializeField]
    private float jumpHeight;
    public float JumpHeight { get => this.jumpHeight; }

    public Movement(int speed, float verticalSpeed) {
        this.speed = speed;
        this.jumpHeight = verticalSpeed;
    }
}
}