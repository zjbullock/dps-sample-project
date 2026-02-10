using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Battle_Action_Event_Spawn_Entities_[Entity Names]", menuName = "ScriptableObjects/Battle Action Event/Spawn Entity Event")]
public class BattleActionEventSpawnEntitySO : BattleActionEventSO
{
    [Tooltip("The summonable entity that will be summoned")]
    [SerializeField]
    private List<SummonedEntityInfoSO> _summonedEntityInfoSOs;

    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return;
    }

    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        if (this._summonedEntityInfoSOs.Count == 0)
        {
            return;
        }

        List<SummonedEntityPartySlot> summonedEntityPartySlots = new List<SummonedEntityPartySlot>();

        for (int i = 0; i < this._summonedEntityInfoSOs.Count; i++)
        {
            Debug.Log("Attempting to create summonable entity");

            SummonedEntityInfoSO summonedEntity = this._summonedEntityInfoSOs[i];

            if (summonedEntity.summonableEntityObject == null || summonedEntity.summonableEntityObject.GetComponent<SummonedEntityBattleObject>() == null)
            {
                continue;
            }

            #nullable enable
            GameObject? spawnedEntityObject = (GameObject)Instantiate(summonedEntity.summonableEntityObject, combatTiles[0].GetHeightForSpriteVector3Offset(false), new Quaternion());

            if (spawnedEntityObject == null)
            {
                continue;
            }

            BattleObject? battleObject = spawnedEntityObject.GetComponentInChildren<BattleObject>();

            if (battleObject == null)
            {
                continue;
            }

            #nullable disable
            Debug.Log("Created the summonable entity.");

            SummonedEntityInfo summonedEntityInfo = new(summonedEntity);
            summonedEntityInfo.GenerateBattleRawStats();

            BattleEntitySlot summonedBattleEntitySlot = new(summonedEntityInfo);

            BattleMember battleMember = new(summonedBattleEntitySlot);

            SummonedEntityPartySlot summonedEntityPartySlot = new(battleMember);

            // Animator anim = spawnedEntityObject.GetComponentInChildren<Animator>();
            // if (anim != null) {
            //     anim.runtimeAnimatorController = ;
            // }

            battleObject.SetBattleMember(battleMember);
            summonedEntityPartySlot.SetBattleObject(battleObject);


            summonedEntityPartySlots.Add(summonedEntityPartySlot);
            combatTiles[0].SetPartyOccupant(battleManager, summonedEntityPartySlot);

            if (BattleProcessingStatic.PartySlotIsPlayerPartySlot(user))
            {
                battleManager.PlayerPartyController.PartySlots.Add(summonedEntityPartySlot);
            }
            else if (BattleProcessingStatic.PartySlotIsEnemyPartySlot(user))
            {
                battleManager.EnemyPartyController.PartySlots.Add(summonedEntityPartySlot);
            }
            else
            {
                Debug.LogError("Summoned entity not summoned by a Party OR enemy type slot.");
            }

            summonedEntityPartySlot.CombatBegin();
            battleManager.AddToFutureTurnOrder(summonedEntityPartySlot);
        }

        user.SetSummonedEntities(summonedEntityPartySlots);
    }

    public override bool CanBeDoneOnTile(List<CombatTileController> combatTiles)
    {

        foreach (CombatTileController combatTile in combatTiles)
        {
            if (combatTile != null && combatTile.HasPartyOccupant())
            {
                return false;
            }
        }

        return true;
    }
}
}