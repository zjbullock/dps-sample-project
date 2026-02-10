using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
public class BattleActionController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Component Refs")]

    [SerializeField]
    protected SimpleAnimatorController simpleAnimatorController;

    [Header("Configruations")]
    [SerializeField] protected bool shouldDeactivateUser = false;

    [SerializeField] protected BattleObject user;

    void Awake()
    {
        this.SetSimpleAnimatorController();
        // if (this.user != null) {
        //     this.SetBattleActionControllerContent(this.user);
        // }
    }

    void Start()
    {
        this.AddDestructionAnimationEvent(() =>
        {
            Debug.Log("Destroy Event, Reactivate Objects");
            this.ToggleDeactivatedObjects(true);
        });
        this.SetSimpleAnimatorControllerParent();
    }

    // void OnDestroy() {
    // }

    private void SetSimpleAnimatorController()
    {
        if (this.simpleAnimatorController != null)
        {
            return;
        }
        this.simpleAnimatorController = GetComponentInChildren<SimpleAnimatorController>();
    }

    private void SetSimpleAnimatorControllerParent()
    {
        if (this.simpleAnimatorController == null)
        {
            return;
        }

        this.simpleAnimatorController.ParentGameObject = this.gameObject;
    }

    public void SetBattleActionControllerContent(BattleAnimationEventSpawn battleAnimationEvent)
    {
        this.user = battleAnimationEvent.Caster;
        this.AddDuringAnimationEvent(battleAnimationEvent.OnAnimationEvent);
        this.AddEndingAnimationEvent(battleAnimationEvent.OnAnimationEnd);
        if (this.shouldDeactivateUser)
        {
            this.ToggleDeactivatedObjects(false);
        }


    }

    private void AddDuringAnimationEvent(System.Action callBack)
    {
        if (this.simpleAnimatorController == null)
        {
            Debug.Log("Battle action present!");
            return;
        }
        Debug.Log("Adding animator event!");

        this.simpleAnimatorController.AddAnimatorProgressEvent(callBack);
    }

    #nullable enable
    private void AddEndingAnimationEvent(System.Action? callBack)
    {
        if (this.simpleAnimatorController == null)
        {
            return;
        }
        this.simpleAnimatorController.AddAnimatorEndEvent(callBack);
    }

    private void AddDestructionAnimationEvent(System.Action? callBack)
    {
        if (this.simpleAnimatorController == null)
        {
            return;
        }
        this.simpleAnimatorController.AddDestroyEvent(callBack);   
    }
    

    
    public void ToggleDeactivatedObjects(bool toggle)
    {
        if (this.user == null)
        {
            return;
        }

        if (this.user.gameObject.activeSelf == toggle)
        {
            return;
        }

        this.user.gameObject.SetActive(toggle);
    }
}
}