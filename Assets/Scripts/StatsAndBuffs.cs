using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsAndBuffs : MonoBehaviour
{
    
}

public class BuffsAndDebuffs
{
    public int Strength;
    public int Defense;

    public int Dot;
    public int LessDamage;
    public int LessArmour;
    public int TakeMoreDamage;

    public void ReduceDebuffsAfterTurnStart()
    {
        if (Dot > 0) Dot--;
        if (LessDamage > 0) LessDamage--;
        if (LessArmour > 0) LessArmour--;
        if (TakeMoreDamage > 0) TakeMoreDamage--;
    }

    public void ResetBuffsAndDebuffs()
    {
        Strength = 0;
        Defense = 0;

        Dot = 0;
        LessArmour = 0;
        LessDamage = 0;
        TakeMoreDamage = 0;
    }

    public void ApplyBuff(BuffType buff, int amount)
    {
        if (buff == BuffType.Defense)
        {
            Defense += amount;
        }
        if (buff == BuffType.Strength)
        {
            Strength += amount;
        }
    }

    public void ApplyDebuff(DebuffTypes debuff, int amount)
    {
        switch (debuff)
        {
            case DebuffTypes.DOT:
                Dot += amount;
                break;
            case DebuffTypes.LessDamage:
                LessDamage += amount;
                break;
            case DebuffTypes.LessArmour:
                LessArmour += amount;
                break;
            case DebuffTypes.TakeMoreDamage:
                TakeMoreDamage += amount;
                break;
        }
    }
}

[System.Serializable]
public class AbilityHelperClass
{
    public string AbilityName;
    public bool DoesAbilityHeal;
    public AbilityHeal Heal;

    public bool DoesAbilityDoDamage;
    public AbilityDamage Damage;

    public AbilityBuff[] Buffs;
    public AbilityDebuff[] Debuffs;
}

[System.Serializable]
public class AbilityHeal
{
    public bool TargetSelf = true;
    public int HealAmount;
}

[System.Serializable]
public class AbilityDamage
{
    public bool TargetSelf = false;
    public int DamageAmount;
    public int NumTimesToHit;
}

[System.Serializable]
public class AbilityBuff
{
    public bool TargetSelf = true;
    public BuffType BuffType;
    public int BuffAmount;
}

[System.Serializable]
public enum BuffType
{
    Strength,
    Defense
}

[System.Serializable]
public enum DebuffTypes
{
    DOT,
    LessDamage,
    LessArmour,
    TakeMoreDamage
}

[System.Serializable]
public class AbilityDebuff
{
    public bool TargetSelf = false;
    public DebuffTypes DebuffType;
    public int DebuffAmount;
}


