using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Poise {

    public Poise(Poise downed) {
       this.poiseState = downed.PoiseState;
        this.poisePoints = downed.poisePoints;
        this.maxPoisePoints = downed.maxPoisePoints;
    }
    public Poise() {
        this.poiseState = PoiseState.None;
        this.poisePoints = 1;
        this.maxPoisePoints = 1;
    }

    public Poise(int maxDownPoints) {
        this.poiseState = PoiseState.None;
        this.poisePoints = maxDownPoints;
        this.maxPoisePoints = maxDownPoints;
    }

    //Represents the number of poisePoints needed to be downed.  If poisePoints hit 0, the target is downed.
    public int poisePoints;

    public int maxPoisePoints;

    private PoiseState poiseState;

    public PoiseState PoiseState { get => this.poiseState; }

    public void ProgressPoiseBrokenState() {
        if (this.PoiseState == PoiseState.None) {
            this.poisePoints--;
            if(this.poisePoints <= 0) {
                this.poiseState = PoiseState.Broken;
            }
        } else if (this.PoiseState == PoiseState.Broken) {
            this.poiseState = PoiseState.Recovering;
        } else if (this.PoiseState == PoiseState.Recovering) {
            this.poiseState = PoiseState.None;
            this.poisePoints = this.maxPoisePoints;
        }
    }

    public bool IsPoiseBroken() {
        return this.PoiseState != PoiseState.None;
    }

    public void ResetPoiseState() {
        this.poiseState = PoiseState.None;
        this.poisePoints = this.maxPoisePoints;
    }

}
[Serializable]
public enum PoiseState {
    None,
    Broken,
    Recovering
}
