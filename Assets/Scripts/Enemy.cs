using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public BuffsAndDebuffs buffsAndDebuffs;

    public CharacterStats Stats;

    public AbilityHelperClass[] AllAbilities;
    public AbilityHelperClass NextAbility { get; set; }

    private int previousAbilityIndex = -1;

    //on start of combat + end of player turn call this
    public void PickAbilityForNextTurn()
    {
        List<int> list = new List<int>();
        for(int i = 0; i < AllAbilities.Length; i++)
        {
            if (i == previousAbilityIndex) continue;
            list.Add(i);
        }

        NextAbility = AllAbilities[Random.Range(0, list.Count)];
    }

    public void DoAbility()
    {
        if(NextAbility.DoesAbilityHeal)
        {
            if(NextAbility.Heal.TargetSelf)
            {
                Stats.Heal(NextAbility.Heal.HealAmount);
            }
            else
            {
                CurrentRunManager.instance.Stats.Heal(NextAbility.Heal.HealAmount);
            }
        }

        if(NextAbility.DoesAbilityDoDamage)
        {
            for (int i = 0; i < NextAbility.Damage.NumTimesToHit; i++)
            {
                if (NextAbility.Damage.TargetSelf)
                {
                    Stats.TakeDamage(NextAbility.Damage.DamageAmount);
                }
                else
                {
                    CurrentRunManager.instance.Stats.TakeDamage(NextAbility.Damage.DamageAmount);
                }
            }
        }

        foreach(var buff in NextAbility.Buffs)
        {
            if(buff.TargetSelf)
            {
                buffsAndDebuffs.ApplyBuff(buff.BuffType, buff.BuffAmount);
            }
            else
            {
                FindObjectOfType<CardPlayingManager>().BuffsAndDebuffs.ApplyBuff(buff.BuffType, buff.BuffAmount);

            }
        }

        foreach(var debuff in NextAbility.Debuffs)
        {
            if (debuff.TargetSelf)
            {
                buffsAndDebuffs.ApplyDebuff(debuff.DebuffType, debuff.DebuffAmount);
            }
            else
            {
                FindObjectOfType<CardPlayingManager>().BuffsAndDebuffs.ApplyDebuff(debuff.DebuffType, debuff.DebuffAmount);
            }
        }
    }
}