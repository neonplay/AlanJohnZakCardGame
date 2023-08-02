using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public void ReduceDebuffsOnTurnStart()
    {
        if (TakeMoreDamage > 0) TakeMoreDamage--;
    }

    public void ReduceDebuffsAfterTurnEnd()
    {
        if (LessDamage > 0) LessDamage--;
        if (LessArmour > 0) LessArmour--;
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
    public int ManaCost;

    public bool DoesAbilityHeal;
    public AbilityHeal Heal;

    public bool DoesAbilityGainArmour;
    public AbilityGainArmour Armour;

    public bool DoesAbilityDoDamage;
    public AbilityDamage Damage;

    public AbilityBuff[] Buffs;
    public AbilityDebuff[] Debuffs;

    public void DoAbility(CharacterStats userStats, CharacterStats opponentStats)
    {
        if (DoesAbilityHeal)
        {
            if (Heal.TargetSelf)
            {
                userStats.Heal(Heal.HealAmount);
            }
            else
            {
                opponentStats.Heal(Heal.HealAmount);
            }
        }

        if (DoesAbilityDoDamage)
        {
            for (int i = 0; i < Damage.NumTimesToHit; i++)
            {
                if (Damage.TargetSelf)
                {
                    userStats.TakeDamage(Damage.DamageAmount, false, i * 0.5f);
                }
                else
                {
                    opponentStats.TakeDamage(GetAdjustedDamage(Damage.DamageAmount), false, i * 0.5f);
                }
            }
        }

        int GetAdjustedDamage(int damage)
        {
            if (AbilityName.Contains("Gigavolt"))
            {
                if (opponentStats.BuffsAndDebuffs.LessDamage > 0)
                    return damage * 2;

                else return damage;
            }

            if (userStats.BuffsAndDebuffs.LessDamage > 0) damage = Mathf.RoundToInt(damage * 0.75f);
            if (opponentStats.BuffsAndDebuffs.TakeMoreDamage > 0) damage = Mathf.RoundToInt(damage * 1.5f);

            return damage;
        }

        if(DoesAbilityGainArmour)
        {
            if(Armour.TargetSelf)
            {
                userStats.GainArmour(Armour.ArmourAmount);
            }
            else
            {
                opponentStats.GainArmour(Armour.ArmourAmount);
            }
        }

        foreach (var buff in Buffs)
        {
            if (buff.TargetSelf)
            {
                userStats.BuffsAndDebuffs.ApplyBuff(buff.BuffType, buff.BuffAmount);
            }
            else
            {
                opponentStats.BuffsAndDebuffs.ApplyBuff(buff.BuffType, buff.BuffAmount);

            }
        }

        foreach (var debuff in Debuffs)
        {
            if (debuff.TargetSelf)
            {
                userStats.BuffsAndDebuffs.ApplyDebuff(debuff.DebuffType, debuff.DebuffAmount);
            }
            else
            {
                opponentStats.BuffsAndDebuffs.ApplyDebuff(debuff.DebuffType, debuff.DebuffAmount);
            }
        }

        //bonus damages
        if (AbilityName.Contains("Chain Lightning"))
        {
            if(opponentStats.BuffsAndDebuffs.LessDamage > 0)
            {
                opponentStats.TakeDamage(2 * opponentStats.BuffsAndDebuffs.LessDamage, false, 0.5f);
            }
        }

        if (AbilityName.Contains("Pyroclasm"))
        {
            if (opponentStats.BuffsAndDebuffs.Dot > 0)
            {
                opponentStats.TakeDamage(opponentStats.BuffsAndDebuffs.Dot, false, 0.5f);
            }
        }
    }

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
public class AbilityGainArmour
{
    public bool TargetSelf = true;
    public int ArmourAmount;
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

[Serializable]
public class CharacterStats
{
    public event Action OnHpZero;
    public static event Action<int, bool, bool, float> HpChanged;

    public bool Player;
    public int MaxMana = 20;
    public int Mana = 20;
    public int MaxHealth;
    public int CurrentHealth;
    public int Armour;

    public BuffsAndDebuffs BuffsAndDebuffs { get; set; } = new BuffsAndDebuffs();

    public void ChangeMana(int amount)
    {
        Mana += amount;
        if (Mana < 0) Mana = 0;
        if (Mana > MaxMana) Mana = MaxMana;
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        HpChanged?.Invoke(amount, Player, false, 0);
    }

    public void GainArmour(int amount)
    {
        Armour += amount;
    }

    public void TakeDamage(int amount, bool dot = false, float showDamageDelay = 0)
    {
        int remainingDamage = amount;

        if (!dot)
        {
            if (Armour > 0)
            {
                remainingDamage -= Armour;
                Armour -= amount;
            }
        }

        if (remainingDamage > 0)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);

            HpChanged?.Invoke(remainingDamage, Player, true, showDamageDelay);

        }

        if (CurrentHealth <= 0)
        {
            OnHpZero.Invoke();
        }
    }

    public void ApplyDotDamage()
    {
        if (BuffsAndDebuffs.Dot > 0)
        {
            TakeDamage(BuffsAndDebuffs.Dot, true);
            BuffsAndDebuffs.Dot--;
        }
    }
}
