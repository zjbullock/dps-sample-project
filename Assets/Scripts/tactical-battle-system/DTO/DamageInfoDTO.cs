namespace DPS.TacticalCombat {
public class DamageInfoDTO
{
    #nullable enable
    public PartySlot? Attacker { get; }

    public PartySlot? Defender { get; }

    public int Damage { get; set; }

    public bool IsCritical { get; }

    public ElementSO Element { get; }

    public DamageInfoDTO(int damage, PartySlot attacker, PartySlot defender, bool isCritical, ElementSO element)
    {
        this.Attacker = attacker;
        this.Defender = defender;
        this.Damage = damage;
        this.IsCritical = isCritical;
        this.Element = element;
    }

    public string GetDamageString()
    {
        return this.IsCritical ? "CRIT"  : "";
    }

    #nullable disable
}

public enum DamageStatusTexts {
        WEAK,
        UNAFFECTED,
        RESIST,
        DRAIN,
        CRITICAL,
        BLOCK,
        MISS
}
}