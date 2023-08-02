using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public CharacterStats Stats;

    public AbilityHelperClass[] AllAbilities;
    public AbilityHelperClass NextAbility { get; set; }

    private int previousAbilityIndex = -1;

    private void Start()
    {
        Stats.OnHpZero += HpHitZero;
    }

    private void HpHitZero()
    {
        FindObjectOfType<CombatManager>().EnemyHpHitZero();
    }

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
        Debug.LogError("doing ability");
        NextAbility.DoAbility(Stats, CurrentRunManager.instance.Stats);
    }
}