using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentRunManager : MonoBehaviour
{
    public static CurrentRunManager instance;

    public List<string> PlayerDeck;

    [Header("Current stats")]
    public CharacterStats Stats;
    public int Gold;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI HpText;
    [SerializeField] private TextMeshProUGUI ManaText;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image manaBar;

    [Header("Buffs")]
    [SerializeField] private TextMeshProUGUI armourText;
    [SerializeField] private GameObject burn;
    [SerializeField] private GameObject paralysis;
    [SerializeField] private GameObject power;
    [SerializeField] private GameObject dodge;

    [SerializeField] private GameObject gameOverScreen;

    [Header("")]
    [SerializeField] private GameObject map;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateHealthAndMana();
        Stats.OnHpZero += HpHitZero;
        Stats.OnArmourChanged += UpdateHealthAndMana;
        CharacterStats.HpChanged += (int x, bool ignore, bool ignore2, float ignore3) => { UpdateHealthAndMana(); };
    }

    private void HpHitZero()
    {
        FindObjectOfType<CombatManager>().PlayerHpReachedZero();
        gameOverScreen.SetActive(true);
    }

    public void UpdateHealthAndMana()
    {
        HpText.text = Stats.CurrentHealth + "/" + Stats.MaxHealth + "HP";
        ManaText.text = Stats.Mana + "/" + Stats.MaxMana + "MP";

        hpBar.fillAmount = (float)Stats.CurrentHealth / (float)Stats.MaxHealth;
        manaBar.fillAmount = (float)Stats.Mana / (float)Stats.MaxMana;

        armourText.text = Stats.Armour.ToString();
        armourText.transform.parent.gameObject.SetActive(Stats.Armour > 0);

        burn.SetActive(Stats.BuffsAndDebuffs.Dot > 0);
        power.SetActive(Stats.BuffsAndDebuffs.Strength > 0);
        paralysis.SetActive(Stats.BuffsAndDebuffs.LessDamage > 0);
        dodge.SetActive(false);
    }

    public void ReturnToMap()
    {
        FindObjectOfType<CombatManager>().CloseCombatScreen();
        map.SetActive(true);
    }
}